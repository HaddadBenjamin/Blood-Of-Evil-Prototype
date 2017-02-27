namespace NGToolsEditor.NGConsole
{
	public interface IStackFrameFilter
	{
		/// <summary>
		/// Checks whether the frame must be removed or not from the stack trace.
		/// </summary>
		/// <param name="frame">Raw content of a frame given by Unity.</param>
		/// <returns></returns>
		bool	Filter(string frame);
	}
}