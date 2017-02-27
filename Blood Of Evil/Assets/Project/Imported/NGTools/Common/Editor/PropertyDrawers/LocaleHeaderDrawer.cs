using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(LocaleHeaderAttribute))]
	internal sealed class LocaleHeaderDrawer : DecoratorDrawer
	{
		public override void	OnGUI(Rect position)
		{
			string	content = LC.G((base.attribute as LocaleHeaderAttribute).key);

			if (string.IsNullOrEmpty(content) == true)
				return;

			float	h = (base.attribute as LocaleHeaderAttribute).height;

			if (h - EditorStyles.boldLabel.lineHeight >= 0F)
				position.y += h - EditorStyles.boldLabel.lineHeight;
			position = EditorGUI.IndentedRect(position);
			GUI.Label(position, LC.G((base.attribute as LocaleHeaderAttribute).key), EditorStyles.boldLabel);
		}

		public override float	GetHeight()
		{
			string	content = LC.G((base.attribute as LocaleHeaderAttribute).key);

			if (string.IsNullOrEmpty(content) == false)
				return (base.attribute as LocaleHeaderAttribute).height;
			return 0F;
		}
	}
}