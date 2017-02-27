using System;

namespace NGToolsEditor.NGRemoteScene
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class TypeHandlerDrawerForAttribute : Attribute
	{
		public readonly Type	type;

		public	TypeHandlerDrawerForAttribute(Type type)
		{
			this.type = type;
		}
	}
}