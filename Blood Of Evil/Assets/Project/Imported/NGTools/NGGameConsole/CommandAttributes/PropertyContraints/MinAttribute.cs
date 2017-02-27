using System;

namespace NGTools.NGGameConsole
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MinAttribute : PropertyConstraintAttribute
	{
		public readonly decimal	min;

		public	MinAttribute(int min)
		{
			this.min = min;
		}

		public	MinAttribute(float min)
		{
			this.min = (decimal)min;
		}

		public	MinAttribute(UInt32 min)
		{
			this.min = min;
		}

		public	MinAttribute(Int16 min)
		{
			this.min = min;
		}

		public	MinAttribute(UInt16 min)
		{
			this.min = min;
		}

		public	MinAttribute(Int64 min)
		{
			this.min = min;
		}

		public	MinAttribute(UInt64 min)
		{
			this.min = min;
		}

		public	MinAttribute(double min)
		{
			this.min = (decimal)min;
		}

		public	MinAttribute(decimal min)
		{
			this.min = min;
		}

		public	MinAttribute(byte min)
		{
			this.min = min;
		}

		public	MinAttribute(sbyte min)
		{
			this.min = min;
		}

		public override bool	Check(object value)
		{
			try
			{
				return Convert.ToDecimal(value) >= this.min;
			}
			catch
			{
				return false;
			}
		}

		public override string	GetDescription()
		{
			return "Must be >= " + this.min;
		}
	}
}