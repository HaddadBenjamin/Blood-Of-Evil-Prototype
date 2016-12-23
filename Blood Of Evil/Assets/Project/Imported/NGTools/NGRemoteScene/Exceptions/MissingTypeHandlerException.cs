using System;

namespace NGTools
{
	/// <summary>
	/// Thrown when trying to fetch a non-implemented TypeHandler.
	/// </summary>
	public class MissingTypeHandlerException : Exception
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