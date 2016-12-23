namespace NGToolsEditor
{
	public class UselessFrameFilter : IStackFrameFilter
	{
		public bool	Filter(string frame)
		{
			for (int i = 0; i < Preferences.Settings.stackTrace.filters.Count; i++)
			{
				if (frame.StartsWith(Preferences.Settings.stackTrace.filters[i]) == true)
					return true;
			}

			return frame.StartsWith("NGDebug:") == true ||
				   frame.StartsWith("NGTools.InternalNGDebug:") == true ||
				   frame.StartsWith("UnityEngine.Debug:Log") == true ||
				   frame.StartsWith("UnityEditor.DockArea:OnGUI") == true;
		}
	}
}