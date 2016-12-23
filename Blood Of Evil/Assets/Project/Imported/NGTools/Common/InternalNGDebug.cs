using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NGTools
{
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial class InternalNGDebug
	{
		public class NGTools : Exception
		{
			public	NGTools(string message) : base(message)
			{
			}
		}

		#region Loggers' variables
		public const char	MultiContextsStartChar = (char)1;
		public const char	MultiContextsEndChar = (char)4;
		public const char	MultiContextsSeparator = ';';

		public const char	DataStartChar = (char)2;
		public const char	DataEndChar = (char)4;
		public const char	DataSeparator = '\n';
		public const char	DataSeparatorReplace = (char)5;

		public const char	MultiTagsStartChar = (char)3;
		public const char	MultiTagsEndChar = (char)4;
		public const char	MultiTagsSeparator = ';';
		#endregion

		private static EventWaitHandle	waitHandle;

		public static string	LogPath = Constants.DefaultDebugLogFilepath;

		static	InternalNGDebug()
		{
			InternalNGDebug.waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "578943af-6fd1-4792-b36c-1713c20a37d9");
		}

		public static void	Log(object message)
		{
			UnityEngine.Debug.Log("[" + Constants.PackageTitle + "] " + message);
		}

		public static void	Log(object message, UnityEngine.Object context)
		{
			UnityEngine.Debug.Log("[" + Constants.PackageTitle + "] " + message, context);
		}

		public static void	Log(int error, string message)
		{
			UnityEngine.Debug.Log("[" + Constants.PackageTitle + "] #" + error + " - " + message);
		}

		public static void	Log(int error, string message, UnityEngine.Object context)
		{
			UnityEngine.Debug.Log("[" + Constants.PackageTitle + "] #" + error + " - " + message, context);
		}

		public static void	LogWarning(string message)
		{
			UnityEngine.Debug.LogWarning("[" + Constants.PackageTitle + "] " + message);
		}

		public static void	LogWarning(string message, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogWarning("[" + Constants.PackageTitle + "] " + message, context);
		}

		public static void	LogWarning(int error, string message)
		{
			UnityEngine.Debug.LogWarning("[" + Constants.PackageTitle + "] #" + error + " - " + message);
		}

		public static void	LogWarning(int error, string message, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogWarning("[" + Constants.PackageTitle + "] #" + error + " - " + message, context);
		}

		public static void	LogError(object message)
		{
			UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + message);
		}

		public static void	LogError(object message, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + message, context);
		}

		public static void	LogError(int error, string message)
		{
			UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] #" + error + " - " + message);
		}

		public static void	LogError(int error, string message, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] #" + error + " - " + message, context);
		}

		public static void	LogException(string message, Exception exception)
		{
			UnityEngine.Debug.LogException(new NGTools(exception.GetType().Name + ": " + message + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace));
		}

		public static void	LogException(Exception exception)
		{
			UnityEngine.Debug.LogException(new NGTools(exception.Message + Environment.NewLine + exception.StackTrace));
		}

		public static void	LogException(Exception exception, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogException(new NGTools(exception.Message + Environment.NewLine + exception.StackTrace), context);
		}

		public static void	LogException(string message, Exception exception, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogException(new NGTools(message + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace), context);
		}

		public static void	LogException(int error, Exception exception)
		{
			UnityEngine.Debug.LogException(new NGTools(error + "#" + exception.GetType().Name + ": " + exception.Message + Environment.NewLine + exception.StackTrace));
		}

		public static void	LogException(int error, Exception exception, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogException(new NGTools(error + "#" + exception.GetType().Name + ": " + exception.Message + Environment.NewLine + exception.StackTrace), context);
		}

		public static void	LogException(int error, string message, Exception exception)
		{
			UnityEngine.Debug.LogException(new NGTools(error + "# " + message + Environment.NewLine + exception.GetType().Name + ": " + exception.Message + Environment.NewLine + exception.StackTrace));
		}

		public static void	LogException(int error, string message, Exception exception, UnityEngine.Object context)
		{
			UnityEngine.Debug.LogException(new NGTools(error + "# " + message + Environment.NewLine + exception.GetType().Name + ": " + exception.Message + Environment.NewLine + exception.StackTrace), context);
		}

		[Conditional(Constants.DebugSymbol)]
		public static void	InternalLog(object message)
		{
			UnityEngine.Debug.Log("[" + Constants.PackageTitle + "] " + message);
		}

		[Conditional(Constants.DebugSymbol)]
		public static void	InternalLogWarning(object message)
		{
			UnityEngine.Debug.LogWarning("[" + Constants.PackageTitle + "] " + message);
		}

		[Conditional(Constants.DebugSymbol)]
		public static void	Assert(bool assertion, object message, UnityEngine.Object context)
		{
			if (assertion == false)
				UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + message, context);
		}

		[Conditional(Constants.DebugSymbol)]
		public static void	Assert(bool assertion, object message)
		{
			if (assertion == false)
				UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + message);
		}

		private static int	lastLogHash;
		private static int	lastLogCounter;

		[Conditional(Constants.DebugSymbol)]
		public static void	LogFile(object log)
		{
			InternalNGDebug.waitHandle.WaitOne();
			{
#if UNITY_EDITOR && !UNITY_WEBPLAYER
				InternalNGDebug.VerboseLog(log);

				int	logHash = log.GetHashCode();
				if (logHash != InternalNGDebug.lastLogHash)
				{
					InternalNGDebug.lastLogHash = logHash;
					InternalNGDebug.lastLogCounter = 0;
					File.AppendAllText(InternalNGDebug.LogPath, log + Environment.NewLine);
				}
				else
				{
					++InternalNGDebug.lastLogCounter;
					if (InternalNGDebug.lastLogCounter <= 2)
						File.AppendAllText(InternalNGDebug.LogPath, log + Environment.NewLine);
					else if (InternalNGDebug.lastLogCounter == 3)
						File.AppendAllText(InternalNGDebug.LogPath, "…" + Environment.NewLine);
				}
#else
				UnityEngine.Debug.Log("[" + Constants.PackageTitle + "] " + log);
#endif
			}
			InternalNGDebug.waitHandle.Set();
		}

		[Conditional(Constants.DebugSymbol)]
		public static void	LogFileException(string message, Exception exception)
		{
			InternalNGDebug.waitHandle.WaitOne();
			{
#if UNITY_EDITOR && !UNITY_WEBPLAYER
				InternalNGDebug.VerboseLog(message);
				InternalNGDebug.VerboseLogException(exception);

				int	logHash = message.GetHashCode() + exception.GetHashCode();
				if (logHash != InternalNGDebug.lastLogHash)
				{
					InternalNGDebug.lastLogHash = logHash;
					InternalNGDebug.lastLogCounter = 0;
					File.AppendAllText(InternalNGDebug.LogPath, message + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace + Environment.NewLine);
				}
				else
				{
					++InternalNGDebug.lastLogCounter;
					if (InternalNGDebug.lastLogCounter <= 2)
						File.AppendAllText(InternalNGDebug.LogPath, message + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace + Environment.NewLine);
					else if (InternalNGDebug.lastLogCounter == 3)
						File.AppendAllText(InternalNGDebug.LogPath, "…" + Environment.NewLine);
				}
#else
				UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + message);
				UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + exception.Message + Environment.NewLine + exception.StackTrace);
