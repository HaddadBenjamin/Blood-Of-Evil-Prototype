using System;
using System.Collections.Generic;

namespace NGToolsEditor.NGAssetsFinder
{
	public abstract class TypeMembersExclusion
	{
		public List<string>	exclusions;

		public	TypeMembersExclusion()
		{
			this.exclusions = new List<string>();
		}

		public abstract bool	CanHandle(Type targetType);

		public bool	IsExcluded(string member)
		{
			for (int i = 0; i < this.exclusions.Count; i++)
			{
				if (this.exclusions[i] == member)
					return true;
			}

			return false;
		}
	}
}