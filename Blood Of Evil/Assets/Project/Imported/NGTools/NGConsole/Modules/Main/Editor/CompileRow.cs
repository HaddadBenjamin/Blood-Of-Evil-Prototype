using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal class CompileRow : Row, ILogContentGetter
	{
		[Serializable]
		struct ErrorLine
		{
			public readonly int		line;
			public readonly string	error;
			public readonly string	message;
			public readonly bool	isError;

			public	ErrorLine(int line, string error, string message, bool isError)
			{
				this.line = line;
				this.error = error;
				this.message = message;
				this.isError = isError;
			}
		}

		public static Color	SubRowHighlightColor = Color.grey;

		public string	HeadMessage { get { return this.logParser.HeadMessage; } }
		public string	FullMessage { get { return this.logParser.FullMessage; } }
		public string	StackTrace { get { return this.logParser.StackTrace; } }
		public bool		HasStackTrace { get { return this.logParser.HasStackTrace; } }
		public string	Category { get { return this.logParser.Category; } }

		/// <summary>
		/// <para>An array of Frames giving parsed data.</para>
		/// <para>Is generated once on demand.</para>
		/// </summary>
		public Frame[]	Frames { get { return this.logParser.Frames; } }

		public readonly string	file;

		public bool	isOpened;
		public bool	hasError;

		[NonSerialized]
		private int	lastGoToFile;

		[NonSerialized]
		private int	selectedSubRow = -1;
		private List<ErrorLine>	errorLines;
		[NonSerialized]
		private LogConditionParser	logParser;

		public	CompileRow(ILogContentGetter logContent)
		{
			this.isOpened = true;
			this.file = logContent.Frames[0].fileName;
			this.errorLines = new List<ErrorLine>();
		}

		public override void	Init(NGConsoleWindow editor, LogEntry log)
		{
			base.Init(editor, log);

			this.logParser = new LogConditionParser(log);

			this.commands.Add(RowsDrawer.ShortCopyCommand, this.ShortCopy);
			this.commands.Add(RowsDrawer.FullCopyCommand, this.FullCopy);
			this.commands.Add(RowsDrawer.HandleKeyboardCommand, this.HandleKeyboard);
		}

		public override void	Uninit()
		{
			this.logParser.Uninit();
		}

		public override float	GetWidth()
		{
			return 0F;
		}

		public override float	GetHeight()
		{
			if (this.isOpened == false)
				return Preferences.Settings.log.height;
			else
				return Preferences.Settings.log.height + this.errorLines.Count * Preferences.Settings.log.height;
		}

		public override void	DrawRow(RowsDrawer rowsDrawer, Rect r, int i, bool? collapse)
		{
			float	originWidth = Utility.drawingWindow.position.width - rowsDrawer.verticalScrollbarWidth + rowsDrawer.currentVars.scrollX;

			// Draw highlight.
			r.x = rowsDrawer.currentVars.scrollX;
			r.width = originWidth;
			r.height = Preferences.Settings.log.height;

			this.DrawBackground(rowsDrawer, r, i);

			r.width = r.height;
			bool	lastValue = this.isOpened;
			if (this.hasError == true)
				this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.ErrorFoldoutStyle);
			else
				this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.WarningFoldoutStyle);
			if (lastValue != this.isOpened)
				rowsDrawer.InvalidateViewHeight();

			r.x += r.width;
			r.width = originWidth - r.width;

			this.HandleDefaultSelection(rowsDrawer, r, i);

			// Toggle on middle-click.
			if (Event.current.type == EventType.MouseDown &&
				Event.current.button == 2 &&
				r.Contains(Event.current.mousePosition) == true)
			{
				this.isOpened = !this.isOpened;
				rowsDrawer.InvalidateViewHeight();
				Event.current.Use();
			}
			// Show menu on right click up.
			else if (Event.current.type == EventType.MouseUp &&
					 Event.current.button == 1 &&
					 r.Contains(Event.current.mousePosition) == true &&
					 rowsDrawer.currentVars.IsSelected(i) == true)
			{
				GenericMenu	menu = new GenericMenu();

				menu.AddItem(new GUIContent(LC.G("CopyCurrentError")), false, this.MenuCopyCurrentError, this);
				menu.AddItem(new GUIContent(LC.G("CopyAllErrors")), false, this.MenuCopyAllErrors, this);

				if (RowsDrawer.GlobalLogContextMenu != null)
					RowsDrawer.GlobalLogContextMenu(menu, this);
				if (rowsDrawer.LogContextMenu != null)
					rowsDrawer.LogContextMenu(menu, this);

				menu.ShowAsContext();

				Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseUp &&
					 Event.current.button == 0 &&
					 r.Contains(Event.current.mousePosition) == true)
			{
				if (RowUtility.LastClickTime + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
				{
					bool	focus = false;

					if ((Event.current.modifiers & Preferences.Settings.log.forceFocusOnModifier) != 0)
						focus = true;

					RowUtility.GoToFileLine(this.file,
											this.errorLines[this.lastGoToFile].line,
											focus);

					++this.lastGoToFile;
					if (this.lastGoToFile >= this.errorLines.Count)
						this.lastGoToFile = 0;
				}

				RowUtility.LastClickTime = EditorApplication.timeSinceStartup;
			}

			GUI.Label(r, this.file + " (" + this.errorLines.Count + ")", Preferences.Settings.log.style);
			r.y += Preferences.Settings.log.height;

			if (this.isOpened == true)
			{
				for (int j = 0; j < this.errorLines.Count; j++)
				{
					r.x = rowsDrawer.currentVars.scrollX;
					r.width = originWidth;

					if (Event.current.type == EventType.Repaint &&
						this.selectedSubRow == j &&
						rowsDrawer.currentVars.CountSelection == 1 &&
						rowsDrawer.currentVars.GetSelection(0) == i)
					{
						EditorGUI.DrawRect(r, CompileRow.SubRowHighlightColor);
					}

					// Handle mouse inputs per log.
					if (r.Contains(Event.current.mousePosition) == true)
					{
						// Toggle on middle click.
						if (Event.current.type == EventType.MouseDown &&
							Event.current.button == 0)
						{
							if (RowUtility.LastClickTime + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
							{
								bool	focus = false;

								if ((Event.current.modifiers & Preferences.Settings.log.forceFocusOnModifier) != 0)
									focus = true;

								RowUtility.GoToFileLine(this.file,
														this.errorLines[j].line,
														focus);
							}
							else
							{
								rowsDrawer.currentVars.ClearSelection();
								rowsDrawer.currentVars.AddSelection(i);

								this.selectedSubRow = j;

								this.log.condition = this.errorLines[j].message;
							}

							RowUtility.LastClickTime = EditorApplication.timeSinceStartup;

							Event.current.Use();
						}
					}

					// Handle inputs.
					Utility.content.text = this.errorLines[j].line.ToString();
					r.width = Preferences.Settings.log.style.CalcSize(Utility.content).x;
					GUI.Label(r,
							  Utility.Color(Utility.content.text, Preferences.Settings.stackTrace.lineColor),
							  Preferences.Settings.log.style);

					r.x += r.width;
					Utility.content.text = this.errorLines[j].error;
					r.width = Preferences.Settings.log.style.CalcSize(Utility.content).x;
					GUI.Label(r,
							  Utility.Color(this.errorLines[j].error,
											this.errorLines[j].isError == true ? Preferences.Settings.mainModule.errorColor : Preferences.Settings.mainModule.warningColor),
							  Preferences.Settings.log.style);

					r.x += r.width;
					r.width = originWidth - r.x;
					GUI.Label(r, this.errorLines[j].message, Preferences.Settings.log.style);

					r.y += Preferences.Settings.log.height;
				}
			}
		}

		public virtual void	AppendRow(Row row)
		{
			string	raw = (row as ILogContentGetter).HeadMessage;
			int		comma = raw.IndexOf(':') + 2;

			// Totaly empty log is possible! Yes it is! No stacktrace, no file, no line, no log at all!
			// Bug ref #718608_dh6en6gdivrpuv0q
			if (comma == -1)
				return;

			raw = raw.Substring(comma); // Skip ':' and ' ' after "File(Line,Col)"

			// Separate the error number and the message.
			comma = raw.IndexOf(':');

			if (comma == -1)
				return;

			string	error = raw.Substring(0, comma);
			string	message = raw.Substring(comma + 2); // Remove the comma and its following space.

			this.errorLines.Add(new ErrorLine(row.log.line,
											error.Substring(error.IndexOf(' ') + 1), // Just keep the error number.
											message,
											error.Contains("error")));

			if (this.hasError == false &&
				(row.log.mode & Mode.ScriptCompileError) != 0)
			{
				this.hasError = true;
			}
		}

		public virtual bool	CanAddRow(Row row)
		{
			ILogContentGetter	logContent = row as ILogContentGetter;

			return this.file == logContent.Frames[0].fileName;
		}

		public virtual void	Clear()
		{
			this.errorLines.Clear();
		}

		private object	ShortCopy(object row)
		{
			if (this.selectedSubRow < 0 || this.selectedSubRow >= this.errorLines.Count)
				return string.Empty;

			ErrorLine	error = this.errorLines[this.selectedSubRow];

			return this.file + " " + error.line + " " + error.error + ' ' + error.message;
		}

		private object	FullCopy(object row)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			buffer.AppendLine(this.file);

			for (int i = 0; i < this.errorLines.Count; i++)
				buffer.AppendLine(this.errorLines[i].line + " " + this.errorLines[i].error + ' ' + this.errorLines[i].message);

			buffer.Length -= Environment.NewLine.Length;

			return Utility.ReturnBuffer(buffer);
		}

		private void	MenuCopyCurrentError(object row)
		{
			EditorGUIUtility.systemCopyBuffer = this.ShortCopy(null) as string;
		}

		private void	MenuCopyAllErrors(object row)
		{
			EditorGUIUtility.systemCopyBuffer = this.FullCopy(null) as string;
		}

		private object	HandleKeyboard(object data)
		{
			RowsDrawer	rowsDrawer = data as RowsDrawer;

			if (Event.current.type == EventType.KeyDown)
			{
				if (Preferences.Settings.inputsManager.Check("Navigation", Constants.CloseLogCommand) == true)
				{
					if (this.isOpened == true)
					{
						this.isOpened = false;
						rowsDrawer.InvalidateViewHeight();
						Utility.drawingWindow.Repaint();
					}
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.OpenLogCommand) == true)
				{
					if (this.isOpened == false)
					{
						this.isOpened = true;
						rowsDrawer.InvalidateViewHeight();
						Utility.drawingWindow.Repaint();
					}
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.GoToLineCommand) == true &&
						 rowsDrawer.currentVars.CountSelection == 1 &&
						 this.Frames.Length > 0)
				{
					string	fileName = this.Frames[0].fileName;
					int		line = this.Frames[0].line;
					bool	focus = (Event.current.modifiers & Preferences.Settings.log.forceFocusOnModifier) != 0;

					RowUtility.GoToFileLine(fileName, line, focus);
					Utility.drawingWindow.Repaint();
					Event.current.Use();
				}
			}

			return null;
		}
	}
}