using NGTools;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	internal sealed class EnumMaskDrawer : PropertyDrawer
	{
		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Enum)
			{
				EditorGUI.BeginChangeCheck();
				int	value = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
				if (EditorGUI.EndChangeCheck() == true)
					property.intValue = value;
			}
			else
			{
				EditorGUI.LabelField(position, "Field must be an Enum.");
			}
		}
	}
}