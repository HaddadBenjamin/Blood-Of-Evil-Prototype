using NGTools;
using System;

namespace NGToolsEditor
{
	public class ClientMethod
	{
		public readonly ClientComponent	parent;
		public readonly string			name;
		public readonly Type[]			argumentTypes;
		public readonly string[]		argumentNames;

		public	ClientMethod(ClientComponent parent, NetMethod method)
		{
			this.parent = parent;
			this.name = method.name;
			this.argumentTypes = method.argumentTypes;
			this.argumentNames = method.argumentNames;
		}
	}
}