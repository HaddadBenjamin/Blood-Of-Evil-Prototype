using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class FontSize12Theme : Theme
	{
		public override void	SetTheme(NGSettings instance)
		{
			instance.log.style.alignment = TextAnchor.MiddleLeft;
			instance.log.style.fontSize = 12;
			instance.log.height = 20F;

			instance.log.timeStyle.fontSize = 11;

			instance.log.foldoutStyle.padding.left = 20;
			instance.log.foldoutStyle.fixedWidth = instance.log.height;

			instance.log.collapseLabelStyle.fixedHeight = 16F;

			instance.log.ResetFoldoutStyles();

			instance.stackTrace.height = 16F;
			instance.stackTrace.style.fontSize = 12;

			instance.stackTrace.previewHeight = 20F;
			instance.stackTrace.previewSourceCodeStyle.alignment = TextAnchor.MiddleLeft;
			instance.stackTrace.previewSourceCodeStyle.fontSize = 12;
		}
	}
}