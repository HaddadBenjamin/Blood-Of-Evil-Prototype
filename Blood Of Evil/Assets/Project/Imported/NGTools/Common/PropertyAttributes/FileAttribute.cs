using UnityEngine;

namespace NGTools
{
	public class FileAttribute : PropertyAttribute
	{
		public enum Mode
		{
			Open,
			Save
		}

		public Mode		mode;
		public string	extension;

		public	FileAttribute(Mode mode, string extension)
		{
			this.mode = mode;
			this.extension = extension;
		}
	}
}