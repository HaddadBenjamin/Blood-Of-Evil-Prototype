using System;

namespace NGToolsEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class CategoryAttribute : Attribute
	{
		public const string	DefaultCategory = "None";

		public readonly string	name;

		public	CategoryAttribute(string name)
		{
			this.name = name;
		}
	}
}