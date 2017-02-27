using System;

namespace NGTools.NGGameConsole
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MinMaxAttribute : PropertyConstraintAttribute
	{
		public readonly decimal	min;
		public readonly decimal	max;

		public	MinMaxAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(float min, float max)
		{
			this.min = (decimal)min;
			this.max = (decimal)max;
		}

		public	MinMaxAttribute(UInt32 min, UInt32 max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(Int16 min, Int16 max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(UInt16 min, UInt16 max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(Int64 min, Int64 max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(UInt64 min, UInt64 max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(double min, double max)
		{
			this.min = (decimal)min;
			this.max = (decimal)max;
		}

		public	MinMaxAttribute(decimal min, decimal max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(byte min, byte max)
		{
			this.min = min;
			this.max = max;
		}

		public	MinMaxAttribute(sbyte min, sbyte max)
		{
			this.min = min;
			this.max = max;
		}

		public override bool	Check(object value)
		{
			try
			{
				decimal	v = Convert.ToDecimal(value);
				return this.min <= v && v <= this.max;
			}
			catch
			{
				return false;
			}
		}

		public override string	GetDescription()
		{
			return "Must be >= " + this.min + " and <= " + this.max;
		}
	}
}