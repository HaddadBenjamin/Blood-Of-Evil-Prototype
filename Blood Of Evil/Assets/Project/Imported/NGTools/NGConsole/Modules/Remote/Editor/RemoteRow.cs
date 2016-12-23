using NGTools;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[Serializable]
	public class RemoteRow : Row
	{
		public string	error;
		public string	result;

		public	RemoteRow()
		{
			this.error = null;
			this.result = null;
		}

		public override float	GetWidth()
		{
			return 0F;
		}

		public override float	GetHeight()
		{
			float	height = Preferences.Settings.log.height;

			// An error occured.
			if (this.error != null)
			{
				height += Preferences.Settings.log.contentStyle.CalcSize(new GUIContent(this.error)).y;
			}
			// Waiting for answer.
			else if (this.result == null)
			{
				height += Preferences.Settings.log.contentStyle.CalcSize(new GUIContent(this.log.file)).y;
			}
			// Answer received.
			else
			{
				height += Preferences.Settings.log.contentStyle.CalcSize(new GUIContent(this.result)).y;
			}

			return height;
		}

		public override void	DrawRow(RowsDrawer rowsDrawer, Rect r, int i, bool? collapse)
		{
			float	originWidth = Utility.drawingWindow.position.width - rowsDrawer.verticalScrollbarWidth + rowsDrawer.currentVars.scrollPosition.x;

			// Draw highlight.
			r.x = rowsDrawer.currentVars.scrollPosition.x;
			r.width = originWidth;
			r.height = Preferences.Settings.log.height;

			this.DrawBackground(rowsDrawer, r, i);

			this.HandleDefaultSelection(rowsDrawer, r, i);

			GUI.Label(r, this.log.condition, Preferences.Settings.log.style);
			r.y += r.height;

			// An error occured.
			if (this.error != null)
			{
				r.width = Preferences.Settings.log.style.CalcSize(new GUIContent(">")).x;
				GUI.Label(r, "<color=" + NGCLI.ErrorCommandColor + ">></color>", Preferences.Settings.log.style);
				r.x += r.width;
				r.width = originWidth - r.x;
				EditorGUI.SelectableLabel(r, this.error, Preferences.Settings.log.contentStyle);
			}
			// Waiting for answer.
			else if (this.result == null)
			{
				r.width = Preferences.Settings.log.style.CalcSize(new GUIContent(">")).x;
				GUI.Label(r, "<color=" + NGCLI.PendingCommandColor + ">></color>", Preferences.Settings.log.style);
				r.x += r.width;
				r.width = originWidth - r.x;
				// Use file to display the default value from RemoteCommand while waiting for the answer.
				EditorGUI.SelectableLabel(r, this.log.file, Preferences.Settings.log.contentStyle);
			}
			// Answer received.
			else
			{
				r.width = Preferences.Settings.log.style.CalcSize(new GUIContent(">")).x;
				GUI.Label(r, "<color=" + NGCLI.ReceivedCommandColor + ">></color>", Preferences.Settings.log.style);
				r.x += r.width;
				r.width = originWidth - r.x;

				r.height = Preferences.Settings.log.contentStyle.CalcSize(new GUIContent(this.result)).y;

				EditorGUI.SelectableLabel(r, this.result, Preferences.Settings.log.contentStyle);
			}
		}
	}
}