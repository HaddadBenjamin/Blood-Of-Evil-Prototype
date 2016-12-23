#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

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
		[ExecuteInEditMode]
		private class EditorExitBehaviour : MonoBehaviour
		{
			protected virtual void	OnDestroy()
			{
				if (NGEditorApplication.EditorExit != null)
					NGEditorApplication.EditorExit();
			}
		}

		public static Action	ChangeScene;
		public static Action	EditorExit;

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
				if (Resources.FindObjectsOfTypeAll<EditorExitBehaviour>().Length == 0)
				{
					GameObject	gameObject = new GameObject("NGEditorExit", typeof(EditorExitBehaviour));
					gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
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