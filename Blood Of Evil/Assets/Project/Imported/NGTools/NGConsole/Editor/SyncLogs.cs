using System;
using UnityEditorInternal;
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
using UnityEngine;
#else
using UnityEngine.Profiling;
#endif

namespace NGToolsEditor.NGConsole
{
	/// <summary>
	/// Interface between Unity's internal Console and NGConsole.
	/// </summary>
	internal sealed class SyncLogs
	{
		public Action<int, UnityLogEntry>	NewLog;
		public Action<int, UnityLogEntry>	UpdateLog;
		public Action						EndNewLog;
		public Action						ResetLog;
		public Action						ClearLog;
		public Action						OptionAltered;

		internal UnityLogEntries	logEntries;
		internal UnityLogEntry		logEntry;

		private NGConsoleWindow	editor;
		private int			lastRawEntryCount = -1;
		private bool		previousCollapse;
		private int			lastFlags;

		public	SyncLogs(NGConsoleWindow editor)
		{
			this.editor = editor;

			var	logEntriesType = typeof(InternalEditorUtility).Assembly.GetType("UnityEditorInternal.LogEntries");
			logEntries = new UnityLogEntries(logEntriesType);

			var	logEntryType = typeof(InternalEditorUtility).Assembly.GetType("UnityEditorInternal.LogEntry");
			logEntry = new UnityLogEntry(logEntryType) { instance = Activator.CreateInstance(logEntryType) };
		}

		/// <summary>
		/// Handles incoming logs. Synchronizes only when required.
		/// </summary>
		public void	Sync()
		{
			Profiler.BeginSample("Sync");
			int	backupFlag = logEntries.consoleFlags;

			logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelLog, true);
			logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelWarning, true);
			logEntries.SetConsoleFlag((int)ConsoleFlags.LogLevelError, true);
			//logEntries.SetConsoleFlag((int)ConsoleFlags.Collapse, true);

			int	totalLogCount = logEntries.GetCount();
			if (totalLogCount == this.lastRawEntryCount)
			{
				// Update rows with new collapse count.
				if (this.UpdateLog != null &&
					(logEntries.consoleFlags & (int)ConsoleFlags.Collapse) != 0 &&
					this.lastRawEntryCount > 0)
				{
					for (int i = 0; i < totalLogCount; i++)
					{
						logEntry.collapseCount = logEntries.GetEntryCount(i);
						if (this.UpdateLog != null)
							this.UpdateLog(i, logEntry);
					}
					Utility.RepaintEditorWindow(typeof(ModuleWindow));
					this.editor.Repaint();
				}

				logEntries.consoleFlags = backupFlag;
				// Repaint console if an option was altered.
				if (backupFlag != this.lastFlags)
				{
					this.lastFlags = backupFlag;
					if (this.OptionAltered != null)
						this.OptionAltered();
					Utility.RepaintEditorWindow(typeof(ModuleWindow));
					this.editor.Repaint();
				}
				Profiler.EndSample();
				return;
			}

			if (this.lastRawEntryCount > totalLogCount)
			{
				// If collapse is disabled, it means this is a call to Clear.
				if ((logEntries.consoleFlags & (int)ConsoleFlags.Collapse) == 0)
				{
					this.lastRawEntryCount = 0;
					this.logEntries.consoleFlags = backupFlag;
					if (this.ClearLog != null)
						this.ClearLog();
					Profiler.EndSample();
					return;
				}

				// Otherwise collapse has just been enabled.
				this.lastRawEntryCount = 0;
				this.logEntries.consoleFlags = backupFlag;
				if (this.ResetLog != null)
					this.ResetLog();
			}
			// If collapse was just enabled, we must force the refresh of previous logs.
			else if ((logEntries.consoleFlags & (int)ConsoleFlags.Collapse) == 0 &&
					 this.previousCollapse == true)
			{
				this.lastRawEntryCount = 0;
				if (this.ResetLog != null)
					this.ResetLog();
			}

			logEntries.StartGettingEntries();

			for (int i = this.lastRawEntryCount; i < totalLogCount; i++)
			{
				if (logEntries.GetEntryInternal(i, logEntry.instance) == true)
				{
					logEntry.collapseCount = logEntries.GetEntryCount(i);

					if (this.NewLog != null)
						this.NewLog(i, logEntry);
				}
			}

			this.logEntries.EndGettingEntries();
			this.lastRawEntryCount = totalLogCount;
			this.logEntries.consoleFlags = backupFlag;
			this.EndNewLog();
			this.previousCollapse = (backupFlag & (int)ConsoleFlags.Collapse) != 0;
			Profiler.EndSample();
		}

		/// <summary>
		/// Resets SyncLogs entry count. Forcing it to resynchronize all logs.
		/// </summary>
		public void	LocalClear()
		{
			this.lastRawEntryCount = 0;
		}

		/// <summary>
		/// Resets local data, then clears and repaints Unity's Console.
		/// </summary>
		public void	Clear()
		{
			this.LocalClear();
			this.logEntries.EndGettingEntries();
			this.logEntries.Clear();

			// An issue happens when a sticky compile error is present and we want to clear, it does not keep the log whereas it was sticky.
			// Just force the refresh.
			logEntries.consoleFlags = logEntries.consoleFlags;

			Utility.RepaintEditorWindow(typeof(ModuleWindow));
			Utility.RepaintConsoleWindow();
		}
	}
}