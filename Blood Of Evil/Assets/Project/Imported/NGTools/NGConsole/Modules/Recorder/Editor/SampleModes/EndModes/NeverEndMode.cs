using System;

namespace NGToolsEditor
{
	[Serializable]
	public class NeverEndMode : EndMode
	{
		public override bool	CheckEnd(Row row)
		{
			return false;
		}
	}
}