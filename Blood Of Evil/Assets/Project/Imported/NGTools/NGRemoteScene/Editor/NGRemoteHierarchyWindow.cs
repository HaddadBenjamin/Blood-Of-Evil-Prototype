using NGTools;
using NGTools.Network;
using NGTools.NGRemoteScene;
using NGToolsEditor.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using InnerUtility = NGTools.Utility;

namespace NGToolsEditor.NGRemoteScene
{
	public enum HierarchyReady
	{
		Layers = 1 << 0
	}

	[InitializeOnLoad]
	public class NGRemoteHierarchyWindow : EditorWindow, IHierarchyManagement, IHasCustomMenu, IUnityData
	{
		private sealed class OptionPopup : PopupWindowContent
		{
			private readonly NGRemoteHierarchyWindow	window;

			public	OptionPopup(NGRemoteHierarchyWindow window)
			{
				this.window = window;
			}

			public override Vector2	GetWindowSize()
			{
				return new Vector2(Mathf.Max(this.window.position.width * .5F, 200F), this.window.IsClientConnected() ? 57F : 38F);
			}

			public override void	OnGUI(Rect r)
			{
				using (LabelWidthRestorer.Get(110F))
				{
					Utility.content.text = LC.G("NGHierarchy_NetworkRefresh");
					Utility.content.tooltip = LC.G("NGHierarchy_NetworkRefreshTooltip");
					this.window.networkRefresh = EditorGUILayout.FloatField(Utility.content, this.window.networkRefresh);
					Utility.content.tooltip = string.Empty;

					Utility.content.text = LC.G("NGHierarchy_SyncTags");
					Utility.content.tooltip = LC.G("NGHierarchy_SyncTagsTooltip");
					this.window.syncTags = EditorGUILayout.Toggle(Utility.content, this.window.syncTags);
					Utility.content.tooltip = string.Empty;

					if (this.window.IsClientConnected() == true)
					{
						if (GUILayout.Button(LC.G("NGHierarchy_RefreshHierarchy")) == true)
							this.window.Client.AddPacket(new ClientRequestHierarchyPacket());
					}
				}
			}
		}

		public const string	NormalTitle = "ƝƓ Ʀemote Ħierarchy";
		public const string	ShortTitle = "ƝƓ Ʀ Ħierarchy";
		public const float	PingInterval = 2F;
		public const float	MaxPingTimeBeforeShutdown = 20F;
		public const float	BlockRequestLifeTime = 10F;
		public const string	ProgressBarConnectingString = "Connecting NG Remote Hierachy";
		public static Color	DropBackgroundColor = Color.yellow;
		[SetColor(50F / 255F, 76F / 255F, 120F / 255F, 1F, 50F / 255F, 100F / 255F, 185F / 255F, 1F)]
		public static Color	SelectedGameObjectBackgroundColor = default(Color);
		[SetColor(60F / 255F, 60F / 255F, 60F/ 255F, 1F, 180F / 255F, 180F / 255F, 180F/ 255F, 1F)]
		public static Color	PathBackgroundColor = default(Color);

		private static ByteBuffer	Buffer = new ByteBuffer(64);
		//private static byte[]		pollDiscBuffer = new byte[1];

		public HierarchyReady	ready { get; private set; }

		public string	address;
		public int		port;
		public float	networkRefresh = .01F;
		public bool		syncTags;

		[NonSerialized]
		public PacketExecuter	executer;

		public event Action	HierarchyConnected;
		public event Action	HierarchyDisconnected;

		public Client	Client { get; private set; }
		public string[] Layers { get; private set; }
		public string	LastInvokeResult { get; private set; }
		public DateTime	LastInvokeResultTime { get; private set; }

		private ClientGameObject					root;
		private Dictionary<int, ClientGameObject>	gameObjectInstanceIDs = new Dictionary<int, ClientGameObject>();
		private Dictionary<Type, string[]>			resourceNames = new Dictionary<Type, string[]>();
		private Dictionary<Type, int[]>				resourceInstanceIds = new Dictionary<Type, int[]>();
		private Dictionary<string, EnumData>		enumData = new Dictionary<string, EnumData>();
		private List<KeyValuePair<float, string>>	updateValueNotifications = new List<KeyValuePair<float, string>>();
		private Dictionary<int, NetMaterial>		materials = new Dictionary<int, NetMaterial>();

		private List<KeyValuePair<int, float>>	blockedRequestChannels = new List<KeyValuePair<int, float>>();

		private Dictionary<NGRemoteInspectorWindow, int>		watchersGameObject = new Dictionary<NGRemoteInspectorWindow, int>();
		private Dictionary<NGRemoteInspectorWindow, int[]>	watchersMaterials = new Dictionary<NGRemoteInspectorWindow, int[]>();

		private float	nextNetworkRefresh;

		private Vector2	scrollPosition = new Vector2();
		private Rect	bodyRect = new Rect();
		private Rect	viewRect = new Rect();

		private string					searchPattern = string.Empty;
		private List<ClientGameObject>	filteredGameObjects = new List<ClientGameObject>();

		private string		valuePath;
		private string[]	paths;
		private byte[]		rawValue;

		private Vector2	dragOriginPosition;

		private AutoDetectUDPListener	udpListener;

		#region Server status
		private Ping								ping;
		private float								lastPing;
		private float								nextPingTime;
		private List<KeyValuePair<AnimFloat, int>>	pings = new List<KeyValuePair<AnimFloat, int>>();
		#endregion

		[NonSerialized]
		private List<NGRemoteWindow>	remoteWindows = new List<NGRemoteWindow>();

