using System;

namespace NGTools.NGGameConsole
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MaxAttribute : PropertyConstraintAttribute
	{
		public readonly decimal	max;

		public	MaxAttribute(int max)
		{
			this.max = max;
		}

		public	MaxAttribute(float max)
		{
			this.max = (decimal)max;
		}

		public	MaxAttribute(UInt32 max)
		{
			this.max = max;
		}

		public	MaxAttribute(Int16 max)
		{
			this.max = max;
		}

		public	MaxAttribute(UInt16 max)
		{
			this.max = max;
		}

		public	MaxAttribute(Int64 max)
		{
			this.max = max;
		}

		public	MaxAttribute(UInt64 max)
		{
			this.max = max;
		}

		public	MaxAttribute(double max)
		{
			this.max = (decimal)max;
		}

		public	MaxAttribute(decimal max)
		{
			this.max = max;
		}

		public	MaxAttribute(byte max)
		{
			this.max = max;
		}

		public	MaxAttribute(sbyte max)
		{
			this.max = max;
		}

		public override bool	Check(object value)
		{
			try
			{
				return Convert.ToDecimal(value) <= this.max;
			}
			catch
			{
				return false;
			}
		}

		public override string	GetDescription()
		{
			return "Must be <= " + this.max;
		}
	}
}