using System;

namespace NGTools.NGGameConsole
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CommandAttribute : Attribute
	{
		public readonly string	name;
		public readonly string	description;

		public	CommandAttribute(string name, string description)
		{
			this.name = name;
			this.description = description;
		}
	}
}