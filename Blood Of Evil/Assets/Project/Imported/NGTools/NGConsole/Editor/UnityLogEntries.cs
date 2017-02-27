using System;
using System.Reflection;

namespace NGToolsEditor.NGConsole
{
	/// <summary>
	/// Class cloned from Unity's editor internal class "UnityEditorInternal.LogEntries".
	/// </summary>
	internal sealed class UnityLogEntries
	{
		public int	consoleFlags
		{
			get { return (int)this.consoleFlagsProperty.GetValue(null, null); }
			set { this.consoleFlagsProperty.SetValue(null, value, null); }
		}

		private PropertyInfo	consoleFlagsProperty;
		private MethodInfo		RowGotDoubleClickedMethod;
		private MethodInfo		GetStatusTextMethod;
		private MethodInfo		GetStatusMaskMethod;
		private MethodInfo		StartGettingEntriesMethod;
		private MethodInfo		SetConsoleFlagMethod;
		private MethodInfo		EndGettingEntriesMethod;
		private MethodInfo		GetCountMethod;
		private MethodInfo		GetCountsByTypeMethod;
		private MethodInfo		GetFirstTwoLinesEntryTextAndModeInternalMethod;
		private MethodInfo		GetEntryInternalMethod;
		private MethodInfo		GetEntryCountMethod;
		private MethodInfo		ClearMethod;
		private MethodInfo		GetStatusViewErrorIndexMethod;
		private MethodInfo		ClickStatusBarMethod;

		public	UnityLogEntries(Type logEntriesType)
		{
			this.consoleFlagsProperty = logEntriesType.GetProperty("consoleFlags", BindingFlags.Static | BindingFlags.Public);

			this.RowGotDoubleClickedMethod = logEntriesType.GetMethod("RowGotDoubleClicked", BindingFlags.Static | BindingFlags.Public);
			this.GetStatusTextMethod = logEntriesType.GetMethod("GetStatusText", BindingFlags.Static | BindingFlags.Public);
			this.GetStatusMaskMethod = logEntriesType.GetMethod("GetStatusMask", BindingFlags.Static | BindingFlags.Public);
			this.StartGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public);
			this.SetConsoleFlagMethod = logEntriesType.GetMethod("SetConsoleFlag", BindingFlags.Static | BindingFlags.Public);
			this.EndGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public);
			this.GetCountMethod = logEntriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public);
			this.GetCountsByTypeMethod = logEntriesType.GetMethod("GetCountsByType", BindingFlags.Static | BindingFlags.Public);
			this.GetFirstTwoLinesEntryTextAndModeInternalMethod = logEntriesType.GetMethod("GetFirstTwoLinesEntryTextAndModeInternal", BindingFlags.Static | BindingFlags.Public);
			this.GetEntryInternalMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
			this.GetEntryCountMethod = logEntriesType.GetMethod("GetEntryCount", BindingFlags.Static | BindingFlags.Public);
			this.ClearMethod = logEntriesType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
			this.GetStatusViewErrorIndexMethod = logEntriesType.GetMethod("GetStatusViewErrorIndex", BindingFlags.Static | BindingFlags.Public);
			this.ClickStatusBarMethod = logEntriesType.GetMethod("ClickStatusBar", BindingFlags.Static | BindingFlags.Public);
		}

		public void		RowGotDoubleClicked(int index) { RowGotDoubleClickedMethod.Invoke(null, new object[1] { index }); }
		public string	GetStatusText() { return (string)GetStatusTextMethod.Invoke(null, new object[0]); }
		public int		GetStatusMask() { return (int)GetStatusMaskMethod.Invoke(null, new object[0]); }
		public int		StartGettingEntries() { return (int)StartGettingEntriesMethod.Invoke(null, new object[0]); }
		public void		SetConsoleFlag(int bit, bool value) { SetConsoleFlagMethod.Invoke(null, new object[2] { bit, value }); }
		public void		EndGettingEntries() { EndGettingEntriesMethod.Invoke(null, new object[0]); }
		public int		GetCount() { return (int)GetCountMethod.Invoke(null, new object[0]); }
		public void		GetCountsByType(ref int errorCount, ref int warningCount, ref int logCount) { GetCountsByTypeMethod.Invoke(null, new object[3] { errorCount, warningCount, logCount }); }
		public void		GetFirstTwoLinesEntryTextAndModeInternal(int row, ref int mask, ref string outString) { GetFirstTwoLinesEntryTextAndModeInternalMethod.Invoke(null, new object[3] { row, mask, outString }); }
		public bool		GetEntryInternal(int row, object outputEntry) { return (bool)GetEntryInternalMethod.Invoke(null, new object[2] { row, outputEntry }); }
		public int		GetEntryCount(int row) { return (int)GetEntryCountMethod.Invoke(null, new object[1] { row }); }
		public void		Clear() { ClearMethod.Invoke(null, new object[0]); }
		public int		GetStatusViewErrorIndex() { return (int)GetStatusViewErrorIndexMethod.Invoke(null, new object[0]); }
		public void		ClickStatusBar(int count) { ClickStatusBarMethod.Invoke(null, new object[1] { count }); }
	}
}