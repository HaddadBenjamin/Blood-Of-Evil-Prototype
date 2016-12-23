using System;

namespace NGTools
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class MaxLengthAttribute : PropertyConstraintAttribute
	{
		public readonly float	length;

		public	MaxLengthAttribute(int length)
		{
			this.length = length;
		}

		public override bool	Check(object value)
		{
			return value is string && ((string)value).Length <= this.length;
		}

		public override string	GetDescription()
		{
			return "String can not have more than " + this.length + " chars.";
		}
	}
}