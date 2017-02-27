using NGTools;
using System;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	[RowLogHandler(3)]
	internal sealed class MultiTagsRow : DefaultRow
	{
		public bool		isParsed;

		public string	message;
		public string[]	tags;

		private static bool	CanDealWithIt(UnityLogEntry log)
		{
			return log.condition[0] == InternalNGDebug.MultiTagsStartChar;
		}

		public override void Init(NGConsoleWindow editor, LogEntry log)
		{
			base.Init(editor, log);

			this.ParseLog();
		}

		/// <summary>
		/// Prepares the row by parsing its log.
		/// </summary>
		public void	ParseLog()
		{
			InternalNGDebug.AssertFile(this.isParsed == false, "Parsed Row is being parsed again.");

			this.isParsed = true;

			int			end = this.log.condition.IndexOf(InternalNGDebug.MultiTagsEndChar);
			string		raw = this.log.condition.Substring(1, end - 1);
			string[]	contexts = raw.Split(InternalNGDebug.MultiTagsSeparator);

			this.message = contexts[0];
			this.tags = new string[contexts.Length - 1];

			for (int i = 1; i < contexts.Length; i++)
				this.tags[i - 1] = contexts[i];
		}

		public override void	DrawLog(Rect r, int i)
		{
			float	originWidth = r.width;

			for (int j = 0; j < this.tags.Length; j++)
			{
				Utility.content.text = '[' + this.tags[j] + "] ";
				r.width = Preferences.Settings.log.timeStyle.CalcSize(Utility.content).x;
				GUI.Label(r, Utility.content, Preferences.Settings.log.style);
				r.x += r.width;
			}

			r.width = originWidth - r.x;
			GUI.Label(r, this.message, Preferences.Settings.log.style);
		}
	}
}