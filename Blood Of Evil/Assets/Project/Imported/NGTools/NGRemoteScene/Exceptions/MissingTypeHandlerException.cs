using System;

namespace NGTools.NGRemoteScene
{
	/// <summary>
	/// Thrown when trying to fetch a non-implemented TypeHandler.
	/// </summary>
	internal sealed class MissingTypeHandlerException : Exception
	{
		public readonly	Type	type;

		public override string Message
		{
			get
			{
				return "TypeHandler of type \"" + this.type.FullName + "\" is not supported.";
			}
		}

		/// <summary>
		/// Thrown when trying to fetch a non-implemented TypeHandler.
		/// </summary>
		public	MissingTypeHandlerException(Type type)
		{
			this.type = type;
		}
	}
}