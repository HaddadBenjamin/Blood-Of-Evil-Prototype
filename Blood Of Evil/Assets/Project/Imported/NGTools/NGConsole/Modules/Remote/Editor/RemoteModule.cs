using NGTools;
using NGTools.Network;
using NGTools.NGConsole;
using NGTools.NGGameConsole;
using NGToolsEditor.Network;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[InitializeOnLoad, Serializable, VisibleModule(200)]
	internal sealed class RemoteModule : Module, IStreams, IRows
	{
		[Serializable]
		private sealed class Vars
		{
			public int	workingStream;
		}

		public sealed class CommandRequest
		{
			public int			id;
			public RemoteRow	row;

			public	CommandRequest(int id, RemoteRow row)
			{
				this.id = id;
				this.row = row;
			}
		}

		public const string		ProgressBarConnectingString = "Connecting Remote module";
		//private static byte[]	pollDiscBuffer = new byte[1];

		public List<StreamLog>	Streams { get { return this.streams; } }
		public int				WorkingStream { get { return this.perWindowVars.Get(Utility.drawingWindow).workingStream; } }

		private List<StreamLog>	streams;

		[Exportable]
		private string			address = string.Empty;
		[Exportable]
		private int				port = NGServerCommand.DefaultPort;
		[NonSerialized]
		private ClientCLIExecuter executer;

		private List<Row>	rows;

		[NonSerialized]
		private Client	client;

		[NonSerialized]
		public RemoteCommandParser	parser;
		[NonSerialized]
		public List<CommandRequest>	pendingCommands;
		private string				command;
		[NonSerialized]
		private AbstractTcpClient[]	tcpClientProviders;
		[NonSerialized]
		private string[]				tcpClientProviderNames;
		private int						selectedTcpClientProvider;

		[NonSerialized]
		private int	idCounter = 0;

		[SerializeField]
		private PerWindowVars<Vars>	perWindowVars;
		[NonSerialized]
		private Vars				currentVars;
		[NonSerialized]
		private SectionDrawer		section;

		[NonSerialized]
		private AutoDetectUDPListener	udpListener;

		public	RemoteModule()
		{
			this.name = "Remote";
			this.streams = new List<StreamLog>();
			this.streams.Add(new StreamLog());
			this.rows = new List<Row>();
			this.perWindowVars = new PerWindowVars<Vars>();
		}

		public override void	OnEnable(NGConsoleWindow editor, int id)
		{
			base.OnEnable(editor, id);

			foreach (var stream in this.streams)
			{
				stream.Init(this.console, this);
				stream.rowsDrawer.SetRowGetter(this);

				for (int i = 0; i < this.rows.Count; i++)
					stream.AddLog(i, this.rows[i]);

				this.console.CheckNewLogConsume -= stream.ConsumeLog;
				this.console.PropagateNewLog -= stream.AddLog;
			}

			for (int i = 0; i < this.rows.Count; i++)
				this.rows[i].Init(this.console, this.rows[i].log);

			this.console.UpdateTick += this.Update;

			// Populate with default commands if missing.
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.SwitchNextStreamCommand, KeyCode.Tab, true);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.SwitchPreviousStreamCommand, KeyCode.Tab, true, true);

			this.executer = new ClientCLIExecuter(this);

			this.parser = new RemoteCommandParser();
			this.parser.CallExec += this.Exec;
			this.pendingCommands = new List<CommandRequest>();
			this.command = string.Empty;

			this.tcpClientProviders = Utility.CreateInstancesOf<AbstractTcpClient>();

			this.tcpClientProviderNames = new string[this.tcpClientProviders.Length];

			for (int i = 0; i < this.tcpClientProviders.Length; i++)
				this.tcpClientProviderNames[i] = this.tcpClientProviders[i].GetType().Name;

			if (this.selectedTcpClientProvider >= this.tcpClientProviders.Length - 1)
				this.selectedTcpClientProvider = this.tcpClientProviders.Length - 1;

			this.udpListener = new AutoDetectUDPListener(this.console, NGServerCommand.UDPPortBroadcastMin, NGServerCommand.UDPPortBroadcastMax);

			if (this.perWindowVars == null)
				this.perWindowVars = new PerWindowVars<Vars>();

			this.section = new SectionDrawer("Remote Module", typeof(NGSettings.RemoteModuleSettings), 40);
		}

		public override void	OnDisable()
		{
			base.OnDisable();

			this.console.UpdateTick -= this.Update;

			foreach (var stream in this.streams)
				stream.Uninit();

			if (this.IsClientConnected() == true)
				this.client.Close();

			this.udpListener.Stop();

			this.section.Uninit();
		}

		public override void	OnGUI(Rect r)
		{
			this.currentVars = this.perWindowVars.Get(Utility.drawingWindow);

			float	yOrigin = r.y;
			float	maxHeight = r.height;
			float	maxWidth = r.width;

			r.height = EditorGUIUtility.singleLineHeight;
			GUILayout.BeginArea(r);
			{
				GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					if (GUILayout.Button("Clear", GeneralStyles.ToolbarButton) == true)
						this.Clear();

					lock (this.udpListener.NGServerInstances)
					{
						EditorGUI.BeginDisabledGroup(this.udpListener.NGServerInstances.Count == 0);
						{
							if (this.udpListener.NGServerInstances.Count == 0)
								Utility.content.text = "No server";
							else if (this.udpListener.NGServerInstances.Count == 1)
								Utility.content.text = "1 server";
							else
								Utility.content.text = this.udpListener.NGServerInstances.Count + " servers";

							Rect	r2 = GUILayoutUtility.GetRect(Utility.content, GeneralStyles.ToolbarDropDown);

							if (GUI.Button(r2, Utility.content, GeneralStyles.ToolbarDropDown) == true)
							{
								GenericMenu	menu = new GenericMenu();
								bool		isConnected = this.IsClientConnected();

								for (int i = 0; i < this.udpListener.NGServerInstances.Count; i++)
								{
									bool	current = false;

									if (isConnected == true && this.udpListener.NGServerInstances[i].server == this.address + ":" + this.port)
										current = true;

									menu.AddItem(new GUIContent(this.udpListener.NGServerInstances[i].server), current, this.OverrideAddressPort, this.udpListener.NGServerInstances[i].server);
								}

								menu.DropDown(r2);
							}
						}
						EditorGUI.EndDisabledGroup();
					}

					EditorGUI.BeginDisabledGroup(this.IsClientConnected());
					{
						this.selectedTcpClientProvider = EditorGUILayout.Popup(this.selectedTcpClientProvider, this.tcpClientProviderNames, GeneralStyles.ToolbarDropDown);

						this.address = EditorGUILayout.TextField(this.address, GUILayout.MinWidth(50F), GUILayout.ExpandWidth(true));
						if  (string.IsNullOrEmpty(this.address) == true)
						{
							Rect	r2 = GUILayoutUtility.GetLastRect();
							EditorGUI.LabelField(r2, LC.G("RemoteModule_Address"), GeneralStyles.TextFieldPlaceHolder);
						}

						string	port = this.port.ToString();
						if (port == "0")
							port = string.Empty;
						EditorGUI.BeginChangeCheck();
						port = EditorGUILayout.TextField(port, GeneralStyles.ToolbarTextField, GUILayout.MaxWidth(40F));
						if (EditorGUI.EndChangeCheck() == true)
						{
							try
							{
								if (string.IsNullOrEmpty(port) == false)
									this.port = Mathf.Clamp(int.Parse(port), 0, Int16.MaxValue - 1);
								else
									this.port = 0;
							}
							catch
							{
								this.port = 0;
								GUI.FocusControl(null);
							}
						}

						if ((port == string.Empty || port == "0") && this.port == 0)
						{
							Rect	r2 = GUILayoutUtility.GetLastRect();
							EditorGUI.LabelField(r2, LC.G("RemoteModule_Port"), GeneralStyles.TextFieldPlaceHolder);
						}
					}
					EditorGUI.EndDisabledGroup();

					if (this.IsClientConnected() == true)
					{
						if (GUILayout.Button(LC.G("RemoteModule_Disconnect"), GeneralStyles.ToolbarButton) == true)
							this.CloseClient();
					}
					else
					{
						EditorGUI.BeginDisabledGroup(Utility.GetAsyncProgressBarInfo() == RemoteModule.ProgressBarConnectingString);
						{
							if (GUILayout.Button(LC.G("RemoteModule_Connect"), GeneralStyles.ToolbarButton) == true)
								this.AsyncOpenClient();
						}
						EditorGUI.EndDisabledGroup();
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
			r.y += r.height;

			r = this.DrawStreamTabs(r);

			r.x = 0F;
			r.width = maxWidth;

			if (this.currentVars.workingStream < 0)
				return;

			if (this.IsClientConnected() == true)
			{
				float	streamHeight = maxHeight - (r.y - yOrigin) - EditorGUIUtility.singleLineHeight;
				r.y += streamHeight;

				// Draw CLI before rows.
				r.height = EditorGUIUtility.singleLineHeight;
				this.DrawCLI(r);
				// Shit of a hack to handle input for completion and its drawing.
				this.parser.PostGUI(r, ref this.command);

				r.y -= streamHeight;
				r.height = streamHeight;

				this.streams[this.currentVars.workingStream].OnGUI(r);
				r.y += streamHeight;

				// Redraw again to display in front.
				this.parser.PostGUI(r, ref this.command);
			}
			else
			{
				r.height = maxHeight - (r.y - yOrigin);
				this.streams[this.currentVars.workingStream].OnGUI(r);
			}
		}

		public override void	OnEnter()
		{
			base.OnEnter();

			this.console.BeforeGUIHeaderRightMenu += this.GUIExport;
		}

		public override void	OnLeave()
		{
			base.OnLeave();

			this.console.BeforeGUIHeaderRightMenu -= this.GUIExport;
		}

		public void	Clear()
		{
			this.rows.Clear();

			foreach (var stream in this.streams)
				stream.Clear();
		}

		public void	FocusStream(int i)
		{
			if (i < 0)
				this.currentVars.workingStream = 0;
			else if (i >= this.streams.Count)
				this.currentVars.workingStream = this.streams.Count - 1;
			else
				this.currentVars.workingStream = i;
		}

		public void	DeleteStream(int i)
		{
			this.streams[i].Uninit();
			this.streams.RemoveAt(i);

			foreach (Vars var in this.perWindowVars.Each())
				var.workingStream = Mathf.Clamp(var.workingStream, 0, this.streams.Count - 1);
		}

		public void	AddRow(Row row)
		{
			this.rows.Add(row);

			int	index = this.rows.Count - 1;

			for (int i = 0; i < this.streams.Count; i++)
				this.streams[i].ConsumeLog(index, row);

			for (int i = 0; i < this.streams.Count; i++)
			{
				this.streams[i].AddLog(index, row);
				this.streams[i].rowsDrawer.UpdateAutoScroll();
			}
		}

		private void	Update()
		{
			if (this.client == null)
				return;

			if (this.DetectClientDisced(this.client) == true)
			{
				Debug.LogError(LC.G("RemoteModule_ClientDisconnected"));
				this.CloseClient();
				return;
			}

			this.client.Write();
			this.client.ExecReceivedCommands(this.executer);
		}

		private Rect	DrawStreamTabs(Rect r)
		{
			r.height = EditorGUIUtility.singleLineHeight;

			// Switch stream
			if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchNextStreamCommand) == true)
			{
				this.currentVars.workingStream += 1;
				if (this.currentVars.workingStream >= this.streams.Count)
					this.currentVars.workingStream = 0;

				Event.current.Use();
			}
			if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchPreviousStreamCommand) == true)
			{
				this.currentVars.workingStream -= 1;
				if (this.currentVars.workingStream < 0)
					this.currentVars.workingStream = this.streams.Count - 1;

				Event.current.Use();
			}

			GUILayout.BeginArea(r);
			{
				GUILayout.BeginHorizontal();
				{
					for (int i = 0; i < this.streams.Count; i++)
						this.streams[i].OnTabGUI(i);

					if (GUILayout.Button("+", Preferences.Settings.general.menuButtonStyle) == true)
					{
						StreamLog	stream = new StreamLog();
						stream.Init(this.console, this);
						stream.rowsDrawer.SetRowGetter(this);
						this.streams.Add(stream);

						this.console.CheckNewLogConsume -= stream.ConsumeLog;
						this.console.PropagateNewLog -= stream.AddLog;

						if (this.streams.Count == 1)
							this.currentVars.workingStream = 0;
					}

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			r.y += r.height + 2F;

			return r;
		}

		private Rect	DrawCLI(Rect r)
		{
			this.parser.HandleKeyboard(ref this.command);

			r.height = EditorGUIUtility.singleLineHeight;
			r.width -= Preferences.Settings.remoteModule.execButtonWidth;

			if (this.parser.root.children.Count == 0)
				EditorGUI.LabelField(r, LC.G("RemoteModule_CLIUnavailable"), Preferences.Settings.remoteModule.commandInputStyle);
			else
			{
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName(CommandParser.CommandTextFieldName);
				this.command = GUI.TextField(r, this.command, Preferences.Settings.remoteModule.commandInputStyle);
				if (EditorGUI.EndChangeCheck() == true)
					this.parser.UpdateMatchesAvailable(this.command);
			}

			if (this.parser.root.children.Count == 0)
				GUI.enabled = false;

			r.x += r.width;
			r.width = Preferences.Settings.remoteModule.execButtonWidth;
			if (GUI.Button(r, "Exec", Preferences.Settings.remoteModule.execButtonStyle) == true)
				this.Exec();

			if (this.parser.root.children.Count == 0)
				GUI.enabled = true;
			r.y += r.height;

			return r;
		}

#if NGTOOLS_FREE
		private int	countExec = 0;
#endif

		private void	Exec()
		{
			string		result = string.Empty;
			ExecResult	returnValue = ExecResult.InvalidCommand;
#if NGTOOLS_FREE
			if (++this.countExec > FreeConstants.MaxCLICommandExecutions)
				result = "Free version does not allow more than " + FreeConstants.MaxCLICommandExecutions + " remote executions.\nAwesomeness has a price. :D";
			else
#endif
			returnValue = this.parser.Exec(this.command, ref result);

			LogEntry	log = new LogEntry();
			log.condition = this.command;
			// Use file to display the default value from RemoteCommand while waiting for the answer.
			log.file = result;

			RemoteRow	row = new RemoteRow();
			row.Init(this.console, log);

			this.AddRow(row);

			if (returnValue == ExecResult.Success)
			{
				// Create a waiter to update the log when the server will answer.
				CommandRequest	cr = new CommandRequest(++this.idCounter, row);
				this.pendingCommands.Add(cr);

				this.client.AddPacket(new ClientSendCommandPacket(cr.id, this.command));
			}
			else
				row.error = result;

			this.command = string.Empty;
			this.parser.Clear();
		}

		private void	AsyncOpenClient()
		{
			Utility.StartAsyncBackgroundTask(this.OpenClient, RemoteModule.ProgressBarConnectingString, 10F, "Connecting to server is too long and was aborted.");
		}

		private void	OpenClient(object task)
		{
			try
			{
				this.client = this.tcpClientProviders[this.selectedTcpClientProvider].CreateClient(this.address, this.port);

				if (this.client != null)
				{
					BlankRow	row = new BlankRow(this.console.position.height);
					LogEntry	log = new LogEntry();
					row.Init(this.console, log);
					this.AddRow(row);

					ClientRequestCommandNodesPacket	packet = new ClientRequestCommandNodesPacket();
					this.client.AddPacket(packet);
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}

		private void	CloseClient()
		{
			// HACK Calling Close() seems to block the instance, even when manually closing the inner stream.
			this.client.Close();
			// Thus reset the instance.
			this.client = null;
		}

		public bool		IsClientConnected()
		{
			return this.client != null &&
				   this.client.tcpClient.Connected == true;
		}

		private void	OverrideAddressPort(object data)
		{
			string	server = data as string;

			if (server == this.address + ":" + this.port && this.IsClientConnected() == true)
				return;

			int	separator = server.LastIndexOf(':');

			this.address = server.Substring(0, separator);
			this.port = int.Parse(server.Substring(separator + 1));

			if (this.IsClientConnected() == true)
				this.CloseClient();
			this.AsyncOpenClient();
		}

		Row		IRows.GetRow(int i)
		{
			return this.rows[i];
		}

		int		IRows.CountRows()
		{
			return this.rows.Count;
		}

		private bool	DetectClientDisced(Client client)
		{
			if (client.tcpClient.Connected == false)
				return true;

			//try
			//{
			//	if (client.tcpClient.Client.Poll(0, SelectMode.SelectRead) == true &&
			//		client.tcpClient.Client.Receive(pollDiscBuffer, SocketFlags.Peek) == 0)
			//	{
			//		return true;
			//	}
			//}
			//catch (Exception ex)
			//{
			//	InternalNGDebug.LogException(ex);
			//}

			return false;
		}

		private void	GUIExport()
		{
			Vars	vars = this.perWindowVars.Get(Utility.drawingWindow);

			if (vars != null) // Happens during the first frame, when OnGUI has not been called yet.
			{
				GUI.enabled = vars.workingStream >= 0 && this.streams[vars.workingStream].rowsDrawer.Count > 0;
				if (GUILayout.Button(LC.G("RemoteModule_ExportStream"), Preferences.Settings.general.menuButtonStyle) == true)
				{
					List<Row>	rows = new List<Row>();

					for (int i = 0; i < this.streams[vars.workingStream].rowsDrawer.Count; i++)
						rows.Add(this.rows[this.streams[vars.workingStream].rowsDrawer[i]]);

					ExportRowsEditorWindow.Export(rows);
				}
				GUI.enabled = true;
			}
		}
	}
}