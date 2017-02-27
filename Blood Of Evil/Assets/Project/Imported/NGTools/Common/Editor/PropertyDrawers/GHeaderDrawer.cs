using NGTools;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(GHeaderAttribute), true)]
	internal sealed class GHeaderDrawer : DecoratorDrawer
	{
		public override float	GetHeight()
		{
			if (GroupDrawer.isMasterDrawing == false &&
				(this.attribute as GHeaderAttribute).first == false)
				return 0;
			return 24F;
		}

		public override void	OnGUI(Rect position)
		{
			if (GroupDrawer.isMasterDrawing == false &&
				(this.attribute as GHeaderAttribute).first == false)
				return;

			position.y += 8f;
			position = EditorGUI.IndentedRect(position);
			GUI.Label(position, (base.attribute as GHeaderAttribute).header, EditorStyles.boldLabel);
		}
	}
}