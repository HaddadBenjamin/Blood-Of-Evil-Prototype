using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal sealed class ErrorCompileRow : CompileRow, ILogContentGetter
	{
		public struct FileLine
		{
			public readonly int		line;
			public readonly string	file;
			public readonly string	message;
			public readonly bool	isError;

			public	FileLine(int line, string file, string message, bool isError)
			{
				this.line = line;
				this.file = file;
				this.message = message;
				this.isError = isError;
			}
		}

		public readonly string	error;
		private readonly string	lookupError;

		[NonSerialized]
		private int	lastGoToFile;

		[NonSerialized]
		private int	selectedSubRow = -1;
		[NonSerialized]
		private List<FileLine>	fileLines;

		public	ErrorCompileRow(ILogContentGetter logContent) : base(logContent)
		{
			this.fileLines = new List<FileLine>();
			this.isOpened = true;

			string	raw = logContent.HeadMessage;
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

			this.hasError = raw.Contains("error");

			this.error = raw.Substring(0, comma);
			this.error = this.error.Substring(this.error.IndexOf(' ') + 1); // Just keep the error number.
			this.lookupError = (this.hasError == true ? "error " : "warning ") + this.error + ":";
		}

		public override float	GetHeight()
		{
			if (this.isOpened == false)
				return Preferences.Settings.log.height;
			else
				return Preferences.Settings.log.height + this.fileLines.Count * Preferences.Settings.log.height;
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
			if (r.Contains(Event.current.mousePosition) == true &&
				Event.current.type == EventType.MouseDown &&
				Event.current.button == 2)
			{
				this.isOpened = !this.isOpened;
				rowsDrawer.InvalidateViewHeight();
				Event.current.Use();
			}
			else if (r.Contains(Event.current.mousePosition) == true &&
					 Event.current.type == EventType.MouseUp &&
					 Event.current.button == 0)
			{
				if (RowUtility.LastClickTime + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
				{
					bool	focus = false;

					if ((Event.current.modifiers & Preferences.Settings.log.forceFocusOnModifier) != 0)
						focus = true;

					RowUtility.GoToFileLine(this.file,
											this.fileLines[this.lastGoToFile].line,
											focus);

					++this.lastGoToFile;
					if (this.lastGoToFile >= this.fileLines.Count)
						this.lastGoToFile = 0;
				}

				RowUtility.LastClickTime = EditorApplication.timeSinceStartup;
			}

			GUI.Label(r, this.error + " (" + this.fileLines.Count + ")", Preferences.Settings.log.style);
			r.y += Preferences.Settings.log.height;

			if (this.isOpened == true)
			{
				for (int j = 0; j < this.fileLines.Count; j++)
				{
					r.x = rowsDrawer.currentVars.scrollX;
					r.width = originWidth;

					if (Event.current.type == EventType.Repaint &&
						this.selectedSubRow == j &&
						rowsDrawer.currentVars.CountSelection > 0 &&
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

								RowUtility.GoToFileLine(this.fileLines[j].file,
														this.fileLines[j].line,
														focus);
							}
							else
							{
								rowsDrawer.currentVars.ClearSelection();
								rowsDrawer.currentVars.AddSelection(i);

								this.selectedSubRow = j;

								this.log.condition = this.fileLines[j].message;
							}

							RowUtility.LastClickTime = EditorApplication.timeSinceStartup;

							Event.current.Use();
						}
					}

					// Handle inputs.
					Utility.content.text = this.fileLines[j].line.ToString();
					r.width = Preferences.Settings.log.style.CalcSize(Utility.content).x;
					GUI.Label(r,
							  Utility.Color(Utility.content.text, Preferences.Settings.stackTrace.lineColor),
							  Preferences.Settings.log.style);

					r.x += r.width;
					r.width = originWidth - r.x;
					GUI.Label(r, this.fileLines[j].message, Preferences.Settings.log.style);

					r.y += Preferences.Settings.log.height;
				}
			}
		}

		public override void	AppendRow(Row row)
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

			string	message = raw.Substring(comma + 2); // Remove the comma and its following space.

			this.fileLines.Add(new FileLine(row.log.line,
											row.log.file,
											message,
											this.hasError));
		}

		public override bool	CanAddRow(Row row)
		{
			ILogContentGetter	logContent = row as ILogContentGetter;

			return logContent.HeadMessage.Contains(this.lookupError);
		}

		public override void Clear()
		{
			base.Clear();

			this.fileLines.Clear();
		}
	}
}