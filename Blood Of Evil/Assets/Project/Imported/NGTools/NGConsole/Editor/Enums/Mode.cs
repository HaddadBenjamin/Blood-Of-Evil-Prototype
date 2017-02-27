using System;

namespace NGToolsEditor.NGConsole
{
	[Flags]
	public enum Mode
	{
		Error = 1,
		Assert = 2,
		Log = 4,
		Fatal = 16,
		DontPreprocessCondition = 32,
		AssetImportError = 64,
		AssetImportWarning = 128,
		ScriptingError = 256,
		ScriptingWarning = 512,
		ScriptingLog = 1024,
		ScriptCompileError = 2048,
		ScriptCompileWarning = 4096,
		StickyError = 8192,
		MayIgnoreLineNumber = 16384,
		ReportBug = 32768,
		DisplayPreviousErrorInStatusBar = 65536,
		ScriptingException = 131072,
		DontExtractStacktrace = 262144,
		ShouldClearOnPlay = 524288,
		GraphCompileError = 1048576,
		ScriptingAssertion = 2097152
	}
}