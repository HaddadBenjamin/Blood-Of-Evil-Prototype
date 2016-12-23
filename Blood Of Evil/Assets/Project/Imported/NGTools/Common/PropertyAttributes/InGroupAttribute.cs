using System;

namespace NGTools
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class InGroupAttribute : Attribute
	{
		public readonly string	group;

		public	InGroupAttribute(string group)
		{
			this.group = group;
		}
	}
}