using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	[Exportable(ExportableAttribute.ArrayOptions.Overwrite | ExportableAttribute.ArrayOptions.Immutable)]
	public class MainStream : StreamLog
	{
		[Exportable]
		public bool	collapse;

		public	MainStream()
		{
			this.name = "Main";
			this.addConsumedLog = true;
		}

		public override void	Init(NGConsoleWindow console, IStreams container)
		{
			base.Init(console, container);

			this.OptionAltered += this.UpdateUnityConsoleOptions;
		}

		public override void	Uninit()
		{
			base.Uninit();

			this.OptionAltered -= this.UpdateUnityConsoleOptions;
		}

		public override void	OnTabGUI(int i)
		{
			EditorGUI.BeginChangeCheck();
			GUILayout.Toggle(i == this.container.WorkingStream, this.name + " (" + this.totalCount + ")", Preferences.Settings.general.menuButtonStyle);
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (Event.current.button == 0)
					this.container.FocusStream(i);
			}
		}

		public override Rect	OnGUI(UnityEngine.Rect r)
		{
			float	yOrigin = r.y;
			float	maxWidth = r.width;
			float	maxHeight = r.height;

			// Draw options.
			r = this.DrawOptions(r);

			// Draw filters on the right.
			r.width = maxWidth - r.x;
			r = this.DrawFilters(r);

			// Draw rows in a new line.
			r.x = 0F;
			r.y += r.height;
			r.width = maxWidth;
			r.height = maxHeight - (r.y - yOrigin);
			return this.rowsDrawer.DrawRows(r, this.collapse);
		}

		protected override Rect	DrawOptions(Rect r)
		{
			float	maxWidth = r.width;

			EditorGUI.BeginChangeCheck();

			r.width = 60F;
			r.height = EditorGUIUtility.singleLineHeight;
			this.collapse = GUI.Toggle(r, this.collapse, "Collapse", Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.logContent.text = this.logCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.logContent).x;
			this.displayLog = GUI.Toggle(r, this.displayLog, this.logContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.warningContent.text = this.warningCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.warningContent).x;
			this.displayWarning = GUI.Toggle(r, this.displayWarning, this.warningContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.errorContent.text = this.errorCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.errorContent).x;
			this.displayError = GUI.Toggle(r, this.displayError, this.errorContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.exceptionContent.text = this.exceptionCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.exceptionContent).x;
			this.displayException = GUI.Toggle(r, this.displayException, this.exceptionContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			// Update Unity Console only if required.
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.RefreshFilteredRows();

				if (this.OptionAltered != null)
					this.OptionAltered();

				Utility.RepaintConsoleWindow();
			}

			r.width = maxWidth - r.width * 4F;

			return r;
		}

		public override void	ConsumeLog(int i, Row row)
		{
		}

		public override void	AddLog(int i, Row row)
		{
			// MainLog does not receive output from compiler.
			if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptCompileWarning)) != 0)
				return;

			// Exclude filtered row.
			if (this.groupFilters.Filter(row) == true)
			{
				//Utility.AssertFile(true, "added " + i);

				// Count row, but do not display if it is not options compliant.
				this.CountLog(row);
				if (this.CanDisplay(row) == true)
					this.rowsDrawer.Add(i);
			}
		}

		private void	UpdateUnityConsoleOptions()
		{
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.Collapse, this.collapse);
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelLog, this.displayLog);
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelWarning, this.displayWarning);
			this.console.syncLogs.logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelError, this.displayError);
		}
	}
}