using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	internal static class FreeOverlay
	{
		private const string	KeyPrefs = "NGTools_Free_windows";
		private const string	LastTimeKeyPref = "NGTools_Free_lastTime";

		private static EditorWindow	lastWindow;
		private static Rect			r;
		private static string		ads;

		private static string	closedWindows = null;

		static	FreeOverlay()
		{
			EditorApplication.delayCall += () => {
				string	path = Path.Combine(Application.persistentDataPath, Path.Combine(Constants.PackageTitle, Constants.SettingsFilename));
				string	timeS = EditorPrefs.GetString(FreeOverlay.LastTimeKeyPref);

				long	time = 0;
				long	now = DateTime.Now.Ticks;

				if (string.IsNullOrEmpty(timeS) == false)
					long.TryParse(timeS, out time);

				// Two ways to check last time. Hehehe, go away hacker! :)
				if (File.Exists(path) == true)
				{
					long	time2;

					if (long.TryParse(File.ReadAllText(path), out time2) == true)
					{
						if (time2 > time)
							time = time2;
					}
				}

				if (now - time > FreeConstants.ResetAdsIntervalHours * 3600L * 10000000L) // 6 hours
				{
					try
					{
						EditorPrefs.SetString(FreeOverlay.KeyPrefs, string.Empty);
						EditorPrefs.SetString(FreeOverlay.LastTimeKeyPref, now.ToString());
						File.WriteAllText(path, now.ToString());
					}
					catch
					{
					}
				}

				FreeOverlay.closedWindows = EditorPrefs.GetString(FreeOverlay.KeyPrefs, string.Empty);
			};
		}

		[Conditional("NGTOOLS_FREE")]
		public static void	First(EditorWindow window, string ads)
		{
			if (FreeOverlay.closedWindows == null)
			{
				window.Repaint();
				return;
			}

			FreeOverlay.lastWindow = window;

			if (FreeOverlay.closedWindows.IndexOf(window.GetType().Name) != -1)
				return;

			FreeOverlay.ads = ads;

			FreeOverlay.r = window.position;
			FreeOverlay.r.x = 0F;
			FreeOverlay.r.y = 0F;

			if (Event.current.type == EventType.MouseDown)
			{
				FreeOverlay.closedWindows += window.GetType().Name;
				EditorPrefs.SetString(FreeOverlay.KeyPrefs, FreeOverlay.closedWindows);
				Event.current.Use();
			}
		}

		[Conditional("NGTOOLS_FREE")]
		public static void	Last()
		{
			if (FreeOverlay.closedWindows == null || FreeOverlay.closedWindows.IndexOf(FreeOverlay.lastWindow.GetType().Name) != -1)
				return;

			if (Event.current.type == EventType.Repaint)
				Utility.DropZone(FreeOverlay.r, FreeOverlay.ads);
		}
	}
}