using System;

namespace NGToolsEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TypeHandlerDrawerForAttribute : Attribute
	{
		public readonly Type	type;

		public	TypeHandlerDrawerForAttribute(Type type)
		{
			this.type = type;
		}
	}
}