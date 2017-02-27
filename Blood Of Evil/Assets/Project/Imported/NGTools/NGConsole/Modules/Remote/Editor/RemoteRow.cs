using NGTools.NGGameConsole;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	internal sealed class RemoteRow : Row
	{
		private static readonly GUIContent	LogPrefix = new GUIContent(">");
		private static float				LogPrefixHeight = -1F;

		public string	error;
		public string	result;

		private float	subHeight;

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
				this.subHeight = Preferences.Settings.log.contentStyle.CalcSize(new GUIContent(this.error)).y;
			// Waiting for answer.
			else if (this.result == null)
				this.subHeight = Preferences.Settings.log.contentStyle.CalcSize(new GUIContent(this.log.file)).y;
			// Answer received.
			else
				this.subHeight = Preferences.Settings.log.contentStyle.CalcSize(new GUIContent(this.result)).y;

			height += this.subHeight;

			return height;
		}

		public override void	DrawRow(RowsDrawer rowsDrawer, Rect r, int i, bool? collapse)
		{
			if (RemoteRow.LogPrefixHeight < 0F)
				RemoteRow.LogPrefixHeight = Preferences.Settings.log.style.CalcSize(RemoteRow.LogPrefix).x;

			float	originWidth = Utility.drawingWindow.position.width - rowsDrawer.verticalScrollbarWidth + rowsDrawer.currentVars.scrollX;

			// Draw highlight.
			r.x = rowsDrawer.currentVars.scrollX;
			r.width = originWidth;
			r.height = Preferences.Settings.log.height;

			this.DrawBackground(rowsDrawer, r, i);

			this.HandleDefaultSelection(rowsDrawer, r, i);

			GUI.Label(r, this.log.condition, Preferences.Settings.log.style);
			r.y += r.height;

			r.width = RemoteRow.LogPrefixHeight;

			// An error occured.
			if (this.error != null)
			{
				GUI.Label(r, "<color=" + NGCLI.ErrorCommandColor + ">></color>", Preferences.Settings.log.style);

				r.x += r.width;
				r.width = originWidth - r.x;
				r.height = this.subHeight;
				EditorGUI.SelectableLabel(r, this.error, Preferences.Settings.log.contentStyle);
			}
			// Waiting for answer.
			else if (this.result == null)
			{
				GUI.Label(r, "<color=" + NGCLI.PendingCommandColor + ">></color>", Preferences.Settings.log.style);

				r.x += r.width;
				r.width = originWidth - r.x;
				r.height = this.subHeight;
				// Use file to display the default value from RemoteCommand while waiting for the answer.
				EditorGUI.SelectableLabel(r, this.log.file, Preferences.Settings.log.contentStyle);
			}
			// Answer received.
			else
			{
				GUI.Label(r, "<color=" + NGCLI.ReceivedCommandColor + ">></color>", Preferences.Settings.log.style);

				r.x += r.width;
				r.width = originWidth - r.x;
				r.height = this.subHeight;
				EditorGUI.SelectableLabel(r, this.result, Preferences.Settings.log.contentStyle);
			}
		}
	}
}