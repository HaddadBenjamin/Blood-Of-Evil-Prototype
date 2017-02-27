namespace NGToolsEditor
{
	internal static class FreeConstants
	{
		public const int	MaxFavorites = 2;
		public const int	MaxSelectionPerFavorite = 4;
		public const int	MaxAssetPerSelection = 4;

		public const int	MaxStreams = 2;
		public const int	MaxFilters = 2;
		public const int	LowestRowGoToLineAllowed = 3;
		public const int	MaxColorMarkers = 4;
		public const int	MaxCLICommandExecutions = 30;

		public const int	MaxHubComponents = 4;

		public const int	MaxAssetReplacements = 10;
		public const int	MaxShaderReplacements = 5;

		public const int	MaxSyncFoldersSlaves = 2;
		public const int	MaxSyncFoldersProfiles = 2;

		public const long	ResetAdsIntervalHours = 8L;

		public static bool	CheckMaxFavorites(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxFavorites, "Free version does not allow more than " + FreeConstants.MaxFavorites + " favorites.\n\nI have to be honest, it is sufficient for me. ;)");
		}

		public static bool	CheckMaxSelections(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxSelectionPerFavorite, "Free version does not allow more than " + FreeConstants.MaxSelectionPerFavorite + " slots.\n\nMaybe you should buy it, might be useful one day. :)");
		}

		public static bool	CheckMaxAssetsPerSelection(int count)
		{
			return FreeConstants.Check(count <= FreeConstants.MaxAssetPerSelection, "Free version does not allow more than " + FreeConstants.MaxAssetPerSelection + " assets per selection.\n\nHey, don't abuse of this! X)");
		}

		public static bool	CheckMaxFilters(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxFilters, "Free version does not allow more than " + FreeConstants.MaxFilters + " filters.\n\nClick no more, I can see you want more! :D");
		}

		public static bool	CheckLowestRowGoToLineAllowed(int count)
		{
			return FreeConstants.Check(count < FreeConstants.LowestRowGoToLineAllowed, "Free version does not allow to go to line on frame lower than " + FreeConstants.LowestRowGoToLineAllowed + ".\n\nYou just asked for a kill feature... No? You disagree? I am truly sorry, but this is madness! XD");
		}

		public static bool	CheckMaxColorMarkers(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxColorMarkers, "Free version does not allow more than " + FreeConstants.MaxColorMarkers + " color markers.\n\nMarkers are truly awesome to pinpoint words or specific logs in a glance. :]");
		}

		public static bool	CheckMaxStreams(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxStreams, "Free version does not allow more than " + FreeConstants.MaxStreams + " streams.\n\nI'm sorry if you feel this feature is a gift from above, but consider above to be selfish sometimes. :p");
		}

		public static bool	CheckMaxHubComponents(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxHubComponents, "Free version does not allow more than " + FreeConstants.MaxHubComponents + " components.\n\nToo bad dude... But if you like this feature, you know what to do. ;}\n\nNote that if you have too many components, NG Hub will extend to the other side.");
		}

		public static bool	CheckMaxAssetReplacements(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxAssetReplacements, "Free version does not allow more than " + FreeConstants.MaxAssetReplacements + " replacements at once.\n\nUse this feature with moderation. Like beer, with moderation. ;D");
		}

		public static bool	CheckMaxShaderReplacements(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxShaderReplacements, "Free version does not allow more than " + FreeConstants.MaxShaderReplacements + " replacements at once.\n\nUse this feature with moderation. Like wine, with moderation. :@");
		}

		public static bool	CheckMaxSyncFoldersSlaves(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxSyncFoldersSlaves, "Free version does not allow more than " + FreeConstants.MaxSyncFoldersSlaves + " slaves.\n\n");
		}

		public static bool	CheckMaxSyncFoldersProfiles(int count)
		{
			return FreeConstants.Check(count < FreeConstants.MaxSyncFoldersProfiles, "Free version does not allow more than " + FreeConstants.MaxSyncFoldersProfiles + " profiles.\n\n");
		}

		private static bool	Check(bool condition, string ad)
		{
#if NGTOOLS_FREE
			if (condition == false)
			{
				UnityEditor.EditorUtility.DisplayDialog(Constants.PackageTitle, ad, "OK");
				return false;
			}
#endif
			return true;
		}
	}
}