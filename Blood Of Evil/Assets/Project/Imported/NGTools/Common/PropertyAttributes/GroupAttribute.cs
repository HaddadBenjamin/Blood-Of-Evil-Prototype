using UnityEngine;

namespace NGTools
{
	public class GroupAttribute : PropertyAttribute
	{
		public readonly string	group;
		public readonly bool	hide;

		public	GroupAttribute(string group, bool hide = false)
		{
			this.group = group;
			this.hide = hide;
			// Force Group to be the first.
			this.order = -1;
		}
	}
}