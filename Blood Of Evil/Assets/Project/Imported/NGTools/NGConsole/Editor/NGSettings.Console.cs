using NGTools;
using NGToolsEditor.NGConsole;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		public enum ModeOpen
		{
			AssetDatabaseOpenAsset,
			NGConsoleOpener
		}

		public enum PathDisplay
		{
			Hidden,
			Visible,
			OnlyIfExist
		}

		[Serializable]
		public class GeneralSettings : Settings
		{
			[Serializable]
			public class EditorExtensions
			{
				[File(FileAttribute.Mode.Open, "*")]
				public string	editor;
				public string	arguments;
				public string[]	extensions;
			}

			[LocaleHeader("General_AutoReplaceUnityConsole")]
			public bool					autoReplaceUnityConsole = false;
			[LocaleHeader("General_Clear")]
			public string				clearLabel = "Clear";
			[LocaleHeader("General_ClearOnPlay")]
			public string				clearOnPlayLabel = "Clear on Play";
			[LocaleHeader("General_ErrorPause")]
			public string				errorPauseLabel = "Error Pause";
			[LocaleHeader("General_OpenMode")]
			public ModeOpen				openMode = ModeOpen.AssetDatabaseOpenAsset;
			[LocaleHeader("General_EditorExtensions")]
			public EditorExtensions[]	editorExtensions = new EditorExtensions[0];
			[LocaleHeader("General_SmoothScrolling")]
			public bool					smoothScrolling;
			//[LocaleHeader("General_HorizontalScrollbar")]
			//public bool					horizontalScrollbar = false;
			[LocaleHeader("General_FilterUselessStackFrame")]
			public bool					filterUselessStackFrame = true;
			[LocaleHeader("General_MenuHeight")]
			public float				menuHeight = Constants.DefaultSingleLineHeight;
			[LocaleHeader("General_MenuButtonStyle")]
			public GUIStyle				menuButtonStyle = new GUIStyle();

			protected override void	InitGUI()
			{
				this.menuButtonStyle = new GUIStyle("ToolbarButton");
			}
		}
		public GeneralSettings	general = new GeneralSettings();

		[Serializable]
		public class LogSettings : Settings
		{
			[LocaleHeader("Log_GiveFocusToEditor")]
			public bool			giveFocusToEditor = true;
			[LocaleHeader("Log_ForceFocusOnModifier")]
			public EventModifiers	forceFocusOnModifier = EventModifiers.Alt;
			[LocaleHeader("Log_SelectObjectOnModifier")]
			public EventModifiers	selectObjectOnModifier = EventModifiers.Shift;
			[LocaleHeader("Log_AlwaysDisplayLogContent")]
			public bool			alwaysDisplayLogContent = true;
			[LocaleHeader("Log_Style")]
			public GUIStyle		style = new GUIStyle();
			[LocaleHeader("Log_Height")]
			public float		height = Constants.DefaultSingleLineHeight;
			[LocaleHeader("Log_SelectedBackground")]
			public Color		selectedBackground ;
			[LocaleHeader("Log_EvenBackground")]
			public Color		evenBackground;
			[LocaleHeader("Log_OddBackground")]
			public Color		oddBackground;
			[LocaleHeader("Log_DisplayTime")]
			public bool			displayTime = false;
			[LocaleHeader("Log_TimeFormat")]
			public string		timeFormat = "HH:mm:ss.fff";
			[LocaleHeader("Log_TimeStyle")]
			public GUIStyle		timeStyle = new GUIStyle();
			[LocaleHeader("Log_DisplayFrameCount")]
			public bool			displayFrameCount = false;
			[LocaleHeader("Log_DisplayRenderedFrameCount")]
			public bool			displayRenderedFrameCount = false;
			[LocaleHeader("Log_FoldoutStyle")]
			public GUIStyle		foldoutStyle = new GUIStyle();

			[NonSerialized]
			private GUIStyle	normalFoldoutStyle = null;
			public GUIStyle		NormalFoldoutStyle { get { return this.normalFoldoutStyle ?? (this.normalFoldoutStyle = this.CreateFoldoutStyleFromColor(Constants.NormalFoldoutColor)); } }
			[NonSerialized]
			private GUIStyle	warningFoldoutStyle;
			public GUIStyle		WarningFoldoutStyle { get { return this.warningFoldoutStyle ?? (this.warningFoldoutStyle = this.CreateFoldoutStyleFromColor(Constants.WarningFoldoutColor)); } }
			[NonSerialized]
			private GUIStyle	errorFoldoutStyle;
			public GUIStyle		ErrorFoldoutStyle { get { return this.errorFoldoutStyle ?? (this.errorFoldoutStyle = this.CreateFoldoutStyleFromColor(Constants.ErrorFoldoutColor)); } }
			[NonSerialized]
			private GUIStyle	exceptionFoldoutStyle;
			public GUIStyle		ExceptionFoldoutStyle { get { return this.exceptionFoldoutStyle ?? (this.exceptionFoldoutStyle = this.CreateFoldoutStyleFromColor(Constants.ExceptionFoldoutColor)); } }

			[LocaleHeader("Log_CollapseLabelStyle")]
			public GUIStyle		collapseLabelStyle = new GUIStyle();
			[LocaleHeader("Log_ContentStyle")]
			public GUIStyle		contentStyle = new GUIStyle();

			public void	ResetFoldoutStyles()
			{
				this.normalFoldoutStyle = null;
				this.warningFoldoutStyle = null;
				this.errorFoldoutStyle = null;
				this.exceptionFoldoutStyle = null;
			}

			protected override void	InitGUI()
			{
				this.style = new GUIStyle(GUI.skin.label);
				this.timeStyle = new GUIStyle(GUI.skin.label);

				this.collapseLabelStyle = new GUIStyle("CN CountBadge");
				this.contentStyle = new GUIStyle(GUI.skin.label);
			}

			private GUIStyle	CreateFoldoutStyleFromColor(Color color)
			{
				GUIStyle	style = new GUIStyle(this.foldoutStyle);
						
				byte[]		c = Convert.FromBase64String(Constants.FoldoutTemplate);
				Texture2D	foldout = Theme.GenerateTexture(c, color);
				Texture2D	foldoutActive = Theme.GenerateTexture(c, Constants.ActiveFoldoutColor);

				foldout.hideFlags = HideFlags.HideAndDontSave;
				foldoutActive.hideFlags = HideFlags.HideAndDontSave;

				style.normal.background = foldout;
				style.active.background = foldoutActive;
				style.focused.background = foldout;

				c = Convert.FromBase64String(Constants.FoldoutOnTemplate);
				foldout = Theme.GenerateTexture(c, color);
				foldoutActive = Theme.GenerateTexture(c, Constants.ActiveFoldoutColor);

				foldout.hideFlags = HideFlags.HideAndDontSave;
				foldoutActive.hideFlags = HideFlags.HideAndDontSave;

				style.onNormal.background = foldout;
				style.onActive.background = foldoutActive;
				style.onFocused.background = foldout;

				return style;
			}
		}
		public LogSettings	log = new LogSettings();

		[Serializable]
		public class StackTraceSettings : Settings
		{
			public enum DisplayReflectedType
			{
				NamespaceAndClass,
				Class,
				None,
			}

			[Serializable]
			public class KeywordsColor
			{
				public Color	color;
				public string[] keywords;
			}

			[Serializable]
			public class MethodCategory
			{
				public string	method;
				public string	category;
			}

			[LocaleHeader("StackTrace_Filters")]
			public List<string>	filters = new List<string>();
			[LocaleHeader("StackTrace_Categories")]
			public List<MethodCategory>	categories = new List<MethodCategory>();
			[LocaleHeader("StackTrace_DisplayFilepath")]
			public PathDisplay	displayFilepath = PathDisplay.OnlyIfExist;
			[LocaleHeader("StackTrace_SkipUnreachableFrame")]
			public bool			skipUnreachableFrame = true;
			[LocaleHeader("StackTrace_DisplayRelativeToAssets")]
			public bool			displayRelativeToAssets = true;
			[LocaleHeader("StackTrace_PingFolderOnModifier")]
			public EventModifiers	pingFolderOnModifier = EventModifiers.Control;
			[LocaleHeader("StackTrace_Style")]
			public GUIStyle		style = new GUIStyle();
			[LocaleHeader("StackTrace_Height")]
			public float		height = Constants.DefaultSingleLineHeight;
			[LocaleHeader("StackTrace_DisplayReturnValue")]
			public bool			displayReturnValue = true;
			[LocaleHeader("StackTrace_IndentAfterReturnType")]
			public bool			indentAfterReturnType = true;
			[LocaleHeader("StackTrace_ReturnValueColor")]
			public Color		returnValueColor = new Color(78F / 255F, 50F / 255F, 255F / 255F);
			[LocaleHeader("StackTrace_DisplayReflectedType")]
			public DisplayReflectedType	displayReflectedType = DisplayReflectedType.Class;
			[LocaleHeader("StackTrace_ReflectedTypeColor")]
			public Color		reflectedTypeColor = new Color(103F / 255F, 103F / 255F, 103F / 255F);
			[LocaleHeader("StackTrace_MethodNameColor")]
			public Color		methodNameColor = new Color(149F / 255F, 68F / 255F, 84F / 255F);
			[LocaleHeader("StackTrace_DisplayArgumentType")]
			public bool			displayArgumentType = true;
			[LocaleHeader("StackTrace_ArgumentTypeColor")]
			public Color		argumentTypeColor = new Color(84F / 255F, 40F / 255, 250F / 255F);
			[LocaleHeader("StackTrace_DisplayArgumentName")]
			public bool			displayArgumentName = true;
			[LocaleHeader("StackTrace_ArgumentNameColor")]
			public Color		argumentNameColor = new Color(170F / 255F, 115F / 255F, 114F / 255F);
			[LocaleHeader("StackTrace_IndentAfterArgument")]
			public bool			indentAfterArgument = true;
			[LocaleHeader("StackTrace_FilepathColor")]
			public Color		filepathColor = new Color(69F / 255F, 69F / 255F, 69F / 255F);
			[LocaleHeader("StackTrace_LineColor")]
			public Color		lineColor = new Color(44F / 255F, 6F / 255F, 199F / 255F);
			[LocaleHeader("StackTrace_PreviewOffset")]
			public Vector2		previewOffset = new Vector2(-15F, 15F);
			[LocaleHeader("StackTrace_PreviewLinesBeforeStackFrame")]
			public int			previewLinesBeforeStackFrame = 3;
			[LocaleHeader("StackTrace_PreviewLinesAfterStackFrame")]
			public int			previewLinesAfterStackFrame = 3;
			[LocaleHeader("StackTrace_DisplayTabAsSpaces")]
			public int			displayTabAsSpaces = 4;
			[LocaleHeader("StackTrace_PreviewTextColor")]
			public Color		previewTextColor = new Color(68F / 255F, 69F / 255F, 69F / 255F);
			[LocaleHeader("StackTrace_PreviewLineColor")]
			public Color		previewLineColor = new Color(44F / 255F, 6F / 255F, 199F / 255F);
			public Color		previewSourceCodeBackgroundColor = new Color(174F / 255F, 177F / 255F, 177F / 255F);
			public Color		previewSourceCodeMainLineBackgroundColor = new Color(161F / 255F, 161F / 255F, 161F / 255F);
			[LocaleHeader("StackTrace_PreviewHeight")]
			public float		previewHeight = 16F;
			public GUIStyle		previewSourceCodeStyle = new GUIStyle();
			[LocaleHeader("StackTrace_Keywords")]
			public KeywordsColor[]	keywords = new KeywordsColor[] {
				new KeywordsColor() {
					color = new Color(96F / 255F, 69F / 255F, 0F / 255F),
					keywords = new string[] { ";", "=", ".", "{", "}", "(", ")", ",", "[", "]", "+", "-", "*", "/", ":", "!", "?", "@", "<", ">" },
				},
				new KeywordsColor() {
					color = new Color(141F / 255F, 47F / 255F, 212F / 255F),
					keywords = new string[] { "this", "if", "else", "new", "foreach", "for", "switch", "while", "as", "is", "get", "set", "try", "catch", "finally", "return", "yield", "public", "private", "protected", "static", "throw", "internal", "virtual", "override", "base", "implicit", "explicit", "ref", "out", "in", "using", "class", "struct", "where", "params", "elif", "endif" },
				},
				new KeywordsColor() {
					color = new Color(186F / 255F, 99F / 255F, 218F / 255F),
					keywords = new string[] { "var", "void", "float", "uint", "int", "string", "object", "bool", "type", "char", "sbyte", "byte", "double", "decimal", "ulong", "long", "ushort", "short", "Boolean", "Char", "String", "SByte", "Byte", "Int16", "Int32", "Int64", "UInt16", "UInt32", "UInt64", "Single", "Double", "Decimal" },
				},
			};

			protected override void	InitGUI()
			{
				this.style = new GUIStyle(GUI.skin.label);
				this.previewSourceCodeStyle = new GUIStyle(GUI.skin.label);
			}
		}
		public StackTraceSettings	stackTrace = new StackTraceSettings();

		[LocaleHeader("ConsoleSettings_InputsManager")]
		public InputsManager	inputsManager = new InputsManager();

		[HideInInspector]
		public MultiDataStorage	serializedModules = new MultiDataStorage();
	}
}