using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class DarkTheme : Theme
	{
		/// <summary></summary>
		/// <remarks>Test your Color in Unity 4, because rich text is buggy over there.</remarks>
		/// <param name="instance"></param>
		public override void	SetTheme(NGSettings instance)
		{
			Color	defaultNormalColor = new Color(180F / 255F, 180F / 255F, 180F / 255F);

			instance.general.menuButtonStyle = new GUIStyle("ToolbarButton");
			instance.general.menuButtonStyle.clipping = TextClipping.Overflow;

			instance.log.selectedBackground = new Color(.2421875F, .37109375F, .5859375F);
			instance.log.evenBackground = new Color(.21484375F, .21484375F, .21484375F);
			instance.log.oddBackground = new Color(.234375F, .234375F, .234375F);

			instance.log.style = new GUIStyle(GUI.skin.label);
			instance.log.style.alignment = TextAnchor.MiddleLeft;
			instance.log.style.wordWrap = false;
			instance.log.style.richText = true;
			instance.log.style.normal.textColor = defaultNormalColor;
			instance.log.style.hover.textColor = defaultNormalColor;
			instance.log.style.active.textColor = defaultNormalColor;
			instance.log.style.focused.textColor = defaultNormalColor;
			instance.log.style.margin.left = 0;
			instance.log.style.margin.right = 0;

			instance.log.timeStyle = new GUIStyle(GUI.skin.label);
			instance.log.timeStyle.alignment = TextAnchor.MiddleLeft;
			instance.log.timeStyle.normal.textColor = new Color(58F / 255F, 206F / 255F, 255F / 255F);

			instance.log.collapseLabelStyle = new GUIStyle("CN CountBadge");
			instance.log.collapseLabelStyle.alignment = TextAnchor.MiddleLeft;
			instance.log.collapseLabelStyle.fontSize = 10;
			instance.log.collapseLabelStyle.contentOffset = new Vector2(0F, 2F);
			instance.log.collapseLabelStyle.richText = false;
			instance.log.collapseLabelStyle.clipping = TextClipping.Overflow;
			instance.log.collapseLabelStyle.margin = new RectOffset(0, 0, 0, 0);
			instance.log.collapseLabelStyle.fixedHeight = 16F;

			instance.log.contentStyle = new GUIStyle(GUI.skin.label);
			instance.log.contentStyle.wordWrap = true;

			instance.log.foldoutStyle.padding.left = (int)instance.log.height;
			instance.log.foldoutStyle.fixedWidth = instance.log.height;

			instance.stackTrace.style = new GUIStyle(instance.log.style);
			instance.stackTrace.style.normal.background = Utility.CreateDotTexture(.2F, .2F, .2F, 1F);
			instance.stackTrace.style.hover.background = Utility.CreateDotTexture(.17F, .17F, .17F, 1F);
			instance.stackTrace.style.active.background = null;
			instance.stackTrace.style.margin.left = 0;
			instance.stackTrace.style.margin.right = 0;

			instance.stackTrace.returnValueColor = new Color(92F / 255F, 193F / 255F, 114F / 255F);
			instance.stackTrace.reflectedTypeColor = new Color(141F / 255F, 141F / 255F, 141F / 255F);
			instance.stackTrace.methodNameColor = new Color(171F / 255F, 171F / 255F, 171F / 255F);
			instance.stackTrace.argumentTypeColor = new Color(92F / 255F, 193F / 255, 114F / 255F);
			instance.stackTrace.argumentNameColor = new Color(4F / 255F, 255F / 255F, 224F / 255F);
			instance.stackTrace.filepathColor = new Color(167F / 255F, 172F / 255F, 172F / 255F);
			instance.stackTrace.lineColor = new Color(141F / 255F, 141F / 255F, 255F / 255F);

			instance.stackTrace.previewTextColor = new Color(167F / 255F, 172F / 255F, 172F / 255F);
			instance.stackTrace.previewLineColor = new Color(141F / 255F, 141F / 255F, 255F / 255F);

			instance.stackTrace.previewSourceCodeBackgroundColor = new Color(.11484375F, .11484375F, .11484375F);
			instance.stackTrace.previewSourceCodeMainLineBackgroundColor = new Color(.01484375F, 0.01484375F, .01484375F);
			instance.stackTrace.previewSourceCodeStyle = new GUIStyle(instance.log.style);
			instance.stackTrace.previewSourceCodeStyle.margin = new RectOffset();

			if (instance.stackTrace.keywords.Length >= 3)
			{
				if (instance.stackTrace.keywords[0].keywords.Length > 0 &&
					instance.stackTrace.keywords[0].keywords[0] == ";")
				{
					instance.stackTrace.keywords[0].color = new Color(4F / 255F, 255F / 255F, 224F / 255F);
				}
				if (instance.stackTrace.keywords[1].keywords.Length > 0 &&
					instance.stackTrace.keywords[1].keywords[0] == "this")
				{
					instance.stackTrace.keywords[1].color = new Color(92F / 255F, 193F / 255, 114F / 255F);
				}
				if (instance.stackTrace.keywords[2].keywords.Length > 0 &&
					instance.stackTrace.keywords[2].keywords[0] == "var")
				{
					instance.stackTrace.keywords[2].color = new Color(52F / 255F, 193F / 255, 94F / 255F);
				}
			}

			Utility.files.Reset();
		}
	}
}