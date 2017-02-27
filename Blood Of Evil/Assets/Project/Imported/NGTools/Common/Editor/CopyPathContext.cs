using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[InitializeOnLoad]
	internal static class CopyPathContext
	{
		static	CopyPathContext()
		{
			Utility.AddMenuItemPicker("GameObject/Copy/Asset Name");
			Utility.AddMenuItemPicker("GameObject/Copy/Hierarchy");
			Utility.AddMenuItemPicker("Assets/Copy/Relative Path");
			Utility.AddMenuItemPicker("Assets/Copy/Absolute Path");
			Utility.AddMenuItemPicker("Assets/Copy/Asset Name");
		}

		[MenuItem("Assets/Copy/Relative Path")]
		private static void	AssetsRelativeCopy(MenuCommand menuCommand)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			for (int i = 0; i < Selection.objects.Length; i++)
			{
				if (Selection.objects[i] != null)
					buffer.AppendLine(AssetDatabase.GetAssetPath(Selection.objects[i]));
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}

		[MenuItem("Assets/Copy/Absolute Path")]
		private static void	AssetsAbsoluteCopy(MenuCommand menuCommand)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			for (int i = 0; i < Selection.objects.Length; i++)
			{
				if (Selection.objects[i] != null)
				{
					buffer.Append(Application.dataPath, 0, Application.dataPath.Length - "Assets".Length);
					buffer.AppendLine(AssetDatabase.GetAssetPath(Selection.objects[i]));
				}
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}

		[MenuItem("Assets/Copy/Asset Name")]
		private static void	AssetsCopyAssetName(MenuCommand menuCommand)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			for (int i = 0; i < Selection.objects.Length; i++)
			{
				if (Selection.objects[i] != null)
					buffer.AppendLine(Selection.objects[i].name);
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}

		[MenuItem("GameObject/Copy/Hierarchy")]
		private static void	GameObjectCopyHierarchy(MenuCommand menuCommand)
		{
			StringBuilder	buffer = Utility.GetBuffer();
			Stack<string>	hierarchy = new Stack<string>(4);

			for (int i = 0; i < Selection.gameObjects.Length; i++)
			{
				if (Selection.gameObjects[i] != null)
				{
					Transform	t = Selection.gameObjects[i].transform;

					while (t != null)
					{
						hierarchy.Push(t.name);
						t = t.parent;
					}

					while (hierarchy.Count > 0)
					{
						buffer.Append(hierarchy.Pop());
						buffer.Append('/');
					}

					buffer.Length -= 1;
					buffer.Append(Environment.NewLine);
				}
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}

		[MenuItem("GameObject/Copy/Asset Name", priority = 12)]
		private static void	GameObjectCopyAssetName(MenuCommand menuCommand)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			for (int i = 0; i < Selection.objects.Length; i++)
			{
				if (Selection.objects[i] != null)
					buffer.AppendLine(Selection.objects[i].name);
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}
	}
}