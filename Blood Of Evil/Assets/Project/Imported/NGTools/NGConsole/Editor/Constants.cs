using UnityEngine;

namespace NGToolsEditor
{
	public static partial class Constants
	{
		#region Console Settings
		public const string	FoldoutTemplate = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABmJLR0QAAAAAAAD5Q7t/AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3wMbEjQH1ei/XgAAAB1pVFh0Q29tbWVudAAAAAAAQ3JlYXRlZCB3aXRoIEdJTVBkLmUHAAAAkUlEQVQ4y8VTOw6AIAy1xJOgIxo3dPfY7upmIqscBZxMXkhpjAx2K8n7lZa0sVVJqaqwamy82yP2bT9RCEEkoCdCCsbSxlJRBO/2mBPIEjTdSNpYQnWOROXsXucWuQgpCetgmGZC6ykIe5bgWJf46Rul6aMqvitu4m/BogNu4tw+1G9ySotEeEypqgRkCX65xhuF/0gtDgYwJwAAAABJRU5ErkJggg==";
		public const string	FoldoutOnTemplate = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABmJLR0QAAAAAAAD5Q7t/AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3wMbEzI62hg+/gAAAB1pVFh0Q29tbWVudAAAAAAAQ3JlYXRlZCB3aXRoIEdJTVBkLmUHAAAAiElEQVQ4y8WTMQ6AMAhFgXTVQ1TH6lq9u7txc+jsUYqT0VSlmA6yQYH3KQGt81BiBIVmrs4WFtYUWefxpkBbnOYaDUUCkETZwsJ1VYnqKEdf54kBAJpuQLFBKvnqW+eR+RTR9iMWrTHG+DzCQX2a+YilSkmzqk+fmBJyb6RNfGtMGpqkCn+/xh0QijlFLt2sHQAAAABJRU5ErkJggg==";

		public const string	ScriptDefaultApp = "kScriptsDefaultApp";

		/// <summary>
		/// Use this control name to prevent RowsDrawer from overwritting copy command.
		/// </summary>
		public const string	CopyControlName = "copyControl";

		public const int	PreAllocatedArray = 512;
		public const float	HorizontalScrollbarWidth = 15F;
		public const float	VerticalScrollbarWidth = 15F;
		public const float	DefaultSingleLineHeight = 16F;

		public static readonly Color	NormalFoldoutColor = new Color(105F / 255F, 105F / 255F, 105F / 255F);
		public static readonly Color	ActiveFoldoutColor = new Color(164F / 255F, 164F / 255F, 164F / 255F);
		public static readonly Color	WarningFoldoutColor = new Color(255F / 255F, 187F / 255F, 0F / 255F);
		public static readonly Color	ErrorFoldoutColor = new Color(236F / 255F, 0F / 255F, 0F / 255F);
		public static readonly Color	ExceptionFoldoutColor = new Color(255F / 255F, 0F / 255F, 0F / 255F);
		#endregion

		#region RowsDrawer
		public const float	RowContentSplitterHeight = 5F;
		public const float	MinRowContentHeight = 32F;
		public const float	MaxRowContentHeightLeft = 100F;
		public const float	CriticalMinimumContentHeight = 5F;
		#endregion

		#region NG Console Commands
		public const string	SwitchNextStreamCommand = "SwitchNextStream";
		public const string	SwitchPreviousStreamCommand = "SwitchPreviousStream";
		public const string	OpenLogCommand = "OpenLog";
		public const string	CloseLogCommand = "CloseLog";
		public const string	FocusTopLogCommand = "FocusTopLog";
		public const string	FocusBottomLogCommand = "FocusBottomLog";
		public const string	MoveUpLogCommand = "MoveUpLog";
		public const string	MoveDownLogcommand = "MoveDownLog";
		public const string	LongMoveUpLogCommand = "LongMoveUpLog";
		public const string	LongMoveDownLogCommand = "LongMoveDownLog";
		public const string	GoToLineCommand = "GoToLine";
		public const string	DeleteLogCommand = "DeleteLog";
		#endregion

		#region Export
		public const int	PreviewRowsCount = 3;
		#endregion
	}
}