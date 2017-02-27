using System;

namespace NGToolsEditor.NGRemoteScene
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class ArgumentDrawerFor : Attribute
	{
		public readonly Type	type;

		public	ArgumentDrawerFor(Type type)
		{
			this.type = type;
		}
	}
}