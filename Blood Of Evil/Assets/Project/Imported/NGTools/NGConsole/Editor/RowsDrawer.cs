using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	public sealed class RowsDrawer
	{
		[Serializable]
		public sealed class Vars
		{
			/// <summary>Contains positions in the array RowsDrawer.rows[].</summary>
			public List<int>	selectedLogs = new List<int>();
			public int			CountSelection { get { return this.selectedLogs.Count; } }

			public float	scrollX;
			public float	scrollY;
			// Update all Vars when lastTotalViewHeight change
			[NonSerialized]
			private Rect	lastRect = new Rect();
			public Rect		LastRect { get { this.lastRect.y = this.yScrollOffset; return this.lastRect; } }
			public float	yScrollOffset;
			public int		lastI;
			public float	originalScrollOffset;

			[NonSerialized]
			public bool	hasInvalidHeight = true;
			[NonSerialized]
			public bool	mustRefreshCachePosition;

			public bool		autoScroll;
			[NonSerialized]
			public bool		updateAutoScroll;

			[NonSerialized]
			public bool		smoothingScroll = false;
			[NonSerialized]
			public bool		abortSmooth;
			[NonSerialized]
			public float	targetScrollPosition;
			[NonSerialized]
			public float	originScrollPosition;
			[NonSerialized]
			public float	smoothScrollStartTime;

			#region Row Content Variables
			[NonSerialized]
			public Vector2	scrollPositionRowContent;
			/// <summary>Defines the height of the area displaying log's content.</summary>
			public float	rowContentHeight = 70F;
			public bool		draggingSplitterBar = false;
			public float	originPositionY;
			public float	originRowContentHeight;
			#endregion

			public int	GetSelection(int i)
			{
				return this.selectedLogs[i];
			}

			public void	AddSelection(int index)
			{
				this.selectedLogs.Add(index);
			}

			public void	AddRangeSelection(int[] range)
			{
				this.selectedLogs.AddRange(range);
			}

			public void	WrapSelection(int i)
			{
				int	min = int.MaxValue;
				int	max = int.MinValue;

				for (int j = 0; j < this.selectedLogs.Count; j++)
				{
					if (this.selectedLogs[j] < min)
						min = this.selectedLogs[j];
					if (this.selectedLogs[j] > max)
						max = this.selectedLogs[j];
				}

				if (min > i)
					min = i;
				if (max < i)
					max = i;

				this.selectedLogs.Clear();
				for (; min <= max; min++)
					this.selectedLogs.Add(min);
			}

			public void	RemoveSelection(int index)
			{
				this.selectedLogs.Remove(index);
			}

			public bool	IsSelected(int index)
			{
				return this.selectedLogs.Contains(index);
			}

			public void	ClearSelection()
			{
				this.selectedLogs.Clear();
			}

			public int[]	GetSelectionArray()
			{
				return this.selectedLogs.ToArray();
			}

			public IEnumerable<int>	Each()
			{
				for (int i = 0; i < this.selectedLogs.Count; i++)
					yield return this.selectedLogs[i];
			}
		}

		public const float	SmoothScrollDuration = .2F;
		public const string	ShortCopyCommand = "ShortCopy";
		public const string	FullCopyCommand = "FullCopy";
		public const string	CopyStackTraceCommand = "CopyStackTrace";
		public const string	HandleKeyboardCommand = "HandleKeyboard";

		public static Func<Rect, int, Row, Rect>	GlobalBeforeFoldout;
		public static Func<Rect, int, Row, Rect>	GlobalBeforeLog;
		public static Func<Rect, int, Row, Rect>	GlobalAfterLog;
		public static Action<GenericMenu, Row>		GlobalLogContextMenu;

		[NonSerialized]
		public Func<Rect, int, Row, Rect>	BeforeFoldout;
		[NonSerialized]
		public Func<Rect, int, Row, Rect>	BeforeLog;
		[NonSerialized]
		public Func<Rect, int, Row, Rect>	AfterLog;
		[NonSerialized]
		public Action<Rect>					AfterAllRows;
		[NonSerialized]
		public Action<GenericMenu, Row>		LogContextMenu;
		[NonSerialized]
		public Action<Rect, Row>			RowHovered;
		[NonSerialized]
		public Action<Rect, Row>			RowClicked;
		[NonSerialized]
		public Action<Row>					RowDeleted;

		private bool	canDelete;
		/// <summary>
		/// Allows or not to delete logs.
		/// </summary>
		public bool		CanDelete { get { return this.canDelete; } set { this.canDelete = value; } }

		[NonSerialized]
		public IRows	rows;

		/// <summary>Contains indexes referencing rows fetched by RowsDrawer.RowGetter().</summary>
		[SerializeField]
		private List<int>	rowIndexes;
		public int	Count { get { return this.rowIndexes.Count; } }
		public int	this[int i]
		{
			get
			{
				return this.rowIndexes[i];
			}
			set
			{
				this.rowIndexes[i] = value;
			}
		}

		[NonSerialized]
		public Vars	currentVars;

		/// <summary>
		/// Defines the area where the rows drawer can draw its rows. It excludes the area where log's content is displayed.
		/// </summary>
		[NonSerialized]
		internal Rect	bodyRect;

		public float	lastMaxViewHeight;
		[NonSerialized]
		public Rect		viewRect;
		[NonSerialized]
		public float	verticalScrollbarWidth;

		[NonSerialized]
		private Vector2	scrollPosition;

		[NonSerialized]
		private NGConsoleWindow		editor;

		public PerWindowVars<Vars>	perWindowVars;

		public	RowsDrawer()
		{
			this.rowIndexes = new List<int>(Constants.PreAllocatedArray);
			this.canDelete = true;
			this.perWindowVars = new PerWindowVars<Vars>();
		}

		/// <summary>
		/// Initializes RowsDrawer. Must be called before doing anything.
		/// </summary>
		/// <param name="editor">An instance of NGConsole to work on.</param>
		/// <param name="rows">A method to fetch a Row from an index. Because Rows are almost always shared between modules.</param>
		public void	Init(NGConsoleWindow editor, IRows rows)
		{
			this.editor = editor;

			// Populate with default commands if missing.
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.OpenLogCommand, KeyCode.RightArrow);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.CloseLogCommand, KeyCode.LeftArrow);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.FocusTopLogCommand, KeyCode.Home);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.FocusBottomLogCommand, KeyCode.End);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.MoveUpLogCommand, KeyCode.UpArrow);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.MoveDownLogcommand, KeyCode.DownArrow);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.LongMoveUpLogCommand, KeyCode.PageUp);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.LongMoveDownLogCommand, KeyCode.PageDown);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.DeleteLogCommand, KeyCode.Delete);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.GoToLineCommand, KeyCode.Return);

			this.rows = rows;

			this.editor.ConsoleCleared += this.ClearUpdateAutoScroll;
			this.editor.syncLogs.EndNewLog += this.UpdateAutoScroll;

			if (this.perWindowVars == null)
				this.perWindowVars = new PerWindowVars<Vars>();

			this.UpdateAutoScroll();
		}

		public void	Uninit()
		{
			this.editor.ConsoleCleared -= this.ClearUpdateAutoScroll;
			this.editor.syncLogs.EndNewLog -= this.UpdateAutoScroll;
		}

		public void	SetRowGetter(IRows rows)
		{
			this.rows = rows;
		}

		public void	Add(int i)
		{
			this.rowIndexes.Add(i);
			this.lastMaxViewHeight += this.rows.GetRow(i).GetHeight();
		}

		public void	AddRange(IEnumerable<int> range)
		{
			this.rowIndexes.AddRange(range);

			foreach (var i in range)
				this.lastMaxViewHeight += this.rows.GetRow(i).GetHeight();
		}

		public void	RemoveAt(int i)
		{
			this.lastMaxViewHeight -= this.rows.GetRow(this.rowIndexes[i]).GetHeight();
			this.perWindowVars.Get(Utility.drawingWindow).RemoveSelection(this.rowIndexes[i]);
			this.rowIndexes.RemoveAt(i);
		}

		public void	Clear()
		{
			this.rowIndexes.Clear();

			this.lastMaxViewHeight = 0;

			foreach (Vars vars in this.perWindowVars.Each())
				vars.ClearSelection();

			this.ClearUpdateAutoScroll();
		}

		public int[]	GetRowsArray()
		{
			return this.rowIndexes.ToArray();
		}

		public void	ClearUpdateAutoScroll()
		{
			foreach (Vars vars in this.perWindowVars.Each())
			{
				vars.updateAutoScroll = true;
				vars.hasInvalidHeight = true;
				vars.lastI = 0;
				vars.yScrollOffset = 0F;
			}
		}

		public void	UpdateAutoScroll()
		{
			foreach (Vars vars in this.perWindowVars.Each())
				vars.updateAutoScroll = true;
		}

		public Rect	DrawRows(Rect r, bool? collapse)
		{
			this.currentVars = this.perWindowVars.Get(Utility.drawingWindow);

			this.viewRect.width = 0;

			float	rowHeight = 0F;
			Row		row;
			int		i = 0;

			if (this.currentVars.hasInvalidHeight == false)
				rowHeight = this.lastMaxViewHeight;
			else
			{
				// Preprocess the size.
				for (; i < this.rowIndexes.Count; i++)
				{
					row = this.rows.GetRow(this.rowIndexes[i]);
					rowHeight += row.GetHeight();

					//if (Preferences.Settings.general.horizontalScrollbar == true)
					//{
					//	float	rowWidth = row.GetWidth();

					//	if (this.viewRect.width < rowWidth)
					//		this.viewRect.width = rowWidth;
					//}
				}

				this.lastMaxViewHeight = rowHeight;
				if (Event.current.type == EventType.Layout)
					this.currentVars.hasInvalidHeight = false;
			}

			//if (Preferences.Settings.general.horizontalScrollbar == false)
				this.viewRect.width = 0F;
			//else
			//	this.viewRect.width += Constants.HorizontalScrollbarWidth;
			this.viewRect.height = rowHeight;

			if (Preferences.Settings.log.alwaysDisplayLogContent == true || this.CanDrawRowContent() == true)
				r.height -= this.currentVars.rowContentHeight + Constants.RowContentSplitterHeight;

			this.verticalScrollbarWidth = (this.viewRect.height > r.height) ? Constants.VerticalScrollbarWidth : 0F;

			r.x = 0F;
			this.bodyRect = r;

			// Use direct scroll values instead of Vector2, because they are serialized and not Vector2.
			this.scrollPosition.x = this.currentVars.scrollX;
			this.scrollPosition.y = this.currentVars.scrollY;

			// Width in bodyRect does not represent the sum of all controls. It does not calculate width from events beforeFoldout, beforeLog, and afterLog.
			this.scrollPosition = GUI.BeginScrollView(r, this.scrollPosition, this.viewRect);
			{
				this.currentVars.scrollX = this.scrollPosition.x;
				this.currentVars.scrollY = this.scrollPosition.y;
				if (this.currentVars.originalScrollOffset != this.currentVars.scrollY)
				{
					this.currentVars.abortSmooth = true;

					if (this.currentVars.lastI < this.rowIndexes.Count)
					{
						// Seek backward.
						if (this.currentVars.scrollY < this.currentVars.originalScrollOffset)
						{
							// Just force the process from the beginning if the new offset is lower than the half, due to potentially less calculus. Dichotomy things... You know.
							if (this.currentVars.scrollY <= this.currentVars.originalScrollOffset * .5)
								this.currentVars.mustRefreshCachePosition = true;
							else
							{
								float	y = this.currentVars.yScrollOffset;

								row = this.rows.GetRow(this.rowIndexes[this.currentVars.lastI]);
								rowHeight = row.GetHeight();
								y += rowHeight;

								while (this.currentVars.lastI >= 0 && y >= this.currentVars.scrollY)
								{
									rowHeight = this.rows.GetRow(this.rowIndexes[this.currentVars.lastI]).GetHeight();
									y -= rowHeight;
									--this.currentVars.lastI;
								}

								if (this.currentVars.lastI < 0)
									this.currentVars.mustRefreshCachePosition = true;
								else
									++this.currentVars.lastI;

								this.currentVars.yScrollOffset = y;
							}
						}
						// Else, the algorithm will automatically found the latest index.
					}
					else
						this.currentVars.mustRefreshCachePosition = true;

					this.currentVars.originalScrollOffset = this.currentVars.scrollY;
				}

				if (this.currentVars.updateAutoScroll == true)
				{
					this.currentVars.updateAutoScroll = false;

					if (this.currentVars.autoScroll == true || this.currentVars.smoothingScroll == true)
					{
						if (Preferences.Settings.general.smoothScrolling == false)
							this.currentVars.scrollY = float.MaxValue;
						else
							this.StartSmoothScroll(this.viewRect.height - r.height);
					}
				}

				this.currentVars.autoScroll = (this.viewRect.height - this.currentVars.scrollY <= r.height);

				bool	firstFound = false;

				if (this.currentVars.mustRefreshCachePosition == false)
				{
					i = this.currentVars.lastI;
					r = this.currentVars.LastRect;
				}
				else
				{
					i = 0;
					r.y = 0F;
					this.currentVars.mustRefreshCachePosition = false;
				}

				// Display logs.
				for (; i < this.rowIndexes.Count; i++)
				{
					row = this.rows.GetRow(this.rowIndexes[i]);
					rowHeight = row.GetHeight();

					if (r.y + rowHeight <= this.currentVars.scrollY)
					{
						r.y += rowHeight;
						continue;
					}

					if (firstFound == false)
					{
						firstFound = true;
						this.currentVars.lastI = i;
						this.currentVars.yScrollOffset = r.y;
					}

					r.width = this.bodyRect.width;
					r.height = rowHeight;
					row.DrawRow(this, r, i, collapse);

					if (this.currentVars.hasInvalidHeight == true)
					{
						this.lastMaxViewHeight -= rowHeight - row.GetHeight();
						this.currentVars.hasInvalidHeight = false;
					}

					r.y += r.height;

					// Check if out of view rect.
					if (r.y - this.currentVars.scrollY > this.bodyRect.height)
						break;
				}

				this.HandleKeyboard();
			}
			GUI.EndScrollView(true);

			// Restore height if selected logs have changed during the drawing.
			if (Preferences.Settings.log.alwaysDisplayLogContent == false && this.CanDrawRowContent() == false)
				this.bodyRect.height += this.currentVars.rowContentHeight + Constants.RowContentSplitterHeight;

			r = this.bodyRect;
			r.y += r.height;

			if (Preferences.Settings.log.alwaysDisplayLogContent == true || this.CanDrawRowContent() == true)
				r = this.DrawRowContent(r);

			if (this.AfterAllRows != null)
				this.AfterAllRows(this.bodyRect);

			return r;
		}

		private bool	CanDrawRowContent()
		{
			return this.currentVars.selectedLogs.Count == 1 &&
				   this.rows.GetRow(this.rowIndexes[this.currentVars.selectedLogs[0]]) is ILogContentGetter;
		}

		private Rect	DrawRowContent(Rect r)
		{
			r.x = 0F;

			Row	row = null;

			if (this.currentVars.selectedLogs.Count == 1 && this.CanDrawRowContent() == true)
			{
				row = this.rows.GetRow(this.rowIndexes[this.currentVars.selectedLogs[0]]);
				Utility.content.text = row.log.condition;
			}
			else
				Utility.content.text = string.Empty;

			float	minHeight = Preferences.Settings.log.contentStyle.CalcHeight(Utility.content, r.width);

			// Handle splitter bar.
			r.height = Constants.RowContentSplitterHeight;
			GUI.Box(r, "");
			EditorGUIUtility.AddCursorRect(r, MouseCursor.ResizeVertical);

			if (this.currentVars.draggingSplitterBar == true &&
				Event.current.type == EventType.MouseDrag)
			{
				this.currentVars.rowContentHeight = Mathf.Clamp(this.currentVars.originRowContentHeight + this.currentVars.originPositionY - Event.current.mousePosition.y,
													Constants.MinRowContentHeight, Utility.drawingWindow.position.height - Constants.MaxRowContentHeightLeft);
				Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseDown &&
					 r.Contains(Event.current.mousePosition) == true)
			{
				this.currentVars.originPositionY = Event.current.mousePosition.y;
				this.currentVars.originRowContentHeight = this.currentVars.rowContentHeight;
				this.currentVars.draggingSplitterBar = true;
				Event.current.Use();
			}
			else if (this.currentVars.draggingSplitterBar == true &&
					 Event.current.type == EventType.MouseUp)
			{
				// Auto adjust height on left click or double click.
				if (r.Contains(Event.current.mousePosition) == true &&
					(Event.current.button == 1 ||
					 (RowUtility.LastClickTime + Constants.DoubleClickTime > EditorApplication.timeSinceStartup &&
					  Mathf.Abs(this.currentVars.originPositionY - Event.current.mousePosition.y) < 5F)))
				{
					// 7F of margin, dont know why it is required. CalcHeight seems to give bad result.
					this.currentVars.rowContentHeight = Mathf.Clamp(minHeight + 7F,
														Constants.MinRowContentHeight, Utility.drawingWindow.position.height - Constants.MaxRowContentHeightLeft);
				}
				RowUtility.LastClickTime = EditorApplication.timeSinceStartup;
				this.currentVars.draggingSplitterBar = false;
				Event.current.Use();
			}

			r.y += r.height;

			// Write log's content.
			r.height = this.currentVars.rowContentHeight;

			if (r.height > Utility.drawingWindow.position.height - Constants.MaxRowContentHeightLeft)
				this.currentVars.rowContentHeight = Utility.drawingWindow.position.height - Constants.MaxRowContentHeightLeft;

			// Smoothly stay at the minimum if not critical under the critical threshold.
			if (this.currentVars.rowContentHeight < Constants.MinRowContentHeight)
				this.currentVars.rowContentHeight = Utility.drawingWindow.position.height - Constants.MaxRowContentHeightLeft;

			// Prevent reaching non-restorable value.
			if (this.currentVars.rowContentHeight <= Constants.CriticalMinimumContentHeight)
				this.currentVars.rowContentHeight = Constants.CriticalMinimumContentHeight;

			if (row != null)
			{
				GUILayout.BeginArea(r);
				{
					this.currentVars.scrollPositionRowContent = GUILayout.BeginScrollView(this.currentVars.scrollPositionRowContent);
					{
						GUI.SetNextControlName(Constants.CopyControlName);
						EditorGUILayout.SelectableLabel(row.log.condition, Preferences.Settings.log.contentStyle,
							new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(true),
								GUILayout.ExpandHeight(true),
								GUILayout.MinHeight(minHeight)
							}
						);
					}
					GUILayout.EndScrollView();
				}
				GUILayout.EndArea();
			}

			return r;
		}

		private void	HandleKeyboard()
		{
			if (Event.current.type == EventType.KeyDown)
			{
				if (this.CanDelete == true &&
					Preferences.Settings.inputsManager.Check("Navigation", Constants.DeleteLogCommand) == true)
				{
					if (this.currentVars.selectedLogs.Count > 0)
					{
						this.currentVars.selectedLogs.Sort();

						int		firstSelected = this.currentVars.selectedLogs[0];

						for (int i = this.currentVars.selectedLogs.Count - 1; i >= 0; --i)
						{
							int	id = this.currentVars.selectedLogs[i];
							Row	row = this.rows.GetRow(this.rowIndexes[id]);
							this.rowIndexes.RemoveAt(id);

							this.lastMaxViewHeight -= row.GetHeight();

							foreach (Vars vars in this.perWindowVars.Each())
								vars.RemoveSelection(id);

							if (this.RowDeleted != null)
								this.RowDeleted(row);
						}

						//this.currentVars.ClearSelection();

						if (firstSelected < this.rowIndexes.Count)
							this.currentVars.selectedLogs.Add(firstSelected);
						else if (this.rowIndexes.Count > 0)
							this.currentVars.selectedLogs.Add(this.rowIndexes.Count - 1);

						Utility.drawingWindow.Repaint();
						Event.current.Use();
					}
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.FocusTopLogCommand) == true)
				{
					this.currentVars.ClearSelection();

					if (this.rowIndexes.Count > 0)
					{
						this.currentVars.AddSelection(0);
						GUI.FocusControl("L0");
						this.perWindowVars.Get(Utility.drawingWindow).scrollY = 0;
						Utility.drawingWindow.Repaint();
					}

					Event.current.Use();
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.FocusBottomLogCommand) == true)
				{
					this.currentVars.ClearSelection();

					if (this.rowIndexes.Count > 0)
					{
						this.currentVars.AddSelection(this.rowIndexes.Count - 1);
						GUI.FocusControl("L" + (this.rowIndexes.Count - 1));
						this.perWindowVars.Get(Utility.drawingWindow).scrollY = float.MaxValue;
						Utility.drawingWindow.Repaint();
					}

					Event.current.Use();
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.MoveDownLogcommand) == true)
				{
					if (this.currentVars.selectedLogs.Count > 0)
					{
						int	highest = int.MinValue;

						foreach (var i in this.currentVars.selectedLogs)
						{
							if (i > highest)
								highest = i;
						}

						++highest;
						if (highest < this.rowIndexes.Count)
						{
							this.currentVars.ClearSelection();
							this.currentVars.AddSelection(highest);
							this.FitFocusedLogInScreen(highest);
							Utility.drawingWindow.Repaint();
							GUI.FocusControl("L" + highest);
						}

						Event.current.Use();
					}
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.MoveUpLogCommand) == true)
				{
					if (this.currentVars.selectedLogs.Count > 0)
					{
						int	lowest = int.MaxValue;

						foreach (var i in this.currentVars.selectedLogs)
						{
							if (i < lowest)
								lowest = i;
						}

						--lowest;
						if (lowest >= 0)
						{
							this.currentVars.ClearSelection();
							this.currentVars.AddSelection(lowest);
							this.FitFocusedLogInScreen(lowest);
							Utility.drawingWindow.Repaint();
							GUI.FocusControl("L" + lowest);
						}

						Event.current.Use();
					}
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.LongMoveDownLogCommand) == true)
				{
					if (this.currentVars.selectedLogs.Count > 0)
					{
						int	highest = int.MinValue;

						foreach (var i in this.currentVars.selectedLogs)
						{
							if (i > highest)
								highest = i;
						}

						float	y = 0F;

						while (y < this.bodyRect.height && highest <= this.rowIndexes.Count - 1)
							y += this.rows.GetRow(highest++).GetHeight();

						if (highest >= this.rowIndexes.Count)
							highest = this.rowIndexes.Count - 1;

						this.currentVars.ClearSelection();
						this.currentVars.AddSelection(highest);
						this.FitFocusedLogInScreen(highest);
						Utility.drawingWindow.Repaint();
						GUI.FocusControl("L" + highest);

						Event.current.Use();
					}
				}
				else if (Preferences.Settings.inputsManager.Check("Navigation", Constants.LongMoveUpLogCommand) == true)
				{
					if (this.currentVars.selectedLogs.Count > 0)
					{
						int	lowest = int.MaxValue;

						foreach (var i in this.currentVars.selectedLogs)
						{
							if (i < lowest)
								lowest = i;
						}

						float	y = 0F;

						while (y < this.bodyRect.height && lowest >= 0)
							y += this.rows.GetRow(lowest--).GetHeight();

						if (lowest < 0)
							lowest = 0;

						this.currentVars.ClearSelection();
						this.currentVars.AddSelection(lowest);
						this.FitFocusedLogInScreen(lowest);
						Utility.drawingWindow.Repaint();
						GUI.FocusControl("L" + lowest);

						Event.current.Use();
					}
				}
			}
			else if (Event.current.type == EventType.ValidateCommand &&
					 GUI.GetNameOfFocusedControl() != Constants.CopyControlName &&
					 (Event.current.commandName == "Copy" ||
					  Event.current.commandName == "Cut"))
			{
				Event.current.Use();
			}
			// Copy head message on Ctrl + C, copy full message on double Ctr + C.
			else if (Event.current.type == EventType.ExecuteCommand &&
					 GUI.GetNameOfFocusedControl() != Constants.CopyControlName &&
					 Event.current.commandName == "Copy")
			{
				if (RowUtility.LastKeyTime + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
					this.MenuCopyLog(null);
				else
					this.MenuCopyLine(null);

				RowUtility.LastKeyTime = EditorApplication.timeSinceStartup;
				Event.current.Use();
			}
			// Copy full message on Ctr + X.
			else if (Event.current.type == EventType.ExecuteCommand &&
					 Event.current.commandName == "Cut")
			{
				this.MenuCopyLog(null);
				Event.current.Use();
			}

			for (int i = 0; i < this.currentVars.selectedLogs.Count; i++)
			{
				Row		row = this.rows.GetRow(this.rowIndexes[this.currentVars.selectedLogs[i]]);
				float	rowHeight = row.GetHeight();

				row.Command(RowsDrawer.HandleKeyboardCommand, this);

				if (this.currentVars.hasInvalidHeight == true)
				{
					this.lastMaxViewHeight -= rowHeight - row.GetHeight();
					this.currentVars.hasInvalidHeight = false;
				}
			}
		}

		public void	InvalidateViewHeight()
		{
			this.perWindowVars.Get(Utility.drawingWindow).hasInvalidHeight = true;
		}

		/// <summary>Scrolls to keep the focused log in the view rect.</summary>
		public void	FitFocusedLogInScreen(int target)
		{
			if (target <= this.currentVars.lastI)
			{
				float	y = this.currentVars.yScrollOffset;

				while (this.currentVars.lastI > 0 && target < this.currentVars.lastI)
				{
					--this.currentVars.lastI;
					y -= this.rows.GetRow(this.rowIndexes[this.currentVars.lastI]).GetHeight();
				}

				this.currentVars.yScrollOffset = y;
				this.currentVars.scrollY = y;
			}
			else
			{
				float	y = this.currentVars.yScrollOffset;

				for (int i = this.currentVars.lastI; i < this.rowIndexes.Count; i++)
				{
					if (i == target)
						break;

					y += this.rows.GetRow(this.rowIndexes[i]).GetHeight();
				}

				if (y + Preferences.Settings.log.height > this.bodyRect.height + this.currentVars.scrollY)
					this.currentVars.scrollY = y + Preferences.Settings.log.height - (this.bodyRect.height);
			}
		}

		public void	MenuCopyLine(object data)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			this.currentVars.selectedLogs.Sort();
			foreach (var i in this.currentVars.selectedLogs)
			{
				Row	r = this.rows.GetRow(this.rowIndexes[i]);
				buffer.AppendLine(r.Command(RowsDrawer.ShortCopyCommand, r).ToString());
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}

		public void	MenuCopyLog(object data)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			this.currentVars.selectedLogs.Sort();
			foreach (var i in this.currentVars.selectedLogs)
			{
				Row	r = this.rows.GetRow(this.rowIndexes[i]);
				buffer.AppendLine(r.Command(RowsDrawer.FullCopyCommand, r).ToString());
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}

		public void	MenuCopyStackTrace(object data)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			this.currentVars.selectedLogs.Sort();
			foreach (var i in this.currentVars.selectedLogs)
			{
				Row	r = this.rows.GetRow(this.rowIndexes[i]);
				buffer.AppendLine(r.Command(RowsDrawer.CopyStackTraceCommand, r).ToString());
			}

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			EditorGUIUtility.systemCopyBuffer = Utility.ReturnBuffer(buffer);
		}

		private void	StartSmoothScroll(float target)
		{
			if (this.viewRect.height < this.bodyRect.height)
				return;

			Vars	vars = this.perWindowVars.Get(Utility.drawingWindow);

			vars.targetScrollPosition = target;
			vars.originScrollPosition = vars.scrollY;
			vars.smoothScrollStartTime = Time.realtimeSinceStartup;

			if (vars.smoothingScroll == false)
			{
				vars.smoothingScroll = true;
				vars.abortSmooth = false;

				EditorApplication.CallbackFunction	smoothScroll = null;

				smoothScroll = delegate()
				{
					if (vars.abortSmooth == true || vars.smoothingScroll == false)
					{
						vars.smoothingScroll = false;
						EditorApplication.update -= smoothScroll;
						return;
					}

					float	rate = (Time.realtimeSinceStartup - vars.smoothScrollStartTime) / RowsDrawer.SmoothScrollDuration;

					if (rate >= 1F)
					{
						vars.scrollY = vars.targetScrollPosition;
						vars.originalScrollOffset = vars.scrollY;
						vars.smoothingScroll = false;
						vars.autoScroll = true;
						EditorApplication.update -= smoothScroll;
					}
					else
					{
						vars.scrollY = Mathf.Lerp(vars.originScrollPosition, vars.targetScrollPosition, rate);
						vars.originalScrollOffset = vars.scrollY;
					}

					Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
				};

				EditorApplication.update += smoothScroll;
			}
		}
	}
}