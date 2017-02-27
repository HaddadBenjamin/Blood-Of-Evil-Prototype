using NGTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[InitializeOnLoad, Exportable(ExportableAttribute.ArrayOptions.Overwrite | ExportableAttribute.ArrayOptions.Immutable)]
	public class NGConsoleWindow : EditorWindow, IRows, ISettingExportable, IHasCustomMenu
	{
		private struct RowType
		{
			public Type						type;
			public RowLogHandlerAttribute	attribute;
		}

		public const string	Title = "ƝƓ Ҁonsole";
		public const int	StartModuleID = 1; // Start ID at 1, because -1 is non-visible and 0 is default(int).

		public static IEditorOpener[]		openers;
		public static IStackFrameFilter[]	stackFrameFilters;
		public static Type[]				logFilterTypes;
		public static string[]				logFilterNames;
		public static Type[]				startModeTypes;
		public static string[]				startModeNames;
		public static Type[]				endModeTypes;
		public static string[]				endModeNames;

		private static Type			nativeConsoleType;
		private static FieldInfo	nativeConsoleWindowField;
		private static object		lastUnityConsoleInstance;

		private static RowType[]	rowDrawers;

		public static ConsoleSettingsEditor	settingsEditor;

		public bool	IsReady { get { return this.initialized; } }

		/// <summary>
		/// <para>Called after a new log is added to rows.</para>
		/// <para>This first pass defines if the log is consumed by one or more components.</para>
		/// <para>You should not add any Row through this call.</para>
		/// <para>Parameter int : Index of Row in NGConsole.rows[].</para>
		/// <para>Parameter Row : Log boxed in a Row.</para>
		/// </summary>
		public event Action<int, Row>	CheckNewLogConsume;
		/// <summary>
		/// <para>Called after a new log is added to NGConsole.rows[].</para>
		/// <para>Now we know if the current Row is consumed.</para>
		/// <para>Parameter int : Index of Row in NGConsole.rows[].</para>
		/// <para>Parameter Row : Log boxed with a Row.</para>
		/// </summary>
		public event Action<int, Row>	PropagateNewLog;
		/// <summary>Called when the current working module has changed.</summary>
		public event Action				WorkingModuleChanged;
		/// <summary>Called after Clear is called.</summary>
		public event Action				ConsoleCleared;
		/// <summary>Called whenever an option has changed.</summary>
		public event Action				OptionAltered;
		/// <summary>Called on Update.</summary>
		public event Action				UpdateTick;
		/// <summary>Called right after header bar has been drawn.</summary>
		public event Func<Rect, Rect>	PostOnGUIHeader;
		/// <summary>Called after all GUI has been drawn.</summary>
		public event Action				PostOnGUI;

		// GUI Events
		/// <summary>Add GUI after header's left menu. (Clear, Collapse...)</summary>
		public event Action	AfterGUIHeaderLeftMenu;
		/// <summary>Add GUI before header's right menu.</summary>
		public event Action	BeforeGUIHeaderRightMenu;
		/// <summary>Add GUI after header's right menu.</summary>
		public event Action	AfterGUIHeaderRightMenu;

		/// <summary>ID of the current visible module the console is displaying.</summary>
		public int	workingModuleId = -1;

		internal List<Row>				rows;
		internal SyncLogs				syncLogs;

		internal bool					initialized;
		private NGSettings				settings;

		private Module[]	visibleModules;
		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		private Module[]	modules;

		private bool	clearOnPlay;
		private bool	breakOnError;

		private Rect		r;
		private Vector2		scrollModulesPosition;
		private bool		hasCompiled;

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGConsoleWindow.Title + " or reset the settings.");

		static	NGConsoleWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGConsoleWindow.Title);
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGConsoleWindow.Title + " Clear %w");

			Preferences.SettingsChanged += NGConsoleWindow.OnSettingsChanged;

			NGConsoleWindow.nativeConsoleType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
			NGConsoleWindow.nativeConsoleWindowField = NGConsoleWindow.nativeConsoleType.GetField("ms_ConsoleWindow", BindingFlags.NonPublic | BindingFlags.Static);

			if (NGConsoleWindow.nativeConsoleWindowField != null)
				EditorApplication.update += NGConsoleWindow.AutoReplaceNativeConsole;

			Type			type = typeof(StartMode);
			List<Type>		listTypes = new List<Type>();
			List<string>	listNames = new List<string>();

			foreach (Type c in Utility.EachSubClassesOf(type))
			{
				listTypes.Add(c);
				if (c.Name.EndsWith(type.Name) == true)
					listNames.Add(Utility.NicifyVariableName(c.Name.Remove(c.Name.Length - type.Name.Length, type.Name.Length)));
				else
					listNames.Add(Utility.NicifyVariableName(c.Name));
			}

			NGConsoleWindow.startModeTypes = listTypes.ToArray();
			NGConsoleWindow.startModeNames = listNames.ToArray();

			type = typeof(EndMode);
			listTypes.Clear();
			listNames.Clear();

			foreach (Type c in Utility.EachSubClassesOf(type))
			{
				listTypes.Add(c);
				if (c.Name.EndsWith(type.Name) == true)
					listNames.Add(Utility.NicifyVariableName(c.Name.Remove(c.Name.Length - type.Name.Length, type.Name.Length)));
				else
					listNames.Add(Utility.NicifyVariableName(c.Name));
			}

			NGConsoleWindow.endModeTypes = listTypes.ToArray();
			NGConsoleWindow.endModeNames = listNames.ToArray();
	
			listTypes.Clear();
			listNames.Clear();

			foreach (Type c in Utility.EachAssignableFrom(typeof(ILogFilter)))
			{
				listTypes.Add(c);
				listNames.Add(c.Name);
			}

			NGConsoleWindow.logFilterTypes = listTypes.ToArray();
			NGConsoleWindow.logFilterNames = listNames.ToArray();

			List<IStackFrameFilter>	filteredStackFrameFilters = new List<IStackFrameFilter>();

			foreach (Type c in Utility.EachAssignableFrom(typeof(IStackFrameFilter)))
			{
				filteredStackFrameFilters.Add((IStackFrameFilter)Activator.CreateInstance(c));
			}

			NGConsoleWindow.stackFrameFilters = filteredStackFrameFilters.ToArray();

			List<IEditorOpener>	filteredOpeners = new List<IEditorOpener>();

			foreach (Type c in Utility.EachAssignableFrom(typeof(IEditorOpener)))
			{
				filteredOpeners.Add((IEditorOpener)Activator.CreateInstance(c));
			}

			NGConsoleWindow.openers = filteredOpeners.ToArray();

			NGConsoleWindow.settingsEditor = new ConsoleSettingsEditor();

			NGSettings.Initialize += NGConsoleWindow.OnInitializeConsole;
		}

		[MenuItem(Constants.MenuItemPath + NGConsoleWindow.Title, priority = Constants.MenuItemPriority + 101)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGConsoleWindow>(false, NGConsoleWindow.Title, true);
		}

		[MenuItem(Constants.MenuItemPath + NGConsoleWindow.Title + " Clear %w", priority = Constants.MenuItemPriority + 102)]
		public static void	ClearNGConsole()
		{
			NGConsoleWindow[]	instances = Resources.FindObjectsOfTypeAll<NGConsoleWindow>();

			for (int i = 0; i < instances.Length; i++)
				instances[i].Clear();
		}

		private static void	OnInitializeConsole(NGSettings settings)
		{
			if (EditorGUIUtility.isProSkin == true)
				new DarkTheme().SetTheme(settings);
			else
				new LightTheme().SetTheme(settings);
		}

		private static void	AutoReplaceNativeConsole()
		{
			if (Preferences.Settings == null || Preferences.Settings.general.autoReplaceUnityConsole == false)
				return;

			object	value = NGConsoleWindow.nativeConsoleWindowField.GetValue(null);

			if (value != null && value != NGConsoleWindow.lastUnityConsoleInstance)
			{
				NGConsoleWindow[]	consoles = Resources.FindObjectsOfTypeAll<NGConsoleWindow>();

				if (consoles.Length == 0)
				{
					EditorWindow[]	nativeConsoles = Resources.FindObjectsOfTypeAll(NGConsoleWindow.nativeConsoleType) as EditorWindow[];

					if (nativeConsoles.Length > 0)
					{
						EditorWindow.GetWindow<NGConsoleWindow>(NGConsoleWindow.Title, true, nativeConsoleType);
						nativeConsoles[0].Close();
					}
				}
			}

			NGConsoleWindow.lastUnityConsoleInstance = value;
		}

		private static void	OnSettingsChanged()
		{
			NGConsoleWindow[]	instances = Resources.FindObjectsOfTypeAll<NGConsoleWindow>();

			for (int i = 0; i < instances.Length; i++)
			{
				instances[i].OnDisable();
				instances[i].OnEnable();
				instances[i].Repaint();
			}
		}

		protected virtual void	OnEnable()
		{
			if (this.initialized == true || Preferences.Settings == null)
				return;

			try
			{
				//Debug.Log("StartEnable");
				int	i = 0;

				PerWindowVars.InitWindow(this, "NGConsole");

				this.syncLogs = new SyncLogs(this);
				this.syncLogs.EndNewLog += this.RepaintWithModules;
				this.syncLogs.UpdateLog += this.UpdateLog;
				this.syncLogs.NewLog += this.ConvertNewLog;
				this.syncLogs.ResetLog += this.LocalResetLogs;
				this.syncLogs.ClearLog += this.Clear;
				this.syncLogs.OptionAltered += this.UpdateConsoleFlags;

				this.rows = new List<Row>(Constants.PreAllocatedArray);

				this.r = new Rect();

				List<RowType>	rowDrawerTypes = new List<RowType>();

				foreach (Type c in Utility.EachSubClassesOf(typeof(Row)))
				{
					object[]	attributes = c.GetCustomAttributes(typeof(RowLogHandlerAttribute), false);

					if (attributes.Length == 0)
						continue;

					MethodInfo	handler = c.GetMethod(RowLogHandlerAttribute.StaticVerifierMethodName, BindingFlags.Static | BindingFlags.NonPublic);

					if (handler == null)
					{
						InternalNGDebug.LogWarning("The class \"" + c + "\" inherits from \"" + typeof(Row) + "\" and has the attribute \"" + typeof(RowLogHandlerAttribute) + "\" must implement: private static bool " + RowLogHandlerAttribute.StaticVerifierMethodName + "(UnityLogEntryLog log).");
						continue;
					}

					RowType	rdt = new RowType() {
						type = c,
						attribute = c.GetCustomAttributes(typeof(RowLogHandlerAttribute), false)[0] as RowLogHandlerAttribute
					};

					rdt.attribute.handler = handler;

					rowDrawerTypes.Add(rdt);
				}

				rowDrawerTypes.Sort((r1, r2) => r2.attribute.priority - r1.attribute.priority);
				NGConsoleWindow.rowDrawers = rowDrawerTypes.ToArray();

				List<Module>	filteredModules = new List<Module>();

				if (Preferences.Settings.serializedModules.Count > 0)
					this.modules = Preferences.Settings.serializedModules.Deserialize<Module>();

				if (this.modules == null)
				{
					foreach (Type t in Utility.EachSubClassesOf(typeof(Module)))
						filteredModules.Add((Module)Activator.CreateInstance(t));
				}
				else
				{
					filteredModules.AddRange(this.modules);

					// Detect new Module.
					foreach (Type c in Utility.EachSubClassesOf(typeof(Module), c => filteredModules.Exists(m => m.GetType() == c) == false))
						filteredModules.Add((Module)Activator.CreateInstance(c));
				}

				this.modules = filteredModules.ToArray();
				this.visibleModules = this.GetVisibleModules(filteredModules);

				// Initialize modules
				int	id = NGConsoleWindow.StartModuleID;

				for (i = 0; i < this.modules.Length; i++)
				{
					if (this.visibleModules.Contains(this.modules[i]) == true)
						this.modules[i].OnEnable(this, id++);
					else
						this.modules[i].OnEnable(this, -1);
				}

				// Do not overflow if there is removed modules.
				if (this.visibleModules.Length > 0)
				{
					if (this.workingModuleId == -1)
						this.workingModuleId = this.visibleModules[0].Id;
					else
						this.workingModuleId = Mathf.Clamp(this.workingModuleId, NGConsoleWindow.StartModuleID, this.visibleModules.Length);

					if (this.visibleModules.Length >= 0)
					{
						Module	module = this.GetModule(this.workingModuleId);
						if (module != null)
							module.OnEnter();
					}
				}
				else
					this.workingModuleId = -1;

				GUI.FocusControl(null);

				Object[]	nativeConsoleInstances = Resources.FindObjectsOfTypeAll(NGConsoleWindow.nativeConsoleType) as Object[];

				if (nativeConsoleInstances.Length > 0)
					NGConsoleWindow.nativeConsoleWindowField.SetValue(null, nativeConsoleInstances[nativeConsoleInstances.Length - 1]);

				this.settings = Preferences.Settings;

				Undo.undoRedoPerformed += this.Repaint;

				this.initialized = true;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}

		protected virtual void	OnDisable()
		{
			if (this.initialized == false)
				return;

			this.initialized = false;

			NGConsoleWindow.nativeConsoleWindowField.SetValue(null, null);

			if (this.syncLogs != null)
			{
				this.syncLogs.EndNewLog -= this.RepaintWithModules;
				this.syncLogs.UpdateLog -= this.UpdateLog;
				this.syncLogs.NewLog -= this.ConvertNewLog;
				this.syncLogs.ResetLog -= this.LocalResetLogs;
				this.syncLogs.ClearLog -= this.Clear;
				this.syncLogs.OptionAltered -= this.UpdateConsoleFlags;
			}

			if (this.modules != null)
			{
				try
				{
					Module	module = this.GetModule(this.workingModuleId);
					if (module != null)
						module.OnLeave();

					for (int i = 0; i < this.modules.Length; i++)
						this.modules[i].OnDisable();

					this.SaveModules();
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException(new Exception(LC.G("Console_IssueEncoutered"), ex));
					if (Conf.DebugMode == Conf.DebugModes.None)
					{
						InternalNGDebug.LogError("NG Console has aborted its uninitialization and has closed for safety. Relaunch NG Console and contact the author using the Contact form through Preferences.");
						this.Close();
					}
				}

				this.modules = null;
			}

			Undo.undoRedoPerformed -= this.Repaint;

			this.settings = null;
		}

		public void	SaveModules()
		{
			this.settings.serializedModules.Serialize(this.modules);
			Preferences.InvalidateSettings(this.settings);
		}

		protected virtual void	OnGUI()
		{
			if (this.initialized == false)
			{
				GUILayout.Label(string.Format(LC.G("RequiringConfigurationFile"), NGConsoleWindow.Title));
				if (GUILayout.Button(LC.G("ShoWPreferencesWindow")) == true)
					Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);
				return;
			}

			FreeOverlay.First(this, NGConsoleWindow.Title + " is restrained to:\n" +
							  "• " + FreeConstants.MaxStreams + " streams.\n" +
							  "• " + FreeConstants.MaxFilters + " filters per stream.\n" +
							  "• " + FreeConstants.MaxColorMarkers + " color markers.\n" +
							  "• " + FreeConstants.MaxCLICommandExecutions + " remote CLI command executions per session.\n" +
							  "• You can not reach a stack frame deeper than " + FreeConstants.LowestRowGoToLineAllowed + ".");

			Utility.drawingWindow = this;

			r.x = 0;
			r.y = 0;
			r.width = this.position.width;
			r.height = this.settings.general.menuHeight;
			r = this.DrawHeader(r);

			if (this.PostOnGUIHeader != null)
			{
				r.x = 0F;
				r.width = this.position.width;
				r.height = this.settings.general.menuHeight;

				try
				{
					foreach (Func<Rect, Rect> x in this.PostOnGUIHeader.GetInvocationList())
					{
						r = x(r);
					}
				}
				catch (Exception ex)
				{
					this.errorPopup.exception = ex;
				}
			}

			if (this.workingModuleId >= NGConsoleWindow.StartModuleID)
			{
				try
				{
					r.x = 0F;
					r.width = this.position.width;
					r.height = this.position.height - r.y;
					this.GetModule(this.workingModuleId).OnGUI(r);
				}
				catch (ExitGUIException)
				{
				}
				catch (Exception ex)
				{
					this.errorPopup.exception = ex;
				}
			}

			try
			{
				if (this.PostOnGUI != null)
					this.PostOnGUI();
			}
			catch (Exception ex)
			{
				this.errorPopup.exception = ex;
			}

			FreeOverlay.Last();
		}

		protected virtual void	Update()
		{
			if (this.initialized == false)
			{
				this.OnDisable();
				this.OnEnable();
				return;
			}

			try
			{
				if (EditorApplication.isCompiling == true)
					this.hasCompiled = true;
				else if (this.hasCompiled == true)
				{
					this.hasCompiled = false;

					// Reset all internal logs and resync' them all. Not well optimized, but it fits well for now.
					this.LocalResetLogs();
					this.syncLogs.LocalClear();
					this.syncLogs.Sync();
				}
				else
					this.syncLogs.Sync();
			}
			catch (Exception ex)
			{
				this.errorPopup.exception = ex;
			}

			if (this.UpdateTick != null)
				this.UpdateTick();
		}

		/// <summary>
		/// Sets the focus on the module associated with the given <paramref name="id"/>.
		/// </summary>
		/// <param name="id"></param>
		public void		SetModule(int id)
		{
			this.GetModule(this.workingModuleId).OnLeave();
			this.workingModuleId = id;
			this.GetModule(this.workingModuleId).OnEnter();
			if (this.WorkingModuleChanged != null)
				this.WorkingModuleChanged();
		}

		/// <summary>
		/// Gets a module by its name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Module	GetModule(string name)
		{
			foreach (var t in this.modules)
			{
				if (t.name == name)
					return t;
			}
			return null;
		}

		/// <summary>
		/// Gets a module by its ID.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Module	GetModule(int id)
		{
			foreach (var t in this.modules)
			{
				if (t.Id == id)
					return t;
			}
			return null;
		}

		private void	UpdateLog(int i, UnityLogEntry unityLog)
		{
			// Just update the bare necessities.
			this.rows[i].log.collapseCount = unityLog.collapseCount;
		}

		private void	ConvertNewLog(int i, UnityLogEntry unityLog)
		{
			for (int j = 0; j < NGConsoleWindow.rowDrawers.Length; j++)
			{
				if ((bool)NGConsoleWindow.rowDrawers[j].attribute.handler.Invoke(null, new object[] { unityLog }) == true)
				{
					Row	row = Activator.CreateInstance(NGConsoleWindow.rowDrawers[j].type) as Row;

					LogEntry	log = new LogEntry();
					log.Set(unityLog);
					log.frameCount = Time.frameCount;
					log.renderedFrameCount = Time.renderedFrameCount;

					if (this.settings.log.displayTime == true)
					{
						try
						{
							log.time = DateTime.Now.ToString(this.settings.log.timeFormat);
						}
						catch
						{
							log.time = "00:00:00";
						}
					}

					row.Init(this, log);

					if (rows.Count <= i)
						rows.Add(row);
					else
					{
						InternalNGDebug.LogFile(rows.Count + " < " + i);
						rows[i] = row;
					}

					if (this.CheckNewLogConsume != null)
						this.CheckNewLogConsume(i, row);
					if (this.PropagateNewLog != null)
						this.PropagateNewLog(i, row);

					return;
				}
			}

			InternalNGDebug.LogFile("No Row can handle this log:" + unityLog);
		}

		/// <summary>
		/// Displays the top menus.
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		private Rect	DrawHeader(Rect r)
		{
			// Draw Unity native features. This way is better because using style Toolbar in BeginHorizontal creates an unwanted left margin.
			GUI.Box(r, GUIContent.none, GeneralStyles.Toolbar);

			GUILayout.BeginArea(r);
			{
				GUILayout.BeginHorizontal();
				{
					if (string.IsNullOrEmpty(this.settings.general.clearLabel) == false)
					{
						if (GUILayout.Button(this.settings.general.clearLabel, this.settings.general.menuButtonStyle) == true)
							this.Clear();
					}

					EditorGUI.BeginChangeCheck();

					ConsoleFlags	flags = (ConsoleFlags)this.syncLogs.logEntries.consoleFlags;
					if (string.IsNullOrEmpty(this.settings.general.clearOnPlayLabel) == false)
						this.clearOnPlay = GUILayout.Toggle((flags & ConsoleFlags.ClearOnPlay) != 0, this.settings.general.clearOnPlayLabel, this.settings.general.menuButtonStyle);
					if (string.IsNullOrEmpty(this.settings.general.errorPauseLabel) == false)
						this.breakOnError = GUILayout.Toggle((flags & ConsoleFlags.ErrorPause) != 0, this.settings.general.errorPauseLabel, this.settings.general.menuButtonStyle);

					if (EditorGUI.EndChangeCheck() == true)
					{
						this.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.ClearOnPlay, this.clearOnPlay);
						this.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.ErrorPause, this.breakOnError);

						if (this.OptionAltered != null)
							this.OptionAltered();

						Utility.RepaintConsoleWindow();
					}

					try
					{
						if (this.AfterGUIHeaderLeftMenu != null)
							this.AfterGUIHeaderLeftMenu();
					}
					catch (Exception ex)
					{
						this.errorPopup.exception = ex;
					}

					GUILayout.Space(5F);

					this.scrollModulesPosition = EditorGUILayout.BeginScrollView(this.scrollModulesPosition, GUI.skin.horizontalScrollbar, GUIStyle.none);
					{
						GUILayout.BeginHorizontal();
						{
							// Draw tabs menu.
							if (this.visibleModules.Length > 1)
							{
								for (int i = 0; i < this.visibleModules.Length; i++)
									this.visibleModules[i].DrawMenu(this.workingModuleId);
							}

							GUILayout.Space(5F);

							// Display right menus.
							GUILayout.FlexibleSpace();

							try
							{
								if (this.BeforeGUIHeaderRightMenu != null)
									this.BeforeGUIHeaderRightMenu();
							}
							catch (Exception ex)
							{
								this.errorPopup.exception = ex;
							}
						}
						GUILayout.EndHorizontal();
					}
					EditorGUILayout.EndScrollView();

					try
					{
						if (this.AfterGUIHeaderRightMenu != null)
							this.AfterGUIHeaderRightMenu();
					}
					catch (Exception ex)
					{
						this.errorPopup.exception = ex;
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			r.y += this.settings.general.menuHeight;

			if (this.errorPopup.exception != null)
			{
				r.height = this.errorPopup.boxHeight;
				this.errorPopup.OnGUIRect(r);

				r.y += r.height;
			}

			return r;
		}

		private void	LocalResetLogs()
		{
			for (int i = 0; i < this.rows.Count; i++)
				this.rows[i].Uninit();
			this.rows.Clear();

			if (this.ConsoleCleared != null)
				this.ConsoleCleared();
			this.Repaint();
		}

		public void	Clear()
		{
			if (this.initialized == false)
				return;

			for (int i = 0; i < this.rows.Count; i++)
				this.rows[i].Uninit();
			this.rows.Clear();

			this.syncLogs.Clear();

			if (this.ConsoleCleared != null)
				this.ConsoleCleared();

			this.RepaintWithModules();
		}

		private void	RepaintWithModules()
		{
			this.Repaint();
			Utility.RepaintEditorWindow(typeof(ModuleWindow));
		}

		private void	UpdateConsoleFlags()
		{
			if (this.OptionAltered != null)
				this.OptionAltered();
		}

		/// <summary>
		/// Modifies the given list to fetch only the visible modules.
		/// </summary>
		/// <param name="modules"></param>
		/// <returns></returns>
		private Module[]	GetVisibleModules(List<Module> modules)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				if (modules[i].GetType().IsDefined(typeof(VisibleModuleAttribute), false) == false)
				{
					modules.RemoveAt(i);
					--i;
				}
			}

			modules.Sort(this.SortModules);

			return modules.ToArray();
		}

		private int	SortModules(Module a, Module b)
		{
			var	aAttribute = a.GetType().GetCustomAttributes(typeof(VisibleModuleAttribute), false)[0] as VisibleModuleAttribute;
			var	bAttribute = b.GetType().GetCustomAttributes(typeof(VisibleModuleAttribute), false)[0] as VisibleModuleAttribute;
			return aAttribute.position - bAttribute.position;
		}

		Row	IRows.GetRow(int i)
		{
			if (i >= this.rows.Count)
			{
				this.errorPopup.exception = new Exception("Overflow " + i + " < " + this.rows.Count);
				InternalNGDebug.LogFile("Overflow " + i + " < " + this.rows.Count);
			}
			return this.rows[i];
		}

		int	IRows.CountRows()
		{
			return this.rows.Count;
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGConsoleWindow.Title, Constants.WikiBaseURL + "#markdown-header-11-ng-console");

			if (Application.platform == RuntimePlatform.OSXEditor)
				menu.AddItem(new GUIContent("Open Player Log"), false, new GenericMenu.MenuFunction(InternalEditorUtility.OpenPlayerConsole));
			menu.AddItem(new GUIContent("Open Editor Log"), false, new GenericMenu.MenuFunction(InternalEditorUtility.OpenEditorConsole));

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1
			System.Collections.IEnumerator	enumerator = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator();
