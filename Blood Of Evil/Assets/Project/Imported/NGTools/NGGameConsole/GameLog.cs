using System;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	public class GameLog
	{
		public readonly string	firstLine;
		public readonly string	condition;
		public readonly string	stackTrace;
		public readonly LogType	type;
		public readonly string	time;

		public bool	opened;
		public int	count;

		public	GameLog(string condition, string stackTrace, LogType type, string timeFormat)
		{
			int	newLine = condition.IndexOf('\n');

			if (newLine > 0)
				this.firstLine = condition.Substring(0, newLine);
			else
				this.firstLine = condition;

			this.condition = condition;
			this.stackTrace = stackTrace;
			this.type = (LogType)(1 << (int)type);
			this.opened = false;
			this.count = 1;

			try
			{
				time = DateTime.Now.ToString(timeFormat);
			}
			catch
			{
			}
		}
	}
}