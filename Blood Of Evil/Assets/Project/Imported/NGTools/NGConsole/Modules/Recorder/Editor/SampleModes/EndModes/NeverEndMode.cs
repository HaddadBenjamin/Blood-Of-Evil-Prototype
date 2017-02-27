using System;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	internal sealed class NeverEndMode : EndMode
	{
		public override bool	CheckEnd(Row row)
		{
			return false;
		}
	}
}