#endif
			}
			InternalNGDebug.waitHandle.Set();
		}

		[Conditional(Constants.DebugSymbol)]
		public static void	LogFileException(Exception exception)
		{
			InternalNGDebug.waitHandle.WaitOne();
			{
#if UNITY_EDITOR && !UNITY_WEBPLAYER
				InternalNGDebug.VerboseLogException(exception);

				int	logHash = exception.GetHashCode();
				if (logHash != InternalNGDebug.lastLogHash)
				{
					InternalNGDebug.lastLogHash = logHash;
					InternalNGDebug.lastLogCounter = 0;
					File.AppendAllText(InternalNGDebug.LogPath, exception.Message + Environment.NewLine + exception.StackTrace + Environment.NewLine);
				}
				else
				{
					++InternalNGDebug.lastLogCounter;
					if (InternalNGDebug.lastLogCounter <= 2)
						File.AppendAllText(InternalNGDebug.LogPath, exception.Message + Environment.NewLine + exception.StackTrace + Environment.NewLine);
					else if (InternalNGDebug.lastLogCounter == 3)
						File.AppendAllText(InternalNGDebug.LogPath, "…" + Environment.NewLine);
				}
#else
				UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + exception.Message + Environment.NewLine + exception.StackTrace);
#endif
			}
			InternalNGDebug.waitHandle.Set();
		}

		[Conditional(Constants.DebugSymbol)]
		public static void	AssertFile(bool assertion, object message)
		{
			if (assertion == false)
			{
#if UNITY_EDITOR && !UNITY_WEBPLAYER
				InternalNGDebug.VerboseAssert((message ?? "NULL").ToString());
				InternalNGDebug.LogFile((message ?? "NULL").ToString());
#else
				UnityEngine.Debug.LogError("[" + Constants.PackageTitle + "] " + (message ?? "NULL").ToString());
#endif
			}
		}

		[Conditional(Constants.VerboseDebugSymbol)]
		private static void	VerboseAssert(object message)
		{
			InternalNGDebug.LogError(message);
		}


		[Conditional(Constants.VerboseDebugSymbol)]
		private static void	VerboseLog(object message)
		{
			InternalNGDebug.Log(message);
		}

		[Conditional(Constants.VerboseDebugSymbol)]
		private static void	VerboseLogException(Exception exception)
		{
			InternalNGDebug.LogException(exception);
		}
	}
}