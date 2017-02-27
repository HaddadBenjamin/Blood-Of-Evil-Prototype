using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class FontSize18Theme : Theme
	{
		public override void	SetTheme(NGSettings instance)
		{
			instance.log.style.alignment = TextAnchor.MiddleLeft;
			instance.log.style.fontSize = 18;
			instance.log.height = 26F;

			instance.log.timeStyle.fontSize = 13;

			instance.log.foldoutStyle.padding.left = 26;
			instance.log.foldoutStyle.fixedWidth = instance.log.height;

			instance.log.collapseLabelStyle.fixedHeight = 16F;

			instance.log.ResetFoldoutStyles();

			instance.stackTrace.height = 22F;
			instance.stackTrace.style.fontSize = 15;

			instance.stackTrace.previewHeight = 26F;
			instance.stackTrace.previewSourceCodeStyle.alignment = TextAnchor.MiddleLeft;
			instance.stackTrace.previewSourceCodeStyle.fontSize = 18;
		}
	}
}