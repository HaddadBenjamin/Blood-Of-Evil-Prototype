using System;
using System.Reflection;

namespace NGTools
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
	}
}