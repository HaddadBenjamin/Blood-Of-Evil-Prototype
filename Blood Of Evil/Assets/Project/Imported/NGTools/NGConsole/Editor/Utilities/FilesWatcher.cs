using System.Collections.Generic;
using System.IO;

namespace NGToolsEditor.NGConsole
{
	internal static class FilesWatcher
	{
		private static Dictionary<int, FileSystemWatcher>	watchers;

		static	FilesWatcher()
		{
			FilesWatcher.watchers = new Dictionary<int, FileSystemWatcher>();
		}

		public static void	Watch(string file)
		{
			int	hash = file.GetHashCode();

			if (FilesWatcher.watchers.ContainsKey(hash) == true)
				return;

			try
			{
				FileSystemWatcher	watcher = new FileSystemWatcher(Path.GetDirectoryName(file),
																	Path.GetFileName(file));

				watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

				// Add event handlers.
				watcher.Changed += new FileSystemEventHandler(OnChanged);
				watcher.Created += new FileSystemEventHandler(OnChanged);
				watcher.Deleted += new FileSystemEventHandler(OnChanged);
				watcher.Renamed += new RenamedEventHandler(OnRenamed);

				// Begin watching.
				watcher.EnableRaisingEvents = true;

				FilesWatcher.watchers.Add(hash, watcher);
			}
			catch
			{
			}
		}

		private static void	OnChanged(object source, FileSystemEventArgs e)
		{
			// Specify what is done when a file is changed, created, or deleted.
			foreach (var pair in watchers)
			{
				if (pair.Value == source)
				{
					// Remove the file from cache to force refresh.
					Utility.files.DeleteFile(pair.Key);
				}
			}
		}

		private static void	OnRenamed(object source, RenamedEventArgs e)
		{
			foreach (var pair in watchers)
			{
				if (pair.Value == source)
				{
					// Remove the file from cache and release its watcher.
					Utility.files.DeleteFile(pair.Key);
					FilesWatcher.watchers.Remove(pair.Key);
					(source as FileSystemWatcher).Dispose();
				}
			}
		}
	}
}