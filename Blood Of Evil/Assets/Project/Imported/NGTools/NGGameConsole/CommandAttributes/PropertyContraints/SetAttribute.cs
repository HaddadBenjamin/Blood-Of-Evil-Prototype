using System;

namespace NGTools.NGGameConsole
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SetAttribute : PropertyConstraintAttribute
	{
		public readonly int[]	values;

		private string	valuesStringified = null;

		public	SetAttribute(params int[] values)
		{
			this.values = values;
		}

		public override bool	Check(object value)
		{
			return value is int && this.AllowValue((int)value) == true;
		}

		public override string	GetDescription()
		{
			if (this.valuesStringified == null)
			{
				Utility.sharedBuffer.Length = 0;

				for (int i = 0; i < this.values.Length; i++)
				{
					Utility.sharedBuffer.Append(this.values[i]);
					Utility.sharedBuffer.Append(',');
				}

				Utility.sharedBuffer.Length -= 1;

				this.valuesStringified = Utility.sharedBuffer.ToString();
			}

			return "Value must be one of the following: " + this.valuesStringified + ".";
		}

		private bool	AllowValue(int value)
		{
			for (int i = 0; i < this.values.Length; i++)
			{
				if (this.values[i] == value)
					return true;
			}

			return false;
		}
	}
}