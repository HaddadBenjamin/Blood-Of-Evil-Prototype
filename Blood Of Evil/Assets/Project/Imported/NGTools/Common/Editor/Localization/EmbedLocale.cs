using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public abstract class EmbedLocale
	{
		public readonly string						language;
		public readonly Texture2D					icon;
		public readonly Dictionary<string, string>	pairs = new Dictionary<string, string>();

		protected	EmbedLocale(string language, string base64Icon)
		{
			this.language = Utility.NicifyVariableName(language);
			this.icon = new Texture2D(0, 0);
			this.icon.hideFlags = HideFlags.DontSave;
			this.icon.LoadImage(Convert.FromBase64String(base64Icon));
		}
	}
}