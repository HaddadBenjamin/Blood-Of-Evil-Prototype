using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class FontSize14Theme : Theme
	{
		public override void	SetTheme(NGSettings instance)
		{
			instance.log.style.alignment = TextAnchor.MiddleLeft;
			instance.log.style.fontSize = 14;
			instance.log.height = 22F;

			instance.log.timeStyle.fontSize = 12;

			instance.log.foldoutStyle.padding.left = 22;
			instance.log.foldoutStyle.fixedWidth = instance.log.height;

			instance.log.collapseLabelStyle.fixedHeight = 16F;

			instance.log.ResetFoldoutStyles();

			instance.stackTrace.height = 18F;
			instance.stackTrace.style.fontSize = 13;

			instance.stackTrace.previewHeight = 22F;
			instance.stackTrace.previewSourceCodeStyle.alignment = TextAnchor.MiddleLeft;
			instance.stackTrace.previewSourceCodeStyle.fontSize = 14;
		}
	}
}