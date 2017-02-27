using System;
using System.IO;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	public static class NGEditorGUILayout
	{
		[Flags]
		public enum FieldButtons
		{
			None = 0,
			All = Browse | Open,
			Browse = 1,
			Open = 2
		}

		public static string	SaveFileField(string label, string path, string defaultName = "", string extension = "", FieldButtons buttons = FieldButtons.All)
		{
			EditorGUILayout.BeginHorizontal();
			{
				path = EditorGUILayout.TextField(label, path);

				if ((buttons & FieldButtons.Browse) != 0)
				{
					if (GUILayout.Button("Browse", buttons == FieldButtons.All ? "ButtonLeft" : GUI.skin.button, GUILayout.ExpandWidth(false)) == true)
					{
						string	directory = string.IsNullOrEmpty(path) == false ? Path.GetDirectoryName(path) : string.Empty;
						string	projectPath = EditorUtility.SaveFilePanel(label, directory, defaultName, extension);

						if (string.IsNullOrEmpty(projectPath) == false)
						{
							path = projectPath;
							GUI.FocusControl(null);
						}
					}
				}

				if ((buttons & FieldButtons.Open) != 0)
				{
					if (GUILayout.Button("Open", buttons == FieldButtons.All ? "ButtonRight" : GUI.skin.button, GUILayout.ExpandWidth(false)) == true)
						Utility.ShowExplorer(path);
				}
			}
			EditorGUILayout.EndHorizontal();

			return path;
		}

		public static string	OpenFolderField(string label, string path, FieldButtons buttons = FieldButtons.All)
		{
			EditorGUILayout.BeginHorizontal();
			{
				path = EditorGUILayout.TextField(label, path);

				if ((buttons & FieldButtons.Browse) != 0)
				{
					if (GUILayout.Button("Browse", buttons == FieldButtons.All ? "ButtonLeft" : GUI.skin.button, GUILayout.ExpandWidth(false)) == true)
					{
						string	projectPath = EditorUtility.OpenFolderPanel(label, path, string.Empty);

						if (string.IsNullOrEmpty(projectPath) == false)
						{
							path = projectPath;
							GUI.FocusControl(null);
						}
					}
				}

				if ((buttons & FieldButtons.Open) != 0)
				{
					if (GUILayout.Button("Open", buttons == FieldButtons.All ? "ButtonRight" : GUI.skin.button, GUILayout.ExpandWidth(false)) == true)
						Utility.ShowExplorer(path);
				}
			}
			EditorGUILayout.EndHorizontal();

			return path;
		}

		private static double		lastClick;
		private static GUIContent	content = new GUIContent();

		public static int	PingObject(string label, UnityEngine.Object asset, params GUILayoutOption[] options)
		{
			NGEditorGUILayout.content.text = label;
			return NGEditorGUILayout.PingObject(NGEditorGUILayout.content, asset, null, options);
		}

		public static int	PingObject(string label, UnityEngine.Object asset, GUIStyle style, params GUILayoutOption[] options)
		{
			NGEditorGUILayout.content.text = label;
			return NGEditorGUILayout.PingObject(NGEditorGUILayout.content, asset, style, options);
		}

		public static int	PingObject(GUIContent content, UnityEngine.Object asset, GUIStyle style, params GUILayoutOption[] options)
		{
			if (style == null)
				style = GUI.skin.button;

			if (GUILayout.Button(content, style, options) == true)
				return NGEditorGUILayout.PingObject(asset);
			return 0;
		}

		public static int	PingObject(Object asset)
		{
			if (Event.current.button == 1 || NGEditorGUILayout.lastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
			{
				Selection.activeObject = asset;
				return 2;
			}
			else
			{
				EditorGUIUtility.PingObject(asset);
				NGEditorGUILayout.lastClick = EditorApplication.timeSinceStartup;
				return 1;
			}
		}
	}
}