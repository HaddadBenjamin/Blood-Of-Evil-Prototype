using NGTools;
using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	[RowLogHandler(20)]
	internal sealed class DataRow : Row, ILogContentGetter
	{
		/// <summary>Defines whether the Row is open or not by RowsDrawer.</summary>
		public bool	isOpened;

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

		/// <summary>Defines if the Row is ready to be used. Do never use a main value (Head message, full message, stack trace) from non-ready Row! IsParsed is used to delay or to skip non-essential Row from computation when receiving massive logs.</summary>
		public bool	isParsed;

		private LogConditionParser	logParser;

		private string	firstLine;
		private string	fields;
		private float	fieldsHeight;

		private static bool	CanDealWithIt(UnityLogEntry log)
		{
			return log.condition[0] == InternalNGDebug.DataStartChar;
		}

		public override void	Init(NGConsoleWindow editor, LogEntry log)
		{
			base.Init(editor, log);

			this.logParser = new LogConditionParser(this.log);

			this.commands.Add(RowsDrawer.ShortCopyCommand, this.ShortCopy);
			this.commands.Add(RowsDrawer.FullCopyCommand, this.FullCopy);
			this.commands.Add(RowsDrawer.CopyStackTraceCommand, this.CopyStackTrace);
			this.commands.Add(RowsDrawer.HandleKeyboardCommand, this.HandleKeyboard);

			this.isOpened = false;
			this.isParsed = false;
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
			if (this.isOpened == true)
				return Preferences.Settings.log.height + this.fieldsHeight;
			return Preferences.Settings.log.height;
		}

		public override void	DrawRow(RowsDrawer rowsDrawer, Rect r, int i, bool? collapse)
		{
			if (this.isParsed == false)
				this.ParseLog();

			float	originWidth = Utility.drawingWindow.position.width - rowsDrawer.verticalScrollbarWidth + rowsDrawer.currentVars.scrollX;

			// Draw highlight.
			r.x = rowsDrawer.currentVars.scrollX;
			r.width = originWidth;
			r.height = Preferences.Settings.log.height;

			this.DrawBackground(rowsDrawer, r, i);

			r.width = r.height;
			EditorGUI.DrawRect(r, Color.gray);
			bool	lastValue = this.isOpened;
			this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.NormalFoldoutStyle);
			if (lastValue != this.isOpened && this.fields != string.Empty)
				rowsDrawer.InvalidateViewHeight();
			r.x = r.width;
			r.width = originWidth - r.x;

			// Handle mouse inputs.
			if (r.Contains(Event.current.mousePosition) == true)
			{
				// Toggle on middle click.
				if (Event.current.type == EventType.MouseDown &&
					Event.current.button == 2)
				{
					if (rowsDrawer.currentVars.IsSelected(i) == false)
					{
						if (Event.current.control == false)
							rowsDrawer.currentVars.ClearSelection();

						rowsDrawer.currentVars.AddSelection(i);

						if (Event.current.control == false)
							rowsDrawer.FitFocusedLogInScreen(i);

						GUI.FocusControl("L" + i);
					}

					if (this.fields != string.Empty)
						this.isOpened = !this.isOpened;
					rowsDrawer.InvalidateViewHeight();
					Event.current.Use();
				}
				// Show menu on right click up.
				else if (Event.current.type == EventType.MouseUp &&
						 Event.current.button == 1 &&
						 rowsDrawer.currentVars.IsSelected(i) == true)
				{
					GenericMenu	menu = new GenericMenu();

					menu.AddItem(new GUIContent(LC.G("CopyLine")), false, rowsDrawer.MenuCopyLine, this);
					menu.AddItem(new GUIContent(LC.G("CopyLog")), false, rowsDrawer.MenuCopyLog, this);

					if (RowsDrawer.GlobalLogContextMenu != null)
						RowsDrawer.GlobalLogContextMenu(menu, this);
					if (rowsDrawer.LogContextMenu != null)
						rowsDrawer.LogContextMenu(menu, this);

					menu.ShowAsContext();

					Event.current.Use();
				}
				// Focus on right click down.
				else if (Event.current.type == EventType.MouseDown &&
						 Event.current.button == 1)
				{
					// Handle multi-selection.
					if (Event.current.control == true)
					{
						if (rowsDrawer.currentVars.IsSelected(i) == false)
							rowsDrawer.currentVars.AddSelection(i);
					}
					else
					{
						if (rowsDrawer.currentVars.IsSelected(i) == false)
						{
							if (Event.current.control == false)
								rowsDrawer.currentVars.ClearSelection();

							rowsDrawer.currentVars.AddSelection(i);
						}

						if (Event.current.control == false)
							rowsDrawer.FitFocusedLogInScreen(i);

						GUI.FocusControl("L" + i);
					}

					Event.current.Use();
				}
				// Focus on left click.
				else if (Event.current.type == EventType.MouseDown &&
						 Event.current.button == 0)
				{
					// Set the selection to the log's object if available.
					if (this.log.instanceID != 0 &&
						(Event.current.modifiers & Preferences.Settings.log.selectObjectOnModifier) != 0)
					{
						Selection.activeInstanceID = this.log.instanceID;
					}
					// Go to line if force focus is available.
					else if ((Event.current.modifiers & Preferences.Settings.log.forceFocusOnModifier) != 0)
					{
						RowUtility.GoToLine(this, this.log, true);
					}
					// Handle multi-selection.
					else if (Event.current.control == true &&
							 rowsDrawer.currentVars.IsSelected(i) == true)
					{
						rowsDrawer.currentVars.RemoveSelection(i);
					}
					else
					{
						if (Event.current.control == false)
						{
							if (rowsDrawer.currentVars.CountSelection != 1)
								rowsDrawer.bodyRect.height -= rowsDrawer.currentVars.rowContentHeight + Constants.RowContentSplitterHeight;
							else
							{
								// Reset last click when selection changes.
								if (rowsDrawer.currentVars.GetSelection(0) != i)
									RowUtility.LastClickTime = 0;
							}
							rowsDrawer.currentVars.ClearSelection();
						}

						rowsDrawer.currentVars.AddSelection(i);

						if (Event.current.control == false)
							rowsDrawer.FitFocusedLogInScreen(i);

						GUI.FocusControl("L" + i);
						Event.current.Use();
					}
				}
				// Handle normal behaviour on left click up.
				else if (Event.current.type == EventType.MouseUp &&
						 Event.current.button == 0)
				{
					// Go to line on double click.
					if (RowUtility.LastClickTime + Constants.DoubleClickTime > EditorApplication.timeSinceStartup &&
						RowUtility.LastClickIndex == i &&
						rowsDrawer.currentVars.IsSelected(i) == true)
					{
						bool	focus = false;

						if ((Event.current.modifiers & Preferences.Settings.log.forceFocusOnModifier) != 0)
							focus = true;

						RowUtility.GoToLine(this, this.log, focus);
					}
					// Ping on simple click.
					else if (this.log.instanceID != 0)
						EditorGUIUtility.PingObject(this.log.instanceID);

					RowUtility.LastClickTime = EditorApplication.timeSinceStartup;
					RowUtility.LastClickIndex = i;

					GUI.FocusControl("L" + i);
					Utility.drawingWindow.Repaint();
					Event.current.Use();
				}
			}

			r = this.DrawPreLogData(rowsDrawer, r);
			r.width = originWidth - r.x;

			GUI.Label(r, this.firstLine);

			if (this.isOpened == true)
			{
				r.y += r.height;
				r.x = rowsDrawer.currentVars.scrollX + 16F;
				r.width = originWidth;
				r.height = this.fieldsHeight;
				GUI.TextArea(r, this.fields, GeneralStyles.RichTextArea);
			}
		}

		private object	ShortCopy(object row)
		{
			return this.firstLine;
		}

		private object	FullCopy(object row)
		{
			return this.fields;
		}

		private object	CopyStackTrace(object row)
		{
			return this.logParser.StackTrace;
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
			}

			return null;
		}

		/// <summary>
		/// Prepares the row by parsing its log.
		/// </summary>
		private void	ParseLog()
		{
			InternalNGDebug.AssertFile(this.isParsed == false, "Parsed Row is being parsed again.");

			this.isParsed = true;

			int		end = this.log.condition.IndexOf(InternalNGDebug.DataEndChar);
			string	raw = this.log.condition.Substring(1, end - 1);
			int		n = raw.IndexOf(InternalNGDebug.DataSeparator);

			if (n != -1)
			{
				this.firstLine = raw.Substring(0, n).Replace(InternalNGDebug.DataSeparatorReplace, InternalNGDebug.DataSeparator);
				this.fields = raw.Substring(n + 1).Replace(InternalNGDebug.DataSeparatorReplace, InternalNGDebug.DataSeparator);

				Utility.content.text = this.fields;
				this.fieldsHeight = GeneralStyles.RichTextArea.CalcHeight(Utility.content, 0F);
			}
			else
			{
				this.firstLine = raw.Replace(InternalNGDebug.DataSeparatorReplace, InternalNGDebug.DataSeparator);
				this.fields = string.Empty;
				this.fieldsHeight = 0F;
			}
		}
	}
}