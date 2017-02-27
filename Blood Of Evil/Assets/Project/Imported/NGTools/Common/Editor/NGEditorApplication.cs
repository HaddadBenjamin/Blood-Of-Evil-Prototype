#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using NGTools;
using System;
using UnityEditor;
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public static class NGEditorApplication
	{
		public static event Action	ChangeScene;
		public static event Action	EditorExit;

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		private static string	currentScene;
#else
		private static Scene	scene;
#endif
		private static int		frameCount;
		private static float	realtimeSinceStartup;
		private static int		renderedFrameCount;

		static	NGEditorApplication()
		{
			EditorApplication.update += NGEditorApplication.DetectChangeScene;

			EditorApplication.delayCall += () => {
				GameObject[]	gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

				for (int i = 0; i < gameObjects.Length; i++)
				{
					if (gameObjects[i].name.Equals("NGEditorExit") == true)
						Object.DestroyImmediate(gameObjects[i]);
				}

				GameObject	gameObject = new GameObject("NGEditorExit", typeof(EditorExitBehaviour));
				gameObject.hideFlags =
#if UNITY_4
				HideFlags.DontSave |
#else
				HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild |
#endif
				HideFlags.HideInHierarchy;
				gameObject.GetComponent<EditorExitBehaviour>().callback = () =>
				{
					if (NGEditorApplication.EditorExit != null)
						NGEditorApplication.EditorExit();
				};
			};
		}

		private static void	DetectChangeScene()
		{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			if (NGEditorApplication.currentScene != EditorApplication.currentScene ||
				// Detect change with the same scene, the real time should fill 99% of cases. Not tested but if you are able to load 2 scenes in a single frame, the time might not work.
				NGEditorApplication.realtimeSinceStartup > Time.realtimeSinceStartup ||
				NGEditorApplication.frameCount > Time.frameCount ||
				NGEditorApplication.renderedFrameCount > Time.renderedFrameCount)
			{
				NGEditorApplication.currentScene = EditorApplication.currentScene;

				if (NGEditorApplication.ChangeScene != null)
					NGEditorApplication.ChangeScene();
			}

			NGEditorApplication.frameCount = Time.frameCount;
			NGEditorApplication.realtimeSinceStartup = Time.realtimeSinceStartup;
			NGEditorApplication.renderedFrameCount = Time.renderedFrameCount;
#else
			if (NGEditorApplication.scene != EditorSceneManager.GetActiveScene() ||
				// Detect change with the same scene, the real time should fill 99% of cases. Not tested but if you are able to load 2 scenes in a single frame, the time might not work.
				NGEditorApplication.realtimeSinceStartup > Time.realtimeSinceStartup ||
				NGEditorApplication.frameCount > Time.frameCount ||
				NGEditorApplication.renderedFrameCount > Time.renderedFrameCount)
			{
				NGEditorApplication.scene = EditorSceneManager.GetActiveScene();

				if (NGEditorApplication.ChangeScene != null)
					NGEditorApplication.ChangeScene();
			}

			NGEditorApplication.frameCount = Time.frameCount;
			NGEditorApplication.realtimeSinceStartup = Time.realtimeSinceStartup;
			NGEditorApplication.renderedFrameCount = Time.renderedFrameCount;
#endif
		}
	}
}