#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
using UnityEditor.SceneManagement;
#endif

namespace NGToolsEditor.NGScenes
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGScenesWindow : EditorWindow
	{
		private class Scene
		{
			public string	path;
			public string	name;
			public Object	asset;

			public	Scene(string path)
			{
				this.path = path;
				this.name = Path.GetFileNameWithoutExtension(path);
				this.asset = AssetDatabase.LoadAssetAtPath(this.path, typeof(Object));
			}
		}

		public const string	Title = "NG Scenes";
		public const string	RecentScenesKey = "NGScenes_RecentScenes";
		public const char	SceneSeparator = ',';

		private GUIListDrawer<Scene>	recentListDrawer;
		private GUIListDrawer<Scene>	allListDrawer;
		private GUIListDrawer<EditorBuildSettingsScene>	buildListDrawer;
		
		private int		enabledScenesCounter = 0;
		private double	lastClick;

		private static List<Scene>	list = new List<Scene>();
		private static Scene[]		allScenes;

		static	NGScenesWindow()
		{
			NGEditorApplication.ChangeScene += NGScenesWindow.UpdateLastScenes;
			// Force update allScenes at next restart.
			EditorApplication.projectWindowChanged += () => NGScenesWindow.allScenes = null;
		}

		private static void	UpdateLastScenes()
		{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			string	lastScene = EditorApplication.currentScene;
#else
			string	lastScene = EditorSceneManager.GetActiveScene().path;
#endif
			if (string.IsNullOrEmpty(lastScene) == true)
				return;

			string	rawScenes = NGEditorPrefs.GetString(NGScenesWindow.RecentScenesKey, string.Empty, true);

			if (string.IsNullOrEmpty(rawScenes) == false)
			{
				string[]		scenes = rawScenes.Split(NGScenesWindow.SceneSeparator);
				List<string>	list = new List<string>(scenes.Length + 1);

				list.Add(lastScene);
				for (int i = 0; i < scenes.Length; i++)
				{
					if (scenes[i] != lastScene && File.Exists(scenes[i]) == true)
						list.Add(scenes[i]);
				}

				NGEditorPrefs.SetString(NGScenesWindow.RecentScenesKey, string.Join(NGScenesWindow.SceneSeparator.ToString(), list.ToArray()), true);
			}
			else
			{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				NGEditorPrefs.SetString(NGScenesWindow.RecentScenesKey, EditorApplication.currentScene, true);
#else
				NGEditorPrefs.SetString(NGScenesWindow.RecentScenesKey, EditorSceneManager.GetActiveScene().name, true);
#endif
			}
		}

		[MenuItem("File/" + NGScenesWindow.Title + " %G", priority = 0)]
		private static void	Open()
		{
			EditorWindow.GetWindow<NGScenesWindow>(true, NGScenesWindow.Title, true);
		}

		protected virtual void	OnEnable()
		{
			if (NGScenesWindow.allScenes == null)
			{
				string[]	allScenes = AssetDatabase.GetAllAssetPaths();

				list.Clear();
				for (int i = 0; i < allScenes.Length; i++)
				{
					if (allScenes[i].EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase) == true)
						list.Add(new Scene(allScenes[i]));
				}

				list.Sort((a, b) => a.name.CompareTo(b.name));
				NGScenesWindow.allScenes = list.ToArray();
			}

			this.allListDrawer = new GUIListDrawer<Scene>();
			this.allListDrawer.array = NGScenesWindow.allScenes;
			this.allListDrawer.ElementGUI = this.DrawSceneRow;

			this.recentListDrawer = new GUIListDrawer<Scene>();
			this.recentListDrawer.ElementGUI = this.DrawSceneRow;

			this.UpdateRecentScenes();

			this.buildListDrawer = new GUIListDrawer<EditorBuildSettingsScene>();
			this.buildListDrawer.drawBackgroundColor = true;
			this.buildListDrawer.handleSelection = true;
			this.buildListDrawer.handleDrag = true;
			this.buildListDrawer.ElementGUI = this.DrawBuildSceneRow;
			this.buildListDrawer.PostGUI = this.DropScene;
			this.buildListDrawer.DeleteSelection = this.DeleteBuildScenes;
			this.buildListDrawer.ArrayReordered = (l) => EditorBuildSettings.scenes = l.array;

			NGEditorApplication.ChangeScene += this.UpdateRecentScenes;

			this.wantsMouseMove = true;
		}

		protected virtual void	OnDestroy()
		{
			NGEditorApplication.ChangeScene -= this.UpdateRecentScenes;
		}

		protected virtual void	OnGUI()
		{
			if (this.maxSize == this.minSize)
			{
				if (GUI.Button(new Rect(this.position.width - 20F, 0F, 20F, 20F), "X") == true)
					this.Close();
			}

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(5F);

				GUILayout.BeginVertical(GUILayout.MinWidth(200F));
				{
					GUILayout.Label("Recent Scenes:", GeneralStyles.Title1);
					this.recentListDrawer.OnGUI(GUILayoutUtility.GetRect(0F, 100F));

					GUILayout.Label("All Scenes:", GeneralStyles.Title1);
					this.allListDrawer.OnGUI(GUILayoutUtility.GetRect(0F, 0F, GUILayout.ExpandHeight(true)));
				}
				GUILayout.EndVertical();

				GUILayout.Space(2F);

				GUILayout.BeginVertical();
				{
					GUILayout.Label("Build Scenes:", GeneralStyles.Title1);

					this.enabledScenesCounter = 0;
					this.buildListDrawer.array = EditorBuildSettings.scenes;
					this.buildListDrawer.OnGUI(GUILayoutUtility.GetRect(0F, 0F, GUILayout.ExpandHeight(true)));

					GUILayout.Space(5F);
				}
				GUILayout.EndVertical();

				GUILayout.Space(5F);
			}
			GUILayout.EndHorizontal();

			if (Event.current.type == EventType.MouseDown)
			{
				if (this.lastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
					this.Close();
				this.lastClick = EditorApplication.timeSinceStartup;
				Event.current.Use();
			}
		}

		private void	DrawSceneRow(Rect r, Scene scene, int i)
		{
			if (r.Contains(Event.current.mousePosition) == false)
				GUI.Label(r, scene.name, GeneralStyles.ToolbarButtonLeft);
			else
			{
				if (Event.current.type == EventType.MouseDrag &&
					Utility.position2D != Vector2.zero &&
					(Utility.position2D - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance)
				{
					DragAndDrop.StartDrag("Drag Object");
					// Dragging from a Button does some hidden stuff, messing up the Drag&Drop.
					EditorGUIUtility.hotControl = 0;
					Event.current.Use();
				}
				else if (Event.current.type == EventType.MouseDown)
				{
					Utility.position2D = Event.current.mousePosition;
					DragAndDrop.PrepareStartDrag();
					DragAndDrop.paths = new string[] { scene.path };
					DragAndDrop.objectReferences = new Object[] { scene.asset };
				}

				r.width -= 60F;
				if (GUI.Button(r, scene.name, GeneralStyles.ToolbarButtonLeft) == true && File.Exists(scene.path) == true)
				{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
					if (Event.current.control == true)
						EditorApplication.OpenSceneAdditive(scene.path);
					else
						EditorApplication.OpenScene(scene.path);
#else
					OpenSceneMode	mode = OpenSceneMode.Single;

					if (Event.current.control == true)
						mode = OpenSceneMode.Additive;
					else if (Event.current.alt == true)
						mode = OpenSceneMode.AdditiveWithoutLoading;
					EditorSceneManager.OpenScene(scene.path, mode);
#endif
				}

				r.x += r.width;
				r.width = 60F;
				if (GUI.Button(r, "+", EditorStyles.toolbarDropDown) == true)
				{
					GenericMenu	menu = new GenericMenu();

					menu.AddItem(new GUIContent("Load single"), false, this.LoadScene, scene);
					menu.AddItem(new GUIContent("Load additive"), false, this.LoadSceneAdditive, scene);
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
					menu.AddItem(new GUIContent("Load additive without loading"), false, this.LoadSceneAdditiveWithoutLoading, scene);
#endif
					menu.AddItem(new GUIContent("Ping"), false, this.PingScene, scene);

					menu.DropDown(r);
				}

				this.Repaint();
			}
		}

		private void	DrawBuildSceneRow(Rect r, EditorBuildSettingsScene scene, int i)
		{
			float	w = r.width - 4F;

			if (Event.current.type == EventType.Repaint && r.Contains(Event.current.mousePosition) == true)
			{
				if (DragAndDrop.visualMode == DragAndDropVisualMode.Move)
				{
					float	h = r.height;
					r.height = 1F;
					EditorGUI.DrawRect(r, Color.green);
					r.height = h;
				}
			}
			else if (Event.current.type == EventType.DragUpdated && r.Contains(Event.current.mousePosition) == true)
			{
				bool	one = false;

				for (int j = 0; j < DragAndDrop.paths.Length; j++)
				{
					if (DragAndDrop.paths[j].EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase) == true)
					{
						one = true;
						break;
					}
				}

				if (one == true)
					DragAndDrop.visualMode = DragAndDropVisualMode.Move;
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			}
			else if (Event.current.type == EventType.DragPerform && r.Contains(Event.current.mousePosition) == true)
			{
				if (DragAndDrop.paths.Length > 0)
				{
					DragAndDrop.AcceptDrag();

					List<EditorBuildSettingsScene>	scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

					for (int j = 0; j < DragAndDrop.paths.Length; j++)
					{
						if (DragAndDrop.paths[j].EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase) == true)
							scenes.Insert(i++, new EditorBuildSettingsScene(DragAndDrop.paths[j], true));
					}

					EditorBuildSettings.scenes = scenes.ToArray();

					Event.current.Use();
				}
			}

			GUI.enabled = File.Exists(scene.path);
			r.x += 4F;
			r.width = 20F;
			EditorGUI.BeginChangeCheck();
			bool	enabled = GUI.Toggle(r, scene.enabled, string.Empty);
			if (EditorGUI.EndChangeCheck() == true)
			{
				EditorBuildSettingsScene[]	scenes = EditorBuildSettings.scenes.Clone() as EditorBuildSettingsScene[];

				scenes[i] = new EditorBuildSettingsScene(scene.path, enabled);
				EditorBuildSettings.scenes = scenes;
			}

			string	path = scene.path;

			if (path.StartsWith("Assets/") == true)
				path = path.Substring("Assets/".Length);
			if (path.EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase) == true)
				path = path.Substring(0, path.Length - ".unity".Length);

			r.x += r.width;
			if (scene.enabled == true)
			{
				Utility.content.text = enabledScenesCounter.ToString();
				float	indexWidth = GUI.skin.label.CalcSize(Utility.content).x;
				r.width = w - r.x - indexWidth;
				GUI.Label(r, path);

				r.x += r.width;
				r.width = indexWidth;
				GUI.Label(r, this.enabledScenesCounter.ToString());
				++this.enabledScenesCounter;
			}
			else
			{
				r.width = w - r.x;
				GUI.Label(r, path);
			}

			GUI.enabled = true;
		}

		private void	DropScene(GUIListDrawer<EditorBuildSettingsScene> list)
		{
			if (Event.current.type == EventType.DragUpdated)
			{
				bool	one = false;

				for (int i = 0; i < DragAndDrop.paths.Length; i++)
				{
					if (DragAndDrop.paths[i].EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase) == true)
					{
						one = true;
						break;
					}
				}

				if (one == true)
					DragAndDrop.visualMode = DragAndDropVisualMode.Move;
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

				Event.current.Use();
			}
			else if (Event.current.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();

				List<EditorBuildSettingsScene>	scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

				for (int i = 0; i < DragAndDrop.paths.Length; i++)
				{
					if (DragAndDrop.paths[i].EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase) == true)
						scenes.Add(new EditorBuildSettingsScene(DragAndDrop.paths[i], true));
				}

				EditorBuildSettings.scenes = scenes.ToArray();

				Event.current.Use();
			}
		}

		private void	DeleteBuildScenes(GUIListDrawer<EditorBuildSettingsScene> list)
		{
			List<EditorBuildSettingsScene>	scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

			for (int i = 0; i < list.selection.Count; i++)
				scenes[list.selection[i]] = null;

			for (int i = 0; i < scenes.Count; i++)
			{
				if (scenes[i] == null)
				{
					scenes.RemoveAt(i);
					--i;
				}
			}

			EditorBuildSettings.scenes = scenes.ToArray();
		}

		private void	LoadScene(object data)
		{
			Scene	scene = data as Scene;
			bool	exist = File.Exists(scene.path);

			if (exist == true)
			{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				EditorApplication.OpenScene(scene.path);
#else
				EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
#endif
			}
		}

		private void	LoadSceneAdditive(object data)
		{
			Scene	scene = data as Scene;
			bool	exist = File.Exists(scene.path);

			if (exist == true)
			{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				EditorApplication.OpenSceneAdditive(scene.path);
#else
				EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
#endif
			}
		}

#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
		private void	LoadSceneAdditiveWithoutLoading(object data)
		{
			Scene	scene = data as Scene;
			bool	exist = File.Exists(scene.path);

			if (exist == true)
				EditorSceneManager.OpenScene(scene.path, OpenSceneMode.AdditiveWithoutLoading);
		}
#endif

		private void	PingScene(object data)
		{
			Scene	scene = data as Scene;

			EditorGUIUtility.PingObject(scene.asset);
		}

		private void	UpdateRecentScenes()
		{
			string		rawScenes = NGEditorPrefs.GetString(NGScenesWindow.RecentScenesKey, string.Empty, true);
			string[]	scenes = rawScenes.Split(NGScenesWindow.SceneSeparator);

			list.Clear();
			for (int i = 0; i < scenes.Length; i++)
				list.Add(new Scene(scenes[i]));

			this.recentListDrawer.array = list.ToArray();
		}
	}
}