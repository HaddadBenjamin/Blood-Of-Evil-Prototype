using System;

namespace NGTools.NGGameConsole
{
	/// <summary>
	/// Thrown when calling GetSetInvoke on an unhandled parameter type.
	/// </summary>
	internal sealed class NotSupportedParameterTypeException : Exception
	{
		public readonly	Type	type;

		public override string Message
		{
			get
			{
				return "Parameter of type \"" + this.type.FullName + "\" is not supported.";
			}
		}

		public	NotSupportedParameterTypeException(Type type)
		{
			this.type = type;
		}
	}
}