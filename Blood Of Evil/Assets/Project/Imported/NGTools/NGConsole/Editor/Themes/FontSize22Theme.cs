using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class FontSize22Theme : Theme
	{
		public override void	SetTheme(NGSettings instance)
		{
			instance.log.style.alignment = TextAnchor.MiddleLeft;
			instance.log.style.fontSize = 22;
			instance.log.height = 32F;

			instance.log.timeStyle.fontSize = 14;

			instance.log.foldoutStyle.padding.left = 32;
			instance.log.foldoutStyle.fixedWidth = instance.log.height;

			instance.log.collapseLabelStyle.fixedHeight = 16F;

			instance.log.ResetFoldoutStyles();

			instance.stackTrace.height = 24F;
			instance.stackTrace.style.fontSize = 16;

			instance.stackTrace.previewHeight = 32F;
			instance.stackTrace.previewSourceCodeStyle.alignment = TextAnchor.MiddleLeft;
			instance.stackTrace.previewSourceCodeStyle.fontSize = 22;
		}
	}
}