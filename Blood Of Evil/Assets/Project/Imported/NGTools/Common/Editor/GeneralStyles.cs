using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public static class GeneralStyles
	{
		public static Color	HighlightActionButton = Color.yellow;
		public static Color	HighlightResultButton = Color.green * .8F;

		private static GUIStyle	mainTitle;
		public static GUIStyle	MainTitle
		{
			get
			{
				if (GeneralStyles.mainTitle == null)
				{
					GeneralStyles.mainTitle = new GUIStyle(EditorStyles.largeLabel);
					GeneralStyles.mainTitle.fontStyle = FontStyle.Bold;
					GeneralStyles.mainTitle.fontSize = 18;
					GeneralStyles.mainTitle.margin.top = 10;
					GeneralStyles.mainTitle.margin.left++;
				}

				return GeneralStyles.mainTitle;
			}
		}

		private static GUIStyle	title1;
		public static GUIStyle	Title1
		{
			get
			{
				if (GeneralStyles.title1 == null)
				{
					GeneralStyles.title1 = new GUIStyle(EditorStyles.label);
					GeneralStyles.title1.fontStyle = FontStyle.Bold;
					GeneralStyles.title1.fontSize = 14;
					GeneralStyles.title1.margin.top = 0;
					GeneralStyles.title1.margin.bottom = 0;
					GeneralStyles.title1.margin.left = 0;
					GeneralStyles.title1.margin.right = 0;
				}

				return GeneralStyles.title1;
			}
		}

		private static GUIStyle	innerBoxText;
		public static GUIStyle	InnerBoxText
		{
			get
			{
				if (GeneralStyles.innerBoxText == null)
				{
					GeneralStyles.innerBoxText = new GUIStyle(EditorStyles.textField);
					GeneralStyles.innerBoxText.richText = true;
					GeneralStyles.innerBoxText.wordWrap = true;
					GeneralStyles.innerBoxText.fontSize = 15;
					GeneralStyles.innerBoxText.margin = new RectOffset(10, 10, 10, 10);
					GeneralStyles.innerBoxText.padding = new RectOffset(10, 10, 10, 10);
				}

				return GeneralStyles.innerBoxText;
			}
		}

		private static GUIStyle	wrapLabel;
		public static GUIStyle	WrapLabel
		{
			get
			{
				if (GeneralStyles.wrapLabel == null)
				{
					GeneralStyles.wrapLabel = new GUIStyle(EditorStyles.label);
					GeneralStyles.wrapLabel.wordWrap = true;
					GeneralStyles.wrapLabel.fontSize = 11;
					GeneralStyles.wrapLabel.fontStyle = FontStyle.Bold;
				}

				return GeneralStyles.wrapLabel;
			}
		}

		private static GUIStyle	errorLabel;
		public static GUIStyle	ErrorLabel
		{
			get
			{
				if (GeneralStyles.errorLabel == null)
				{
					GeneralStyles.errorLabel = new GUIStyle(EditorStyles.label);
					GeneralStyles.errorLabel.normal.textColor = Color.red;
				}

				return GeneralStyles.errorLabel;
			}
		}

		private static GUIStyle	lockButton;
		public static GUIStyle	LockButton
		{
			get
			{
				if (GeneralStyles.lockButton == null)
					GeneralStyles.lockButton = new GUIStyle("IN LockButton");

				return GeneralStyles.lockButton;
			}
		}

		private static GUIStyle	componentName;
		public static GUIStyle	ComponentName
		{
			get
			{
				if (GeneralStyles.componentName == null)
				{
					GeneralStyles.componentName = new GUIStyle(EditorStyles.label);
					GeneralStyles.componentName.fontStyle = FontStyle.Bold;
				}

				return GeneralStyles.componentName;
			}
		}

		private static GUIStyle	textFieldPlaceHolder;
		public static GUIStyle	TextFieldPlaceHolder
		{
			get
			{
				if (GeneralStyles.textFieldPlaceHolder == null)
				{
					GeneralStyles.textFieldPlaceHolder = new GUIStyle(EditorStyles.label);
					GeneralStyles.textFieldPlaceHolder.normal.textColor = new Color(125F / 255F, 125F / 255F, 125F / 255F);
				}

				return GeneralStyles.textFieldPlaceHolder;
			}
		}

		private static GUIStyle	smallLabel;
		public static GUIStyle	SmallLabel
		{
			get
			{
				if (GeneralStyles.smallLabel == null)
				{
					GeneralStyles.smallLabel = new GUIStyle(EditorStyles.label);
					GeneralStyles.smallLabel.alignment = TextAnchor.LowerRight;
					GeneralStyles.smallLabel.fontSize = 8;
				}

				return GeneralStyles.smallLabel;
			}
		}

		private static GUIStyle	bigCenterText;
		public static GUIStyle	BigCenterText
		{
			get
			{
				if (GeneralStyles.bigCenterText == null)
				{
					GeneralStyles.bigCenterText = new GUIStyle(EditorStyles.label);
					GeneralStyles.bigCenterText.alignment = TextAnchor.MiddleCenter;
					GeneralStyles.bigCenterText.fontSize = 15;
					GeneralStyles.bigCenterText.wordWrap = true;
				}

				return GeneralStyles.bigCenterText;
			}
		}

		private static GUIStyle	horizontalCenteredText;
		public static GUIStyle	HorizontalCenteredText
		{
			get
			{
				if (GeneralStyles.horizontalCenteredText == null)
				{
					GeneralStyles.horizontalCenteredText = new GUIStyle(EditorStyles.label);
					GeneralStyles.horizontalCenteredText.alignment = TextAnchor.MiddleLeft;
					GeneralStyles.horizontalCenteredText.wordWrap = true;
				}

				return GeneralStyles.horizontalCenteredText;
			}
		}

		private static GUIStyle	centerText;
		public static GUIStyle	CenterText
		{
			get
			{
				if (GeneralStyles.centerText == null)
				{
					GeneralStyles.centerText = new GUIStyle(EditorStyles.label);
					GeneralStyles.centerText.alignment = TextAnchor.MiddleCenter;
					GeneralStyles.centerText.wordWrap = true;
				}

				return GeneralStyles.centerText;
			}
		}

		private static GUIStyle	leftButton;
		public static GUIStyle	LeftButton
		{
			get
			{
				if (GeneralStyles.leftButton == null)
				{
					GeneralStyles.leftButton = new GUIStyle(GUI.skin.button);
					GeneralStyles.leftButton.alignment = TextAnchor.MiddleLeft;
				}

				return GeneralStyles.leftButton;
			}
		}

		private static GUIStyle	bigButton;
		public static GUIStyle	BigButton
		{
			get
			{
				if (GeneralStyles.bigButton == null)
				{
					GeneralStyles.bigButton = new GUIStyle(GUI.skin.button);
					GeneralStyles.bigButton.margin = new RectOffset(10, 10, 10, 10);
					GeneralStyles.bigButton.padding = new RectOffset(30, 30, 10, 10);
					GeneralStyles.bigButton.alignment = TextAnchor.MiddleCenter;
					GeneralStyles.bigButton.wordWrap = true;
				}

				return GeneralStyles.bigButton;
			}
		}

		private static GUIStyle	centerButton;
		public static GUIStyle	CenterButton
		{
			get
			{
				if (GeneralStyles.centerButton == null)
				{
					GeneralStyles.centerButton = new GUIStyle(GUI.skin.button);
					GeneralStyles.centerButton.alignment = TextAnchor.MiddleCenter;
					GeneralStyles.centerButton.wordWrap = true;
				}

				return GeneralStyles.centerButton;
			}
		}

		private static GUIStyle	toolbar;
		public static GUIStyle	Toolbar
		{
			get
			{
				if (GeneralStyles.toolbar == null)
					GeneralStyles.toolbar = new GUIStyle("Toolbar");

				return GeneralStyles.toolbar;
			}
		}

		private static GUIStyle	toolbarDropDown;
		public static GUIStyle	ToolbarDropDown
		{
			get
			{
				if (GeneralStyles.toolbarDropDown == null)
					GeneralStyles.toolbarDropDown = new GUIStyle("ToolbarDropDown");

				return GeneralStyles.toolbarDropDown;
			}
		}

		private static GUIStyle	toolbarCloseButton;
		public static GUIStyle	ToolbarCloseButton
		{
			get
			{
				if (GeneralStyles.toolbarCloseButton == null)
				{
					GeneralStyles.toolbarCloseButton = new GUIStyle("ToolbarButton");
					GeneralStyles.toolbarCloseButton.normal.textColor = EditorGUIUtility.isProSkin == true ? Color.grey : Color.black;
					GeneralStyles.toolbarCloseButton.alignment = TextAnchor.MiddleCenter;
					GeneralStyles.toolbarCloseButton.fontSize = 12;
					GeneralStyles.toolbarCloseButton.fontStyle = FontStyle.Italic;
				}

				return GeneralStyles.toolbarCloseButton;
			}
		}

		private static GUIStyle	toolbarValidButton;
		public static GUIStyle	ToolbarValidButton
		{
			get
			{
				if (GeneralStyles.toolbarValidButton == null)
				{
					GeneralStyles.toolbarValidButton = new GUIStyle("ToolbarButton");
					GeneralStyles.toolbarValidButton.normal.textColor = EditorGUIUtility.isProSkin == true ? Color.green : Color.blue;
					GeneralStyles.toolbarValidButton.alignment = TextAnchor.MiddleCenter;
					GeneralStyles.toolbarValidButton.fontSize = 20;
				}

				return GeneralStyles.toolbarValidButton;
			}
		}

		private static GUIStyle	toolbarAltButton;
		public static GUIStyle	ToolbarAltButton
		{
			get
			{
				if (GeneralStyles.toolbarAltButton == null)
				{
					GeneralStyles.toolbarAltButton = new GUIStyle("ToolbarButton");
					GeneralStyles.toolbarAltButton.normal.textColor = EditorGUIUtility.isProSkin == true ? Color.cyan : Color.grey;
					GeneralStyles.toolbarAltButton.alignment = TextAnchor.MiddleCenter;
					GeneralStyles.toolbarAltButton.fontSize = 14;
				}

				return GeneralStyles.toolbarAltButton;
			}
		}

		private static GUIStyle	toolbarButton;
		public static GUIStyle	ToolbarButton
		{
			get
			{
				if (GeneralStyles.toolbarButton == null)
				{
					GeneralStyles.toolbarButton = new GUIStyle("ToolbarButton");
					GeneralStyles.toolbarButton.fontStyle = FontStyle.Bold;
				}

				return GeneralStyles.toolbarButton;
			}
		}

		private static GUIStyle	toolbarToggle;
		public static GUIStyle	ToolbarToggle
		{
			get
			{
				if (GeneralStyles.toolbarToggle == null)
					GeneralStyles.toolbarToggle = new GUIStyle("ToolbarButton");

				return GeneralStyles.toolbarToggle;
			}
		}

		private static GUIStyle	bigFontToolbarButton;
		public static GUIStyle	BigFontToolbarButton
		{
			get
			{
				if (GeneralStyles.bigFontToolbarButton == null)
				{
					GeneralStyles.bigFontToolbarButton = new GUIStyle("ToolbarButton");
					GeneralStyles.bigFontToolbarButton.fontSize = 18;
				}

				return GeneralStyles.bigFontToolbarButton;
			}
		}

		private static GUIStyle	toolbarButtonLeft;
		public static GUIStyle	ToolbarButtonLeft
		{
			get
			{
				if (GeneralStyles.toolbarButtonLeft == null)
				{
					GeneralStyles.toolbarButtonLeft = new GUIStyle("ToolbarButton");
					GeneralStyles.toolbarButtonLeft.alignment = TextAnchor.MiddleLeft;
				}

				return GeneralStyles.toolbarButtonLeft;
			}
		}

		private static GUIStyle	toolbarTextField;
		public static GUIStyle	ToolbarTextField
		{
			get
			{
				if (GeneralStyles.toolbarTextField == null)
					GeneralStyles.toolbarTextField = new GUIStyle("ToolbarTextField");

				return GeneralStyles.toolbarTextField;
			}
		}

		private static GUIStyle	toolbarSearchTextField;
		public static GUIStyle	ToolbarSearchTextField
		{
			get
			{
				if (GeneralStyles.toolbarSearchTextField == null)
					GeneralStyles.toolbarSearchTextField = new GUIStyle("ToolbarSeachTextField");

				return GeneralStyles.toolbarSearchTextField;
			}
		}

		private static GUIStyle	toolbarSearchCancelButton;
		public static GUIStyle	ToolbarSearchCancelButton
		{
			get
			{
				if (GeneralStyles.toolbarSearchCancelButton == null)
					GeneralStyles.toolbarSearchCancelButton = new GUIStyle("ToolbarSeachCancelButton");

				return GeneralStyles.toolbarSearchCancelButton;
			}
		}

		private static GUIStyle	unityObjectPicker;
		public static GUIStyle	UnityObjectPicker
		{
			get
			{
				if (GeneralStyles.unityObjectPicker == null)
					GeneralStyles.unityObjectPicker = new GUIStyle(EditorStyles.objectField);

				return GeneralStyles.unityObjectPicker;
			}
		}

		private static GUIStyle	selectionText;
		public static GUIStyle	SelectionText
		{
			get
			{
				if (GeneralStyles.selectionText == null)
				{
					GeneralStyles.selectionText = new GUIStyle(EditorStyles.label);
					GeneralStyles.selectionText.fontSize = 16;
					GeneralStyles.selectionText.alignment = TextAnchor.MiddleLeft;
				}

				return GeneralStyles.selectionText;
			}
		}

		private static GUIStyle	version;
		public static GUIStyle	Version
		{
			get
			{
				if (GeneralStyles.version == null)
				{
					GeneralStyles.version = new GUIStyle(EditorStyles.label);
					GeneralStyles.version.alignment = TextAnchor.MiddleRight;
					GeneralStyles.version.fontSize = 10;
				}

				return GeneralStyles.version;
			}
		}

		private static GUIStyle	richLabel;
		public static GUIStyle	RichLabel
		{
			get
			{
				if (GeneralStyles.richLabel == null)
				{
					GeneralStyles.richLabel = new GUIStyle(EditorStyles.label);
					GeneralStyles.richLabel.richText = true;
				}

				return GeneralStyles.richLabel;
			}
		}

		private static GUIStyle	richTextArea;
		public static GUIStyle	RichTextArea
		{
			get
			{
				if (GeneralStyles.richTextArea == null)
				{
					GeneralStyles.richTextArea = new GUIStyle(EditorStyles.textArea);
					GeneralStyles.richTextArea.richText = true;
				}

				return GeneralStyles.richTextArea;
			}
		}

		private static GUIStyle	verticalCenterTextField;
		public static GUIStyle	VerticalCenterTextField
		{
			get
			{
				if (GeneralStyles.verticalCenterTextField == null)
				{
					GeneralStyles.verticalCenterTextField = new GUIStyle(EditorStyles.textField);
					GeneralStyles.verticalCenterTextField.alignment = TextAnchor.MiddleLeft;
				}

				return GeneralStyles.verticalCenterTextField;
			}
		}

		private static GUIStyle	verticalCenterLabel;
		public static GUIStyle	VerticalCenterLabel
		{
			get
			{
				if (GeneralStyles.verticalCenterLabel == null)
				{
					GeneralStyles.verticalCenterLabel = new GUIStyle(EditorStyles.label);
					GeneralStyles.verticalCenterLabel.alignment = TextAnchor.MiddleLeft;
				}

				return GeneralStyles.verticalCenterLabel;
			}
		}
	}
}