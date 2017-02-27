using System;

namespace NGToolsEditor.NGRemoteScene
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class TypeDrawerForAttribute : Attribute
	{
		public readonly Type	type;

		public	TypeDrawerForAttribute(Type type)
		{
			this.type = type;
		}
	}
}