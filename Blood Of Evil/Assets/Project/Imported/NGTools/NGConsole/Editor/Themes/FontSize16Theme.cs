using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class FontSize16Theme : Theme
	{
		public override void	SetTheme(NGSettings instance)
		{
			instance.log.style.alignment = TextAnchor.MiddleLeft;
			instance.log.style.fontSize = 16;
			instance.log.height = 24F;

			instance.log.timeStyle.fontSize = 12;

			instance.log.foldoutStyle.padding.left = 24;
			instance.log.foldoutStyle.fixedWidth = instance.log.height;

			instance.log.collapseLabelStyle.fixedHeight = 16F;

			instance.log.ResetFoldoutStyles();

			instance.stackTrace.height = 20F;
			instance.stackTrace.style.fontSize = 14;

			instance.stackTrace.previewHeight = 24F;
			instance.stackTrace.previewSourceCodeStyle.alignment = TextAnchor.MiddleLeft;
			instance.stackTrace.previewSourceCodeStyle.fontSize = 16;
		}
	}
}