using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class SampleStream : StreamLog
	{
		private const float	ConditionSpacing = 3F;
		private const float	ResetButtonWidth = 70F;

		private List<int>	rawRows;
		private bool		wasPreviouslyPlaying;

		private bool	hasConsumedLog;
		private bool	hasStarted;
		private bool	hasEnded;

		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		private StartMode[]	startModes;
		[Exportable]
		private int			currentStartMode;
		[Exportable( ExportableAttribute.ArrayOptions.Immutable)]
		private EndMode[]	endModes;
		[Exportable]
		private int			currentEndMode;

		public	SampleStream()
		{
			this.name = "New sample";
			this.addConsumedLog = true;

			this.rawRows = new List<int>();

			this.startModes = new StartMode[NGConsoleWindow.startModeTypes.Length];
			for (int i = 0; i < this.startModes.Length; i++)
			{
				this.startModes[i] = (StartMode)Activator.CreateInstance(NGConsoleWindow.startModeTypes[i]);
				this.startModes[i].Init(this);
			}

			this.endModes = new EndMode[NGConsoleWindow.endModeTypes.Length];
			for (int i = 0; i < this.endModes.Length; i++)
			{
				this.endModes[i] = (EndMode)Activator.CreateInstance(NGConsoleWindow.endModeTypes[i]);
				this.endModes[i].Init(this);
			}
		}

		public override void	Init(NGConsoleWindow console, IStreams container)
		{
			base.Init(console, container);

			EditorApplication.playmodeStateChanged += this.StartListening;

			this.RefreshFilteredRows();
		}

		public override Rect	OnGUI(Rect r)
		{
			Rect	rectModes = r;

			r.width = rectModes.width - SampleStream.ResetButtonWidth - SampleStream.ConditionSpacing - SampleStream.ConditionSpacing;

			r.width *= .5F;
			r.height = EditorGUIUtility.singleLineHeight;
			using (BgColorContentRestorer.Get(this.hasStarted, Color.green))
				this.currentStartMode = EditorGUI.Popup(r, LC.G("SampleStream_StartMode"), this.currentStartMode, NGConsoleWindow.startModeNames);

			r.y += r.height;

			GUILayout.BeginArea(r);
			{
				this.startModes[this.currentStartMode].OnGUI();
			}
			GUILayout.EndArea();

			r.y -= r.height;

			r.x += r.width + SampleStream.ConditionSpacing;
			using (BgColorContentRestorer.Get(this.hasEnded, Color.green))
				this.currentEndMode = EditorGUI.Popup(r, LC.G("SampleStream_EndMode"), this.currentEndMode, NGConsoleWindow.endModeNames);
			r.y += r.height;

			GUILayout.BeginArea(r);
			{
				this.endModes[this.currentEndMode].OnGUI();
			}
			GUILayout.EndArea();

			r.x += r.width + SampleStream.ConditionSpacing;
			r.y -= r.height;
			r.width = SampleStream.ResetButtonWidth;
			r.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight;
			if (GUI.Button(r, LC.G("Reset")) == true)
				this.Clear();
			r.y += r.height;

			r.x = rectModes.x;
			r.height = rectModes.height - (r.y - rectModes.y);
			r.width = rectModes.width;

			return base.OnGUI(r);
		}

		/// <summary>
		/// <para>Consumes the log if start condition is filled and end condition is not encountered yet.</para>
		/// <para>Start and end logs might be included in the sample depending on filters.</para>
		/// </summary>
		/// <param name="row"></param>
		public override void	ConsumeLog(int i, Row row)
		{
			this.hasConsumedLog = false;

			if (this.hasEnded == true ||
				EditorApplication.isPlaying == false)
			{
				return;
			}

			if (this.hasStarted == false &&
				this.startModes[this.currentStartMode].CheckStart(row) == true)
			{
				this.hasStarted = true;
			}

			if (this.hasStarted == true)
			{
				if (this.CanDisplay(row) == true)
				{
					this.hasConsumedLog = true;
					row.isConsumed = true;
				}

				if (this.endModes[this.currentEndMode].CheckEnd(row) == true)
					this.hasEnded = true;
			}
		}

		public override void	AddLog(int i, Row row)
		{
			if (this.hasConsumedLog == true)
			{
				base.AddLog(i, row);
				this.rawRows.Add(i);
			}
		}

		public override void	RefreshFilteredRows()
		{
			if (this.console.rows.Count == 0)
				EditorApplication.delayCall += this.RestoredLogs;
			else
				this.RestoredLogs();
		}

		public override void	Clear()
		{
			base.Clear();

			this.hasStarted = false;
			this.hasEnded = false;

			this.rawRows.Clear();
		}

		private void	RestoredLogs()
		{
			base.Clear();

			for (int i = 0; i < this.rawRows.Count; i++)
			{
				// Clean everything if sample is overflowing. Might be due to a Unity Editor's fresh start.
				if (this.rawRows[i] < this.console.rows.Count)
				{
					this.Clear();
					break;
				}
				base.AddLog(this.rawRows[i], this.console.rows[this.rawRows[i]]);
			}
		}

		private void	StartListening()
		{
			//Debug.Log(EditorApplication.isPlaying + " " +
			//	EditorApplication.isCompiling + " " +
			//	EditorApplication.isPaused + " " +
			//	EditorApplication.isPlayingOrWillChangePlaymode + " " +
			//	EditorApplication.isRemoteConnected + " " +
			//	EditorApplication.isUpdating);

			if (EditorApplication.isPlayingOrWillChangePlaymode == true)
			{
				if (this.wasPreviouslyPlaying == false)
				{
					this.wasPreviouslyPlaying = true;

					this.Clear();
				}
			}
			else
			{
				this.wasPreviouslyPlaying = false;
				this.hasStarted = false;
				this.hasEnded = false;
			}
		}
	}
}