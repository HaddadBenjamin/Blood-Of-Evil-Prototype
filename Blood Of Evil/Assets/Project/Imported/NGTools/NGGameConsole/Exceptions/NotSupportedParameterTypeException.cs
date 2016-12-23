using System;
using System.Reflection;

namespace NGTools
{
	/// <summary>
	/// Thrown when calling GetSetInvoke on an unhandled property type.
	/// </summary>
	public class NotSupportedPropertyTypeException : Exception
	{
		public readonly	PropertyInfo	type;

		public override string	Message
		{
			get
			{
				return "Property \"" + this.type.Name + "\" of type \"" + this.type.PropertyType.Name + "\" is not supported.";
			}
		}

		public	NotSupportedPropertyTypeException(PropertyInfo type)
		{
			this.type = type;
		}
	}
}