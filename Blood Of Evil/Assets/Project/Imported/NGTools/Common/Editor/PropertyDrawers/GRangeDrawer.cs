using NGTools;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(GRangeAttribute), true)]
	internal sealed class GRangeDrawer : PropertyDrawer
	{
		public override float	GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (GroupDrawer.isMasterDrawing == false)
				return -2;
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (GroupDrawer.isMasterDrawing == false)
				return;

			if (property.propertyType == SerializedPropertyType.Float)
			{
				GRangeAttribute	attribute = (base.attribute as GRangeAttribute);
				EditorGUI.Slider(position, property, attribute.min, attribute.max);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				GRangeAttribute	attribute = (base.attribute as GRangeAttribute);
				EditorGUI.IntSlider(position, property, (int)attribute.min, (int)attribute.max);
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
			}
		}
	}
}