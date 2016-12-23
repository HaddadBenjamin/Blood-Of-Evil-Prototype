using NGTools;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public static class Preferences
	{
		private const float	MaxProcessTimePerFrame = 1F / 24F;

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

		static	Preferences()
		{
#if NGT_DEBUG && UNITY_EDITOR_WIN
			Utility.AddMenuItemPicker(Constants.MenuItemPath + "Get NG Log");
#endif

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
					EditorApplication.delayCall += () => GUICallbackWindow.Open(() => Preferences.LoadNGSettingFromEditorPrefs());

				try
				{
					string	rootPath = Path.Combine(Preferences.RootPath, Constants.RelativeLocaleFolder);
					Preferences.languages = Directory.GetDirectories(rootPath);

					for (int i = 0; i < Preferences.languages.Length; i++)
						Preferences.languages[i] = Utility.NicifyVariableName(Preferences.languages[i].Substring(rootPath.Length));

					string	prefLanguage = Localization.CurrentLanguage();

					for (Preferences.currentLanguage = 0; Preferences.currentLanguage < Preferences.languages.Length; Preferences.currentLanguage++)
					{
						if (prefLanguage == Preferences.languages[Preferences.currentLanguage])
							break;
					}

					if (Preferences.currentLanguage >= Preferences.languages.Length)
						Preferences.currentLanguage = 0;
				}
				catch
				{
					Preferences.languages = new string[0];
					Preferences.languageIcons = new Texture[0];
				}

				Preferences.languageIcons = new Texture[Preferences.languages.Length];

				for (int i = 0; i < Preferences.languages.Length; i++)
					Preferences.languageIcons[i] = AssetDatabase.LoadAssetAtPath(Path.Combine(Preferences.RootPath, Constants.RelativeLocaleFolder + Preferences.languages[i] + "/" + Preferences.languages[i]) + ".png", typeof(Texture)) as Texture;

				EditorApplication.delayCall += () => {
					Preferences.SettingsChanged += Preferences.CheckSettingsVersion;
					Preferences.CheckSettingsVersion();
					InternalEditorUtility.RepaintAllViews();
				};
			};

			//new SectionDrawer("Common", typeof(NGSettings.MiscSettings), 5);
		}

		private static void	CheckSettingsVersion()
		{
			if (Preferences.settings != null && Preferences.settings.version < NGSettings.Version)
			{
				if (EditorUtility.DisplayDialog(Constants.PackageTitle, LC.G("Preferences_AskResetSettings"), LC.G("Yes"), LC.G("No")) == true)
				{
					GUICallbackWindow.Open(() => {
						SerializedObject	obj = new SerializedObject(Preferences.settings);
						NGSettings			newSettings = ScriptableObject.CreateInstance<NGSettings>();
						FieldInfo[]			fields = typeof(NGSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);

						newSettings.hideFlags = Preferences.settings.hideFlags;

						if (Preferences.editorPrefsSettings == Preferences.settings)
							Preferences.editorPrefsSettings = newSettings;

						for (int i = 0; i < fields.Length; i++)
						{
							NGSettings.Settings	settings = fields[i].GetValue(newSettings) as NGSettings.Settings;

							if (settings != null)
								settings.InternalInitGUI();
						}

						if (EditorGUIUtility.isProSkin == true)
							new DarkTheme().SetTheme(newSettings);
						else
							new LightTheme().SetTheme(newSettings);

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

		private static void	LoadNGSettingFromEditorPrefs(bool skipLoad = false)
		{
			if (Preferences.editorPrefsSettings != null)
			{
				Preferences.Settings = Preferences.editorPrefsSettings;

				if (Preferences.SettingsChanged != null)
					Preferences.SettingsChanged();
				return;
			}

			FieldInfo[]	fields = typeof(NGSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);

			Preferences.Settings = ScriptableObject.CreateInstance<NGSettings>();
			Preferences.settings.hideFlags = HideFlags.DontSave;
			Preferences.editorPrefsSettings = Preferences.settings;

			for (int i = 0; i < fields.Length; i++)
			{
				NGSettings.Settings	settings = fields[i].GetValue(Preferences.settings) as NGSettings.Settings;

				if (settings != null)
					settings.InternalInitGUI();
			}

			if (EditorGUIUtility.isProSkin == true)
				new DarkTheme().SetTheme(Preferences.settings);
			else
				new LightTheme().SetTheme(Preferences.settings);

			if (skipLoad == false)
				Utility.LoadEditorPref(Preferences.settings, NGEditorPrefs.GetPerProjectPrefix());

			if (Preferences.SettingsChanged != null)
				Preferences.SettingsChanged();
		}

		private static void	SaveNGSettingsToEditorPrefs(NGSettings settings = null)
		{
			if (settings == null)
				settings = Preferences.settings;

			Utility.SaveEditorPref(settings, NGEditorPrefs.GetPerProjectPrefix());
		}

		public static void	InvalidateSettings(NGSettings settings = null)
		{
			if (settings == null)
				settings = Preferences.settings;

			if ((settings.hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
				Preferences.SaveNGSettingsToEditorPrefs(settings);
			else if (settings != null)
				EditorUtility.SetDirty(settings);
		}

#if NGT_DEBUG && UNITY_EDITOR_WIN
		[MenuItem(Constants.MenuItemPath + "Get NG Log", priority = Constants.MenuItemPriority + 1001)]
		private static void	Diagnose(MenuCommand command)
		{
			Utility.ShowExplorer(InternalNGDebug.LogPath);
		}
#endif

		[PreferenceItem(Constants.PreferenceTitle)]
		private static void	PreferencesGUI()
		{
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(LC.G("Language"));

					GUILayout.FlexibleSpace();

					if (Preferences.languageIcons != null)
					{
						for (int i = 0; i < Preferences.languages.Length; i++)
						{
							Rect	r = GUILayoutUtility.GetRect(24F, 16F);

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
								AssetDatabase.CreateAsset(Preferences.settings, path);

								FieldInfo[]	fields = typeof(NGSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);
								for (int i = 0; i < fields.Length; i++)
								{
									NGSettings.Settings	settings = fields[i].GetValue(Preferences.settings) as NGSettings.Settings;

									if (settings != null)
										settings.InternalInitGUI();
								}

								if (EditorGUIUtility.isProSkin == true)
									new DarkTheme().SetTheme(Preferences.settings);
								else
									new LightTheme().SetTheme(Preferences.settings);

								AssetDatabase.SaveAssets();

								NGEditorPrefs.SetString(Constants.ConfigPathPref, path, true);
								Utility.ResetConsolesSettings();
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
					Utility.content.text = LC.G("Preferences_Reset");
					float	width = GUI.skin.button.CalcSize(Utility.content).x;

					EditorGUILayout.BeginHorizontal();
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.Toggle(Preferences.settings != null && (Preferences.settings.hideFlags & HideFlags.DontSave) == HideFlags.DontSave, LC.G("Preferences_UseEditorPrefs"), "Button");
						if (EditorGUI.EndChangeCheck() == true && (Preferences.settings == null || (Preferences.settings.hideFlags & HideFlags.DontSave) != HideFlags.DontSave))
						{
							Preferences.LoadNGSettingFromEditorPrefs();
							NGEditorPrefs.SetString(Constants.ConfigPathPref, null, true);
							InternalEditorUtility.RepaintAllViews();
						}

						if (GUILayout.Button(LC.G("Preferences_Reset"), GUILayout.Width(width)) == true && EditorUtility.DisplayDialog(LC.G("Preferences_ConfirmResetEditorPrefsSettingsTitle"), LC.G("Preferences_ConfirmResetEditorPrefsSettings"), LC.G("Yes"), LC.G("No")) == true)
						{
							Preferences.editorPrefsSettings = null;
							Preferences.LoadNGSettingFromEditorPrefs(true);
							NGEditorPrefs.SetString(Constants.ConfigPathPref, null, true);
							InternalEditorUtility.RepaintAllViews();
						}
					}
					EditorGUILayout.EndHorizontal();

					Utility.content.text = LC.G("Preferences_Focus");
					width = GUI.skin.button.CalcSize(Utility.content).x;

					if (Preferences.assets != null)
					{
						for (int i = 0; i < Preferences.assets.Length; i++)
						{
							if (Preferences.assets[i] == null || (Preferences.assets[i].hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
								continue;

							EditorGUILayout.BeginHorizontal();
							{
								if (GUILayout.Button(Utility.content.text, GUILayout.Width(width)) == true)
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

				Preferences.DrawEditLogPath();

				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(LC.G("Console_ExportSettings")) == true)
					{
						ScriptableWizard.GetWindow<ExportSettingsWizard>(true, "Export Settings", true);
					}

					if (GUILayout.Button(LC.G("Console_ImportSettings")) == true)
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

				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(LC.G("Preferences_Contact")) == true)
					{
						// Force the wizard to be unique.
						ScriptableWizard.GetWindow<ContactFormWizard>().Close();
						ScriptableWizard.DisplayWizard<ContactFormWizard>(string.Empty);
					}

					if (GUILayout.Button(LC.G("Preferences_LinkUnityThread")) == true)
					{
						Application.OpenURL(Constants.SupportForumUnityThread);
					}

					if (GUILayout.Button(LC.G("Preferences_Tips")) == true)
					{
						EditorWindow.GetWindow<TipsWindow>(true, string.Empty, true);
					}

					if (Utility.ExistSymbol(Constants.DebugSymbol) == true)
					{
						if (GUILayout.Button(LC.G("Preferences_DebugOn")) == true)
						{
							if (EditorUtility.DisplayDialog(LC.G("Preferences_DebugOn"), string.Format(LC.G("Preferences_DebugOn_Warning"), Constants.DebugSymbol), LC.G("Yes"), LC.G("No")) == true)
							{
								Utility.ToggleSymbol(Constants.DebugSymbol);
								Utility.RemoveSymbol(Constants.VerboseDebugSymbol);
							}
						}
					}
					else if (GUILayout.Button(LC.G("Preferences_DebugOff")) == true)
					{
						int	n = EditorUtility.DisplayDialogComplex(LC.G("Preferences_DebugOff"), string.Format(LC.G("Preferences_DebugOff_Warning"), Constants.DebugSymbol), LC.G("Yes"), LC.G("No"), LC.G("Verbose"));
						if (n == 0)
						{
							Utility.ToggleSymbol(Constants.DebugSymbol);
						}
						else if (n == 2)
						{
							Utility.ToggleSymbol(Constants.DebugSymbol);
							Utility.ToggleSymbol(Constants.VerboseDebugSymbol);
						}
					}

					GUILayout.Label(Constants.Version, GeneralStyles.Version);
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		[Conditional(Constants.DebugSymbol)]
		private static void	DrawEditLogPath()
		{
			using (LabelWidthRestorer.Get(60F))
			{
				EditorGUI.BeginChangeCheck();
				string	logPath = EditorGUILayout.TextField("Log Path", InternalNGDebug.LogPath);
				if (EditorGUI.EndChangeCheck() == true)
				{
					InternalNGDebug.LogPath = logPath;
					NGEditorPrefs.SetString(Constants.DebugLogFilepathKeyPref, InternalNGDebug.LogPath, true);
				}
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
			Preferences.assets = null;
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