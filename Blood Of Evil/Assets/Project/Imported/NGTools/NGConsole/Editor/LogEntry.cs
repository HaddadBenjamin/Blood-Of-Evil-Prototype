using System;
using System.Collections.Generic;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class LogEntry
	{
		private static Dictionary<int, string>	cachedFiles = new Dictionary<int, string>(64);

		public string	time;
		public int		frameCount;
		public int		renderedFrameCount;

		// Unity
		public string	condition;
		public int		errorNum;
		public string	file
		{
			get
			{
				try
				{
					return cachedFiles[this.fileHash];
				}
				catch
				{
					return "UnknownFile";
				}
			}
			set
			{
				this.fileHash = this.GetHashForFile(value);
			}
		}
		public int		fileHash;
		public int		line;
		public Mode		mode;
		public int		instanceID;
		// They are unused, freeze them, if one day they are to be used, unfroze them. This way we win 8 bytes...
		//public int		identifier;
		//public int		isWorldPlaying;

		public int	collapseCount;

		public void	Set(UnityLogEntry uLogEntry)
		{
			this.condition = uLogEntry.condition;
			this.errorNum = uLogEntry.errorNum;
			this.fileHash = this.GetHashForFile(uLogEntry.file);
			this.line = uLogEntry.line;
			this.mode = (Mode)uLogEntry.mode;
			this.instanceID = uLogEntry.instanceID;
			//this.identifier = uLogEntry.identifier;
			//this.isWorldPlaying = uLogEntry.isWorldPlaying;

			this.collapseCount = uLogEntry.collapseCount;
		}

		public override string	ToString()
		{
			return
				"Condition=" + this.condition + Environment.NewLine +
				"ErrorNum=" + this.errorNum + Environment.NewLine +
				"File=" + this.file + Environment.NewLine +
				"Line=" + this.line + Environment.NewLine +
				"Mode=" + this.mode + Environment.NewLine +
				"InstanceID=" + this.instanceID + Environment.NewLine;// +
				//"Identifier=" + this.identifier + Environment.NewLine +
				//"IsWorldPlaying=" + this.isWorldPlaying + Environment.NewLine;
		}

		private int	GetHashForFile(string file)
		{
			int	hash = file.GetHashCode();
			if (cachedFiles.ContainsKey(hash) == false)
				cachedFiles.Add(hash, file);
			return hash;
		}
	}
}