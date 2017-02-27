using NGToolsEditor;
using System.Reflection;
using UnityEngine;
using NGConstants = NGTools.Constants;

[assembly: AssemblyTitle(Constants.PackageTitle)]
[assembly: AssemblyDescription("Plugin made for Unity.")]
[assembly: AssemblyProduct("Unity Editor")]
[assembly: AssemblyCompany("Michaël Nguyen")]
[assembly: AssemblyCopyright("Copyright © 2016 - Infinite")]
[assembly: AssemblyVersion(Constants.Version)]
[assembly: AssemblyInformationalVersion(NGConstants.TargetUnityVersion)]

namespace NGToolsEditor
{
	public static partial class Constants
	{
		#region NG Tools
		public const string	Version = NGConstants.Version;
		public const string	InternalPackageTitle = NGConstants.InternalPackageTitle;
		public const string	PackageTitle = NGConstants.PackageTitle;
		public const string	PreferenceTitle = NGConstants.PackageTitle;
		public const string	RootFolderName = "NGTools";
		public const string	SettingsFilename = "NGSettings.asset";
		public const string	NestedMenuSymbol = NGConstants.NestedMenuSymbol;
		public const string	DebugLogFilepathKeyPref = NGConstants.DebugLogFilepathKeyPref;
		public const string	DefaultDebugLogFilepath = NGConstants.DefaultDebugLogFilepath;
		public const string	ConfigPathPref = "NGTConfigPath";
		public const string	WikiBaseURL = NGConstants.WikiBaseURL;
		public const string	AllowSendStatsKeyPref = "NGTools_AllowSendStats";
		#endregion

		#region Localization
		public const string	DefaultLanguage = "english";
		public const string	RelativeLocaleFolder = "Locales/";
		public const string	LanguageEditorPref = "NGTLocalizationLanguage";
		#endregion

		#region Inputs Manager
		public const float	CheckFadeoutCooldown = 6F;
		#endregion

		#region Export
		public const string	LastExportPathPref = "NGTLastExportPath";
		#endregion

		#region Contact
		public const string	SupportForumUnityThread = "http://forum.unity3d.com/threads/released-ng-tools-skyrocket-your-unity-workflow-efficiency.378040/";
		public const string	SupportEmail = "ngtoolscontact@yahoo.fr";
		#endregion

		#region Common
		public const EventModifiers	ByPassPromptModifier = EventModifiers.Shift;
		public const float	MinStartDragDistance = 40F;
		public const double	DoubleClickTime = .3D;
		public const int	MenuItemPriority = 1000;
		public const string	MenuItemPath = "Window/" + Constants.PackageTitle + "/";
		#endregion
	}
}