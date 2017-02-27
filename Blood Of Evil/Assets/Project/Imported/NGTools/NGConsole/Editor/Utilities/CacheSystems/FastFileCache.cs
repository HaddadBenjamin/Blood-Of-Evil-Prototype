using NGTools;
using System;
using System.Collections.Generic;
using System.IO;

namespace NGToolsEditor.NGConsole
{
	internal sealed class FastFileCache
	{
		private sealed class HashFile
		{
			public readonly int		hash;
			public string[]			lines;

			internal	HashFile(int hash)
			{
				this.hash = hash;
			}
		}

		private HashFile[]					files;
		private Dictionary<string, bool>	cachedFileExists;
		private int							indexLeft;

		public	FastFileCache()
		{
			this.files = new HashFile[32];
			this.cachedFileExists = new Dictionary<string, bool>();
			this.indexLeft = 0;
		}

		/// <summary>
		/// Clears all cache.
		/// </summary>
		public void	Reset()
		{
			for (int i = 0; i < this.files.Length; i++)
				this.files[i] = null;
			this.indexLeft = 0;
			this.cachedFileExists.Clear();
		}

		/// <summary>
		/// Checks if a file exists in the computer.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public bool	FileExist(string fileName)
		{
			bool	exist;

			if (this.cachedFileExists.TryGetValue(fileName, out exist) == false)
			{
				exist = File.Exists(fileName);
				this.cachedFileExists.Add(fileName, exist);
			}

			return exist;
		}

		/// <summary>
		/// Finds the cached version or populates a new entry.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public string[]	GetFile(string fileName)
		{
			int	hash = fileName.GetHashCode();

			// Check if cached.
			for (int i = 0; i < this.indexLeft; i++)
			{
				if (this.files[i].hash == hash)
					return this.files[i].lines;
			}

			try
			{
				// Populate new file.
				HashFile	hashFile = new HashFile(hash);

				hashFile.lines = File.ReadAllLines(fileName);

				InternalNGDebug.Assert(Preferences.Settings != null, "NGSettings is null.");
				InternalNGDebug.Assert(Preferences.Settings.stackTrace != null, "StackTraceSettings is null.");

				for (int i = 0; i < hashFile.lines.Length; i++)
					hashFile.lines[i] = Utility.Color((i + 1).ToString(), Preferences.Settings.stackTrace.previewLineColor) + Utility.ColorLine(hashFile.lines[i]);

				// Check array overflow.
				if (this.indexLeft == this.files.Length)
					Array.Resize(ref this.files, this.files.Length << 1);

				this.files[this.indexLeft] = hashFile;
				++this.indexLeft;

				//Debug.Log("Cached file:" + fileName);
				return hashFile.lines;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}

			return null;
		}

		public void	DeleteFile(int hash)
		{
			// Check if cached.
			for (int i = 0; i < this.indexLeft; i++)
			{
				// Replace file by the last of the array.
				if (this.files[i].hash == hash)
				{
					--this.indexLeft;
					this.files[i] = this.files[this.indexLeft];
					this.files[this.indexLeft] = null;
					break;
				}
			}
		}
	}
}