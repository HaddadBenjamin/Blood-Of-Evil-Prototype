using NGTools;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(HideIfAttribute))]
	internal sealed class HideIfDrawer : PropertyDrawer
	{
		private ConditionalRenderer	renderer;

		public override float	GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (this.renderer == null)
				this.renderer = new ConditionalRenderer("HideIf", this, base.GetPropertyHeight, false);

			return this.renderer.GetPropertyHeight(property, label);
		}

		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.renderer.OnGUI(position, property, label);
		}
	}
}