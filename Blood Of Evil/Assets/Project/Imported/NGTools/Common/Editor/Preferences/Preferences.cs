using NGTools;
using NGToolsEditor.NGRemoteScene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public static class Preferences
	{
		private sealed class KudosCommentPopup : PopupWindowContent
		{
			private readonly EditorWindow	window;

			private readonly int	kudos;

			private string	comment = string.Empty;

			public	KudosCommentPopup(EditorWindow window, int kudos)
			{
				this.window = window;
				this.kudos = kudos;
			}

			public override Vector2	GetWindowSize()
			{
				return new Vector2(Mathf.Max(this.window.position.width, 175F), 60F);
			}

			public override void	OnGUI(Rect r)
			{
				using (LabelWidthRestorer.Get(200F))
					EditorGUILayout.PrefixLabel("Comment (" + this.comment.Length + " / 255 chars max)");

				this.comment = EditorGUILayout.TextField(this.comment);
				if (this.comment.Length > 255)
					this.comment = this.comment.Remove(255);

				if (GUILayout.Button("Send " + (this.kudos > 0 ? ":)" : ":(")) == true)
				{
					Preferences.SendKudos(this.kudos, this.comment);
					this.window.Focus();
				}
			}
		}

		public const string	Title = "ƝƓ Ᵽreferences";
		public const float	MaxProcessTimePerFrame = 1F / 24F;

		//private static Type	PreferencesWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.PreferencesWindow");

		private static NGSettings	editorPrefsSettings;
		private static NGSettings	settings;
		/// <summary>
		/// <para>Defines variables that you should use to have a coherent whole.</para>
		/// <para>WARNING! Settings can be null at almost any time, you must prevent doing anything when this case happens!</para>
		/// </summary>
		public static NGSettings	LastSettings { get; private set; }
		public static NGSettings	Settings
		{
			get
			{
				return Preferences.settings;
			}
			set
			{
				Preferences.LastSettings = Preferences.settings;
				Preferences.settings = value;
			}
		}

		public static event Action	SettingsChanged;

		public static string	RootPath;

		private static int		maxAssetsToLoad;
		private static int		assetsLoaded;

		private static Vector2		scrollPosition;
		private static string[]		languages;
		private static int			currentLanguage;
		private static NGSettings[]	assets;
		private static string[]		names;
		private static Texture[]	languageIcons;

		private static string					karma = "Karma";
		private static BgColorContentAnimator	karmaFeedback;

		static	Preferences()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + "Get NG Log");

			EditorApplication.projectWindowChanged += Preferences.ResetAssets;

			 Preferences.RootPath = Utility.GetPackagePath();

			if (Preferences.RootPath == string.Empty)
			{
				Debug.LogWarning(Constants.RootFolderName + " folder was not found.");
				return;
			}

			Preferences.scrollPosition = new Vector2();

			// Delay load, because Unity has not loaded its resources yet.
			EditorApplication.delayCall += () =>
			{
				// Since Unity 5.4, a lot of variables are not accessible from static loading.
				InternalNGDebug.LogPath = NGEditorPrefs.GetString(Constants.DebugLogFilepathKeyPref, InternalNGDebug.LogPath, true);
				Conf.DebugMode = (Conf.DebugModes)PlayerPrefs.GetInt(Conf.DebugModeKeyPref, (int)Conf.DebugMode);

				string	path = NGEditorPrefs.GetString(Constants.ConfigPathPref, null, true);

				if (string.IsNullOrEmpty(path) == false)
				{
					Preferences.settings = AssetDatabase.LoadAssetAtPath(path, typeof(NGSettings)) as NGSettings;
					InternalNGDebug.Assert(Preferences.settings != null, "NG Settings is null at \"" + path + "\".");
					Preferences.LoadAssets();

					if (Preferences.SettingsChanged != null)
						Preferences.SettingsChanged();
				}
				else
					EditorApplication.delayCall += () => GUICallbackWindow.Open(() => Preferences.LoadSharedNGSetting());

				try
				{
					string			rootPath = Path.Combine(Preferences.RootPath, Constants.RelativeLocaleFolder);
					List<string>	languages = new List<string>();

					if (Directory.Exists(rootPath) == true)
						languages.AddRange(Directory.GetDirectories(rootPath));

					for (int i = 0; i < languages.Count; i++)
						languages[i] = Utility.NicifyVariableName(languages[i].Substring(rootPath.Length));

					for (int i = 0; i < Localization.embedLocales.Length; i++)
					{
						if (languages.Contains(Localization.embedLocales[i].language) == false)
							languages.Add(Localization.embedLocales[i].language);
					}

					string	prefLanguage = Localization.CurrentLanguage();

					for (Preferences.currentLanguage = 0; Preferences.currentLanguage < languages.Count; Preferences.currentLanguage++)
					{
						if (prefLanguage == languages[Preferences.currentLanguage])
							break;
					}

					if (Preferences.currentLanguage >= languages.Count)
						Preferences.currentLanguage = 0;

					Preferences.languages = languages.ToArray();
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					Preferences.languages = new string[0];
					Preferences.languageIcons = new Texture[0];
				}

				Preferences.languageIcons = new Texture[Preferences.languages.Length];

				for (int i = 0; i < Preferences.languages.Length; i++)
				{
					Preferences.languageIcons[i] = AssetDatabase.LoadAssetAtPath(Path.Combine(Preferences.RootPath, Constants.RelativeLocaleFolder + Preferences.languages[i] + "/" + Preferences.languages[i]) + ".png", typeof(Texture)) as Texture;
					if (Preferences.languageIcons[i] == null)
					{
						for (int j = 0; j < Localization.embedLocales.Length; j++)
						{
							if (Preferences.languages[i] == Localization.embedLocales[j].language)
							{
								Preferences.languageIcons[i] = Localization.embedLocales[j].icon;
								break;
							}
						}
					}
				}

				EditorApplication.delayCall += () => {
					Preferences.SettingsChanged += Preferences.CheckSettingsVersion;
					Preferences.CheckSettingsVersion();
					InternalEditorUtility.RepaintAllViews();
				};

				if (NGEditorPrefs.GetBool(Constants.AllowSendStatsKeyPref, true) == true)
					Preferences.SendStats();
			};
		}

		[MenuItem(Constants.MenuItemPath + "Get NG Log", priority = Constants.MenuItemPriority + 1001)]
		public static void	GetNGLog()
		{
			Utility.ShowExplorer(InternalNGDebug.LogPath);
		}

		private static void	CheckSettingsVersion()
		{
			if (Preferences.settings != null && Preferences.settings.version < NGSettings.Version)
			{
				if ((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(Constants.PackageTitle, LC.G("Preferences_AskResetSettings"), LC.G("Yes"), LC.G("No")) == true)
				{
					GUICallbackWindow.Open(() => {
						SerializedObject	obj = new SerializedObject(Preferences.settings);
						NGSettings			newSettings = ScriptableObject.CreateInstance<NGSettings>();

						newSettings.hideFlags = Preferences.settings.hideFlags;

						if (Preferences.editorPrefsSettings == Preferences.settings)
							Preferences.editorPrefsSettings = newSettings;

						newSettings.Init();

						SerializedObject	newObject = new SerializedObject(newSettings);
						SerializedProperty	it = obj.GetIterator();

						it.Next(true);

						SerializedProperty	end = it.GetEndProperty();

						while (SerializedProperty.EqualContents(it, end) == false && it.Next(true) == true)
							newObject.CopyFromSerializedProperty(it);

						newObject.ApplyModifiedProperties();

						string	path = AssetDatabase.GetAssetPath(Preferences.settings.GetInstanceID());

						if (string.IsNullOrEmpty(path) == false)
							AssetDatabase.CreateAsset(newSettings, path);

						if (Preferences.assets != null)
						{
							for (int i = 0; i < Preferences.assets.Length; i++)
							{
								if (Preferences.assets[i] == Preferences.settings)
								{
									Preferences.assets[i] = newSettings;
									break;
								}
							}
						}

						Preferences.settings = newSettings;
					});
				}
			}
		}

		private static void	LoadSharedNGSetting(bool skipLoad = false)
		{
			if (skipLoad == false && Preferences.editorPrefsSettings != null)
			{
				Preferences.Settings = Preferences.editorPrefsSettings;

				if (Preferences.SettingsChanged != null)
					Preferences.SettingsChanged();
				return;
			}

			string	settingsPath = Preferences.GetSettingsPath();

			if (skipLoad == false)
			{
				Object[]	assets = InternalEditorUtility.LoadSerializedFileAndForget(settingsPath);

				for (int i = 0; i < assets.Length; i++)
				{
					if (assets[i] is NGSettings)
					{
						Preferences.Settings = assets[i] as NGSettings;
						break;
					}
				}
			}

			if (skipLoad == true || Preferences.settings == null)
			{
				Preferences.Settings = ScriptableObject.CreateInstance<NGSettings>();
				Preferences.settings.Init();

				Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, Constants.PackageTitle));
				InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { Preferences.settings }, settingsPath, true);
			}

			Preferences.settings.hideFlags = HideFlags.DontSave;
			Preferences.editorPrefsSettings = Preferences.settings;

			if (Preferences.SettingsChanged != null)
				Preferences.SettingsChanged();
		}

		private static void	SaveSharedNGSettings(NGSettings settings = null)
		{
			if (settings == null)
				settings = Preferences.settings;

			Utility.RegisterIntervalCallback(Preferences.DelayWriteToDisk, 200);
		}

		private static void	DelayWriteToDisk()
		{
			Utility.UnregisterIntervalCallback(Preferences.DelayWriteToDisk);

			string	settingsPath = Preferences.GetSettingsPath();;

			try
			{
				InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { Preferences.editorPrefsSettings }, settingsPath, true);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("An error occured when " + Constants.PackageTitle + " tried to write data into \"" + settingsPath + "\".", ex);
			}
		}

		private static string	GetSettingsPath()
		{
			return Path.Combine(Application.persistentDataPath, Path.Combine(Constants.PackageTitle, Constants.SettingsFilename));
		}

		public static void	InvalidateSettings(NGSettings settings = null)
		{
			if (settings == null)
				settings = Preferences.settings;

			if ((settings.hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
				Preferences.SaveSharedNGSettings(settings);
			else if (settings != null)
				EditorUtility.SetDirty(settings);
		}

		[PreferenceItem(Constants.PreferenceTitle)]
		private static void	PreferencesGUI()
		{
#if NGTOOLS_FREE
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.HelpBox("Update to NG Tools Pro in the Asset Store", MessageType.Info);
				if (GUILayout.Button("Buy NG Tools Pro", GUILayout.Height(36F)) == true && EditorUtility.DisplayDialog("", "You are opening the Asset Store page on your browser. Do you confirm?", "Yes", "No") == true)
				{
					Help.BrowseURL("https://www.assetstore.unity3d.com/en/#!/content/34109");
				}
			}
			EditorGUILayout.EndHorizontal();
#endif

			if (Preferences.languages != null && Preferences.languages.Length > 1)
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(LC.G("Language"));

					GUILayout.FlexibleSpace();

					if (Preferences.languageIcons != null)
					{
						for (int i = 0; i < Preferences.languages.Length; i++)
						{
							Rect	r = GUILayoutUtility.GetRect(16F, 11F);

							if (Event.current.type == EventType.MouseDown &&
								r.Contains(Event.current.mousePosition) == true)
							{
								Preferences.currentLanguage = i;
								Localization.SaveLanguage(Preferences.languages[Preferences.currentLanguage]);
								if (Localization.LoadLanguage(Preferences.languages[Preferences.currentLanguage]) == true)
									Debug.Log(string.Format(LC.G("Preferences_LoadedLocale"), Preferences.languages[Preferences.currentLanguage]));

								EditorWindow.focusedWindow.Repaint();
							}

							EditorGUI.DrawTextureTransparent(r, Preferences.languageIcons[i]);

							if (i == Preferences.currentLanguage && Event.current.type == EventType.Repaint)
								Utility.DrawUnfillRect(r, Color.white);

							GUILayout.Space(5F);
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			GUILayout.Space(10F);

			EditorGUILayout.BeginHorizontal();
			{
				if (GUILayout.Button(LC.G("Preferences_GenerateConfigFile")) == true)
				{
					string	path = EditorUtility.SaveFilePanelInProject(LC.G("Preferences_SaveConfigFile"), "NGSettings", "asset", LC.G("Preferences_ChoosePathConfigFile"));

					if (string.IsNullOrEmpty(path) == false)
					{
						try
						{
							Preferences.Settings = ScriptableObject.CreateInstance<NGSettings>();
							Preferences.settings.Init();

							AssetDatabase.CreateAsset(Preferences.settings, path);
							AssetDatabase.SaveAssets();

							NGEditorPrefs.SetString(Constants.ConfigPathPref, path, true);

							Preferences.LoadAssets();

							if (Preferences.SettingsChanged != null)
								Preferences.SettingsChanged();

							// Need to skip many frames before really writing the data. Don't know why it requires 2 frames.
							EditorApplication.delayCall += () =>
							{
								EditorApplication.delayCall += () =>
								{
									Preferences.InvalidateSettings();
									AssetDatabase.SaveAssets();
								};
							};
						}
						catch (Exception ex)
						{
							InternalNGDebug.LogException(ex);
							Preferences.Settings = null;
						}
					}
				}

				GUILayout.Space(5F);

				if (GUILayout.Button(LC.G("Refresh")) == true)
					Utility.StartBackgroundTask(Preferences.TaskLoadAssets(), () => Preferences.LoadAssets());
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10F);

			EditorGUILayout.LabelField(LC.G("Preferences_ConfigurationFilesAvailable"));

			Preferences.scrollPosition = EditorGUILayout.BeginScrollView(Preferences.scrollPosition);
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(LC.G("Open"), GUILayout.ExpandWidth(false)) == true)
						Utility.ShowExplorer(Preferences.GetSettingsPath());

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(Preferences.settings != null && (Preferences.settings.hideFlags & HideFlags.DontSave) == HideFlags.DontSave, LC.G("Preferences_SharedSettings"), GUI.skin.button);
					if (EditorGUI.EndChangeCheck() == true && (Preferences.settings == null || (Preferences.settings.hideFlags & HideFlags.DontSave) != HideFlags.DontSave))
					{
						Preferences.LoadSharedNGSetting();
						NGEditorPrefs.SetString(Constants.ConfigPathPref, null, true);
						InternalEditorUtility.RepaintAllViews();
					}

					if (GUILayout.Button(LC.G("Preferences_Reset"), GUILayout.ExpandWidth(false)) == true && ((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(LC.G("Preferences_ConfirmResetEditorPrefsSettingsTitle"), LC.G("Preferences_ConfirmResetEditorPrefsSettings"), LC.G("Yes"), LC.G("No")) == true))
					{
						Preferences.editorPrefsSettings = null;
						Preferences.LoadSharedNGSetting(true);
						NGEditorPrefs.SetString(Constants.ConfigPathPref, null, true);
						InternalEditorUtility.RepaintAllViews();
					}
				}
				EditorGUILayout.EndHorizontal();

				if (Preferences.assets != null)
				{
					for (int i = 0; i < Preferences.assets.Length; i++)
					{
						if (Preferences.assets[i] == null || (Preferences.assets[i].hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
							continue;

						EditorGUILayout.BeginHorizontal();
						{
							EditorGUI.BeginChangeCheck();
							GUILayout.Toggle(Selection.activeObject == Preferences.assets[i], LC.G("Preferences_Focus"), GUI.skin.button, GUILayout.ExpandWidth(false));
							if (EditorGUI.EndChangeCheck() == true)
								Selection.activeObject = Preferences.assets[i];

							EditorGUI.BeginChangeCheck();
							GUILayout.Toggle(Preferences.assets[i] == Preferences.settings, Preferences.names[i], GeneralStyles.LeftButton);
							if (EditorGUI.EndChangeCheck() == true && Preferences.assets[i] != Preferences.settings)
							{
								Preferences.Settings = Preferences.assets[i];
								NGEditorPrefs.SetString(Constants.ConfigPathPref, Preferences.names[i], true);
								if (Preferences.SettingsChanged != null)
									Preferences.SettingsChanged();
								InternalEditorUtility.RepaintAllViews();
							}
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndScrollView();
			}

			EditorGUILayout.Space();

			using (LabelWidthRestorer.Get(60F))
			{
				EditorGUI.BeginChangeCheck();
				string	logPath = NGEditorGUILayout.SaveFileField("Log Path", InternalNGDebug.LogPath);
				if (EditorGUI.EndChangeCheck() == true)
				{
					InternalNGDebug.LogPath = logPath;
					NGEditorPrefs.SetString(Constants.DebugLogFilepathKeyPref, InternalNGDebug.LogPath, true);
				}
			}

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button(LC.G("Console_ExportSettings"), GeneralStyles.ToolbarButton) == true)
					ScriptableWizard.GetWindow<ExportSettingsWizard>(true, "Export Settings", true);

				if (GUILayout.Button(LC.G("Console_ImportSettings"), GeneralStyles.ToolbarButton) == true)
				{
					string	filePath = EditorUtility.OpenFilePanel(LC.G("Console_ImportSettings"), NGEditorPrefs.GetString(Constants.LastExportPathPref), "");

					if (string.IsNullOrEmpty(filePath) == false)
					{
						NGEditorPrefs.SetString(Constants.LastExportPathPref, Directory.GetParent(filePath).FullName);
						SettingsExporter.Import(filePath);
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button(LC.G("Preferences_Contact"), GeneralStyles.ToolbarButton) == true)
				{
					// Force the wizard to be unique.
					ScriptableWizard.GetWindow<ContactFormWizard>().Close();
					ScriptableWizard.DisplayWizard<ContactFormWizard>(string.Empty);
				}

				if (GUILayout.Button(LC.G("Preferences_LinkUnityThread"), GeneralStyles.ToolbarButton) == true)
					Application.OpenURL(Constants.SupportForumUnityThread);

				if (GUILayout.Button(LC.G("Preferences_Tips"), GeneralStyles.ToolbarButton) == true)
					EditorWindow.GetWindow<TipsWindow>(true, string.Empty, true);
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUI.BeginChangeCheck();
				Rect	r = GUILayoutUtility.GetRect(0F, 16F, "GV Gizmo DropDown");
				bool	value = Utility.ExistSymbol(Constants.NestedMenuSymbol);
				if (GUI.Button(r, "Rooted Menu:" + (value == true ? LC.G("Yes") : LC.G("No")), "GV Gizmo DropDown") == true)
				{
					GenericMenu	menu = new GenericMenu();
					menu.AddItem(new GUIContent(LC.G("Yes")), value == true, SetNestedMode, true);
					menu.AddItem(new GUIContent(LC.G("No")), value == false, SetNestedMode, false);
					menu.DropDown(r);
				}

				r = GUILayoutUtility.GetRect(0F, 16F, "GV Gizmo DropDown");
				bool	sendStats = NGEditorPrefs.GetBool(Constants.AllowSendStatsKeyPref, true);
				if (GUI.Button(r, "Send Stats:" + (sendStats ? LC.G("Yes") : LC.G("No")), "GV Gizmo DropDown") == true)
				{
					GenericMenu	menu = new GenericMenu();
					menu.AddItem(new GUIContent("Yes"), sendStats == true, SetSendStats, true);
					menu.AddItem(new GUIContent("No"), sendStats == false, SetSendStats, false);
					menu.DropDown(r);
				}

				r = GUILayoutUtility.GetRect(0F, 16F, "GV Gizmo DropDown");
				if (GUI.Button(r, "Debug:" + Conf.DebugMode.ToString(), "GV Gizmo DropDown") == true)
				{
					GenericMenu	menu = new GenericMenu();
					menu.AddItem(new GUIContent("None"), Conf.DebugMode == Conf.DebugModes.None, SetDebugMode, Conf.DebugModes.None);
					menu.AddItem(new GUIContent("Active"), Conf.DebugMode == Conf.DebugModes.Active, SetDebugMode, Conf.DebugModes.Active);
					menu.AddItem(new GUIContent("Verbose"), Conf.DebugMode == Conf.DebugModes.Verbose, SetDebugMode, Conf.DebugModes.Verbose);
					menu.DropDown(r);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				using (BgColorContentRestorer.Get(Color.green * .9F))
				{
					if (GUILayout.Button(":)", GeneralStyles.ToolbarButton) == true)
						Preferences.SendKudos(1, string.Empty);
					if (GUILayout.Button("", "GV Gizmo DropDown", GUILayout.Width(15F)) == true)
						PopupWindow.Show(new Rect(0, EditorWindow.focusedWindow.position.height, 0F, 0F), new KudosCommentPopup(EditorWindow.focusedWindow, 1));
				}

				if (Preferences.karmaFeedback != null)
				{
					using (ColorContentRestorer.Get(Preferences.karmaFeedback.af.isAnimating, Preferences.karmaFeedback.Value * Color.yellow))
					{
						GUILayout.Label(Preferences.karma, GeneralStyles.CenterText);
					}

					if (Preferences.karmaFeedback.af.isAnimating == false)
						Preferences.karmaFeedback = null;
				}
				else
					GUILayout.Label(Preferences.karma, GeneralStyles.CenterText);

				using (BgColorContentRestorer.Get(Color.red * .6F))
				{
					if (GUILayout.Button(":(", GeneralStyles.ToolbarButton) == true)
					{
#if NGTOOLS_FREE
						EditorUtility.DisplayDialog(Constants.PackageTitle, "Stop! Hate is only available for Pro version.\n\nIf you want to criticize, buy it first.\n\nHehehe! X)", "OK");
#else
						Preferences.SendKudos(-1, string.Empty);
#endif
					}
					if (GUILayout.Button("", "GV Gizmo DropDown", GUILayout.Width(15F)) == true)
					{
#if NGTOOLS_FREE
						EditorUtility.DisplayDialog(Constants.PackageTitle, "Stop! Hate is only available for Pro version.\n\nIf you want to criticize, buy it first.\n\nHehehe! X)", "OK");
#else
						PopupWindow.Show(new Rect(0, EditorWindow.focusedWindow.position.height, 0F, 0F), new KudosCommentPopup(EditorWindow.focusedWindow, -1));
#endif
					}
				}

				GUILayout.Label(Constants.Version, GeneralStyles.Version, GUILayout.ExpandWidth(false));
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void	SetNestedMode(object value)
		{
			if (EditorApplication.isPlaying == true)
			{
				EditorUtility.DisplayDialog(Constants.PackageTitle, LC.G("Preferences_WarningPlayMode"), LC.G("Ok"));
				return;
			}

			if ((bool)value == true)
			{
				if (Utility.ExistSymbol(Constants.NestedMenuSymbol) == false &&
					EditorUtility.DisplayDialog(LC.G("Preferences_NestedMenu"), string.Format(LC.G("Preferences_NestedMenuOn_Warning"), Constants.NestedMenuSymbol), LC.G("Yes"), LC.G("No")) == true)
				{
					Utility.AppendSymbol(Constants.NestedMenuSymbol);
				}
			}
			else
			{
				if (Utility.ExistSymbol(Constants.NestedMenuSymbol) == true &&
					EditorUtility.DisplayDialog(LC.G("Preferences_NestedMenu"), string.Format(LC.G("Preferences_NestedMenuOff_Warning"), Constants.NestedMenuSymbol), LC.G("Yes"), LC.G("No")) == true)
				{
					Utility.RemoveSymbol(Constants.NestedMenuSymbol);
				}
			}
		}

		private static void	SetSendStats(object sendStats)
		{
			if ((bool)sendStats == false)
				Preferences.SendStats(false);

			NGEditorPrefs.SetBool(Constants.AllowSendStatsKeyPref, (bool)sendStats);
		}

		private static void	SetDebugMode(object mode)
		{
			PlayerPrefs.SetInt(Conf.DebugModeKeyPref, (int)mode);
			Conf.DebugMode = (Conf.DebugModes)mode;
		}

		/// <summary>
		/// For the curious who might want to know why I send these stats.
		/// I need some info about Unity Editor usage, especially because Unity does not provide them.
		/// In order to keep supporting old versions or platforms.
		/// </summary>
		/// <param name="sendStats"></param>
		private static void	SendStats(bool sendStats = true)
		{
			try
			{
				string		path = Path.Combine(Application.persistentDataPath, Path.Combine(Constants.InternalPackageTitle, "sendStats.txt"));
				bool		sentOnce = false;
				DateTime	now = DateTime.Now;
				string		today = now.Year.ToString() + now.Month + now.Day;

				if (File.Exists(path) == true)
				{
					string	lastTime = File.ReadAllText(path);

					if (lastTime == today)
						sentOnce = true;
				}

				if (sentOnce == false || sendStats == false)
				{
					if (sentOnce == false)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(path));
						File.WriteAllText(path, today);
					}

					StringBuilder	buffer = Utility.GetBuffer("http://ngtools.tech/unityeditor.php?u=");

					buffer.Append(Application.unityVersion);
					buffer.Append("&o=");
					buffer.Append(SystemInfo.operatingSystem);
					buffer.Append("&p=");
					buffer.Append(Constants.Version);
					buffer.Append("&n=");
					buffer.Append(SystemInfo.deviceName);

					if (sendStats == false)
						buffer.Append("&s");

					HttpWebRequest	request = (HttpWebRequest)WebRequest.Create(Utility.ReturnBuffer(buffer));
					request.BeginGetResponse(r => request.EndGetResponse(r), null);
				}
			}
			catch
			{
			}
		}

		private static void	SendKudos(int n, string comment)
		{
			try
			{
				StringBuilder	buffer = Utility.GetBuffer("http://ngtools.tech/kudos.php?n=");

				buffer.Append(SystemInfo.deviceName);

				if (n < 0)
					buffer.Append("&bk");

				if (string.IsNullOrEmpty(comment) == false)
				{
					buffer.Append("&c=");
					buffer.Append(comment);
				}

				HttpWebRequest	request = (HttpWebRequest)WebRequest.Create(Utility.ReturnBuffer(buffer));
				request.BeginGetResponse(r => {
					HttpWebResponse	response = (HttpWebResponse)request.GetResponse();
					Stream			receiveStream = response.GetResponseStream();

					// Pipes the stream to a higher level stream reader with the required encoding format. 
					using (StreamReader	readStream = new StreamReader(receiveStream, Encoding.UTF8))
					{
						int	karma;

						EditorApplication.delayCall += () => {
							Preferences.karmaFeedback = new BgColorContentAnimator(EditorWindow.focusedWindow.Repaint, 0F, 1F);
						};

						if (int.TryParse(readStream.ReadToEnd(), out karma) == true)
							Preferences.karma = "Karma (" + karma + ")";
						else
							Preferences.karma = "Karma (E)";

						response.Close();
					}
				}, null);
			}
			catch
			{
			}
		}

		private static IEnumerator	TaskLoadAssets()
		{
			string[]	assets = Directory.GetFiles("Assets", "*.asset", SearchOption.AllDirectories);
			float		lastFrame = Time.realtimeSinceStartup;

			Preferences.maxAssetsToLoad = assets.Length;
			Preferences.assetsLoaded = 0;

			for (int j = 0; j < assets.Length; j++)
			{
				AssetDatabase.LoadAssetAtPath(assets[j], typeof(NGSettings));
				++Preferences.assetsLoaded;

				if (Time.realtimeSinceStartup - lastFrame >= Preferences.MaxProcessTimePerFrame)
				{
					lastFrame += Preferences.MaxProcessTimePerFrame;

					if (Preferences.assetsLoaded < Preferences.maxAssetsToLoad)
						EditorUtility.DisplayProgressBar(Constants.PackageTitle, "Looking for NG Settings in all assets. (" + Preferences.assetsLoaded + " / " + Preferences.maxAssetsToLoad + ")", (float)Preferences.assetsLoaded / (float)Preferences.maxAssetsToLoad);
					else
						EditorUtility.ClearProgressBar();

					yield return null;
				}
			}

			EditorUtility.ClearProgressBar();
		}

		private static void	ResetAssets()
		{
			if (Preferences.assets != null)
			{
				List<NGSettings>	assets = new List<NGSettings>(Preferences.assets);
				List<string>		names = new List<string>(Preferences.names);

				for (int i = 0; i < assets.Count; i++)
				{
					if (assets[i] == null)
					{
						assets.RemoveAt(i);
						names.RemoveAt(i);
					}
				}

				Preferences.assets = assets.ToArray();
				Preferences.names = names.ToArray();
			}

			if (Preferences.settings == null)
			{
				NGEditorPrefs.DeleteKey(Constants.ConfigPathPref, true);
				GUICallbackWindow.Open(() => Preferences.LoadSharedNGSetting());
			}
		}

		private static void	LoadAssets()
		{
			Preferences.assets = Resources.FindObjectsOfTypeAll<NGSettings>();
			if (Preferences.assets.Length == 0)
				return;

			Preferences.names = new string[Preferences.assets.Length];

			for (int i = 0; i < Preferences.assets.Length; i++)
			{
				if ((Preferences.assets[i].hideFlags & HideFlags.DontSave) != HideFlags.DontSave)
					Preferences.names[i] = AssetDatabase.GetAssetPath(Preferences.assets[i].GetInstanceID());
			}
		}
	}
}