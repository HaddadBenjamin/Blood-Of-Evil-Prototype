using NGTools;
using System;
using System.IO;
using System.Text;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	public static class RowUtility
	{
		public static double	LastKeyTime;
		public static double	LastClickTime;
		public static int		LastClickIndex;

		// Preview's fields.
		private static EditorWindow	previewEditorWindow;
		private static Rect			previewRect;
		private static Frame		previewFrame;
		private static string[]		previewLines;
		private static RowsDrawer	rowsDrawer;

		public static void	ClearPreview()
		{
			if (RowUtility.previewLines == null)
				return;

			RowUtility.previewEditorWindow = null;
			RowUtility.previewLines = null;
			RowUtility.previewFrame = null;
			RowUtility.rowsDrawer.AfterAllRows -= RowUtility.DrawPreview;
			RowUtility.rowsDrawer = null;
		}

		public static void	PreviewStackFrame(RowsDrawer rowsDrawer, Rect r, Frame frame)
		{
			if (frame.fileExist == true &&
				RowUtility.previewFrame != frame)
			{
				try
				{
					RowUtility.previewLines = Utility.files.GetFile(frame.fileName);
					RowUtility.rowsDrawer = rowsDrawer;
					RowUtility.previewFrame = frame;
					RowUtility.previewRect = r;

					FilesWatcher.Watch(frame.fileName);

					RowUtility.previewEditorWindow = Utility.drawingWindow;
					RowUtility.rowsDrawer.AfterAllRows -= RowUtility.DrawPreview;
					RowUtility.rowsDrawer.AfterAllRows += RowUtility.DrawPreview;
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException(ex);
					RowUtility.ClearPreview();
				}
			}
		}

		private static void	DrawPreview(Rect bodyRect)
		{
			Rect	r = RowUtility.previewRect;

			r.x += bodyRect.x;
			r.y += bodyRect.y;

			// Out of window.
			if (EditorWindow.mouseOverWindow != RowUtility.previewEditorWindow)
			{
				RowUtility.ClearPreview();
				return;
			}
			// Out of stacktrace.
			//else if (Event.current.type == EventType.MouseMove)
			if (Event.current.type == EventType.MouseMove && r.Contains(Event.current.mousePosition) == false)
			{
				RowUtility.ClearPreview();
				return;
			}

			if (RowUtility.previewFrame.line <= RowUtility.previewLines.Length)
			{
				float	maxWidth = float.MinValue;

				r.x = Event.current.mousePosition.x + Preferences.Settings.stackTrace.previewOffset.x;
				r.y = Event.current.mousePosition.y + Preferences.Settings.stackTrace.previewOffset.y;
				r.width = RowUtility.previewRect.width;
				r.height = Preferences.Settings.stackTrace.previewHeight;

				for (int i = RowUtility.previewFrame.line - Preferences.Settings.stackTrace.previewLinesBeforeStackFrame - 1,
					 max = Mathf.Min(RowUtility.previewFrame.line + Preferences.Settings.stackTrace.previewLinesAfterStackFrame, RowUtility.previewLines.Length);
					 i < max; i++)
				{
					Utility.content.text = RowUtility.previewLines[i];
					Vector2	size = Preferences.Settings.stackTrace.previewSourceCodeStyle.CalcSize(Utility.content);
					if (size.x > maxWidth)
						maxWidth = size.x;
				}

				r.width = maxWidth;
				for (int i = RowUtility.previewFrame.line - Preferences.Settings.stackTrace.previewLinesBeforeStackFrame - 1,
					 max = Mathf.Min(RowUtility.previewFrame.line + Preferences.Settings.stackTrace.previewLinesAfterStackFrame, RowUtility.previewLines.Length);
					 i < max; i++)
				{
					if (Event.current.type == EventType.Repaint)
					{
						if (i + 1 != RowUtility.previewFrame.line)
							EditorGUI.DrawRect(r, Preferences.Settings.stackTrace.previewSourceCodeBackgroundColor);
						else
							EditorGUI.DrawRect(r, Preferences.Settings.stackTrace.previewSourceCodeMainLineBackgroundColor);
					}

					GUI.Label(r, RowUtility.previewLines[i], Preferences.Settings.stackTrace.previewSourceCodeStyle);
					r.y += r.height;
				}

				Utility.drawingWindow.Repaint();
			}
		}

		public static void	GoToFileLine(string file, int line, bool focus)
		{
			if (file.StartsWith("Assets") == true)
				file = Path.Combine(Application.dataPath, file.Substring(7)).Replace('/', '\\');
			else
				file = Application.dataPath + '/' + file;

			if (Preferences.Settings.general.openMode == NGSettings.ModeOpen.AssetDatabaseOpenAsset)
			{
				Object	sourceFile = AssetDatabase.LoadAssetAtPath(@"Assets\" + file.Substring(Application.dataPath.Length + 1), typeof(TextAsset));
				if (sourceFile != null)
					AssetDatabase.OpenAsset(sourceFile, line);
				else
					Debug.LogWarning(string.Format(LC.G("Console_AssetNotText"), @"Assets\" + file.Substring(Application.dataPath.Length + 1)));
			}
			else if (Preferences.Settings.general.openMode == NGSettings.ModeOpen.NGConsoleOpener)
			{
				NGSettings.GeneralSettings.EditorExtensions editorExtensions = null;
				string	editorPath = NGEditorPrefs.GetString(Constants.ScriptDefaultApp);
				string	fileExtension = Path.GetExtension(file);

				if ((string.IsNullOrEmpty(editorPath) == true ||
					 editorPath == "internal") &&
					string.IsNullOrEmpty(fileExtension) == false)
				{
					fileExtension = fileExtension.Substring(1);
					for (int i = 0; i < Preferences.Settings.general.editorExtensions.Length; i++)
					{
						for (int j = 0; j < Preferences.Settings.general.editorExtensions[i].extensions.Length; j++)
						{
							if (Preferences.Settings.general.editorExtensions[i].extensions[j].Equals(fileExtension, StringComparison.OrdinalIgnoreCase) == true)
							{
								editorExtensions = Preferences.Settings.general.editorExtensions[i];
								editorPath = Preferences.Settings.general.editorExtensions[i].editor;
								goto doubleBreak;
							}
						}
					}
				}
				doubleBreak:

				// It is required to delay the opening, due editor sometimes falling into error state during the same frame.

				// Fallback when it is log without any reachable file. (Happened with a non-usable dll.)
				if (string.IsNullOrEmpty(editorPath) == true || editorExtensions == null)
				{
					Object	sourceFile = AssetDatabase.LoadAssetAtPath(@"Assets\" + file.Substring(Application.dataPath.Length + 1), typeof(Object));
					if (sourceFile != null)
						EditorApplication.delayCall += () => AssetDatabase.OpenAsset(sourceFile, line);
					else
					{
						EditorApplication.delayCall += () => EditorUtility.OpenWithDefaultApp(file);
					}
					return;
				}

				foreach (var opener in NGConsoleWindow.openers)
				{
					if (opener.CanHandleEditor(editorPath) == true)
					{
						EditorApplication.delayCall += () => opener.Open(editorPath, editorExtensions.arguments, file, line);

						// Easy trick to give focus to the application.
						if (Preferences.Settings.log.giveFocusToEditor == true || focus == true)
							EditorApplication.delayCall += () => EditorUtility.OpenWithDefaultApp(file);
						break;
					}
				}
			}
		}

		public static Rect	DrawStackTrace(ILogContentGetter log, RowsDrawer rowsDrawer, Rect r, int i, Row row)
		{
			float	width = r.width;

			// Substract viewRect to avoid scrollbar.
			r.height = Preferences.Settings.stackTrace.height;

			// Display the stack trace.
			int	j = 0;
			foreach (var frame in log.Frames)
			{
				// Hide invisible frames.
				if (r.y - rowsDrawer.currentVars.scrollY > rowsDrawer.bodyRect.height)
					break;

				r.x = 0F;
				r.width = width - 16F;
				GUI.SetNextControlName("SF" + i + j);
				if (GUI.Button(r, frame.frameString, Preferences.Settings.stackTrace.style) == true)
				{
					if (FreeConstants.CheckLowestRowGoToLineAllowed(j) == true)
					{
						GUI.FocusControl("SF" + i + j);

						r.x -= rowsDrawer.currentVars.scrollX;
						r.y -= rowsDrawer.currentVars.scrollY;
						RowUtility.GoToLine(r, frame);
						r.x += rowsDrawer.currentVars.scrollX;
						r.y += rowsDrawer.currentVars.scrollY;
					}
				}

				// Handle hover overflow.
				if (r.y - rowsDrawer.currentVars.scrollY + r.height > rowsDrawer.bodyRect.height)
					r.height = rowsDrawer.bodyRect.height - r.y + rowsDrawer.currentVars.scrollY;

				if (Event.current.type == EventType.MouseMove && r.Contains(Event.current.mousePosition) == true)
				{
					r.x -= rowsDrawer.currentVars.scrollX;
					r.y -= rowsDrawer.currentVars.scrollY;
					RowUtility.PreviewStackFrame(rowsDrawer, r, frame);
					r.x += rowsDrawer.currentVars.scrollX;
					r.y += rowsDrawer.currentVars.scrollY;
				}

				r.x = r.width;
				r.width = 16F;
				if (GUI.Button(r, "+", Preferences.Settings.stackTrace.style) == true)
				{
					GenericMenu	menu = new GenericMenu();

					menu.AddItem(new GUIContent("Set as Category"), false, RowUtility.SetAsCategory, frame.raw);

					string	f = RowUtility.GetFilterNamespace(frame.raw);
					if (f != null)
						menu.AddItem(new GUIContent("Skip Namespace \"" + f + "\""), false, RowUtility.FilterNamespace, frame.raw);

					f = RowUtility.GetFilterClass(frame.raw);
					if (f != null)
						menu.AddItem(new GUIContent("Skip Class \"" + f + "\""), false, RowUtility.FilterClass, frame.raw);

					f = RowUtility.GetFilterMethod(frame.raw);
					if (f != null)
						menu.AddItem(new GUIContent("Skip Method \"" + f + "\""), false, RowUtility.FilterMethod, frame.raw);

					menu.AddItem(new GUIContent("Manage filters"), false, RowUtility.GoToSettings);
					menu.ShowAsContext();
				}

				++j;
				r.y += r.height;
			}

			return r;
		}

		private static void	GoToSettings()
		{
			EditorWindow.GetWindow<NGSettingsWindow>("NG Settings", true).Focus(NGConsoleWindow.Title);
			ConsoleSettingsEditor	editor = NGConsoleWindow.settingsEditor;
			editor.currentTab = ConsoleSettingsEditor.MainTabs.General;
			editor.currentGeneralTab = ConsoleSettingsEditor.GeneralTabs.StackTrace;
			editor.generalStackTraceScrollPosition = Vector2.zero;
		}

		private static void	SetAsCategory(object data)
		{
			string	frame = data as string;

			// Fetch "namespace.class[:.]method(".
			int	n = frame.IndexOf("(");
			if (n == -1)
				return;

			string	method = frame.Substring(0, n);
			string	placeholder = "Category Name";

			for (int i = 0; i < Preferences.Settings.stackTrace.categories.Count; i++)
			{
				if (Preferences.Settings.stackTrace.categories[i].method == method)
				{
					placeholder = Preferences.Settings.stackTrace.categories[i].category;
					break;
				}
			}

			PromptWindow.Start(placeholder, RowUtility.SetCategory, method);
		}

		private static void	SetCategory(object data, string category)
		{
			string	method = data as string;

			for (int i = 0; i < Preferences.Settings.stackTrace.categories.Count; i++)
			{
				if (Preferences.Settings.stackTrace.categories[i].method == method)
				{
					if (string.IsNullOrEmpty(category) == false)
						Preferences.Settings.stackTrace.categories[i].category = category;
					else
						Preferences.Settings.stackTrace.categories.RemoveAt(i);

					LogConditionParser.cachedFrames.Clear();
					LogConditionParser.cachedFramesArrays.Clear();
					MainModule.methodsCategories.Clear();
					return;
				}
			}

			if (string.IsNullOrEmpty(category) == false)
			{
				Preferences.Settings.stackTrace.categories.Add(new NGSettings.StackTraceSettings.MethodCategory() { category = category, method = method });

				LogConditionParser.cachedFrames.Clear();
				LogConditionParser.cachedFramesArrays.Clear();
				MainModule.methodsCategories.Clear();
			}
		}

		private static void	FilterNamespace(object data)
		{
			RowUtility.AddFrameFilter(RowUtility.GetFilterNamespace(data as string));
		}

		private static void	FilterClass(object data)
		{
			RowUtility.AddFrameFilter(RowUtility.GetFilterClass(data as string));
		}

		private static void	FilterMethod(object data)
		{
			RowUtility.AddFrameFilter(RowUtility.GetFilterMethod(data as string));
		}

		private static string	GetFilterNamespace(object data)
		{
			string	frame = data as string;

			// Fetch "namespace.class[:.]method(".
			int	n = frame.IndexOf("(");
			if (n == -1)
				return null;

			// Reduce to "namespace.class[:.]".
			int	n2 = frame.IndexOf(":", 0, n);
			if (n2 == -1)
			{
				n = frame.LastIndexOf(".", n);
				if (n == -1)
					return null;
			}
			else
				n = n2;

			// Reduce to "namespace.".
			n = frame.IndexOf(".", 0, n);
			if (n == -1)
				return null;

			return frame.Substring(0, n + 1);
		}

		private static string	GetFilterClass(string frame)
		{
			// Fetch "namespace.class[:.]method(".
			int	n = frame.IndexOf("(");
			if (n == -1)
				return null;

			// Reduce to "namespace.class[:.]".
			int	n2 = frame.IndexOf(":", 0, n);
			if (n2 == -1)
			{
				n = frame.LastIndexOf(".", n);
				if (n == -1)
					return null;
			}
			else
				n = n2;

			return frame.Substring(0, n + 1);
		}

		private static string	GetFilterMethod(object data)
		{
			string	frame = data as string;

			// Fetch "namespace.class[:.]method(".
			int	n = frame.IndexOf("(");
			if (n == -1)
				return null;

			return frame.Substring(0, n + 1);
		}

		private static void	AddFrameFilter(string filter)
		{
			if (Preferences.Settings.stackTrace.filters.Contains(filter) == false)
			{
				Preferences.Settings.stackTrace.filters.Add(filter);
				Preferences.InvalidateSettings();
				EditorUtility.DisplayDialog(Constants.PackageTitle, "\"" + filter + "\" has been added to the filters.", "OK");
			}
			else
				EditorUtility.DisplayDialog(Constants.PackageTitle, "\"" + filter + "\" is already a filter.", "OK");
		}

		private static void	GoToLine(Rect r, Frame frame)
		{
			if (Event.current.button == 0 &&
				frame.fileExist == true)
			{
				// Ping folder on click + modifiers.
				if ((Event.current.modifiers & Preferences.Settings.stackTrace.pingFolderOnModifier) != 0)
				{
					int	i = frame.frameString.LastIndexOf('	') + 1;

					Utility.content.text = frame.frameString.Substring(0, i);
					var	v = Preferences.Settings.stackTrace.style.CalcSize(Utility.content);

					StringBuilder	buffer = Utility.GetBuffer();

					if (Event.current.mousePosition.x >= v.x)
					{
						int	i2 = frame.frameString.IndexOf('/', i + 1);

						// Skip Assets folder.
						string	folder = frame.frameString.Substring(i, i2 - i).Split('>')[1];

						Utility.content.text += folder;
						v = Preferences.Settings.stackTrace.style.CalcSize(Utility.content);

						if (Event.current.mousePosition.x > v.x)
						{
							i = i2;
							buffer.Append(folder);

							while (Event.current.mousePosition.x >= v.x)
							{
								i2 = frame.frameString.IndexOf('/', i + 1);
								if (i2 == -1)
									break;

								folder = frame.frameString.Substring(i, i2 - i);
								buffer.Append(folder);
								Utility.content.text += folder;
								v = Preferences.Settings.stackTrace.style.CalcSize(Utility.content);
								i = i2;
							}

							EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(Utility.ReturnBuffer(buffer), typeof(Object)));
						}
					}
				}
				// Or go to line.
				else
				{
					bool	focus = (Event.current.modifiers & Preferences.Settings.log.forceFocusOnModifier) != 0;

					RowUtility.GoToFileLine(frame.fileName, frame.line, focus);

					Event.current.Use();
				}
			}
		}

		public static void	GoToLine(ILogContentGetter log, LogEntry logEntry, bool focus)
		{
			// Prefer using instanceID as much as possible, more reliable.
			// Try to reach the object, it might not be a TextAsset.
			if (logEntry.instanceID != 0)
			{
				string	path = AssetDatabase.GetAssetPath(logEntry.instanceID);
				if (string.IsNullOrEmpty(path) == false)
				{
					RowUtility.GoToFileLine(path,
											logEntry.line,
											focus);
					return;
				}
			}

			// Go to the first reachable frame.
			for (int j = 0; j < log.Frames.Length; j++)
			{
				if (log.Frames[j].fileExist == true)
				{
					RowUtility.GoToFileLine(log.Frames[j].fileName,
											log.Frames[j].line,
											focus);
					break;
				}
			}
		}
	}
}