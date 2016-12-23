using System;

namespace NGToolsEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TypeDrawerForAttribute : Attribute
	{
		public readonly Type	type;

		public	TypeDrawerForAttribute(Type type)
		{
			this.type = type;
		}
	}
}