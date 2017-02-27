using System;

namespace NGToolsEditor.NGConsole
{
	[Flags]
	internal enum ConsoleFlags
	{
		Collapse = 1,
		ClearOnPlay = 2,
		ErrorPause = 4,
		Verbose = 8,
		StopForAssert = 16,
		StopForError = 32,
		Autoscroll = 64,
		LogLevelLog = 128,
		LogLevelWarning = 256,
		LogLevelError = 512
	}
}