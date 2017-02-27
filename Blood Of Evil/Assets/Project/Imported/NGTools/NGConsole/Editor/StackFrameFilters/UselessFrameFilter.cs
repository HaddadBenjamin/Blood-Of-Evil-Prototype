namespace NGToolsEditor.NGConsole
{
	internal sealed class UselessFrameFilter : IStackFrameFilter
	{
		public bool	Filter(string frame)
		{
			for (int i = 0; i < Preferences.Settings.stackTrace.filters.Count; i++)
			{
				if (frame.StartsWith(Preferences.Settings.stackTrace.filters[i]) == true)
					return true;
			}

			return frame.StartsWith("NGDebug:") == true ||
				   frame.Contains(".InternalNGDebug:") == true ||
				   frame.StartsWith("UnityEngine.Debug:") == true ||
				   frame.StartsWith("UnityEditor.DockArea:OnGUI") == true;
		}
	}
}