using System;
using System.Reflection;

namespace NGTools.NGGameConsole
{
	public class MethodCommand : CommandNode
	{
		public override bool	IsLeaf { get { return true; } }

		private MethodInfo			methodInfo;
		private ParameterInfo[]		parameters;

		public	MethodCommand(CommandAttribute attribute, MethodInfo m, object instance) : base(instance, attribute.name, attribute.description)
		{
			this.methodInfo = m;
			this.parameters = this.methodInfo.GetParameters();
		}

		public override string	GetSetInvoke(params string[] args)
		{
			// Check arguments count.
			if (this.parameters.Length != args.Length)
				return "Not matching arguments.";

			try
			{
				// Convert arguments.
				object[]	arguments = new object[this.parameters.Length];

				for (int i = 0; i < this.parameters.Length; i++)
					arguments[i] = this.GetValueFromType(this.parameters[i], args[i]);

				return this.methodInfo.Invoke(this.instance, arguments).ToString();
			}
			catch
			{
				return "Invoke failed.";
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="pi"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="NotSupportedParameterTypeException">Thrown when a parameter is of an unsupported type.</exception>
		/// <exception cref="ArgumentException">Thrown when a property is assigned with an invalid boolean.</exception>
		private object	GetValueFromType(ParameterInfo pi, string value)
		{
			if (pi.ParameterType == typeof(Int32))
				return Int32.Parse(value);
			else if (pi.ParameterType == typeof(Single))
				return Single.Parse(value);
			else if (pi.ParameterType == typeof(String))
				return value;
			else if (pi.ParameterType == typeof(Boolean))
			{
				if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase) == true)
					return true;
				else if (value.Equals("false", StringComparison.CurrentCultureIgnoreCase) == true)
					return false;
				else
					throw new ArgumentException("Value \"" + value + "\" is not a valid boolean (True or False case insensitive).");
			}
			else if (pi.ParameterType.IsEnum == true)
				return Enum.Parse(pi.ParameterType, value, true);
			else if (pi.ParameterType == typeof(Char))
				return Char.Parse(value);
			else if (pi.ParameterType == typeof(UInt32))
				return UInt32.Parse(value);
			else if (pi.ParameterType == typeof(Int16))
				return Int16.Parse(value);
			else if (pi.ParameterType == typeof(UInt16))
				return UInt16.Parse(value);
			else if (pi.ParameterType == typeof(Int64))
				return Int64.Parse(value);
			else if (pi.ParameterType == typeof(UInt64))
				return UInt64.Parse(value);
			else if (pi.ParameterType == typeof(Double))
				return Double.Parse(value);
			else if (pi.ParameterType == typeof(Decimal))
				return Decimal.Parse(value);
			else if (pi.ParameterType == typeof(Byte))
				return Byte.Parse(value);
			else if (pi.ParameterType == typeof(SByte))
				return SByte.Parse(value);

			throw new NotSupportedParameterTypeException(pi.ParameterType);
		}
	}
}