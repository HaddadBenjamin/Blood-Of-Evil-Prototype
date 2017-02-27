using NGTools;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle(Constants.PackageTitle)]
[assembly: AssemblyDescription("Plugin made for Unity.")]
[assembly: AssemblyProduct("Unity Editor")]
[assembly: AssemblyCompany("Michaël Nguyen")]
[assembly: AssemblyCopyright("Copyright © 2016 - Infinite")]
[assembly: AssemblyVersion(Constants.Version)]
[assembly: AssemblyInformationalVersion(Constants.TargetUnityVersion)]
[assembly: InternalsVisibleTo("NGToolsEditor")]
[assembly: InternalsVisibleTo("NGToolsEditor" + Constants.TargetUnityVersion)]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-firstpass")]

namespace NGTools
{
	public static partial class Constants
	{
		public const string	Version = "0.8.30";
		public const string	InternalPackageTitle = "NGTools";
#if NGTOOLS_FREE
		public const string	PackageTitle = "NG Tools Free";
#else
		public const string	PackageTitle = "NG Tools Pro";
#endif
		public const string	NestedMenuSymbol = "NGT_NESTED_MENU";
		public const string	DebugLogFilepathKeyPref = "NGToolsLogPath";
		public const string	DefaultDebugLogFilepath = "NGTLogs.txt";
		public const string	WikiBaseURL = "https://bitbucket.org/Mikilo/neguen-tools/wiki/Documentation/0.8.27";

		internal const string	TargetUnityVersion = 
#if UNITY_4_5
	"4.5"
#elif UNITY_4_6
	"4.6"
#elif UNITY_4_7
	"4.7"
#elif UNITY_5_0
	"5.0"
#elif UNITY_5_1
	"5.1"
#elif UNITY_5_2
	"5.2"
#elif UNITY_5_3
	"5.3"
#elif UNITY_5_4
	"5.4"
#elif UNITY_5_5
	"5.5"
#elif UNITY_5_6
	"5.6"
#elif UNITY_5_7
	"5.7"
#else
	"X.X"
#endif
		;
	}
}
