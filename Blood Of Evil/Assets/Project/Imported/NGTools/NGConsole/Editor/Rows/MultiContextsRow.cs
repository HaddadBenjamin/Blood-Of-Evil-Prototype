using NGTools;
using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	[RowLogHandler(10)]
	internal sealed class MultiContextsRow : Row, ILogContentGetter
	{
		public const float	Spacing = 2F;

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

		/// <summary>Defines whether the Row is open or not by RowsDrawer.</summary>
		public bool	isOpened;

		private LogConditionParser	logParser;

		private int[]	instanceIDs;

		private static bool	CanDealWithIt(UnityLogEntry log)
		{
			return log.condition[0] == InternalNGDebug.MultiContextsStartChar;
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
		}

		/// <summary>
		/// Prepares the row by parsing its log.
		/// </summary>
		public void	ParseCondition()
		{
			InternalNGDebug.AssertFile(this.instanceIDs == null, "Parsed Row is being parsed again.");

			int			end = this.log.condition.IndexOf(InternalNGDebug.MultiContextsEndChar);
			string		raw = this.log.condition.Substring(1, end - 1);

			if (string.IsNullOrEmpty(raw) == false)
			{
				string[]	contexts = raw.Split(InternalNGDebug.MultiContextsSeparator);

				this.instanceIDs = new int[contexts.Length];

				for (int i = 0; i < contexts.Length; i++)
					this.instanceIDs[i] = int.Parse(contexts[i]);
			}
			else
				this.instanceIDs = new int[0];
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
			if (this.instanceIDs == null)
				this.ParseCondition();

			float	originWidth = Utility.drawingWindow.position.width - rowsDrawer.verticalScrollbarWidth + rowsDrawer.currentVars.scrollX;

			// Draw highlight.
			r.x = rowsDrawer.currentVars.scrollX;
			r.width = originWidth;
			r.height = Preferences.Settings.log.height;

			this.DrawBackground(rowsDrawer, r, i);

			if (Event.current.type == EventType.MouseDrag &&
				Utility.position2D != Vector2.zero &&
				DragAndDrop.GetGenericData(Utility.DragObjectDataName) != null &&
				(Utility.position2D - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance)
			{
				DragAndDrop.StartDrag("Drag Object");
				Event.current.Use();
			}

			r.width = r.height;
			EditorGUI.DrawRect(r, Color.cyan);
			bool	lastValue = this.isOpened;
			this.isOpened = EditorGUI.Foldout(r, this.isOpened, "", Preferences.Settings.log.NormalFoldoutStyle);
			if (lastValue != this.isOpened)
				rowsDrawer.InvalidateViewHeight();
			r.x = r.width;
			r.width = originWidth - r.x;

			r = this.DrawPreLogData(rowsDrawer, r);
			r.width = originWidth - r.x;

			// Draw contexts.
			for (int j = 0; j < this.instanceIDs.Length; j++)
			{
				Texture2D	icon = null;

				r.width = Preferences.Settings.log.height;
				if (this.instanceIDs[j] != 0)
				{
					icon = Utility.GetIcon(this.instanceIDs[j]);

					if (Event.current.type == EventType.MouseDown &&
						r.Contains(Event.current.mousePosition) == true)
					{
						if (Event.current.button == 0)
						{
							Utility.position2D = Event.current.mousePosition;

							DragAndDrop.PrepareStartDrag();
							DragAndDrop.objectReferences = new Object[] { EditorUtility.InstanceIDToObject(this.instanceIDs[j]) };
							DragAndDrop.SetGenericData(Utility.DragObjectDataName, this.instanceIDs[j]);

							if (RowUtility.LastClickTime + Constants.DoubleClickTime > EditorApplication.timeSinceStartup ||
								(Event.current.modifiers & Preferences.Settings.log.selectObjectOnModifier) != 0)
							{
								Selection.activeInstanceID = this.instanceIDs[j];
							}
							else
								EditorGUIUtility.PingObject(this.instanceIDs[j]);

							RowUtility.LastClickTime = EditorApplication.timeSinceStartup;
						}
						else if (Event.current.button == 1)
							Selection.activeInstanceID = this.instanceIDs[j];
					}
				}

				if (icon != null)
					GUI.DrawTexture(r, icon);
				else
					EditorGUI.DrawRect(r, Color.black);

				if (r.Contains(Event.current.mousePosition) == true)
				{
					r.x += r.width;
					r.width = originWidth - r.x;

					Object	context = null;

					if (this.instanceIDs[j] != 0)
						context = EditorUtility.InstanceIDToObject(this.instanceIDs[j]);

					if (context != null)
						EditorGUI.LabelField(r, context.ToString());
					else
						EditorGUI.LabelField(r, LC.G("MultiContextsRow_ObjectNotAvailable"));

					Utility.drawingWindow.Repaint();
				}

				r.x += r.width + MultiContextsRow.Spacing;
			}

			r.x = rowsDrawer.currentVars.scrollX;
			r.width = originWidth;
			this.HandleDefaultSelection(rowsDrawer, r, i);

			if (this.isOpened == true)
			{
				r.x = 0F;
				r.y += r.height;
				r.width = originWidth;
				r = RowUtility.DrawStackTrace(this, rowsDrawer, r, i, this);
			}
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