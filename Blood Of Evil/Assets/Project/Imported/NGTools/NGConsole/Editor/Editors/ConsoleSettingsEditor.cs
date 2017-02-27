using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	public sealed class ConsoleSettingsEditor
	{
		public enum MainTabs
		{
			General,
			Inputs,
			Themes,
			Presets,
		}

		public enum GeneralTabs
		{
			General,
			Log,
			StackTrace
		}

		private static Color	HighlightInput = new Color(.4F, 9F, .25F);

		public MainTabs		currentTab = MainTabs.General;
		public GeneralTabs	currentGeneralTab = GeneralTabs.General;
		public Vector2		generalGeneralScrollPosition;
		public Vector2		generalLogScrollPosition;
		public Vector2		generalStackTraceScrollPosition;

		private int			selectedInputsGroup = 0;

		private Type[]		themeTypes;
		private string[]	themeNames;
		private Type[]		presetTypes;
		private string[]	presetNames;

		private Vector2			inputScrollPosition;
		private List<GUITimer>	testInputAnimationFeedback;
		private InputCommand	registeringCommand;
		private bool			shiftPressed;

		private SectionDrawer	sectionGeneral;
		private SectionDrawer	sectionLog;
		private SectionDrawer	sectionStackTrace;

		private GUIStyle	menuButtonStyle;

		public	ConsoleSettingsEditor()
		{
			var	types = new List<Type>();
			var	names = new List<string>();

			foreach (Type c in Utility.EachSubClassesOf(typeof(Theme)))
			{
				types.Add(c);
				names.Add(Utility.NicifyVariableName(c.Name));
			}

			this.themeTypes = types.ToArray();
			this.themeNames = names.ToArray();

			types.Clear();
			names.Clear();

			foreach (Type c in Utility.EachSubClassesOf(typeof(Preset)))
			{
				types.Add(c);
				names.Add(Utility.NicifyVariableName(c.Name));
			}

			this.presetTypes = types.ToArray();
			this.presetNames = names.ToArray();

			NGSettingsWindow.AddSection(NGConsoleWindow.Title, this.OnGUI, 10);

			this.inputScrollPosition = new Vector2();
			this.testInputAnimationFeedback = new List<GUITimer>();

			this.sectionGeneral = new SectionDrawer(typeof(NGSettings.GeneralSettings));
			this.sectionLog = new SectionDrawer(typeof(NGSettings.LogSettings));
			this.sectionStackTrace = new SectionDrawer(typeof(NGSettings.StackTraceSettings));
		}

		public void	Uninit()
		{
			NGSettingsWindow.RemoveSection(NGConsoleWindow.Title);

			this.sectionGeneral.Uninit();
			this.sectionLog.Uninit();
			this.sectionStackTrace.Uninit();
		}

		public void	OnGUI()
		{
			if (Preferences.Settings == null)
			{
				GUILayout.Label(LC.G("ConsoleSettings_NullTarget"));
				return;
			}

			if (this.menuButtonStyle == null)
				this.menuButtonStyle = new GUIStyle("ToolbarButton");

			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Toggle(this.currentTab == MainTabs.General, LC.G("ConsoleSettings_General"), this.menuButtonStyle) == true)
					this.currentTab = MainTabs.General;
				if (GUILayout.Toggle(this.currentTab == MainTabs.Inputs, LC.G("ConsoleSettings_Inputs"), this.menuButtonStyle) == true)
					this.currentTab = MainTabs.Inputs;
				if (GUILayout.Toggle(this.currentTab == MainTabs.Themes, LC.G("ConsoleSettings_Themes"), this.menuButtonStyle) == true)
					this.currentTab = MainTabs.Themes;
				if (GUILayout.Toggle(this.currentTab == MainTabs.Presets, LC.G("ConsoleSettings_Presets"), this.menuButtonStyle) == true)
					this.currentTab = MainTabs.Presets;
			}
			GUILayout.EndHorizontal();

			EditorGUI.BeginChangeCheck();
			{
				if (this.currentTab == MainTabs.General)
					this.OnGUIGeneral();
				else if (this.currentTab == MainTabs.Inputs)
					this.OnGUIInputs();
				else if (this.currentTab == MainTabs.Themes)
					this.OnGUIThemes();
				else if (this.currentTab == MainTabs.Presets)
					this.OnGUIPresets();
			}
			if (EditorGUI.EndChangeCheck() == true)
				Preferences.InvalidateSettings();
		}

		public void	OnGUIGeneral()
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Toggle(this.currentGeneralTab == GeneralTabs.General, LC.G("ConsoleSettings_General_General"), this.menuButtonStyle) == true)
					this.currentGeneralTab = GeneralTabs.General;
				if (GUILayout.Toggle(this.currentGeneralTab == GeneralTabs.Log, LC.G("ConsoleSettings_General_Log"), this.menuButtonStyle) == true)
					this.currentGeneralTab = GeneralTabs.Log;
				if (GUILayout.Toggle(this.currentGeneralTab == GeneralTabs.StackTrace, LC.G("ConsoleSettings_General_StackTrace"), this.menuButtonStyle) == true)
					this.currentGeneralTab = GeneralTabs.StackTrace;
			}
			GUILayout.EndHorizontal();

			if (this.currentGeneralTab == GeneralTabs.General)
				this.OnGUIGeneralGeneral();
			else if (this.currentGeneralTab == GeneralTabs.Log)
				this.OnGUIGeneralLog();
			else if (this.currentGeneralTab == GeneralTabs.StackTrace)
				this.OnGUIGeneralStackTrace();
		}

		private void	OnGUIGeneralGeneral()
		{
			this.generalGeneralScrollPosition = EditorGUILayout.BeginScrollView(this.generalGeneralScrollPosition);
			{
				this.sectionGeneral.OnGUI();
				GUILayout.Space(10F);
			}
			EditorGUILayout.EndScrollView();
		}

		private void	OnGUIGeneralLog()
		{
			this.generalLogScrollPosition = EditorGUILayout.BeginScrollView(this.generalLogScrollPosition);
			{
				EditorGUI.BeginChangeCheck();
				this.sectionLog.OnGUI();
				if (EditorGUI.EndChangeCheck() == true)
					Preferences.Settings.log.ResetFoldoutStyles();

				GUILayout.Space(10F);
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.BeginVertical(GUILayout.Height(16F + 5F * Preferences.Settings.log.height));
			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			EditorGUILayout.LabelField("Preview :");
			EditorGUILayout.EndHorizontal();

			this.DrawRow(0, 0, false, "A selected row.", "");
			this.DrawRow(1, 0, false, "A normal even row.", "1");
			this.DrawRow(2, 1, false, "A warning odd row.", "23");
			this.DrawRow(3, 2, false, "An error even row.", "456");
			this.DrawRow(4, 3, false, "An exception odd row.", "7890");

			EditorGUILayout.EndVertical();
		}

		private void	DrawRow(int i, int foldType, bool fold, string content, string collapseCount)
		{
			Rect	r = GUILayoutUtility.GetRect(0F, Preferences.Settings.log.height, Preferences.Settings.log.style);
			float	originX = r.x;
			float	originWidth = r.width;

			if (Event.current.type == EventType.Repaint)
			{
				if (i == 0)
					EditorGUI.DrawRect(r, Preferences.Settings.log.selectedBackground);
				else if ((i & 1) == 0)
					EditorGUI.DrawRect(r, Preferences.Settings.log.evenBackground);
				else
					EditorGUI.DrawRect(r, Preferences.Settings.log.oddBackground);
			}

			r.width = r.height;
			if (foldType == 0)
				EditorGUI.Foldout(r, fold, "", Preferences.Settings.log.NormalFoldoutStyle);
			else if (foldType == 1)
				EditorGUI.Foldout(r, fold, "", Preferences.Settings.log.WarningFoldoutStyle);
			else if (foldType == 2)
				EditorGUI.Foldout(r, fold, "", Preferences.Settings.log.ErrorFoldoutStyle);
			else if (foldType == 3)
				EditorGUI.Foldout(r, fold, "", Preferences.Settings.log.ExceptionFoldoutStyle);

			r.xMin += r.width;

			// Draw time.
			if (Preferences.Settings.log.displayTime == true)
			{
				Utility.content.text = DateTime.Now.ToString(Preferences.Settings.log.timeFormat);
				r.width = Preferences.Settings.log.timeStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.timeStyle);
				r.x += r.width;
			}

			// Draw frame count.
			if (Preferences.Settings.log.displayFrameCount == true)
			{
				Utility.content.text = Time.frameCount.ToString();
				r.width = Preferences.Settings.log.timeStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.timeStyle);
				r.x += r.width;
			}

			// Draw rendered frame count.
			if (Preferences.Settings.log.displayRenderedFrameCount == true)
			{
				Utility.content.text = Time.renderedFrameCount.ToString();
				r.width = Preferences.Settings.log.timeStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.timeStyle);
				r.x += r.width;
			}
			r.width = originWidth - (r.x - originX);

			GUI.Button(r, content, Preferences.Settings.log.style);

			if (collapseCount != string.Empty)
			{
				// Draw collapse count.
				Utility.content.text = collapseCount;
				r.xMin += r.width - Preferences.Settings.log.collapseLabelStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.collapseLabelStyle);
			}
		}

		private void	OnGUIGeneralStackTrace()
		{
			this.generalStackTraceScrollPosition = EditorGUILayout.BeginScrollView(this.generalStackTraceScrollPosition);
			{
				EditorGUI.BeginChangeCheck();
				this.sectionStackTrace.OnGUI();
				if (EditorGUI.EndChangeCheck() == true)
				{
					LogConditionParser.cachedFrames.Clear();
					LogConditionParser.cachedFramesArrays.Clear();
					MainModule.methodsCategories.Clear();
				}

				GUILayout.Space(10F);
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.BeginVertical(GUILayout.Height(16F + 2 * Preferences.Settings.log.height + 8 * EditorGUIUtility.singleLineHeight));
			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			EditorGUILayout.LabelField("Preview :");
			EditorGUILayout.EndHorizontal();

			if (Preferences.Settings.stackTrace.skipUnreachableFrame == true)
				this.DrawStackFrame(0, "Sub Frame.", "Assets/An/Existing/File.cs", true);
			else
			{
				this.DrawStackFrame(0, "Top Frame.", "A/File/Somewhere/That/Does/Not/Exist.cs", false);
				this.DrawStackFrame(1, "Sub Frame.", "Assets/An/Existing/File.cs", true);
			}

			this.DrawStackFrameCode(1, false, "using UnityEngine;");
			this.DrawStackFrameCode(2, false, "private static class Foo : Object");
			this.DrawStackFrameCode(3, false, "{");
			this.DrawStackFrameCode(4, false, "	public internal void	Func(Vector2 v)");
			this.DrawStackFrameCode(5, false, "	{");
			this.DrawStackFrameCode(6, true, "		Debug.Log(\"Someting\");");
			this.DrawStackFrameCode(7, false, "	}");
			this.DrawStackFrameCode(8, false, "}");

			EditorGUILayout.EndVertical();
		}

		private void DrawStackFrameCode(int i, bool mainLine, string content)
		{
			Rect	r = GUILayoutUtility.GetRect(0F, Preferences.Settings.stackTrace.previewHeight, Preferences.Settings.stackTrace.previewSourceCodeStyle);

			if (Event.current.type == EventType.Repaint)
			{
				if (mainLine == false)
					EditorGUI.DrawRect(r, Preferences.Settings.stackTrace.previewSourceCodeBackgroundColor);
				else
					EditorGUI.DrawRect(r, Preferences.Settings.stackTrace.previewSourceCodeMainLineBackgroundColor);
			}

			GUI.Label(r, Utility.Color(i.ToString(), Preferences.Settings.stackTrace.previewLineColor) + Utility.ColorLine(content), Preferences.Settings.stackTrace.previewSourceCodeStyle);
			r.y += r.height;
		}

		private void DrawStackFrame(int i, string a, string filepath, bool fileExist)
		{
			Rect	r = GUILayoutUtility.GetRect(0F, Preferences.Settings.stackTrace.height, Preferences.Settings.stackTrace.style);
			float	width = r.width;

			// Substract viewRect to avoid scrollbar.
			r.height = Preferences.Settings.stackTrace.height;

			// Display the stack trace.
			r.width = width - 16F;
			FrameBuilder.Clear();
			FrameBuilder.returnType = "Void";
			FrameBuilder.namespaceName = "namespace";
			FrameBuilder.classType = "class";
			FrameBuilder.methodName = "func";
			FrameBuilder.fileName = filepath;
			FrameBuilder.fileExist = fileExist;
			FrameBuilder.line = 1234;
			FrameBuilder.parameterTypes.Add("Rect");
			FrameBuilder.parameterNames.Add("r");
			FrameBuilder.parameterTypes.Add("int");
			FrameBuilder.parameterNames.Add("a");

			if (i == 0)
				GUI.Button(r, FrameBuilder.ToString("→ ", Preferences.Settings.stackTrace), Preferences.Settings.stackTrace.style);
			else
				GUI.Button(r, FrameBuilder.ToString("↑ ", Preferences.Settings.stackTrace), Preferences.Settings.stackTrace.style);

			r.x = r.width;
			r.width = 16F;
			GUI.Button(r, "+", Preferences.Settings.stackTrace.style);

			r.y += r.height;
		}

		private void	OnGUIThemes()
		{
			for (int i = 0; i < this.themeTypes.Length; i++)
			{
				if (GUILayout.Button(this.themeNames[i]) == true &&
					((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(this.themeNames[i], LC.G("NGSettings_ConfirmApply"), LC.G("Yes"), LC.G("No")) == true))
				{
					Theme	theme = Activator.CreateInstance(this.themeTypes[i]) as Theme;

					theme.SetTheme(Preferences.Settings);

#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
					EditorUtility.UnloadUnusedAssets();
#else
					EditorUtility.UnloadUnusedAssetsImmediate();
#endif
					Preferences.InvalidateSettings();
					Debug.Log("Theme \"" + this.themeNames[i] + "\" applied on " + Preferences.Settings + ".");
				}
			}
		}

		private void	OnGUIPresets()
		{
			for (int i = 0; i < this.presetTypes.Length; i++)
			{
				if (GUILayout.Button(this.presetNames[i]) == true &&
					((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(this.presetNames[i], LC.G("NGSettings_ConfirmApply"), LC.G("Yes"), LC.G("No")) == true))
				{
					Preset	preset = Activator.CreateInstance(this.presetTypes[i]) as Preset;

					preset.SetSettings(Preferences.Settings);

#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
					EditorUtility.UnloadUnusedAssets();
#else
					EditorUtility.UnloadUnusedAssetsImmediate();
#endif
					Preferences.InvalidateSettings();
					Debug.Log("Preset \"" + this.presetNames[i] + "\" applied on " + Preferences.Settings + ".");
				}
			}
		}

		private void	OnGUIInputs()
		{
			for (int n = 0, i = 0; i < Preferences.Settings.inputsManager.groups.Count; ++i, ++n)
			{
				if (GUILayout.Toggle(i == this.selectedInputsGroup, LC.G("InputGroup_" + Preferences.Settings.inputsManager.groups[i].name), this.menuButtonStyle) == true)
					this.selectedInputsGroup = i;
			}

			if (this.selectedInputsGroup < Preferences.Settings.inputsManager.groups.Count)
			{
				this.inputScrollPosition = EditorGUILayout.BeginScrollView(this.inputScrollPosition);
				{
					for (int n = 0, j = 0; j < Preferences.Settings.inputsManager.groups[this.selectedInputsGroup].commands.Count; ++j, ++n)
					{
						while (n >= this.testInputAnimationFeedback.Count)
						{
							var	af = new AnimFloat(0F, EditorWindow.focusedWindow.Repaint);
							af.speed = 1F;
							af.target = 0F;
							this.testInputAnimationFeedback.Add(new GUITimer(EditorWindow.focusedWindow.Repaint, Constants.CheckFadeoutCooldown, 0F));
						}

						if (Preferences.Settings.inputsManager.groups[this.selectedInputsGroup].commands[j].Check() == true)
						{
							this.testInputAnimationFeedback[n].Start();
#if UNITY_4_6 || UNITY_5
							this.testInputAnimationFeedback[n].af.valueChanged.RemoveAllListeners();
							this.testInputAnimationFeedback[n].af.valueChanged.AddListener(EditorWindow.focusedWindow.Repaint);
#endif
						}

						if (this.testInputAnimationFeedback[n].Value > 0F)
						{
#if UNITY_4_5
							this.DrawInputCommand(Preferences.Settings.inputsManager.groups[this.selectedInputsGroup].commands[j]);
							Rect	r = GUILayoutUtility.GetLastRect();
							r.width = 2F;
							EditorGUI.DrawRect(r, Color.Lerp(GUI.contentColor, ConsoleSettingsEditor.HighlightInput, this.testInputAnimationFeedback[n].Value));
#else
							using (ColorContentRestorer.Get(Color.Lerp(GUI.contentColor, ConsoleSettingsEditor.HighlightInput, this.testInputAnimationFeedback[n].Value)))
								this.DrawInputCommand(Preferences.Settings.inputsManager.groups[this.selectedInputsGroup].commands[j]);
#endif
						}
						else
							this.DrawInputCommand(Preferences.Settings.inputsManager.groups[this.selectedInputsGroup].commands[j]);
					}

					GUILayout.Space(10F);
				}
				EditorGUILayout.EndScrollView();
			}
		}

		private void	DrawInputCommand(InputCommand command)
		{
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label(LC.G("Input_" + command.name), GeneralStyles.Title1, GUILayout.ExpandWidth(false));

				if (this.registeringCommand == command)
				{
					if (Event.current.type == EventType.KeyUp)
					{
						if (Event.current.keyCode == KeyCode.LeftControl || Event.current.keyCode == KeyCode.RightControl)
							command.control = !command.control;
						else if (Event.current.keyCode == KeyCode.LeftShift || Event.current.keyCode == KeyCode.RightShift)
							command.shift = !command.shift;
						else if (Event.current.keyCode == KeyCode.LeftAlt || Event.current.keyCode == KeyCode.RightAlt)
							command.alt = !command.alt;
						else
							command.keyCode = Event.current.keyCode;

						Event.current.Use();
					}
					else if (Event.current.shift == true)
						this.shiftPressed = true;
					else if (this.shiftPressed == true)
					{
						command.shift = !command.shift;
						this.shiftPressed = false;
					}

					if (GUILayout.Button(LC.G("Stop"), GUILayout.MaxWidth(80F)) == true)
						registeringCommand = null;

					GUILayout.Label(LC.G("ConsoleSettings_PressAny"));

					// Force repaint to handle shift input.
					//this.Repaint();
				}
				else if (GUILayout.Button(LC.G("Edit"), GUILayout.MaxWidth(80F)) == true)
				{
					Utility.content.text = LC.G("InputsWizard_PressAnythingToEditCommand");
					registeringCommand = command;
					this.shiftPressed = false;
				}
			}
			EditorGUILayout.EndHorizontal();

			string	description = LC.G("Input_" + command.name + InputCommand.DescriptionLocalizationSuffix);

			if (string.IsNullOrEmpty(description) == false)
				EditorGUILayout.LabelField(description, GeneralStyles.WrapLabel);

			EditorGUILayout.BeginHorizontal();
			{
				command.keyCode = (KeyCode)EditorGUILayout.EnumPopup(command.keyCode, GUILayout.Width(130F));

				command.control = GUILayout.Toggle(command.control, "Ctrl", this.menuButtonStyle);
				command.shift = GUILayout.Toggle(command.shift, "Shift", this.menuButtonStyle);
				command.alt = GUILayout.Toggle(command.alt, "Alt", this.menuButtonStyle);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}