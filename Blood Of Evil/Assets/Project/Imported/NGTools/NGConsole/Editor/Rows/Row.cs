using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	public abstract class Row
	{
		/// <summary>Defines if one or more components have consumed this Row.</summary>
		public bool	isConsumed;

		[Exportable]
		public LogEntry	log;

		[NonSerialized]
		protected NGConsoleWindow	editor;
		[NonSerialized]
		protected Dictionary<string, Func<object, object>>	commands;

		public virtual void	Init(NGConsoleWindow editor, LogEntry log)
		{
			this.editor = editor;
			this.log = log;

			this.commands = new Dictionary<string, Func<object, object>>();
		}

		public virtual void	Uninit()
		{
		}

		/// <summary>
		/// Returns the current width of the <paramref name="row"/>.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public abstract float	GetWidth();

		/// <summary>
		/// Returns the current height of the <paramref name="row"/>.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public abstract float	GetHeight();

		/// <summary>
		/// </summary>
		/// <param name="rowsDrawer">The drawer.</param>
		/// <param name="rect">The area to work on.</param>
		/// <param name="i">Index from the RowsDrawer.rows[].</param>
		/// <param name="collapse">Defines if row should displayed its collapse label. Do not used it.</param>
		public abstract void	DrawRow(RowsDrawer rowsDrawer, Rect rect, int i, bool? collapse);

		public virtual object	Command(string commandName, object data)
		{
			Func<object, object>	callback;

			if (this.commands.TryGetValue(commandName, out callback) == true)
				return callback(data) ?? string.Empty;
			return string.Empty;
		}

		/// <summary>
		/// Draws background when focus, even or odd.
		/// </summary>
		/// <param name="rowsDrawer">The drawer.</param>
		/// <param name="r">The area to work on.</param>
		/// <param name="i">Index from the RowsDrawer.rows[].</param>
		protected void	DrawBackground(RowsDrawer rowsDrawer, Rect r, int i)
		{
			if (Event.current.type != EventType.Repaint)
				return;

			if (rowsDrawer.currentVars.IsSelected(i) == true)
				EditorGUI.DrawRect(r, Preferences.Settings.log.selectedBackground);
			else if ((i & 1) == 0)
				EditorGUI.DrawRect(r, Preferences.Settings.log.evenBackground);
			else
				EditorGUI.DrawRect(r, Preferences.Settings.log.oddBackground);
		}

		/// <summary>
		/// Handles events to toggle Row from the selection.
		/// </summary>
		/// <param name="rowsDrawer">The drawer.</param>
		/// <param name="r">The area to work on.</param>
		/// <param name="i">Index from the RowsDrawer.rows[].</param>
		protected void	HandleDefaultSelection(RowsDrawer rowsDrawer, Rect r, int i)
		{
			if (r.Contains(Event.current.mousePosition) == true)
			{
				// Focus on left click.
				if (Event.current.type == EventType.MouseDown &&
					Event.current.button == 0)
				{
					// Handle multi-selection.
					if (Event.current.control == true &&
						rowsDrawer.currentVars.IsSelected(i) == true)
					{
						rowsDrawer.currentVars.RemoveSelection(i);
					}
					else
					{
						if (Event.current.control == false)
							rowsDrawer.currentVars.ClearSelection();

						rowsDrawer.currentVars.AddSelection(i);

						if (Event.current.control == false)
							rowsDrawer.FitFocusedLogInScreen(i);
					}

					Utility.drawingWindow.Repaint();

					Event.current.Use();
				}
			}
		}

		protected Rect	DrawPreLogData(RowsDrawer rowsDrawer, Rect r)
		{
			// Draw time.
			if (Preferences.Settings.log.displayTime == true)
			{
				Utility.content.text = this.log.time;
				r.width = Preferences.Settings.log.timeStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.timeStyle);
				r.x += r.width;
			}

			// Draw frame count.
			if (Preferences.Settings.log.displayFrameCount == true)
			{
				Utility.content.text = this.log.frameCount.ToString();
				r.width = Preferences.Settings.log.timeStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.timeStyle);
				r.x += r.width;
			}

			// Draw rendered frame count.
			if (Preferences.Settings.log.displayRenderedFrameCount == true)
			{
				Utility.content.text = this.log.renderedFrameCount.ToString();
				r.width = Preferences.Settings.log.timeStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.timeStyle);
				r.x += r.width;
			}

			return r;
		}
	}
}