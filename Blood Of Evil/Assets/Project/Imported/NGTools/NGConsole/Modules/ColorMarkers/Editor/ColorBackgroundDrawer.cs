using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[CustomPropertyDrawer(typeof(ColorBackground))]
	internal sealed class ColorBackgroundDrawer : PropertyDrawer
	{
		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty	name = property.FindPropertyRelative("name");
			SerializedProperty	color = property.FindPropertyRelative("color");

			position.xMax -= 100F;
			name.stringValue = EditorGUI.TextField(position, name.stringValue);

			position.xMin = position.xMax;
			position.width = 100F;
			color.colorValue = EditorGUI.ColorField(position, color.colorValue);
		}
	}
}