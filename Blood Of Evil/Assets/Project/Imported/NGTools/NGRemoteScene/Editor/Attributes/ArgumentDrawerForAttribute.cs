using System;

namespace NGToolsEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class ArgumentDrawerFor : Attribute
	{
		public readonly Type	type;

		public	ArgumentDrawerFor(Type type)
		{
			this.type = type;
		}
	}
}