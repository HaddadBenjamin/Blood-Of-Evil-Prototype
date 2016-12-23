using UnityEngine;

namespace NGToolsEditor
{
	public class LightTheme : Theme
	{
		public override void	SetTheme(NGSettings instance)
		{
			Color	defaultNormalColor = new Color(0F / 255F, 0F / 255F, 0F / 255F, 1F);

			instance.general.menuButtonStyle = new GUIStyle("ToolbarButton");
			instance.general.menuButtonStyle.clipping = TextClipping.Overflow;

			instance.log.selectedBackground = new Color(62F / 255F, 125F /231F, 231F / 255F, 1F);
			instance.log.evenBackground = new Color(216F / 255F, 216F / 255F, 216F / 255F, 1F);
			instance.log.oddBackground = new Color(222F / 255F, 222F / 255F, 222F / 255F, 1F);

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
			instance.log.timeStyle.normal.textColor = new Color(111F / 255F, 85F / 255F, 0F / 255F, 1F);

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
			instance.stackTrace.style.normal.background = Utility.CreateDotTexture(.9F, .9F, .9F, 1F);
			instance.stackTrace.style.hover.background = Utility.CreateDotTexture(.87F, .87F, .87F, 1F);
			instance.stackTrace.style.active.background = null;
			instance.stackTrace.style.margin.left = 0;
			instance.stackTrace.style.margin.right = 0;

			instance.stackTrace.returnValueColor = new Color(78F / 255F, 50F / 255F, 255F / 255F, 1F);
			instance.stackTrace.reflectedTypeColor = new Color(62F / 255F, 49F / 255F, 49F / 255F, 1F);
			instance.stackTrace.methodNameColor = new Color(49F / 255F, 68F / 255F, 84F / 255F, 1F);
			instance.stackTrace.argumentTypeColor = new Color(84F / 255F, 40F / 255, 250F / 255F, 1F);
			instance.stackTrace.argumentNameColor = new Color(170F / 255F, 115F / 255F, 114F / 255F, 1F);
			instance.stackTrace.filepathColor = new Color(107F / 255F, 72F / 255F, 142F / 255F, 1F);
			instance.stackTrace.lineColor = new Color(63F / 255F, 42F / 255F, 214F / 255F, 1F);

			instance.stackTrace.previewTextColor = new Color(42F / 255F, 51F / 255F, 51F / 255F, 1F);
			instance.stackTrace.previewLineColor = new Color(63F / 255F, 42F / 255F, 214F / 255F, 1F);

			instance.stackTrace.previewSourceCodeBackgroundColor = new Color(216F / 255F, 219F / 255F, 219F / 255F, 1F);
			instance.stackTrace.previewSourceCodeMainLineBackgroundColor = new Color(208F / 255F, 203F / 255F, 203F / 255F, 1F);
			instance.stackTrace.previewSourceCodeStyle = new GUIStyle(instance.log.style);
			instance.stackTrace.previewSourceCodeStyle.margin = new RectOffset();

			if (instance.stackTrace.keywords.Length >= 3)
			{
				if (instance.stackTrace.keywords[0].keywords.Length > 0 &&
					instance.stackTrace.keywords[0].keywords[0] == ";")
				{
					instance.stackTrace.keywords[0].color = new Color(255F / 255F, 82F / 255F, 4F / 255F, 1F);
				}
				if (instance.stackTrace.keywords[1].keywords.Length > 0 &&
					instance.stackTrace.keywords[1].keywords[0] == "this")
				{
					instance.stackTrace.keywords[1].color = new Color(141F / 255F, 47F / 255F, 212F / 255F, 1F);
				}
				if (instance.stackTrace.keywords[2].keywords.Length > 0 &&
					instance.stackTrace.keywords[2].keywords[0] == "var")
				{
					instance.stackTrace.keywords[2].color = new Color(186F / 255F, 99F / 255F, 218F / 255F, 1F);
				}
			}

			Utility.files.Reset();
		}
	}
}