using NGTools;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	[Exportable(ExportableAttribute.ArrayOptions.Overwrite | ExportableAttribute.ArrayOptions.Immutable)]
	internal sealed class CompilerStream : StreamLog, IRows
	{
		private const float	SpeedWarning = 2.5F;

		[Exportable]
		public bool	collapse;
		[Exportable]
		public bool	sortByError;

		[NonSerialized]
		private List<CompileRow>	compileRows;
		[NonSerialized]
		private bool				aware;
		[NonSerialized]
		private bool				isCompiling;
		[NonSerialized]
		private BgColorContentRestorer	restorer;

		public	CompilerStream()
		{
			this.name = "Compiler";
			this.rowsDrawer.CanDelete = false;
		}

		/// <summary>
		/// A special Init. Requires container to be a MainModule and an IStreams.
		/// </summary>
		/// <param name="console"></param>
		/// <param name="container">An instance of both MainModule and IStreams.</param>
		public override void	Init(NGConsoleWindow console, IStreams container)
		{
			base.Init(console, container);

			this.console.syncLogs.EndNewLog += this.UpdateAwareness;

			this.compileRows = new List<CompileRow>();
			this.rowsDrawer.SetRowGetter(this);
			this.restorer = new BgColorContentRestorer();

			this.console.UpdateTick += this.DetectCompile;

			this.OptionAltered += this.UpdateUnityConsoleOptions;
		}

		public override void	Uninit()
		{
			base.Uninit();

			this.console.UpdateTick -= this.DetectCompile;

			this.OptionAltered -= this.UpdateUnityConsoleOptions;
		}

		public override void	OnTabGUI(int i)
		{
			if (this.totalCount <= 0)
			{
				if (i == this.container.WorkingStream)
					this.container.FocusStream(1);

				this.aware = false;
				return;
			}

			EditorGUI.BeginChangeCheck();

			if (this.aware == true)
				GUILayout.Toggle(i == this.container.WorkingStream, this.name + " (" + this.totalCount + ")", Preferences.Settings.general.menuButtonStyle);
			else
			{
				using (this.restorer.Set(1F,
										 Mathf.PingPong((float)EditorApplication.timeSinceStartup * CompilerStream.SpeedWarning, 1F),
										 Mathf.PingPong((float)EditorApplication.timeSinceStartup * CompilerStream.SpeedWarning, 1F),
										 1F))
				{
					GUILayout.Toggle(i == this.container.WorkingStream, this.name + " (" + this.totalCount + ")", Preferences.Settings.general.menuButtonStyle);
				}
				this.console.Repaint();
			}

			if (EditorGUI.EndChangeCheck() == true)
			{
				if (Event.current.button == 0)
					this.container.FocusStream(i);
			}
		}

		public override Rect	OnGUI(Rect r)
		{
			this.aware = true;

			float	yOrigin = r.y;
			//float	maxWidth = r.width;
			float	maxHeight = r.height;

			// Draw options.
			r = this.DrawOptions(r);

			// Draw filters on the right.
			//r.width = maxWidth - r.x;
			//r = this.DrawFilters(r);

			// Draw rows in a new line.
			r.x = 0F;
			r.y += r.height;
			r.width = this.console.position.width;
			r.height = maxHeight - (r.y - yOrigin);
			return this.rowsDrawer.DrawRows(r, this.collapse);
		}

		protected override Rect	DrawOptions(Rect r)
		{
			float	maxWidth = r.width;

			EditorGUI.BeginChangeCheck();

			r.width = 80F;
			r.height = EditorGUIUtility.singleLineHeight;

			this.sortByError = GUI.Toggle(r, this.sortByError, "Sort by Error", Preferences.Settings.general.menuButtonStyle);
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.Clear();
				this.RefreshFilteredRows();
				Utility.RepaintConsoleWindow();
			}
			r.x += r.width;

			EditorGUI.BeginChangeCheck();
			r.width = 50F;
			this.warningContent.text = this.warningCount.ToString();
			this.displayWarning = GUI.Toggle(r, this.displayWarning, this.warningContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.errorContent.text = this.errorCount.ToString();
			this.displayError = GUI.Toggle(r, this.displayError, this.errorContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			// Update Unity Console only if required.
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.RefreshFilteredRows();
				Utility.RepaintConsoleWindow();
			}

			r.width = maxWidth - r.width * 4F;

			return r;
		}

		public override void	Clear()
		{
			base.Clear();

			this.compileRows.Clear();
		}

		/// <summary>
		/// Manually reset all counters and arrays.
		/// </summary>
		public override void	RefreshFilteredRows()
		{
			this.totalCount = 0;
			this.warningCount = 0;
			this.errorCount = 0;

			for (int i = 0; i < this.compileRows.Count; i++)
				this.compileRows[i].Clear();

			// Save current selected logs.
			var	selectedLogs = this.rowsDrawer.currentVars.GetSelectionArray();

			// Reimport all logs.
			for (int i = 0; i < this.console.rows.Count; i++)
				this.AddLog(i, this.console.rows[i]);

			int	logIndex = 0;
			// Try to restore selected logs.
			for (int i = 0; i < this.rowsDrawer.Count && logIndex < selectedLogs.Length; i++)
			{
				for (int j = logIndex; j < selectedLogs.Length; j++)
				{
					if (selectedLogs[j] == this.rowsDrawer[i])
					{
						this.rowsDrawer.currentVars.AddSelection(selectedLogs[j]);
						++logIndex;
						break;
					}
				}
			}
		}

		public override void	ConsumeLog(int i, Row row)
		{
			if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptCompileWarning)) != 0)
				row.isConsumed = true;
		}

		public override void	AddLog(int i, Row row)
		{
			// After compiling, need to clear the array before receiving brand new logs, because CompilerLog can not know when there is new logs.
			if (this.isCompiling == true)
			{
				this.Clear();
				this.isCompiling = false;
			}

			if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptCompileWarning)) != 0)
			{
				this.CountLog(row);
				if (this.CanDisplay(row) == true)
					this.AppendLogToRow(row);
			}
		}

		/// <summary>
		/// <para>Adds a new CompileRow or appends the given <paramref name="row"/> to an existing CompileRow.</para>
		/// <para>From here, we should assume that every logs are compiler outputs, therefore we should not care much about the format and issues when parsing it.</para>
		/// </summary>
		/// <param name="row"></param>
		private void	AppendLogToRow(Row row)
		{
			try
			{
				ILogContentGetter	logContent = row as ILogContentGetter;

				InternalNGDebug.AssertFile(logContent != null, "CompilerLog has received a non-usable Row." + Environment.NewLine + row.log);

				for (int i = 0; i < this.compileRows.Count; i++)
				{
					if (this.compileRows[i].CanAddRow(row) == true)
					{
						this.compileRows[i].AppendRow(row);
						return;
					}
				}

				if (this.sortByError == false)
				{
					CompileRow	crow = new CompileRow(logContent);
					crow.Init(this.console, row.log);
					crow.AppendRow(row);
					this.compileRows.Add(crow);
				}
				else
				{
					ErrorCompileRow	crow = new ErrorCompileRow(logContent);
					crow.Init(this.console, row.log);
					crow.AppendRow(row);
					this.compileRows.Add(crow);
				}

				this.rowsDrawer.Add(this.rowsDrawer.Count);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogFileException(ex);
			}
		}

		private void	DetectCompile()
		{
			if (EditorApplication.isCompiling == true)
				this.isCompiling = true;
			else
			{
				this.isCompiling = false;
			}
		}

		Row	IRows.GetRow(int i)
		{
			return this.compileRows[i];
		}

		int	IRows.CountRows()
		{
			return this.compileRows.Count;
		}

		private void	UpdateUnityConsoleOptions()
		{
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.Collapse, this.collapse);
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelLog, this.displayLog);
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelWarning, this.displayWarning);
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelError, this.displayError);
		}

		private void	UpdateAwareness()
		{
			if (Preferences.Settings.mainModule.alertOnWarning == false)
			{
				for (int j = 0; j < this.compileRows.Count; j++)
				{
					if (this.compileRows[j].hasError == true)
						return;
				}

				this.aware = true;
			}
			else
				this.aware = false;
		}
	}
}