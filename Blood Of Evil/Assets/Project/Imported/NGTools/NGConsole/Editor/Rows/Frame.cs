using System;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class Frame
	{
		public string	raw;
		public string	frameString;
		public string	fileName;
		public int		line;
		public bool		fileExist;
		public string	category;

		public override string ToString()
		{
			return "Frame=" + this.frameString + Environment.NewLine +
				"File=" + this.fileName + Environment.NewLine +
				"Line=" + this.line + Environment.NewLine +
				"Exist=" + this.fileExist + Environment.NewLine +
				"Raw=" + this.raw + Environment.NewLine +
				"Category=" + this.category;
		}
	}
}