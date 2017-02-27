using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class DefaultFontSizeTheme : Theme
	{
		public override void	SetTheme(NGSettings instance)
		{
			instance.log.style.alignment = TextAnchor.MiddleLeft;
			instance.log.style.fontSize = 0;
			instance.log.height = 16F;

			instance.log.timeStyle.fontSize = 0;

			instance.log.foldoutStyle.padding.left = 16;
			instance.log.foldoutStyle.fixedWidth = instance.log.height;

			instance.log.collapseLabelStyle.fixedHeight = instance.log.height;

			instance.log.ResetFoldoutStyles();

			instance.stackTrace.height = instance.log.height;
			instance.stackTrace.style.fontSize = 0;

			instance.stackTrace.previewHeight = 16F;
			instance.stackTrace.previewSourceCodeStyle.alignment = TextAnchor.MiddleLeft;
			instance.stackTrace.previewSourceCodeStyle.fontSize = 0;
		}
	}
}