using NGTools.NGRemoteScene;
using System;
using System.Text;

namespace NGToolsEditor.NGRemoteScene
{
	public sealed class ClientMethod
	{
		public readonly ClientComponent	parent;
		public readonly string			name;
		public readonly Type			returnType;
		public readonly Type[]			argumentTypes;
		public readonly string[]		argumentNames;

		public	ClientMethod(ClientComponent parent, NetMethod method)
		{
			this.parent = parent;
			this.name = method.name;
			this.returnType = method.returnType;
			this.argumentTypes = method.argumentTypes;
			this.argumentNames = method.argumentNames;
		}

		public string	GetSignature()
		{
			StringBuilder	buffer = Utility.GetBuffer();

			buffer.Append(this.returnType.Name);
			buffer.Append(' ');
			buffer.Append(this.name);
			buffer.Append('(');

			for (int i = 0; i < this.argumentTypes.Length; i++)
			{
				if (i > 0)
					buffer.Append(',');
				buffer.Append(this.argumentTypes[i].Name);
			}

			buffer.Append(')');
			return Utility.ReturnBuffer(buffer);
		}
	}
}