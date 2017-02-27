#if NGT_NESTED_MENU
using NGToolsEditor.NGAssetsFinder;
using NGToolsEditor.NGComponentReplacer;
using NGToolsEditor.NGComponentsInspector;
using NGToolsEditor.NGConsole;
using NGToolsEditor.NGFav;
using NGToolsEditor.NGHub;
using NGToolsEditor.NGNavSelection;
using NGToolsEditor.NGPrefs;
using NGToolsEditor.NGRemoteScene;
using NGToolsEditor.NGRenamer;
using NGToolsEditor.NGShaderFinder;
using NGToolsEditor.NGSyncFolders;
using UnityEditor;

namespace NGToolsEditor
{
	[InitializeOnLoad]
	internal static class ExternalNGMenuItems
	{
		public const string	MenuItemPath = Constants.PackageTitle + "/";

		static	ExternalNGMenuItems()
		{
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGSettingsWindow.Title);

			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGConsoleWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGConsoleWindow.Title + " Clear %w");

			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGRemoteHierarchyWindow.NormalTitle);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGRemoteCameraWindow.NormalTitle + "	[BETA]");
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGRemoteInspectorWindow.NormalTitle);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGRemoteProjectWindow.NormalTitle);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGReplayWindow.Title + "	[BETA]");

			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGHubWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGHubWindow.Title + " as Dock %#H");

			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGFavWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGPrefsWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGNavSelectionWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGComponentsInspectorWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGAssetsFinderWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGShaderFinderWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGRenamerWindow.Title);
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGMissingScriptRecoveryWindow.Title + "	[BETA]");
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGComponentReplacerWindow.Title + "	[BETA]");
			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGCheckGUIDWindow.Title);

			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + NGSyncFoldersWindow.Title + "	[BETA]");

			Utility.AddMenuItemPicker(ExternalNGMenuItems.MenuItemPath + "Get NG Log");
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGSettingsWindow.Title, priority = Constants.MenuItemPriority + 1)]
		private static void	OpenNGSettings()
		{
			NGSettingsWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGConsoleWindow.Title, priority = Constants.MenuItemPriority + 101)]
		public static void	OpenNGConsole()
		{
			NGConsoleWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGConsoleWindow.Title + " Clear %w", priority = Constants.MenuItemPriority + 102)]
		public static void	ClearNGConsole()
		{
			NGConsoleWindow.ClearNGConsole();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGRemoteHierarchyWindow.NormalTitle, priority = Constants.MenuItemPriority + 210)]
		public static void	OpenNGRemoteHierarchy()
		{
			NGRemoteHierarchyWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGRemoteInspectorWindow.NormalTitle, priority = Constants.MenuItemPriority + 215)]
		public static void	OpenNGRemoteInspector()
		{
			NGRemoteInspectorWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGRemoteProjectWindow.NormalTitle, priority = Constants.MenuItemPriority + 219)]
		public static void	OpenNGRemoteProject()
		{
			NGRemoteProjectWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGRemoteCameraWindow.NormalTitle + "	[BETA]", priority = Constants.MenuItemPriority + 220)]
		public static void	OpenNGRemoteCamera()
		{
			NGRemoteCameraWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGReplayWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 230)]
		private static void	OpenNGReplay()
		{
			NGReplayWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGHubWindow.Title, priority = Constants.MenuItemPriority + 300)]
		public static void	OpenNGHub()
		{
			NGHubWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGHubWindow.Title + " as Dock %#H", priority = Constants.MenuItemPriority + 301)]
		public static void	OpenNGHubAsDock()
		{
			NGHubWindow.OpenAsDock();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGFavWindow.Title, priority = Constants.MenuItemPriority + 307)]
		private static void	OpenNGFav()
		{
			NGFavWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGPrefsWindow.Title, priority = Constants.MenuItemPriority + 310)]
		public static void	OpenNGPrefs()
		{
			NGPrefsWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGNavSelectionWindow.Title, priority = Constants.MenuItemPriority + 315)]
		public static void	OpenNGNavSelection()
		{
			NGNavSelectionWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGComponentsInspectorWindow.Title, priority = Constants.MenuItemPriority + 340)]
		private static void	OpenNGComponentsInpsector()
		{
			NGComponentsInspectorWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGAssetsFinderWindow.Title, priority = Constants.MenuItemPriority + 330)]
		private static void	OpenNGAssetsFinder()
		{
			NGAssetsFinderWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGShaderFinderWindow.Title, priority = Constants.MenuItemPriority + 335)]
		private static void	OpenNGShaderFinder()
		{
			NGShaderFinderWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGRenamerWindow.Title, priority = Constants.MenuItemPriority + 340)]
		private static void	OpenNGRenamer()
		{
			NGRenamerWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGMissingScriptRecoveryWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 345)]
		public static void	OpenNGMissingScriptRecovery()
		{
			NGMissingScriptRecoveryWindow.Open();
		}

#if !NGTOOLS_FREE
		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGComponentReplacerWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 350)]
		private static void OpenNGComponentsReplacer()
		{
			NGComponentReplacerWindow.Open();
		}
#endif

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGCheckGUIDWindow.Title, priority = Constants.MenuItemPriority + 369)]
		private static void	OpenNGCheckGUID()
		{
			NGCheckGUIDWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + NGSyncFoldersWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 380)]
		private static void	Open()
		{
			NGSyncFoldersWindow.Open();
		}

		[MenuItem(ExternalNGMenuItems.MenuItemPath + "Get NG Log", priority = Constants.MenuItemPriority + 1001)]
		private static void	GetNGLog()
		{
			Preferences.GetNGLog();
		}
	}
}
#endif