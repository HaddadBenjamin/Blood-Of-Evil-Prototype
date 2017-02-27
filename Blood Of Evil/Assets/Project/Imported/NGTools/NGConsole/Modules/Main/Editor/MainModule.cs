using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[InitializeOnLoad, Serializable, VisibleModule(50)]
	internal sealed class MainModule : Module, IStreams
	{
		[Serializable]
		private sealed class Vars
		{
			public int	workingStream;
		}

		public List<StreamLog>	Streams { get { return this.streams; } }
		public int				WorkingStream { get { return this.perWindowVars.Get(Utility.drawingWindow).workingStream; } }

		private CompilerStream	compilerStream { get { return this.streams[0] as CompilerStream; } }
		private MainStream		mainStream { get { return this.streams[1] as MainStream; } }

		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		private List<StreamLog>	streams;

		[SerializeField]
		private PerWindowVars<Vars>	perWindowVars;

		static	MainModule()
		{
			new SectionDrawer("Main Module", typeof(NGSettings.MainModuleSettings), 20);
		}

		public	MainModule()
		{
			this.name = "Main";
			this.streams = new List<StreamLog>();
			this.streams.Add(new CompilerStream());
			this.streams.Add(new MainStream());
			this.perWindowVars = new PerWindowVars<Vars>();
		}

		public override void	OnEnable(NGConsoleWindow console, int id)
		{
			base.OnEnable(console, id);

			foreach (var stream in this.streams)
			{
				stream.Init(this.console, this);
				stream.FilterAltered += this.console.SaveModules;
				stream.OptionAltered += this.console.SaveModules;
			}

			this.console.CheckNewLogConsume += this.CreateStreamForCategory;
			this.console.OptionAltered += this.UpdateFilteredRows;
			this.console.ConsoleCleared += this.Clear;
			this.console.wantsMouseMove = true;

			// Populate with default commands if missing.
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.SwitchNextStreamCommand, KeyCode.Tab, true);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.SwitchPreviousStreamCommand, KeyCode.Tab, true, true);

			if (this.perWindowVars == null)
				this.perWindowVars = new PerWindowVars<Vars>();
		}

		public override void	OnDisable()
		{
			base.OnDisable();

			this.console.CheckNewLogConsume -= this.CreateStreamForCategory;
			this.console.OptionAltered -= this.UpdateFilteredRows;
			this.console.ConsoleCleared -= this.Clear;
			this.console.wantsMouseMove = false;

			foreach (var stream in this.streams)
			{
				stream.Uninit();
				stream.FilterAltered -= this.console.SaveModules;
				stream.OptionAltered -= this.console.SaveModules;
			}
		}

		public override void	OnGUI(Rect r)
		{
			float	yOrigin = r.y;
			float	maxHeight = r.height;
			float	maxWidth = r.width;

			r = this.DrawStreamTabs(r);

			r.x = 0F;
			r.width = maxWidth;
			r.height = maxHeight - (r.y - yOrigin);
			this.streams[this.perWindowVars.Get(Utility.drawingWindow).workingStream].OnGUI(r);
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
			foreach (var stream in this.streams)
				stream.Clear();
		}

		public void	FocusStream(int i)
		{
			if (i < 0)
				this.perWindowVars.Get(Utility.drawingWindow).workingStream = 0;
			else if (i >= this.streams.Count)
				this.perWindowVars.Get(Utility.drawingWindow).workingStream = this.streams.Count - 1;
			else
				this.perWindowVars.Get(Utility.drawingWindow).workingStream = i;

			this.console.SaveModules();
		}

		public void	DeleteStream(int i)
		{
			this.streams[i].Uninit();
			this.streams.RemoveAt(i);

			foreach (Vars var in this.perWindowVars.Each())
				var.workingStream = Mathf.Clamp(var.workingStream, 0, this.streams.Count - 1);

			this.console.SaveModules();
		}

		private Rect	DrawStreamTabs(Rect r)
		{
			r.height = EditorGUIUtility.singleLineHeight;

			Vars	vars = this.perWindowVars.Get(Utility.drawingWindow);

			// Switch stream
			if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchNextStreamCommand) == true)
			{
				vars.workingStream += 1;
				if (vars.workingStream >= this.streams.Count)
					vars.workingStream = 0;

				Event.current.Use();
			}
			else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchPreviousStreamCommand) == true)
			{
				vars.workingStream -= 1;
				// Handle CompilerStream.
				if (vars.workingStream == 0 && this.compilerStream.totalCount == 0)
					vars.workingStream = this.streams.Count - 1;
				if (vars.workingStream < 0)
					vars.workingStream = this.streams.Count - 1;

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
						if (FreeConstants.CheckMaxStreams(this.streams.Count - 2) == true)
						{
							StreamLog	stream = new StreamLog();
							stream.Init(this.console, this);
							stream.RefreshFilteredRows();
							this.streams.Add(stream);
						
							this.console.SaveModules();

							if (this.streams.Count == 1)
								vars.workingStream = 0;
						}
					}

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			r.y += r.height + 2F;

			return r;
		}

		public static readonly Dictionary<int, string>	methodsCategories = new Dictionary<int, string>();

		private void	CreateStreamForCategory(int i, Row row)
		{
			// StreamLog does not receive output from compiler.
			if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptCompileWarning)) != 0)
				return;

			// Category has a priority over all rules.
			string	category;
			int		hash = row.log.fileHash + row.log.line;

			if (MainModule.methodsCategories.TryGetValue(hash, out category) == false)
			{
				ILogContentGetter	log = row as ILogContentGetter;

				if (log != null)
					category = log.Category;
				else
					category = null;

				MainModule.methodsCategories.Add(hash, category);
			}

			if (category != null)
			{
				for (int j = 0; j < this.streams.Count; j++)
				{
					if (this.streams[j].onlyCategory == true && this.streams[j].name == category)
						return;
				}

				StreamLog	stream = new StreamLog();
				stream.onlyCategory = true;
				stream.name = category;
				stream.Init(this.console, this);
				this.streams.Add(stream);
			}
		}

		private void	UpdateFilteredRows()
		{
			ConsoleFlags	flags = (ConsoleFlags)this.console.syncLogs.logEntries.consoleFlags;

			this.mainStream.collapse = (flags & ConsoleFlags.Collapse) != 0;
			this.mainStream.displayLog = (flags & ConsoleFlags.LogLevelLog) != 0;
			this.mainStream.displayWarning = (flags & ConsoleFlags.LogLevelWarning) != 0;
			this.mainStream.displayError = (flags & ConsoleFlags.LogLevelError) != 0;
			this.mainStream.OptionAltered();
		}

		private void	GUIExport()
		{
			Vars	vars = this.perWindowVars.Get(Utility.drawingWindow);

			GUI.enabled = this.streams[vars.workingStream].rowsDrawer.Count > 0;
			if (GUILayout.Button(LC.G("MainModule_ExportStream"), Preferences.Settings.general.menuButtonStyle) == true)
			{
				List<Row>	rows = new List<Row>();

				for (int i = 0; i < this.streams[vars.workingStream].rowsDrawer.Count; i++)
					rows.Add(this.console.rows[this.streams[vars.workingStream].rowsDrawer[i]]);

				ExportRowsEditorWindow.Export(rows);
			}
			GUI.enabled = true;
		}
	}
}