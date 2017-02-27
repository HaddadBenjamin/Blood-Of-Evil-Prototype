using System;

namespace NGTools.NGGameConsole
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MinLengthAttribute : PropertyConstraintAttribute
	{
		public readonly float	length;

		public	MinLengthAttribute(int length)
		{
			this.length = length;
		}

		public override bool	Check(object value)
		{
			return value is string && ((string)value).Length >= this.length;
		}

		public override string	GetDescription()
		{
			return "String can not have less than " + this.length + " chars.";
		}
	}
}