using NGTools;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(FileAttribute))]
	internal sealed class FileDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				using (LabelWidthRestorer.Get(EditorGUIUtility.labelWidth + 50F))
				{
					property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
				}

				position.x += EditorGUIUtility.labelWidth;
				position.width = 50F;
				if (GUI.Button(position, "File") == true)
				{
					FileAttribute	file = (this.attribute as FileAttribute);
					string			path;
					string			directoryPath = Application.dataPath;

					if (string.IsNullOrEmpty(property.stringValue) == false)
						directoryPath = Path.GetDirectoryName(property.stringValue);

					if (file.mode == FileAttribute.Mode.Open)
						path = EditorUtility.OpenFilePanel("File", directoryPath, file.extension);
					else
					{
						string	defaultFile = string.Empty;

						if (string.IsNullOrEmpty(property.stringValue) == false)
							defaultFile = Path.GetFileName(property.stringValue);

						path = EditorUtility.SaveFilePanel("File", directoryPath, defaultFile, file.extension);
					}

					if (string.IsNullOrEmpty(path) == false)
						property.stringValue = path;
				}
			}
			else
				EditorGUI.LabelField(position, "File attribute must be used on string.");
		}
	}
}