		static	NGRemoteHierarchyWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGRemoteHierarchyWindow.NormalTitle);
		}

		[MenuItem(Constants.MenuItemPath + NGRemoteHierarchyWindow.NormalTitle, priority = Constants.MenuItemPriority + 210)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGRemoteHierarchyWindow>(NGRemoteHierarchyWindow.ShortTitle);
		}

		protected virtual void	OnEnable()
		{
			this.executer = new ClientSceneExecuter(this);

			this.udpListener = new AutoDetectUDPListener(this, NGServerScene.UDPPortBroadcastMin, NGServerScene.UDPPortBroadcastMax);

			Utility.RepaintEditorWindow(typeof(NGRemoteWindow));
		}

		protected virtual void	OnDisable()
		{
			for (int i = 0; i < this.remoteWindows.Count; i++)
				this.remoteWindows[i].SetHierarchy(null);

			this.udpListener.Stop();

			if (this.IsClientConnected() == true)
				this.CloseClient();

			Utility.RepaintEditorWindow(typeof(NGRemoteWindow));
		}

		protected virtual void	OnGUI()
		{
			FreeOverlay.First(this, "NG Remote Scene is exclusive to NG Tools Pro.\n\nFree version is restrained to read-only.");

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button("☰", "GV Gizmo DropDown", GUILayout.ExpandWidth(false)) == true)
					PopupWindow.Show(new Rect(0F, 16F, 0F, 0F), new OptionPopup(this));

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

						Rect	r = GUILayoutUtility.GetRect(Utility.content, GeneralStyles.ToolbarDropDown);

						if (GUI.Button(r, Utility.content, GeneralStyles.ToolbarDropDown) == true)
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

							menu.DropDown(r);
						}
					}
					EditorGUI.EndDisabledGroup();
				}

				GUI.enabled = (this.Client == null || this.Client.tcpClient.Connected == false);

				this.address = EditorGUILayout.TextField(this.address, GeneralStyles.ToolbarTextField, GUILayout.MinWidth(50F), GUILayout.ExpandWidth(true));
				if  (string.IsNullOrEmpty(this.address) == true)
				{
					Rect	r = GUILayoutUtility.GetLastRect();
					EditorGUI.LabelField(r, LC.G("NGHierarchy_Address"), GeneralStyles.TextFieldPlaceHolder);
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
					Rect	r = GUILayoutUtility.GetLastRect();
					EditorGUI.LabelField(r, LC.G("NGHierarchy_Port"), GeneralStyles.TextFieldPlaceHolder);
				}

				GUI.enabled = true;

				if (this.IsClientConnected() == true)
				{
					if (GUILayout.Button(LC.G("NGHierarchy_Disconnect"), GeneralStyles.ToolbarButton) == true)
						this.CloseClient();
				}
				else
				{
					EditorGUI.BeginDisabledGroup(Utility.GetAsyncProgressBarInfo() == NGRemoteHierarchyWindow.ProgressBarConnectingString);
					{
						if (GUILayout.Button(LC.G("NGHierarchy_Connect"), GeneralStyles.ToolbarButton) == true)
							this.AsyncOpenClient();
					}
					EditorGUI.EndDisabledGroup();
				}
			}
			EditorGUILayout.EndHorizontal();

			if (this.IsClientConnected() == true)
			{
				// Server status
				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					GUILayout.Label(string.Format(LC.G("NGHierarchy_Latency"), this.lastPing));
					GUILayout.Label(string.Format(LC.G("NGHierarchy_SentBytes"), this.Client.bytesSent) + " (" + (this.Client.bytesSent / 1000000) + " MB) | " + string.Format(LC.G("NGHierarchy_ReceivedBytes"), this.Client.bytesReceived) + " (" + (this.Client.bytesReceived / 1000000) + " MB)");
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUI.BeginChangeCheck();
				this.searchPattern = EditorGUILayout.TextField(this.searchPattern, GeneralStyles.ToolbarSearchTextField);
				if (EditorGUI.EndChangeCheck() == true)
					this.UpdateFilteredIndexes();

				if (GUILayout.Button("", GeneralStyles.ToolbarSearchCancelButton) == true)
				{
					this.searchPattern = string.Empty;
					GUI.FocusControl(null);
					this.UpdateFilteredIndexes();
				}
			}
			EditorGUILayout.EndHorizontal();

			if (this.IsClientConnected() == false)
			{
				GUILayout.Label(LC.G("NGRemote_NotConnected"), GeneralStyles.BigCenterText, GUILayout.ExpandHeight(true));
				FreeOverlay.Last();
				return;
			}

			if (this.root != null &&
				this.root.children.Count > 0)
			{
				this.viewRect.height = 0F;

				this.bodyRect = GUILayoutUtility.GetLastRect();
				this.bodyRect.x = 0F;
				this.bodyRect.y += this.bodyRect.height;
				this.bodyRect.width = this.position.width;
				this.bodyRect.height = this.position.height - this.bodyRect.y;

				if (string.IsNullOrEmpty(this.searchPattern) == false)
				{
					viewRect.height = this.filteredGameObjects.Count * EditorGUIUtility.singleLineHeight;

					// Calcul parents height.

					ClientGameObject	last = this.GetLastSelected();

					if (last != null)
					{
						bodyRect.height -= EditorGUIUtility.singleLineHeight;

						while (last.Parent != null)
						{
							bodyRect.height -= EditorGUIUtility.singleLineHeight;

							last = last.Parent;
						}

						if (bodyRect.height < EditorGUIUtility.singleLineHeight)
							bodyRect.height = EditorGUIUtility.singleLineHeight;
					}
				}
				else
				{
					for (int i = 0, p = 0; i < this.root.children.Count; ++i, ++p)
						viewRect.height += this.GetGameObjectHeight(this.root.children[i]);
				}

				Rect	r = bodyRect;

				this.scrollPosition = GUI.BeginScrollView(r, this.scrollPosition, viewRect);
				{
					r.y = 0F;
					r.height = EditorGUIUtility.singleLineHeight;

					if (string.IsNullOrEmpty(this.searchPattern) == false)
					{
						for (int i = 0, p = 0; i < this.filteredGameObjects.Count; ++i, ++p)
						{
							if (r.y + r.height <= this.scrollPosition.y)
							{
								r.y += r.height;
								continue;
							}

							r.width = this.position.width;
							r = this.DrawGameObject(r, ref p, this.filteredGameObjects[i], true);

							if (r.y - this.scrollPosition.y > bodyRect.height)
								break;
						}
					}
					else
					{
						for (int i = 0, p = 0; i < this.root.children.Count; ++i, ++p)
						{
							float	height = this.GetGameObjectHeight(this.root.children[i]);

							if (r.y + height <= this.scrollPosition.y)
							{
								r.y += height;
								continue;
							}

							r.width = this.position.width;
							r = this.DrawGameObject(r, ref p, this.root.children[i]);

							if (r.y - this.scrollPosition.y > this.bodyRect.height)
								break;
						}
					}
				}
				GUI.EndScrollView();

				if (string.IsNullOrEmpty(this.searchPattern) == false)
				{
					ClientGameObject	last = this.GetLastSelected();

					if (last != null)
					{
						bodyRect.y += bodyRect.height;

						if (Event.current.type == EventType.Repaint)
						{
							bodyRect.height = this.position.height - bodyRect.y;
							EditorGUI.DrawRect(bodyRect, NGRemoteHierarchyWindow.PathBackgroundColor);

							bodyRect.y += 2F;
							bodyRect.height = 1F;

							EditorGUI.DrawRect(bodyRect, Color.black);
						}

						bodyRect.height = EditorGUIUtility.singleLineHeight;
						GUI.Label(bodyRect, " Path:");

						bodyRect.x += 16F;
						while (last.Parent != null)
						{
							bodyRect.y += EditorGUIUtility.singleLineHeight;
							GUI.Label(bodyRect, last.name);

							last = last.Parent;
						}
					}

					FreeOverlay.Last();

					return;
				}

				if (Event.current.type == EventType.MouseDown)
				{
					this.root.ClearSelection();
					GUI.FocusControl(null);
					Event.current.Use();
				}
				else if (Event.current.type == EventType.KeyDown)
				{
					if (Event.current.keyCode == KeyCode.RightArrow)
					{
						ClientGameObject	node = this.GetLastSelected();

						if (node.children.Count > 0)
						{
							if (node.fold == true)
							{
								if (node.children.Count > 0)
								{
									this.root.ClearSelection();
									node.children[0].Selected = true;
								}
							}
							else
								node.fold = true;
						}

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.LeftArrow)
					{
						ClientGameObject	node = this.GetLastSelected();

						if (node.children.Count > 0)
						{
							if (node.fold == false)
							{
								if (node.Parent != this.root)
								{
									this.root.ClearSelection();
									node.Parent.Selected = true;
								}
							}
							else
								node.fold = false;
						}
						else
						{
							if (node.Parent != this.root)
							{
								this.root.ClearSelection();
								node.Parent.Selected = true;
							}
						}

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.Delete)
					{
						ClientDeleteGameObjectsPacket	packet = new ClientDeleteGameObjectsPacket();
						ClientGameObject[]				selection = this.GetSelectedGameObjects();

						for (int i = 0; i < selection.Length; i++)
							packet.Add(selection[i].instanceID);

						this.Client.AddPacket(packet);

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.UpArrow)
					{
						this.SelectPrevious();

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.DownArrow)
					{
						this.SelectNext();

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.Home)
					{
						this.root.ClearSelection();

						if (this.root.children.Count > 0)
							this.root.children[0].Selected = true;

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.End)
					{
						this.root.ClearSelection();

						ClientGameObject	n = this.root;

						rewind:
						for (int i = n.children.Count - 1; i >= 0;)
						{
							if (n.children[i].fold == true)
							{
								n = n.children[i];
								goto rewind;
							}
							n = n.children[i];
							break;
						}

						n.Selected = true;

						Event.current.Use();
					}
				}
			}

			FreeOverlay.Last();
		}

		protected virtual void	Update()
		{
			if (this.Client == null)
				return;

			if (this.IsClientConnected() == true)
			{
				if (this.ping == null)
				{
					if (this.nextPingTime <= Time.realtimeSinceStartup)
					{
						this.ping = new Ping(this.address);
						this.nextPingTime = Time.realtimeSinceStartup;
					}
				}
				else if (this.ping.isDone == true)
				{
					this.lastPing = this.ping.time;
					this.ping.DestroyPing();
					this.ping = null;
					this.nextPingTime = Time.realtimeSinceStartup + NGRemoteHierarchyWindow.PingInterval;
					this.Repaint();
				}
				else if (Time.realtimeSinceStartup - this.nextPingTime > NGRemoteHierarchyWindow.MaxPingTimeBeforeShutdown)
				{
					InternalNGDebug.Log(LC.G("NGHierarchy_ClientDisconnected") + " (" + this.Client.tcpClient.Client.LocalEndPoint + ") " + Time.realtimeSinceStartup + " - " + this.nextPingTime);
					this.CloseClient();
					return;
				}
			}

			if (this.nextNetworkRefresh > Time.realtimeSinceStartup)
				return;

			this.nextNetworkRefresh = Time.realtimeSinceStartup + this.networkRefresh;

			//if (this.DetectClientDisced(this.Client) == true)
			//{
			//	InternalNGDebug.Log(LC.G("NGHierarchy_ClientDisconnected") + " (" + this.Client.tcpClient.Client.LocalEndPoint + ")");
			//	this.CloseClient();
			//	return;
			//}

			this.Client.Write();
			this.Client.ExecReceivedCommands(this.executer);

			this.Repaint();
		}

		public void		AddRemoteWindow(NGRemoteWindow window)
		{
			InternalNGDebug.Assert(this.remoteWindows.Contains(window) == false, "NG Remote Window \"" + window + "\" is already linked to NG Remote Hierarchy.");
			this.remoteWindows.Add(window);
			window.SetHierarchy(this);
		}

		public void		RemoveRemoteWindow(NGRemoteWindow window)
		{
			for (int i = 0; i < this.remoteWindows.Count; i++)
			{
				if (this.remoteWindows[i] == window)
				{
					this.remoteWindows[i].SetHierarchy(null);
					break;
				}
			}
		}

		public void		AsyncOpenClient()
		{
			Utility.StartAsyncBackgroundTask(this.OpenClient, NGRemoteHierarchyWindow.ProgressBarConnectingString, 10F, "Connecting to server is too long and was aborted.");
		}

		private void	OpenClient(object task)
		{
			try
			{
				TcpClient	tcp = new TcpClient();
				tcp.Connect(this.address, this.port);

				this.Client = new Client(tcp);
				this.Client.AddPacket(new ClientRequestHierarchyPacket());
				this.Client.AddPacket(new ClientRequestLayersPacket());

				this.root = new ClientGameObject();

				this.searchPattern = string.Empty;

				this.nextNetworkRefresh = 0F;

				EditorApplication.delayCall += () =>
				{
					if (this.HierarchyConnected != null)
						this.HierarchyConnected();
#if NGTOOLS_FREE
					this.notifyOnce = false;
#endif
				};

				this.LastInvokeResult = string.Empty;

				string	server = this.address + ':' + this.port;

				lock (this.udpListener.NGServerInstances)
				{
					if (this.udpListener.NGServerInstances.Exists(s => s.server == server) == false)
						this.udpListener.NGServerInstances.Add(new AutoDetectUDPListener.NGServerInstance() { server = server, pingMaxLastTime = Utility.ConvertToUnixTimestamp(DateTime.Now) + AutoDetectUDPListener.UDPServerPingLifetime });
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(Errors.Scene_Exception, ex);
			}
		}

		public void		CloseClient()
		{
			this.nextNetworkRefresh = 0F;

			InternalNGDebug.Assert(this.Client != null, "NG Hierarchy is closing a null Client!");

			this.Client.AddPacket(new ClientHasDisconnectedPacket());

			Client	client = this.Client;

			this.Client = null;

			// Give Client the time to send the disconnect packet.
			EditorApplication.delayCall += () => Utility.StartBackgroundTask(this.DelayDiscAndClean(client));
		}

		public bool		IsClientConnected()
		{
			return this.Client != null &&
				   this.Client.tcpClient.Connected == true;
		}

		public void		SetRootGameObjects(NetGameObject[] children)
		{
			this.gameObjectInstanceIDs.Clear();
			this.root.children.Clear();

			for (int i = 0; i < children.Length; i++)
			{
				ClientGameObject	child = new ClientGameObject(this.root, children[i], this);
				child.Parent = this.root;
				this.RegisterGameObjectAndChildren(child);
			}
		}

		public void		SetLayers(string[] layers)
		{
			// Shrink to the bare necessary.
			for (int i = ServerSendLayersPacket.MaxLayers - 1; i >= 0; --i)
			{
				if (string.IsNullOrEmpty(layers[i]) == false)
				{
					this.Layers = new string[i + 1];

					for (; i >= 0; --i)
						this.Layers[i] = layers[i];

					break;
				}
			}

			this.ready |= HierarchyReady.Layers;
		}

		public void		SetResources(Type type, string[] resourceNames, int[] instanceIDs)
		{
			if (this.resourceNames.ContainsKey(type) == true)
			{
				this.resourceNames[type] = resourceNames;
				this.resourceInstanceIds[type] = instanceIDs;
			}
			else
			{
				this.resourceNames.Add(type, resourceNames);
				this.resourceInstanceIds.Add(type, instanceIDs);
			}

			ResourcesPickerWindow[]	pickers = Resources.FindObjectsOfTypeAll<ResourcesPickerWindow>();

			for (int i = 0; i < pickers.Length; i++)
				pickers[i].Repaint();
		}

		public ClientGameObject	GetGameObject(int instanceID)
		{
			ClientGameObject	n = null;

			this.gameObjectInstanceIDs.TryGetValue(instanceID, out n);
			return n;
		}

		public void		SelectGameObject(int instanceID)
		{
			ClientGameObject	n = this.GetGameObject(instanceID);

			if (n != null)
			{
				this.root.ClearSelection();
				n.Selected = true;

				NGRemoteInspectorWindow[]	inspectors = EditorWindow.FindObjectsOfType<NGRemoteInspectorWindow>();

				for (int i = 0; i < inspectors.Length; i++)
					inspectors[i].Repaint();

				this.Repaint();
			}
		}

		public ClientGameObject[]	GetSelectedGameObjects()
		{
			if (this.root == null || this.root.HasSelection == false)
				return ClientGameObject.EmptyGameObjectArray;

			List<ClientGameObject>	selection = new List<ClientGameObject>();

			this.BrowseSelected(this.root, selection);

			return selection.ToArray();
		}

		public bool		SetSibling(int instanceID, int instanceIDParent, int siblingIndex)
		{
			ClientGameObject	a;
			ClientGameObject	b = null;

			if (this.gameObjectInstanceIDs.TryGetValue(instanceID, out a) == true &&
				(instanceIDParent == -1 || this.gameObjectInstanceIDs.TryGetValue(instanceIDParent, out b) == true))
			{
				if (instanceIDParent == -1)
					a.Parent = this.root;
				else
					a.Parent = b;

				a.SetSiblingIndex(siblingIndex);
			}
			else
			{
				InternalNGDebug.Log(Errors.Scene_GameObjectNotFound, "GameObject (" + instanceID + " or " + instanceIDParent + ") was not found.");
				return false;
			}

			return true;
		}

		private int		GetGameObjectHeight(ClientGameObject gameObject, ClientGameObject stopChild)
		{
			int	height = 1;

			for (int i = 0; i < gameObject.children.Count; i++)
			{
				if (gameObject.children[i] == stopChild)
					break;

				if (gameObject.children[i].fold == true)
					height += this.GetGameObjectHeight(gameObject.children[i], null);
				else
					++height;
			}

			return height;
		}

		public void		PingObject(int gameObjectInstanceId)
		{
			if (gameObjectInstanceId != 0)
			{
				ClientGameObject	gameObject = this.GetGameObject(gameObjectInstanceId);

				if (gameObject != null)
				{
					int	position = 0;

					if (gameObject.Parent != null)
					{
						while (gameObject.Parent != null)
						{
							gameObject.Parent.fold = true;

							position += this.GetGameObjectHeight(gameObject.Parent, gameObject);

							gameObject = gameObject.Parent;
						}
					}
					else
						position = gameObject.GetSiblingIndex();

					pings.Add(new KeyValuePair<AnimFloat, int>(new AnimFloat(1F, this.Repaint), gameObjectInstanceId));
					pings[pings.Count - 1].Key.target = 0F;

					this.Repaint();
				}
			}
		}

		/// <summary>Opens the window to pick an Unity Object in a list.</summary>
		/// <param name="type">Type inheriting from UnityEngine.Object.</param>
		/// <param name="valuePath">The path of the value to update.</param>
		/// <param name="packetGenerator">A callback that will create the adequate Packet using value's path and the new value as byte[].</param>
		public void		PickupResource(Type type, string valuePath, Func<string, byte[], Packet> packetGenerator, int initialInstanceID)
		{
			ResourcesPickerWindow.Init(this, type, valuePath, packetGenerator, initialInstanceID);

			this.LoadResources(type);
		}

		/// <summary>
		/// Requests resources from server if not loaded yet. You can use it to prewarm.
		/// </summary>
		/// <param name="type"></param>
		public void		LoadResources(Type type)
		{
			if (this.resourceNames.ContainsKey(type) == false &&
				this.BlockRequestChannel(type.GetHashCode()) == true)
			{
				this.Client.AddPacket(new ClientRequestResourcesPacket(type));
			}
		}

		public string	GetResourceName(Type type, int instanceID)
		{
			int[]	IDs;

			if (this.resourceInstanceIds.TryGetValue(type, out IDs) == true)
			{
				for (int i = 0; i < IDs.Length; i++)
				{
					if (IDs[i] == instanceID)
						return this.resourceNames[type][i];
				}
			}
			else if (this.BlockRequestChannel(type.GetHashCode()) == true)
				this.Client.AddPacket(new ClientRequestResourcesPacket(type));

			return null;
		}

		/// <summary>
		/// Gets resources' name and instanceID if they are available now. Loads resources if they are missing.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="resourceNames"></param>
		/// <param name="instanceIDs"></param>
		public void		GetResources(Type type, out string[] resourceNames, out int[] instanceIDs)
		{
			this.LoadResources(type);

			if (this.resourceNames.TryGetValue(type, out resourceNames) == true)
				this.resourceInstanceIds.TryGetValue(type, out instanceIDs);
			else
				instanceIDs = null;
		}

		public void		SetEnumData(string type, bool hasAttributeValue, string[] names, int[] values)
		{
			this.enumData.Add(type, new EnumData(hasAttributeValue, names, values));
		}

		public EnumData	GetEnumData(string type)
		{
			EnumData	enumData = null;

			if (this.enumData.TryGetValue(type, out enumData) == false)
			{
				if (this.BlockRequestChannel(type.GetHashCode()) == true)
					this.Client.AddPacket(new ClientRequestEnumDataPacket(type));
			}

			return enumData;
		}

		public void		DeleteGameObject(int instanceID)
		{
			ClientGameObject	go = this.GetGameObject(instanceID);

			if (go != null)
			{
				go.Destroy();
				this.gameObjectInstanceIDs.Remove(instanceID);
			}

			this.Repaint();
		}

		public void		DeleteGameObjects(List<int> instanceIDs)
		{
			for (int i = 0; i < instanceIDs.Count; i++)
				this.DeleteGameObject(instanceIDs[i]);
		}

		public void		AddComponent(int instanceID, NetComponent netComponent)
		{
			ClientGameObject	go = this.GetGameObject(instanceID);

			if (go != null)
				go.AddComponent(netComponent);
		}

		public void		DeleteComponents(List<int> gameObjectInstanceIDs, List<int> instanceIDs)
		{
			for (int i = 0; i < gameObjectInstanceIDs.Count; i++)
			{
				ClientGameObject	go = this.GetGameObject(gameObjectInstanceIDs[i]);

				if (go != null)
					go.RemoveComponent(instanceIDs[i]);
			}
		}

		public string	GetGameObjectName(int instanceID)
		{
			ClientGameObject	go = this.GetGameObject(instanceID);

			if (go != null)
				return go.name;
			return string.Empty;
		}

		public string	GetBehaviourName(int gameObjectInstanceID, int instanceID)
		{
			ClientGameObject	go = this.GetGameObject(gameObjectInstanceID);

			if (go != null)
			{
				ClientComponent	b = go.GetComponent(instanceID);

				if (b != null)
					return b.name;
			}

			return string.Empty;
		}

		public void		WatchGameObject(NGRemoteInspectorWindow inspector, ClientGameObject gameObject)
		{
			if (gameObject == null)
			{
				if (this.watchersGameObject.ContainsKey(inspector) == true)
					this.watchersGameObject.Remove(inspector);
			}
			else
			{
				if (this.watchersGameObject.ContainsKey(inspector) == false)
					this.watchersGameObject.Add(inspector, gameObject.instanceID);
				else
					this.watchersGameObject[inspector] = gameObject.instanceID;
			}

			int[]	instanceIDs = new int[this.watchersGameObject.Count];

			this.watchersGameObject.Values.CopyTo(instanceIDs, 0);

			this.Client.AddPacket(new ClientWatchGameObjectsPacket(instanceIDs));
		}

		public void		WatchMaterials(NGRemoteInspectorWindow inspector, List<int> materialIDs)
		{
			if (materialIDs == null || materialIDs.Count == 0)
			{
				if (this.watchersMaterials.ContainsKey(inspector) == true)
					this.watchersMaterials.Remove(inspector);
			}
			else
			{
				if (this.watchersMaterials.ContainsKey(inspector) == false)
					this.watchersMaterials.Add(inspector, materialIDs.ToArray());
				else
					this.watchersMaterials[inspector] = materialIDs.ToArray();
			}

			materialIDs.Clear();

			foreach (var item in this.watchersMaterials)
			{
				for (int i = 0; i < item.Value.Length; i++)
				{
					if (materialIDs.Contains(item.Value[i]) == false)
						materialIDs.Add(item.Value[i]);
				}
			}

			this.Client.AddPacket(new ClientWatchMaterialsPacket(materialIDs.ToArray()));
		}

		public bool		GetUpdateNotification(string valuePath)
		{
			for (int i = 0; i < this.updateValueNotifications.Count; i++)
			{
				if (this.updateValueNotifications[i].Value.Equals(valuePath) == true)
				{
					this.updateValueNotifications.RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		/// <summary>Resolves the path and updates the value with <paramref name="rawValue"/>.</summary>
		/// <param name="valuePath"></param>
		/// <param name="rawValue"></param>
		/// <remarks>See <see cref="NGConsoleWindow.ServerSceneManager.UpdateFieldValue" /> for the server equivalent.</remarks>
		/// <exception cref="System.MissingFieldException">Thrown when an unknown field from ClientGameObject is being assigned.</exception>
		/// <exception cref="System.InvalidCastException">Thrown when an array is assigned but the array is not supported.</exception>
		/// <exception cref="System.ArgumentException">Thrown when the path seems to be not resolvable.</exception>
		public void		UpdateFieldValue(string valuePath, byte[] rawValue)
		{
			string[]	paths = valuePath.Split('.');

			try
			{
				ClientGameObject	go = this.GetGameObject(int.Parse(paths[0]));

				if (go == null)
				{
					InternalNGDebug.Log(Errors.Scene_ComponentNotFound, "GameObject (" + paths[0] + ") was not found.");
					return;
				}

				int	instanceID;

				// Is a field for ClientGameObject.
				if (int.TryParse(paths[1], out instanceID) == false)
				{
					// No time to waste on abstracting ClientGameObject, maybe later.
					switch (paths[1])
					{
						case "tag":
						case "name":
							TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(typeof(string));

							NGRemoteHierarchyWindow.Buffer.Clear();
							NGRemoteHierarchyWindow.Buffer.Append(rawValue);

							if (paths[1] == "tag")
								go.tag = typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, typeof(string)) as string;
							else if (paths[1] == "name")
								go.name = typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, typeof(string)) as string;
							break;

						case "active":
						case "isStatic":
							typeHandler = TypeHandlersManager.GetTypeHandler(typeof(bool));

							NGRemoteHierarchyWindow.Buffer.Clear();
							NGRemoteHierarchyWindow.Buffer.Append(rawValue);

							if (paths[1] == "active")
								go.active = (bool)typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, typeof(bool));
							else if (paths[1] == "isStatic")
								go.isStatic = (bool)typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, typeof(bool));
							break;

						case "layer":
							typeHandler = TypeHandlersManager.GetTypeHandler(typeof(int));

							NGRemoteHierarchyWindow.Buffer.Clear();
							NGRemoteHierarchyWindow.Buffer.Append(rawValue);

							go.layer = (int)typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, typeof(int));
							break;

						default:
							throw new MissingFieldException(valuePath);
					}

					this.AddUpdateNotification(valuePath);
					return;
				}

				ClientComponent	b = go.GetComponent(instanceID);
				if (b == null)
				{
					InternalNGDebug.Log(Errors.Scene_ComponentNotFound, "Component (" + paths[1] + ") was not found.");
					return;
				}

				ClientField	f = b.GetField(paths[2]);
				if (f == null)
				{
					InternalNGDebug.Log(Errors.Scene_PathNotResolved, "Path (" + valuePath + ") is invalid.");
					return;
				}

				this.valuePath = valuePath;
				this.paths = paths;
				this.rawValue = rawValue;

				object	workingObject = f;
				Type	field = f.fieldType;

				this.SetValue(workingObject, paths, 2, this.Resolve(field, workingObject, 2));

				this.AddUpdateNotification(this.valuePath);
			}
			catch (Exception ex)
			{
				InternalNGDebug.Log(Errors.Scene_Exception, "Path="+ this.valuePath + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		/// <summary></summary>
		/// <param name="instanceID"></param>
		/// <param name="propertyName"></param>
		/// <param name="rawValue"></param>
		/// <exception cref="UnityEngine.MissingFieldException">Thrown when the property was not found in the material.</exception>
		public void		UpdateMaterialProperty(int instanceID, string propertyName, byte[] rawValue)
		{
			NetMaterial	material;

			if (this.materials.TryGetValue(instanceID, out material) == true)
			{
				for (int i = 0; i < material.properties.Length; i++)
				{
					if (material.properties[i].name.Equals(propertyName) == true)
					{
						Type		type = null;
						TypeHandler	typeHandler;

						if (material.properties[i].type == NGShader.ShaderPropertyType.Color)
							type = typeof(Color);
						else if (material.properties[i].type == NGShader.ShaderPropertyType.Float ||
								 material.properties[i].type == NGShader.ShaderPropertyType.Range)
							type = typeof(float);
						else if (material.properties[i].type == NGShader.ShaderPropertyType.TexEnv)
							type = typeof(Texture);
						else if (material.properties[i].type == NGShader.ShaderPropertyType.Vector)
							type = typeof(Vector4);

						typeHandler = TypeHandlersManager.GetTypeHandler(type);
						InternalNGDebug.Assert(typeHandler != null, "TypeHandler for " + material.properties[i].name + " is not supported.");

						ByteBuffer	buffer = Utility.GetBBuffer(rawValue);

						if (material.properties[i].type == NGShader.ShaderPropertyType.Color)
							material.properties[i].colorValue = (Color)typeHandler.Deserialize(buffer, type);
						else if (material.properties[i].type == NGShader.ShaderPropertyType.Float ||
								 material.properties[i].type == NGShader.ShaderPropertyType.Range)
						{
							material.properties[i].floatValue = (float)typeHandler.Deserialize(buffer, type);
						}
						else if (material.properties[i].type == NGShader.ShaderPropertyType.TexEnv)
							material.properties[i].textureValue = (UnityObject)typeHandler.Deserialize(buffer, type);
						else if (material.properties[i].type == NGShader.ShaderPropertyType.Vector)
							material.properties[i].vectorValue = (Vector4)typeHandler.Deserialize(buffer, type);

						Utility.RestoreBBuffer(buffer);

						return;
					}
				}
			}

			throw new MissingFieldException("Material " + instanceID + " does not contain property \"" + propertyName + "\".");
		}

		/// <summary></summary>
		/// <param name="instanceID"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <exception cref="UnityEngine.MissingFieldException">Thrown when the property was not found in the material.</exception>
		public void		UpdateMaterialVector2(int instanceID, string propertyName, Vector2 value, MaterialVector2Type type)
		{
			NetMaterial	material;

			if (this.materials.TryGetValue(instanceID, out material) == true)
			{
				for (int i = 0; i < material.properties.Length; i++)
				{
					if (material.properties[i].name.Equals(propertyName) == true)
					{
						if (type == MaterialVector2Type.Offset)
							material.properties[i].textureOffset = value;
						else if (type == MaterialVector2Type.Scale)
							material.properties[i].textureScale = value;
						return;
					}
				}
			}

			throw new MissingFieldException("Material " + instanceID + " does not contain property \"" + propertyName + "\".");
		}

		public NetMaterial	GetMaterial(int instanceID)
		{
			NetMaterial	material = null;

			if (this.materials.TryGetValue(instanceID, out material) == false &&
				this.BlockRequestChannel(instanceID) == true)
			{
				this.Client.AddPacket(new ClientRequestMaterialDataPacket(instanceID));
			}

			return material;
		}

		public void		CreateMaterialData(NetMaterial netMaterial)
		{
			NetMaterial	material;

			if (this.materials.TryGetValue(netMaterial.instanceID, out material) == false)
				this.materials.Add(netMaterial.instanceID, netMaterial);
			else
				this.materials[netMaterial.instanceID] = netMaterial;
		}

		public void		LoadBigArray(string arrayPath)
		{
			if (this.BlockRequestChannel(arrayPath.GetHashCode()) == true)
				this.Client.AddPacket(new ClientLoadBigArrayPacket(arrayPath));
		}

		public void		SetInvokeResult(string result)
		{
			this.LastInvokeResult = result;
			this.LastInvokeResultTime = DateTime.Now;
		}

#if NGTOOLS_FREE
		private bool	notifyOnce = false;
#endif
		/// <summary>
		/// Adds a packet into the client's queue only if pro version.
		/// </summary>
		/// <param name="packet"></param>
		public void		AddPacket(Packet packet)
		{
#if NGTOOLS_FREE
			if (this.notifyOnce == false)
			{
				this.notifyOnce = true;
				EditorUtility.DisplayDialog(Constants.PackageTitle, "NG Remote Scene is exclusive to NG Tools Pro.\n\nThe free version is read-only.\n\nIt allows to see everything but you can only toggle the GameObject's active state.", "OK");
			}
#else
			this.Client.AddPacket(packet);
#endif
		}

		private void	AddUpdateNotification(string valuePath)
		{
			for (int i = 0; i < this.updateValueNotifications.Count; i++)
			{
				if (this.updateValueNotifications[i].Value.Equals(valuePath) == true)
					return;
			}

			this.updateValueNotifications.Add(new KeyValuePair<float, string>(0, valuePath));
		}

		private object	GetValue(object instance, string fieldName)
		{
			ClientField	field = instance as ClientField;

			if (field != null)
				return field.value;

			GenericClass	genericClass = instance as GenericClass;

			if (genericClass != null)
				return genericClass.GetValue(fieldName);

			FieldInfo	fieldInfo = instance.GetType().GetField(fieldName, InnerUtility.ExposedBindingFlags);

			if (fieldInfo != null)
				return fieldInfo.GetValue(instance);

			PropertyInfo	propertyInfo = instance.GetType().GetProperty(fieldName, InnerUtility.ExposedBindingFlags);

			if (propertyInfo != null)
				return propertyInfo.GetValue(instance, null);

			throw new Exception();
		}

		private object	SetValue(object instance, string[] paths, int i, object value)
		{
			ClientField	field = instance as ClientField;

			if (field != null)
			{
				field.value = value;
				return field;
			}

			GenericClass	genericClass = instance as GenericClass;

			if (genericClass != null)
			{
				genericClass.SetValue(paths[i], value);
				return genericClass;
			}

			FieldInfo	fieldInfo = instance.GetType().GetField(paths[i], InnerUtility.ExposedBindingFlags);

			if (fieldInfo != null)
			{
				fieldInfo.SetValue(instance, value);
				return instance;
			}

			PropertyInfo	propertyInfo = instance.GetType().GetProperty(paths[i], InnerUtility.ExposedBindingFlags);

			if (propertyInfo != null)
			{
				propertyInfo.SetValue(instance, value, null);
				return instance;
			}

			throw new Exception();
		}

		private	object	NextPath(object instance, string path)
		{
			ClientField	field = instance as ClientField;

			if (field != null)
				return field.value;

			GenericClass	genericClass = instance as GenericClass;

			if (genericClass != null)
				return genericClass.GetValue(path);

			FieldInfo	fieldInfo = instance.GetType().GetField(path, InnerUtility.ExposedBindingFlags);

			if (fieldInfo != null)
				return fieldInfo.GetValue(instance);

			PropertyInfo	propertyInfo = instance.GetType().GetProperty(path, InnerUtility.ExposedBindingFlags);

			if (propertyInfo != null)
				return propertyInfo.GetValue(instance, null);

			throw new Exception("Part \"" + path + "\" in \"" + string.Join(".", paths) + "\" is not in " + instance + ".");
		}

		private Type	GetPathType(object instance, string[] paths, int i)
		{
			ClientField	field = instance as ClientField;

			if (field != null)
				return field.fieldType;

			GenericClass	genericClass = instance as GenericClass;

			if (genericClass != null)
				return genericClass.GetType(paths[i]);

			FieldInfo	fieldInfo = instance.GetType().GetField(paths[i], InnerUtility.ExposedBindingFlags);

			if (fieldInfo != null)
				return fieldInfo.FieldType;

			PropertyInfo	propertyInfo = instance.GetType().GetProperty(paths[i], InnerUtility.ExposedBindingFlags);

			if (propertyInfo != null)
				return propertyInfo.PropertyType;

			throw new Exception("Path \"" + string.Join(".", paths) + "\" at " + i + " is not " + instance + ".");
		}

		private void	BrowseSelected(ClientGameObject n, List<ClientGameObject> selection)
		{
			if (n.Selected == true)
				selection.Add(n);

			for (int i = 0; i < n.children.Count; i++)
			{
				if (n.children[i].HasSelection == true)
					this.BrowseSelected(n.children[i], selection);
			}
		}

		private ClientGameObject	GetFirstSelected()
		{
			ClientGameObject	n = this.root;

			rewind:
			for (int i = 0; i < n.children.Count; i++)
			{
				if (n.children[i].HasSelection == true)
				{
					if (n.children[i].Selected == true)
						return n.children[i];

					n = n.children[i];
					goto rewind;
				}
			}

			if (n == this.root)
				return null;

			return n;
		}

		private ClientGameObject	GetLastSelected()
		{
			ClientGameObject	n = this.root;

			rewind:
			for (int i = n.children.Count - 1; i >= 0; --i)
			{
				if (n.children[i].HasSelection == true)
				{
					n = n.children[i];
					goto rewind;
				}
			}

			if (n == this.root)
				return null;

			return n;
		}

		/// <summary></summary>
		/// <param name="r"></param>
		/// <param name="p"></param>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		private Rect	DrawGameObject(Rect r, ref int p, ClientGameObject gameObject, bool hideChildren = false)
		{
			if (Event.current.type == EventType.Repaint)
			{
				for (int i = 0; i < this.pings.Count; i++)
				{
					if (this.pings[i].Value == gameObject.instanceID)
					{
						if (this.pings[i].Key.value > 0F)
						{
							float	x = r.x;
							float	width = r.width;

							r.x = 0F;
							r.width = this.position.width;
							EditorGUI.DrawRect(r, new Color(.3F, this.pings[i].Key.value, .3F, .5F));
							r.x = x;
							r.width = width;
						}
						else
						{
							this.pings.RemoveAt(i);
							--i;
							this.Repaint();
						}
					}
				}
			}

			if (gameObject.Selected == true)
			{
				float	originX = r.x;

				r.x = 0F;
				r.width += originX;
				EditorGUI.DrawRect(r, NGRemoteHierarchyWindow.SelectedGameObjectBackgroundColor);
				r.width -= originX;
				r.x = originX;
			}

			if (hideChildren == false && gameObject.children.Count > 0)
			{
				float	originWidth = r.width;

				r.width = 16F;
				gameObject.fold = EditorGUI.Foldout(r, gameObject.fold, GUIContent.none);
				r.width = originWidth;
			}

			if (r.Contains(Event.current.mousePosition) == true)
			{
				if (Event.current.type == EventType.MouseUp)
				{
					if (Event.current.control == false)
					{
						this.root.ClearSelection();
						gameObject.Selected = true;

						Event.current.Use();
					}
				}
				else if (Event.current.type == EventType.MouseDown)
				{
					if (Event.current.control == true)
						gameObject.Selected = !gameObject.Selected;

					this.dragOriginPosition = Event.current.mousePosition;

					GUI.FocusControl(null);

					// Initialize drag data.
					DragAndDrop.PrepareStartDrag();

					UnityObject	unityObject = new UnityObject(typeof(GameObject), gameObject.instanceID);

					DragAndDrop.SetGenericData("r", unityObject);
					DragAndDrop.SetGenericData("p", p);

					Event.current.Use();
				}

				if (DragAndDrop.GetGenericData("r") is UnityObject &&
					DragAndDrop.GetGenericData("p") is int)
				{
					if (Event.current.type == EventType.MouseDrag && (this.dragOriginPosition - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance)
					{
						DragAndDrop.StartDrag("Dragging Game Object");
						Event.current.Use();
					}
					else if (Event.current.type == EventType.DragUpdated)
					{
						if ((DragAndDrop.GetGenericData("r") as UnityObject).instanceID != gameObject.instanceID)
							DragAndDrop.visualMode = DragAndDropVisualMode.Move;
						else
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

						Event.current.Use();
					}
					else if (Event.current.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();

						UnityObject			dragItem = DragAndDrop.GetGenericData("r") as UnityObject;
						ClientGameObject	dragGameObject = this.GetGameObject(dragItem.instanceID);

						// Top
						if (Event.current.mousePosition.y - r.y <= 3F)
						{
							// Prevent dragging around itself.
							if (dragGameObject.Parent != gameObject.Parent || dragGameObject.GetSiblingIndex() != gameObject.GetSiblingIndex() - 1)
							{
								int		dragPosition = (int)DragAndDrop.GetGenericData("p");
								int		instanceIDParent = (gameObject.Parent != this.root) ? gameObject.Parent.instanceID : -1;
								Packet	packet = new ClientSetSiblingPacket(dragItem.instanceID, instanceIDParent, gameObject.GetSiblingIndex() - (dragPosition < p && dragGameObject.Parent == gameObject.Parent ? 1 : 0));

								this.Client.AddPacket(packet);
							}
						}
						// Bottom
						else if (Event.current.mousePosition.y - r.y >= 13F)
						{
							// Prevent dragging around itself.
							if (dragGameObject.Parent != gameObject.Parent || dragGameObject.GetSiblingIndex() != gameObject.GetSiblingIndex() + 1)
							{
								int		dragPosition = (int)DragAndDrop.GetGenericData("p");
								int		instanceIDParent = (gameObject.Parent != this.root) ? gameObject.Parent.instanceID : -1;
								Packet	packet = new ClientSetSiblingPacket(dragItem.instanceID, instanceIDParent, gameObject.GetSiblingIndex() + (dragPosition < p && dragGameObject.Parent == gameObject.Parent ? 0 : 1));

								this.Client.AddPacket(packet);
							}
						}
						else
						{
							Packet	packet = new ClientSetSiblingPacket(dragItem.instanceID, gameObject.instanceID, int.MaxValue);
							this.Client.AddPacket(packet);
						}
					}
					else if (Event.current.type == EventType.Repaint &&
							 DragAndDrop.visualMode == DragAndDropVisualMode.Move)
					{
						Rect	r2 = r;

						r2.width += r2.x;
						r2.x = 0F;

						// Top
						if (Event.current.mousePosition.y - r.y <= 3F)
						{
							r2.height = 2F;
							r2.y -= 1F;
							EditorGUI.DrawRect(r2, NGRemoteHierarchyWindow.DropBackgroundColor);
						}
						// Bottom
						else if (Event.current.mousePosition.y - r.y >= 13F)
						{
							r2.height = 2F;
							r2.y += 15F;
							EditorGUI.DrawRect(r2, NGRemoteHierarchyWindow.DropBackgroundColor);
						}
						else
						{
							EditorGUI.DrawRect(r2, NGRemoteHierarchyWindow.DropBackgroundColor);
						}
					}
				}
			}

			r.x += 16F;
			r.width -= r.x;
			EditorGUI.BeginDisabledGroup(gameObject.active == false);
			EditorGUI.LabelField(r, gameObject.name);
			EditorGUI.EndDisabledGroup();
			r.width += r.x;
			r.x -= 16F;

			r.y += r.height;

			if (hideChildren == false && gameObject.fold == true && gameObject.children.Count > 0)
			{
				r.x += 16F;
				r.width -= 16F;
				for (int i = 0; i < gameObject.children.Count; i++)
				{
					++p;
					r = this.DrawGameObject(r, ref p, gameObject.children[i]);

					if (r.y - this.scrollPosition.y > this.bodyRect.height)
						return r;
				}
				r.width += 16F;
				r.x -= 16F;
			}

			return r;
		}

		private IEnumerator	DelayDiscAndClean(Client client)
		{
			double	time = EditorApplication.timeSinceStartup + 10D; // Give　time to send last packets before closing the client.

			this.root = null;
			this.Layers = null;
			this.ready = 0;
			this.blockedRequestChannels.Clear();
			this.gameObjectInstanceIDs.Clear();
			this.resourceNames.Clear();
			this.resourceInstanceIds.Clear();
			this.watchersGameObject.Clear();

			if (this.HierarchyDisconnected != null)
				this.HierarchyDisconnected();

			this.Repaint();

			while (client.tcpClient.Connected == true && client.PendingPacketsCount > 0 && time > EditorApplication.timeSinceStartup)
				yield return null;

			// HACK Calling Close() seems to block the instance, even when manually closing the inner stream.
			client.Close();

			this.Repaint();
		}

		//private bool	DetectClientDisced(Client client)
		//{
		//	if (client.tcpClient.Connected == false)
		//		return true;

		//	try
		//	{
		//		if (client.tcpClient.Client.Poll(0, SelectMode.SelectRead) == true &&
		//			client.tcpClient.Client.Receive(pollDiscBuffer, SocketFlags.Peek) == 0)
		//		{
		//			return true;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		InternalNGDebug.Log(Errors.Scene_Exception, ex.Message + Environment.NewLine + ex.StackTrace);
		//	}

		//	return false;
		//}

		private void	SelectPrevious()
		{
			ClientGameObject	node = this.GetFirstSelected();
			ClientGameObject	cursor = node;

			this.root.ClearSelection();

			int	p = cursor.Parent.children.IndexOf(cursor);

			if (p == 0)
			{
				if (cursor.Parent == this.root)
				{
					cursor.Selected = true;
					return;
				}

				cursor.Parent.Selected = true;
			}
			else
			{
				cursor.Parent.children[p - 1].Selected = true;
			}
		}

		private void	SelectNext()
		{
			ClientGameObject	node = this.GetLastSelected();
			ClientGameObject	cursor = node;

			this.root.ClearSelection();

			if (cursor.fold == true && cursor.children.Count > 0)
			{
				cursor.children[0].Selected = true;
				return;
			}

			parent:
			int	p = cursor.Parent.children.IndexOf(cursor);

			if (p == cursor.Parent.children.Count - 1)
			{
				if (cursor.Parent.Parent != null)
				{
					cursor = cursor.Parent;
					goto parent;
				}
				else
					node.Selected = true;
			}
			else
			{
				cursor.Parent.children[p + 1].Selected = true;
			}
		}

		private void	RegisterGameObjectAndChildren(ClientGameObject node)
		{
			InternalNGDebug.Assert(this.gameObjectInstanceIDs.ContainsKey(node.instanceID) == false, "Registering GameObject " + node.instanceID + ", but already registered.");

			this.gameObjectInstanceIDs.Add(node.instanceID, node);

			for (int i = 0; i < node.children.Count; i++)
				this.RegisterGameObjectAndChildren(node.children[i]);
		}

		private object	Resolve(Type type, object instance, int i)
		{
			if (i == this.paths.Length - 1)
			{
				// If an array resize.
				if (type.IsUnityArray() == true)
				{
					NGRemoteHierarchyWindow.Buffer.Clear();
					NGRemoteHierarchyWindow.Buffer.Append(rawValue);

					TypeHandler	typeHandler;
					ArrayData	array = this.NextPath(instance, paths[i]) as ArrayData;
					Type		subType = Utility.GetArraySubType(array.type);

					if (array.array == null)
					{
						typeHandler = TypeHandlersManager.GetTypeHandler(array.type);
						InternalNGDebug.Assert(typeHandler != null, "TypeHandler for type " + array.type + " does not exist.");
						ArrayData	newArray = typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, array.type) as ArrayData;

						array.array = newArray.array;

						return array;
					}

					typeHandler = TypeHandlersManager.GetTypeHandler(typeof(int));
					int	newSize = (int)typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, typeof(int));
					if (array.array.Length != newSize)
					{
						Array	newArray = Array.CreateInstance(subType, newSize);
						int		k = 0;

						for (; k < newSize && k < array.array.Length; k++)
							newArray.SetValue(array.array.GetValue(k), k);

						this.SetValue(instance, paths, i, newArray);

						array.array = newArray;
					}

					return array;
				}
				else
				{
					TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(type);
					InternalNGDebug.Assert(typeHandler != null, "TypeHandler for type " + type + " does not exist.");

					NGRemoteHierarchyWindow.Buffer.Clear();
					NGRemoteHierarchyWindow.Buffer.Append(rawValue);

					object	currentValue = this.NextPath(instance, paths[i]);
					object	v = typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, type);

					GenericClass	currentGenericClass = currentValue as GenericClass;

					if (currentGenericClass != null)
					{
						GenericClass	newGenericClass = v as GenericClass;

						currentGenericClass.SetAll(newGenericClass.names, newGenericClass.types, newGenericClass.values);
					}
					else if (currentValue.GetType() != typeof(string) &&
							 (currentValue.GetType().IsClass == true ||
							  currentValue.GetType().IsStruct() == true))
					{
						ComponentExposer[]	exposers = ComponentExposersManager.GetComponentExposers(type);
						FieldInfo[]			fields = InnerUtility.GetExposedFields(currentValue.GetType(), exposers);
						PropertyInfo[]		properties = InnerUtility.GetExposedProperties(currentValue.GetType(), exposers);

						for (int j = 0; j < fields.Length; j++)
							fields[j].SetValue(currentValue, fields[j].GetValue(v));

						for (int j = 0; j < properties.Length; j++)
							properties[j].SetValue(currentValue, properties[j].GetValue(v, null), null);
					}
					else
						currentValue = v;

					return currentValue;
				}
			}
			else if (type.IsUnityArray() == true) // Any Array or IList
			{
				ArrayData	array = this.NextPath(instance, paths[i]) as ArrayData;
				Type		subType = Utility.GetArraySubType(array.type);
				int			index = int.Parse(paths[i + 1]);

				if (array.array.Length > index)
					array.array.SetValue(this.ResolveArray(subType, array.array.GetValue(index), i + 1), index);

				return array;
			}
			else if (type.IsClass == true || // Any class.
					 type.IsStruct() == true) // Any struct.
			{
				object	subInstance = this.NextPath(instance, paths[i]);

				type = this.GetPathType(subInstance, paths, i + 1);

				return this.SetValue(subInstance, paths, i + 1, this.Resolve(type, subInstance, i + 1));
			}

			throw new InvalidOperationException("Resolve failed at " + i + " with " + instance + " of type " + type + ".");
		}

		private object	ResolveArray(Type type, object instance, int i)
		{
			if (i == this.paths.Length - 1)
			{
				TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(type);
				InternalNGDebug.Assert(typeHandler != null, "TypeHandler for type " + type + " does not exist.");

				NGRemoteHierarchyWindow.Buffer.Clear();
				NGRemoteHierarchyWindow.Buffer.Append(rawValue);

				return typeHandler.Deserialize(NGRemoteHierarchyWindow.Buffer, type);
			}

			if (type.IsClass == true || // Any class.
				type.IsStruct() == true) // Any struct.
			{
				type = this.GetPathType(instance, paths, i + 1);

				return this.SetValue(instance, paths, i + 1, this.Resolve(type, instance, i + 1));
			}

			return this.Resolve(this.GetPathType(instance, paths, i + 1), instance, i + 1);
		}

		/// <summary>
		/// Gets the height of a GameObject including its children.
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		private float	GetGameObjectHeight(ClientGameObject gameObject)
		{
			float	height = EditorGUIUtility.singleLineHeight;

			if (gameObject.fold == true)
			{
				for (int i = 0; i < gameObject.children.Count; i++)
					height += this.GetGameObjectHeight(gameObject.children[i]);
			}

			return height;
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

		public void	UnblockRequestChannel(int id)
		{
			for (int i = 0; i < this.blockedRequestChannels.Count; i++)
			{
				if (this.blockedRequestChannels[i].Value <= Time.realtimeSinceStartup ||
					this.blockedRequestChannels[i].Key == id)
				{
					this.blockedRequestChannels.RemoveAt(i);
					--i;
				}
			}
		}

		/// <summary>
		/// Checks if a channel is free to use. Use it to prevent sending many times the same packets, like connection, requesting resources or else. Refer to NGHierarchyWindow.BlockRequestLifeTime for the lifetime.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool	BlockRequestChannel(int id)
		{
			for (int i = 0; i < this.blockedRequestChannels.Count; i++)
			{
				if (this.blockedRequestChannels[i].Value <= Time.realtimeSinceStartup)
				{
					if (this.blockedRequestChannels[i].Key == id)
					{
						this.blockedRequestChannels.RemoveAt(i);
						return true;
					}

					this.blockedRequestChannels.RemoveAt(i);
					--i;
					continue;
				}

				if (this.blockedRequestChannels[i].Key == id)
					return false;
			}

			this.blockedRequestChannels.Add(new KeyValuePair<int, float>(id, Time.realtimeSinceStartup + NGRemoteHierarchyWindow.BlockRequestLifeTime));

			return true;
		}

		private void	UpdateFilteredIndexes()
		{
			this.filteredGameObjects.Clear();

			string[]	patterns = this.searchPattern.Split(' ');

			foreach (var item in this.gameObjectInstanceIDs)
			{
				int	i = 0;

				for (; i < patterns.Length; i++)
				{
					if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(item.Value.name, patterns[i], CompareOptions.IgnoreCase) < 0)
						break;
				}

				if (i == patterns.Length)
					this.filteredGameObjects.Add(item.Value);
			}
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			NGRemoteHierarchyWindow.AddTabMenus(menu);
			menu.AddSeparator("");
			Utility.AddNGMenuItems(menu, this, NGRemoteHierarchyWindow.NormalTitle, Constants.WikiBaseURL + "#markdown-header-131-ng-remote-hierarchy");

			if (Conf.DebugMode != Conf.DebugModes.None && this.Client != null)
				menu.AddItem(new GUIContent("Monitor Client"), false, this.OpenMonitorClientWindow);
		}

		private void	OpenMonitorClientWindow()
		{
			MonitorClientPacketsWindow.Open(this.Client);
		}

		public static void	AddTabMenus(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Add Tab/" + NGRemoteInspectorWindow.NormalTitle), false, new GenericMenu.MenuFunction(NGRemoteHierarchyWindow.AddTabInspector));
			menu.AddItem(new GUIContent("Add Tab/" + NGRemoteProjectWindow.NormalTitle), false, new GenericMenu.MenuFunction(NGRemoteHierarchyWindow.AddTabProject));
			menu.AddItem(new GUIContent("Add Tab/" + NGRemoteCameraWindow.NormalTitle), false, new GenericMenu.MenuFunction(NGRemoteHierarchyWindow.AddTabCamera));
			menu.AddSeparator("");
		}

		private static void	AddTabInspector()
		{
			NGRemoteInspectorWindow	inspector = EditorWindow.GetWindow<NGRemoteInspectorWindow>(NGRemoteInspectorWindow.ShortTitle, false, typeof(NGRemoteHierarchyWindow));
			inspector.SetHierarchy(EditorWindow.GetWindow<NGRemoteHierarchyWindow>(NGRemoteHierarchyWindow.ShortTitle));
			inspector.Show();
		}

		private static void	AddTabProject()
		{
			NGRemoteProjectWindow	project = EditorWindow.GetWindow<NGRemoteProjectWindow>(NGRemoteProjectWindow.ShortTitle, false, typeof(NGRemoteHierarchyWindow));
			project.SetHierarchy(EditorWindow.GetWindow<NGRemoteHierarchyWindow>(NGRemoteHierarchyWindow.ShortTitle));
			project.Show();
		}

		private static void	AddTabCamera()
		{
			NGRemoteCameraWindow	camera = EditorWindow.GetWindow<NGRemoteCameraWindow>(NGRemoteCameraWindow.ShortTitle, false, typeof(NGRemoteHierarchyWindow));
			camera.SetHierarchy(EditorWindow.GetWindow<NGRemoteHierarchyWindow>(NGRemoteHierarchyWindow.ShortTitle));
			camera.Show();
		}
	}
}