#if !UNITY_5_2 && !UNITY_5_3
			System.Collections.IEnumerator	enumerator2 = Enum.GetValues(typeof(UnityEngine.LogType)).GetEnumerator();
#endif

			try
			{
#if UNITY_5_2 || UNITY_5_3
				while (enumerator.MoveNext())
				{
					StackTraceLogType	stackTraceLogType = (StackTraceLogType)((int)enumerator.Current);
					menu.AddItem(new GUIContent("Stack Trace Logging/" + stackTraceLogType), Application.stackTraceLogType == stackTraceLogType, new GenericMenu.MenuFunction2(this.ToggleLogStackTraces), stackTraceLogType);
				}
#else
				while (enumerator2.MoveNext())
				{
					UnityEngine.LogType	logType = (UnityEngine.LogType)((int)enumerator2.Current);
					while (enumerator.MoveNext())
					{
						StackTraceLogType	stackTraceLogType = (StackTraceLogType)((int)enumerator.Current);
						menu.AddItem(new GUIContent("Stack Trace Logging/" + logType + "/" + stackTraceLogType), Application.GetStackTraceLogType(logType) == stackTraceLogType,
						() => {
							Application.SetStackTraceLogType(logType, stackTraceLogType);
						});
					}

					enumerator.Reset();
				}
#endif
			}
			finally
			{
				IDisposable	disposable = enumerator as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
#if UNITY_5_2 || UNITY_5_3
		}

		private void	ToggleLogStackTraces(object userData)
		{
			Application.stackTraceLogType = (StackTraceLogType)((int)userData);
#endif
#endif
		}

		#region Export Settings
		public void	PreExport()
		{
		}

		public void	PreImport()
		{
			this.OnDisable();
		}

		public void	PostImport()
		{
			this.OnEnable();
		}
		#endregion
	}
}