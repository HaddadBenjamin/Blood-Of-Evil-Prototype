using System;

namespace NGTools
{
	public static partial class Conf
	{
		public enum DebugModes
		{
			None,
			Active,
			Verbose
		}

		public const string	DebugModeKeyPref = "NGTools_DebugMode";

		public static Action	DebugModeChanged = null;

		private static DebugModes	debugMode;
		public static DebugModes	DebugMode
		{
			get
			{
				return Conf.debugMode;
			}
			set
			{
				if (Conf.debugMode != value)
				{
					Conf.debugMode = value;
					if (Conf.DebugModeChanged != null)
						Conf.DebugModeChanged();
				}
			}
		}
	}
}