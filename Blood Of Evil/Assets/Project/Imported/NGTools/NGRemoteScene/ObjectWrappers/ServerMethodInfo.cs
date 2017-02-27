using System;
using System.Reflection;
using System.Text;

namespace NGTools.NGRemoteScene
{
	public sealed class ServerMethodInfo
	{
		public readonly MethodInfo	methodInfo;
		public readonly Type[]		argumentTypes;
		public readonly string[]	argumentNames;

		public	ServerMethodInfo(MethodInfo method)
		{
			this.methodInfo = method;

			ParameterInfo[]	parameters = method.GetParameters();

			this.argumentTypes = new Type[parameters.Length];
			this.argumentNames = new string[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				this.argumentTypes[i] = parameters[i].ParameterType;
				this.argumentNames[i] = parameters[i].Name;
			}
		}

		public string	GetSignature()
		{
			StringBuilder	buffer = Utility.sharedBuffer;

			buffer.Length = 0;
			buffer.Append(this.methodInfo.ReturnType.Name);
			buffer.Append(' ');
			buffer.Append(this.methodInfo.Name);
			buffer.Append('(');

			for (int i = 0; i < this.argumentTypes.Length; i++)
			{
				if (i > 0)
					buffer.Append(',');
				buffer.Append(this.argumentTypes[i].Name);
			}

			buffer.Append(')');
			return buffer.ToString();
		}
	}
}