using NGTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NGToolsEditor.NGSyncFolders
{
	[Serializable]
	public sealed class Profile
	{
		public string			name = string.Empty;
		public string			relativePath = string.Empty;
		public Project			master = new Project();
		public List<Project>	slaves = new List<Project>();

		public bool				useCache = true;
		public List<string>		inclusiveFilters = new List<string>();
		public List<string>		exclusiveFilters = new List<string>();
	}

	[InitializeOnLoad]
	public class NGSyncFoldersWindow : EditorWindow, IHasCustomMenu
	{
		private enum ButtonMode
		{
			Scan,
			ScanAndWatch
		}

		public const string	Title = "NG Ȿync Ḟolders";
		public const string	CachedHashedFile = "NGSyncFolders/{0}.txt";
		public static Color	CreateColor = Color.blue;
		public static Color	DeleteColor = Color.red;
		public static Color	RestoreColor = Color.green;

		private static string	cachePath;

		private static Dictionary<string, string[]>	cachedHashes;

		public Profile			profile = new Profile();
		public int				currentProfile = 0;

		[SerializeField]
		private ButtonMode	mode = ButtonMode.Scan;

		private ReorderableList	inclusiveList;
		private ReorderableList	exclusiveList;
		private Vector2			scrollPosition;
		private bool			showFilters;
		[NonSerialized]
		private string			cacheFileSize;

		static	NGSyncFoldersWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGSyncFoldersWindow.Title + "	[BETA]");
		}

		[MenuItem(Constants.MenuItemPath + NGSyncFoldersWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 380)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGSyncFoldersWindow>(NGSyncFoldersWindow.Title);
		}

		public static void	SaveCachedHashes()
		{
			StringBuilder	buffer = Utility.GetBuffer();

			foreach (var pair in NGSyncFoldersWindow.cachedHashes)
			{
				buffer.AppendLine(pair.Key); // File
				buffer.AppendLine(pair.Value[0]); // Last change time
				buffer.AppendLine(pair.Value[1]); // Hash
			}

			Directory.CreateDirectory(Path.GetDirectoryName(NGSyncFoldersWindow.cachePath));
			System.IO.File.WriteAllText(NGSyncFoldersWindow.cachePath, Utility.ReturnBuffer(buffer));
		}

		public static string	TryGetCachedHash(string file)
		{
			if (NGSyncFoldersWindow.cachedHashes == null)
				NGSyncFoldersWindow.LoadHashesCache();

			string[]	values;

			if (NGSyncFoldersWindow.cachedHashes.TryGetValue(file, out values) == true)
			{
				string	currentWriteTime = System.IO.File.GetLastWriteTime(file).Ticks.ToString();

				if (currentWriteTime != values[0])
				{
					values[0] = currentWriteTime;
					values[1] = NGSyncFoldersWindow.ProcessHash(file);
				}

				return values[1];
			}

			string	hash = NGSyncFoldersWindow.ProcessHash(file);
			NGSyncFoldersWindow.cachedHashes.Add(file, new string[] { System.IO.File.GetLastWriteTime(file).Ticks.ToString(), hash });
			return hash;
		}

		private static void	LoadHashesCache()
		{
			if (System.IO.File.Exists(NGSyncFoldersWindow.cachePath) == true)
			{
				try
				{
					string[]	lines = System.IO.File.ReadAllLines(NGSyncFoldersWindow.cachePath);

					if (NGSyncFoldersWindow.cachedHashes == null)
						NGSyncFoldersWindow.cachedHashes = new Dictionary<string, string[]>(lines.Length / 3);

					for (int i = 0; i < lines.Length; i += 3)
						NGSyncFoldersWindow.cachedHashes.Add(lines[i], new string[] { lines[i + 1], lines[i + 2] });
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException(ex);
					NGSyncFoldersWindow.cachedHashes.Clear();
				}
			}
			else if (NGSyncFoldersWindow.cachedHashes == null)
				NGSyncFoldersWindow.cachedHashes = new Dictionary<string, string[]>();
		}

		private static string	ProcessHash(string path)
		{
			if (System.IO.File.Exists(path) == false)
				return string.Empty;

			try
			{
				using (var md5 = MD5.Create())
				using (var stream = System.IO.File.OpenRead(path))
					return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","‌​").ToLower();
			}
			catch
			{
			}

			return string.Empty;
		}

		protected virtual void	OnEnable()
		{
			Utility.LoadEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());

			this.inclusiveList = new ReorderableList(null, typeof(string), true, true, true, true);
			this.inclusiveList.drawHeaderCallback = (r) => GUI.Label(r, "Inclusive Filters");
			this.inclusiveList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				EditorGUI.BeginChangeCheck();
				string	filter = EditorGUI.TextField(rect, Preferences.Settings.syncProfiles[this.currentProfile].inclusiveFilters[index]);
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Alter inclusive filter");
					Preferences.Settings.syncProfiles[this.currentProfile].inclusiveFilters[index] = filter;
					Preferences.InvalidateSettings();
				}
			};
			this.inclusiveList.onAddCallback = (r) => r.list.Add(r.list.Count > 0 ? r.list[r.list.Count - 1] : string.Empty);

			this.exclusiveList = new ReorderableList(null, typeof(string), true, true, true, true);
			this.exclusiveList.drawHeaderCallback = (r) => GUI.Label(r, "Exclusive Filters");
			this.exclusiveList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				EditorGUI.BeginChangeCheck();
				string	filter = EditorGUI.TextField(rect, Preferences.Settings.syncProfiles[this.currentProfile].exclusiveFilters[index]);
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Alter exclusive filter");
					Preferences.Settings.syncProfiles[this.currentProfile].exclusiveFilters[index] = filter;
					Preferences.InvalidateSettings();
				}
			};
			this.exclusiveList.onAddCallback = (r) => r.list.Add(r.list.Count > 0 ? r.list[r.list.Count - 1] : string.Empty);

			profile.master.FileChanged += this.ReplicateOnSlaves;

			Preferences.SettingsChanged += this.Repaint;
			Utility.RegisterIntervalCallback(this.Repaint, 250);
			Undo.undoRedoPerformed += this.Repaint;
		}

		protected virtual void	OnDisable()
		{
			profile.master.FileChanged -= this.ReplicateOnSlaves;

			profile.master.Dispose();
			for (int i = 0; i < profile.slaves.Count; i++)
				profile.slaves[i].Dispose();

			Utility.SaveEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
			Preferences.SettingsChanged -= this.Repaint;
			Utility.UnregisterIntervalCallback(this.Repaint);
			Undo.undoRedoPerformed -= this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			if (Preferences.Settings == null)
			{
				GUILayout.Label(string.Format(LC.G("RequiringConfigurationFile"), NGSyncFoldersWindow.Title));
				if (GUILayout.Button(LC.G("ShoWPreferencesWindow")) == true)
					Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);
				return;
			}

			FreeOverlay.First(this, NGSyncFoldersWindow.Title + " is restrained to:\n" +
							  "• " + FreeConstants.MaxSyncFoldersProfiles + " profiles.\n" +
							  "• " + FreeConstants.MaxSyncFoldersSlaves + " slaves.");

			// Guarantee there is always one in the list.
			if (Preferences.Settings.syncProfiles.Count == 0)
				Preferences.Settings.syncProfiles.Add(new Profile() { name = "Profile 1" });

			this.currentProfile = Mathf.Clamp(this.currentProfile, 0, Preferences.Settings.syncProfiles.Count - 1);

			Profile	profile = Preferences.Settings.syncProfiles[this.currentProfile];

			if (NGSyncFoldersWindow.cachePath == null)
				NGSyncFoldersWindow.cachePath = Path.Combine(Application.persistentDataPath, Path.Combine(Constants.InternalPackageTitle, string.Format(NGSyncFoldersWindow.CachedHashedFile, this.currentProfile)));

			this.inclusiveList.list = profile.inclusiveFilters;
			this.exclusiveList.list = profile.exclusiveFilters;

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button("", GeneralStyles.ToolbarDropDown, GUILayout.Width(20F)) == true)
				{
					GenericMenu	menu = new GenericMenu();

					for (int i = 0; i < Preferences.Settings.syncProfiles.Count; i++)
						menu.AddItem(new GUIContent((i + 1) + " - " + Preferences.Settings.syncProfiles[i].name), i == this.currentProfile, this.SwitchProfile, i);

					menu.AddSeparator("");
					menu.AddItem(new GUIContent(LC.G("Add")), false, this.AddProfile);

					Rect	r = GUILayoutUtility.GetLastRect();
					r.y += 16F;
					menu.DropDown(r);
					GUI.FocusControl(null);
				}

				EditorGUI.BeginChangeCheck();
				string	name = EditorGUILayout.TextField(profile.name, GeneralStyles.ToolbarTextField, GUILayout.ExpandWidth(true));
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Rename profile");
					profile.name = name;
					Preferences.InvalidateSettings();
				}

				EditorGUI.BeginDisabledGroup(Preferences.Settings.syncProfiles.Count <= 1);
				if (GUILayout.Button(LC.G("Erase"), GeneralStyles.ToolbarButton) == true &&
					((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 ||
					 EditorUtility.DisplayDialog(LC.G("NGSyncFolders_EraseSave"), string.Format(LC.G("NGSyncFolders_EraseSaveQuestion"), profile.name), LC.G("Yes"), LC.G("No")) == true))
				{
					Undo.RecordObject(Preferences.Settings, "Erase profile");
					Preferences.Settings.syncProfiles.RemoveAt(this.currentProfile);
					this.UpdateCacheFiles(this.currentProfile);
					this.currentProfile = Mathf.Clamp(this.currentProfile, 0, Preferences.Settings.syncProfiles.Count - 1);
					this.cacheFileSize = null;
					NGSyncFoldersWindow.cachePath = null;
					Preferences.InvalidateSettings();
					this.Focus();
					return;
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();

			using (LabelWidthRestorer.Get(85F))
			{
				GUILayout.Space(5F);

				EditorGUI.BeginChangeCheck();
				string	folderPath = NGEditorGUILayout.OpenFolderField("Master Folder", profile.master.folderPath);
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Alter master path");
					profile.master.folderPath = folderPath;
					Preferences.InvalidateSettings();
				}

				EditorGUI.BeginChangeCheck();
				string	relativePath = NGEditorGUILayout.OpenFolderField("Relative Path", profile.relativePath, NGEditorGUILayout.FieldButtons.Open);
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Alter relative path");
					profile.relativePath = relativePath;
					Preferences.InvalidateSettings();
				}

				string	path = Path.Combine(profile.master.folderPath, profile.relativePath);
				if (Directory.Exists(path) == false)
					EditorGUILayout.HelpBox("Master at \"" + path + "\" does not exist.", MessageType.Error);

				for (int i = 0; i < profile.slaves.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUI.BeginChangeCheck();
						bool	active = EditorGUILayout.ToggleLeft("Slave Folder", profile.slaves[i].active, GUILayout.Width(90F));
						if (EditorGUI.EndChangeCheck() == true)
						{
							Undo.RecordObject(Preferences.Settings, "Toggle slave");
							profile.slaves[i].active = active;
							Preferences.InvalidateSettings();
						}

						EditorGUI.BeginDisabledGroup(profile.slaves[i].active == false);
						EditorGUI.BeginChangeCheck();
						folderPath = NGEditorGUILayout.OpenFolderField("", profile.slaves[i].folderPath);
						if (EditorGUI.EndChangeCheck() == true)
						{
							Undo.RecordObject(Preferences.Settings, "Alter slave path");
							profile.slaves[i].folderPath = folderPath;
							Preferences.InvalidateSettings();
						}
						EditorGUI.EndDisabledGroup();

						if (GUILayout.Button("X", GUILayout.ExpandWidth(false)) == true)
						{
							Undo.RecordObject(Preferences.Settings, "Delete slave");
							profile.slaves.RemoveAt(i);
							Preferences.InvalidateSettings();
							return;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}

			GUILayout.Space(5F);

			if (GUILayout.Button("Add Slave") == true)
			{
				if (FreeConstants.CheckMaxSyncFoldersSlaves(profile.slaves.Count) == true)
				{
					Undo.RecordObject(Preferences.Settings, "Add slave");
					profile.slaves.Add(new Project() { folderPath = profile.slaves.Count > 0 ? profile.slaves[profile.slaves.Count - 1].folderPath : string.Empty });
					Preferences.InvalidateSettings();
				}
			}

			GUILayout.Space(5F);

			EditorGUILayout.BeginVertical("ButtonLeft");
			{
				this.showFilters = EditorGUILayout.Foldout(this.showFilters, "Filters");
				if (this.showFilters == true)
				{
					this.inclusiveList.DoLayoutList();
					this.exclusiveList.DoLayoutList();
				}
			}
			EditorGUILayout.EndVertical();

			if (string.IsNullOrEmpty(this.cacheFileSize) == true)
				this.UpdateCacheFileSize();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.BeginChangeCheck();
				bool	useCache = EditorGUILayout.Toggle(this.cacheFileSize, profile.useCache);
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Toggle use cache");
					profile.useCache = useCache;
					Preferences.InvalidateSettings();
				}

				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Open", "ButtonLeft", GUILayout.ExpandWidth(false)) == true)
					Utility.ShowExplorer(NGSyncFoldersWindow.cachePath);
				if (GUILayout.Button("Clear", "ButtonRight", GUILayout.ExpandWidth(false)) == true)
				{
					System.IO.File.Delete(NGSyncFoldersWindow.cachePath);
					this.UpdateCacheFileSize();
				}
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(5F);

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(10F);

				using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
				{
					if (GUILayout.Button(this.mode == ButtonMode.Scan ? "Scan" : "Scan & Watch", "ButtonLeft", GUILayout.Height(32F), GUILayout.Width(150F)) == true)
					{
						try
						{
							EditorUtility.DisplayProgressBar(NGSyncFoldersWindow.Title, "Loading cache...", 0F);
							if (profile.useCache == false)
							{
								if (NGSyncFoldersWindow.cachedHashes == null)
									NGSyncFoldersWindow.cachedHashes = new Dictionary<string, string[]>();
								else
									NGSyncFoldersWindow.cachedHashes.Clear();
							}
							else
							{
								NGSyncFoldersWindow.cachedHashes = null;
								NGSyncFoldersWindow.LoadHashesCache();
							}

							EditorUtility.DisplayProgressBar(NGSyncFoldersWindow.Title, "Scanning master...", 1F / (2F + profile.slaves.Count));
							profile.master.Scan(this.mode == ButtonMode.ScanAndWatch, profile.relativePath, profile.inclusiveFilters, profile.exclusiveFilters);
							for (int i = 0; i < profile.slaves.Count; i++)
							{
								EditorUtility.DisplayProgressBar(NGSyncFoldersWindow.Title, "Scanning slave " + i + "...", (i + 2F) / (2F + profile.slaves.Count));
								if (profile.slaves[i].active == true)
									profile.slaves[i].ScanDiff(this.mode == ButtonMode.ScanAndWatch, profile.master, profile.relativePath, profile.inclusiveFilters, profile.exclusiveFilters);
								else
									profile.slaves[i].Dispose();
							}

							if (profile.useCache == true)
							{
								NGSyncFoldersWindow.SaveCachedHashes();
								this.UpdateCacheFileSize();
							}
						}
						finally
						{
							EditorUtility.ClearProgressBar();
						}
					}

					if (GUILayout.Button("☰", "ButtonRight", GUILayout.Height(32F), GUILayout.ExpandWidth(false)) == true)
					{
						GenericMenu	menu = new GenericMenu();
						menu.AddItem(new GUIContent("Scan"), this.mode == ButtonMode.Scan, () => this.mode = ButtonMode.Scan);
						menu.AddItem(new GUIContent("Scan and Watch"), this.mode == ButtonMode.ScanAndWatch, () => this.mode = ButtonMode.ScanAndWatch);
						menu.ShowAsContext();
					}
				}

				GUILayout.FlexibleSpace();

				if (profile.master.IsScanned == true)
				{
					using (BgColorContentRestorer.Get(GeneralStyles.HighlightResultButton))
					{
						if (GUILayout.Button("Sync All", "Button", GUILayout.Height(32F), GUILayout.Width(150F)) == true)
						{
							for (int i = 0; i < profile.slaves.Count; i++)
							{
								if (profile.slaves[i].IsScanned == true)
									profile.slaves[i].SyncAll(profile.master);
							}
							profile.master.Reset();
						}
					}
					GUILayout.Space(10F);
				}
			}
			EditorGUILayout.EndHorizontal();

			if (this.mode == ButtonMode.ScanAndWatch)
				EditorGUILayout.HelpBox("Scan and Watch may induce huge freeze after a compilation when watching a lot of files. This mode is not recommended for programmers.", MessageType.Warning);
			 
			if (profile.master.IsScanned == true)
			{
				GUILayout.Space(3F);

				Rect	bodyRect = GUILayoutUtility.GetRect(0F, 0F, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				Rect	viewRect = new Rect(0F, 0F, bodyRect.width, profile.master.GetHeight());

				for (int i = 0; i < profile.slaves.Count; i++)
					viewRect.height += profile.slaves[i].GetHeight(profile.master);

				this.scrollPosition = GUI.BeginScrollView(bodyRect, this.scrollPosition, viewRect);
				{
					Rect	r = new Rect();
					r.y = 0F;
					r.x = 0F;
					r.width = bodyRect.width;
					r.height = profile.master.GetHeight();

					if (viewRect.height > bodyRect.height)
						r.width -= 15F;

					if (r.yMax > this.scrollPosition.y)
						profile.master.OnGUI(r, this.scrollPosition.y, this.scrollPosition.y + bodyRect.height);

					r.y += r.height;

					for (int i = 0; i < profile.slaves.Count; i++)
					{
						r.height = profile.slaves[i].GetHeight(profile.master);

						if (r.yMax > this.scrollPosition.y)
							profile.slaves[i].OnGUI(r, this.scrollPosition.y, this.scrollPosition.y + bodyRect.height, profile.master);

						r.y += r.height;

						if (r.y - this.scrollPosition.y > bodyRect.height)
							break;
					}
				}
				GUI.EndScrollView();
			}

			FreeOverlay.Last();
		}

		private void	UpdateCacheFiles(int currentProfile)
		{
			string	directory = Path.GetDirectoryName(NGSyncFoldersWindow.cachePath);

			System.IO.File.Delete(NGSyncFoldersWindow.cachePath);

			List<string>	files = new List<string>(Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly));

			try
			{
				// Make sure they are ordered, to downgrade file by file.
				files.Sort();

				for (int i = 0; i < files.Count; i++)
				{
					string	number = Path.GetFileNameWithoutExtension(files[i].Substring(directory.Length));
					int		n = int.Parse(number);

					if (n >= currentProfile)
						System.IO.File.Move(files[i], Path.Combine(directory, --n + ".txt"));
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private void	UpdateCacheFileSize()
		{
			if (System.IO.File.Exists(NGSyncFoldersWindow.cachePath) == false)
				this.cacheFileSize = "Use Cache";
			else
			{
				long	size = new FileInfo(NGSyncFoldersWindow.cachePath).Length;

				if (size >= 1000L * 1000L)
					this.cacheFileSize = "Use Cache (" + ((float)(size / (1000F * 1000F))).ToString("N2") + " MB)";
				else if (size >= 1000L)
					this.cacheFileSize = "Use Cache (" + ((float)(size / 1000F)).ToString("N2") + " KB)";
				else
					this.cacheFileSize = "Use Cache (" + size + " B)";
			}
		}

		private void	ReplicateOnSlaves(File master)
		{
			for (int i = 0; i < profile.slaves.Count; i++)
			{
				if (profile.slaves[i].IsScanned == false)
					continue;

				string	slavePath = master.path.Replace(profile.master.folderPath, profile.slaves[i].folderPath);
				File	slave = profile.slaves[i].FindClosest(slavePath);

				if (master.masterState == MasterStates.Deleted)
				{
					if (slave.initialState != InitialStates.Origin)
						slave.Delete();
				}
				else
					profile.slaves[i].Generate(master.path);
			}
		}

		private void	SwitchProfile(object data)
		{
			Undo.RecordObject(this, "Switch profile");
			this.currentProfile = Mathf.Clamp((int)data, 0, Preferences.Settings.syncProfiles.Count - 1);
			this.cacheFileSize = null;
			NGSyncFoldersWindow.cachePath = null;
		}

		private void	AddProfile()
		{
			if (FreeConstants.CheckMaxSyncFoldersProfiles(Preferences.Settings.syncProfiles.Count) == true)
			{
				Undo.RecordObject(Preferences.Settings, "Add profile");
				Preferences.Settings.syncProfiles.Add(new Profile() { name = "Profile " + (Preferences.Settings.syncProfiles.Count + 1) });
				this.currentProfile = Preferences.Settings.syncProfiles.Count - 1;
				this.cacheFileSize = null;
				NGSyncFoldersWindow.cachePath = null;
				Preferences.InvalidateSettings();
			}
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGSyncFoldersWindow.Title, Constants.WikiBaseURL + "#markdown-header-115-ng-sync-folders");
		}
	}
}