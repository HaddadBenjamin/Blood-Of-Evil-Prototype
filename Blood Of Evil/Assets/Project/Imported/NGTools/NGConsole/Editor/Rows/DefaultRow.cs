using System;
using UnityEditor;
using UnityEditorInternal;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	[RowLogHandler(0)]
	internal class DefaultRow : Row, ILogContentGetter
	{
		/// <summary>Defines whether the Row is open or not by RowsDrawer.</summary>
		public bool		isOpened;

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

		protected LogConditionParser	logParser;

		[NonSerialized]
		private Texture2D	icon;

		private static bool	CanDealWithIt(UnityLogEntry log)
		{
			return true;
		}

		public override void	Init(NGConsoleWindow editor, LogEntry log)
		{
			base.Init(editor, log);

			this.logParser = new LogConditionParser(this.log);

			this.commands.Add(RowsDrawer.ShortCopyCommand, this.ShortCopy);
			this.commands.Add(RowsDrawer.FullCopyCommand, this.FullCopy);
			this.commands.Add(RowsDrawer.CopyStackTraceCommand, this.CopyStackTrace);
			this.commands.Add(RowsDrawer.HandleKeyboardCommand, this.HandleKeyboard);
		}

		public override void	Uninit()
		{
			this.logParser.Uninit();

			this.icon = null;
		}

		public override float	GetWidth()
		{
			return 0F;
		}

		public override float	GetHeight()
		{
			if (this.isOpened == true)
				return Preferences.Settings.log.height + Preferences.Settings.stackTrace.height * this.Frames.Length;
			return Preferences.Settings.log.height;
		}

		public override void	DrawRow(RowsDrawer rowsDrawer, Rect r, int i, bool? collapse)
		{
			float	originWidth = r.width - rowsDrawer.verticalScrollbarWidth + rowsDrawer.currentVars.scrollX;

			// Draw highlight.
			r.x = rowsDrawer.currentVars.scrollX;
			r.width = originWidth;
			r.height = Preferences.Settings.log.height;

			this.DrawBackground(rowsDrawer, r, i);

			// Handle row events.
			if (r.Contains(Event.current.mousePosition) == true)
			{
				if (Event.current.type == EventType.MouseMove ||
					Event.current.type == EventType.MouseDrag)
				{
					if (Event.current.type == EventType.MouseDrag &&
						Utility.position2D != Vector2.zero &&
						DragAndDrop.GetGenericData(Utility.DragObjectDataName) != null &&
						(Utility.position2D - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance)
					{
						Utility.position2D = Vector2.zero;
						DragAndDrop.StartDrag("Drag Object");
					}

					if (rowsDrawer.RowHovered != null)
					{
						r.x -= rowsDrawer.currentVars.scrollX;
						r.y -= rowsDrawer.currentVars.scrollY;
						rowsDrawer.RowHovered(r, this);
						r.x += rowsDrawer.currentVars.scrollX;
						r.y += rowsDrawer.currentVars.scrollY;
					}
				}
				else if (Event.current.type == EventType.MouseDown)
				{
					if (rowsDrawer.RowClicked != null)
					{
						r.x -= rowsDrawer.currentVars.scrollX;
						r.y -= rowsDrawer.currentVars.scrollY;
						rowsDrawer.RowClicked(r, this);
						r.x += rowsDrawer.currentVars.scrollX;
						r.y += rowsDrawer.currentVars.scrollY;
					}
				}
			}
			r.x = 0F;

			if (RowsDrawer.GlobalBeforeFoldout != null)
			{
				r.width = originWidth - r.x;
				r = RowsDrawer.GlobalBeforeFoldout(r, i, this);
			}
			if (rowsDrawer.BeforeFoldout != null)
			{
				r.width = originWidth - r.x;
				r = rowsDrawer.BeforeFoldout(r, i, this);
			}

			if (Event.current.type == EventType.MouseDown &&
				Event.current.button == 0 &&
				r.Contains(Event.current.mousePosition) == true)
			{
				Utility.position2D = Event.current.mousePosition;

				if (this.log.instanceID != 0)
				{
					DragAndDrop.objectReferences = new Object[] { EditorUtility.InstanceIDToObject(this.log.instanceID) };
					DragAndDrop.SetGenericData(Utility.DragObjectDataName, this.log.instanceID);
				}
			}

			// Draw foldout.
			r.width = r.height;

			GUI.enabled = this.HasStackTrace;

			bool	lastValue = this.isOpened;
			if ((this.log.mode & Mode.ScriptingException) != 0)
				this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.ExceptionFoldoutStyle);
			else if ((this.log.mode & (Mode.ScriptCompileError | Mode.ScriptingError | Mode.Fatal | Mode.Error | Mode.Assert | Mode.AssetImportError | Mode.ScriptingAssertion)) != 0)
				this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.ErrorFoldoutStyle);
			else if ((this.log.mode & (Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning)) != 0)
				this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.WarningFoldoutStyle);
			else
				this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.NormalFoldoutStyle);
			if (lastValue != this.isOpened)
				rowsDrawer.InvalidateViewHeight();

			if (GUI.enabled == false)
				this.isOpened = false;
			GUI.enabled = true;

			r.x = r.width;

			if (this.log.instanceID != 0)
			{
				if (this.icon == null)
				{
					this.icon = Utility.GetIcon(this.log.instanceID);
					if (this.icon == null)
						this.icon = InternalEditorUtility.GetIconForFile(this.log.file);
				}

				if (icon != null)
				{
					r.width = Preferences.Settings.log.height;
					GUI.DrawTexture(r, icon);
					r.x += r.width;
				}
			}

			if (RowsDrawer.GlobalBeforeLog != null)
			{
				r.width = originWidth - r.x;
				r = RowsDrawer.GlobalBeforeLog(r, i, this);
			}
			if (rowsDrawer.BeforeLog != null)
			{
				r.width = originWidth - r.x;
				r = rowsDrawer.BeforeLog(r, i, this);
			}

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

					if (string.IsNullOrEmpty(this.StackTrace) == false)
						menu.AddItem(new GUIContent(LC.G("CopyStackTrace")), false, rowsDrawer.MenuCopyStackTrace, this);

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
						RowUtility.GoToLine(this, this.log, true);
					// Handle multi-selection.
					else if (Event.current.control == true &&
							 rowsDrawer.currentVars.IsSelected(i) == true)
					{
						rowsDrawer.currentVars.RemoveSelection(i);
					}
					else
					{
						if (Event.current.shift == true)
							rowsDrawer.currentVars.WrapSelection(i);
						else if (Event.current.control == false)
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

						if (Event.current.shift == false)
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

			this.DrawLog(r, i);

			r = this.DrawCollapseLabel(r, collapse);

			if (RowsDrawer.GlobalAfterLog != null)
				r = RowsDrawer.GlobalAfterLog(r, i, this);
			if (rowsDrawer.AfterLog != null)
				r = rowsDrawer.AfterLog(r, i, this);

			r.y += r.height;

			if (this.isOpened == true)
			{
				r.x = 0F;
				r.width = originWidth;
				RowUtility.DrawStackTrace(this, rowsDrawer, r, i, this);
			}
		}

		public virtual void	DrawLog(Rect r, int i)
		{
			GUI.SetNextControlName("L" + i);
			GUI.Button(r, this.HeadMessage, Preferences.Settings.log.style);
		}

		private Rect	DrawCollapseLabel(Rect r, bool? collapse)
		{
			if (collapse.HasValue == true && collapse.Value == true)
			{
				// Draw collapse count.
				Utility.content.text = this.log.collapseCount.ToString();
				Vector2	vector = Preferences.Settings.log.collapseLabelStyle.CalcSize(Utility.content);

				r.x += r.width - vector.x;
				r.width = vector.x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.collapseLabelStyle);
			}
			return r;
		}

		private object	ShortCopy(object row)
		{
			return this.HeadMessage;
		}

		private object	FullCopy(object row)
		{
			return this.FullMessage;
		}

		private object	CopyStackTrace(object row)
		{
			return this.StackTrace;
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
						RowUtility.ClearPreview();
					}
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.OpenLogCommand) == true)
				{
					if (this.isOpened == false)
					{
						this.isOpened = true;
						rowsDrawer.InvalidateViewHeight();
						Utility.drawingWindow.Repaint();
						RowUtility.ClearPreview();
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