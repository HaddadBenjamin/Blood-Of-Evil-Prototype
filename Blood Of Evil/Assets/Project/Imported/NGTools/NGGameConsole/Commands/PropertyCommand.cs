using System;
using System.Reflection;

namespace NGTools.NGGameConsole
{
	public class PropertyCommand : CommandNode
	{
		public override bool	IsLeaf { get { return true; } }

		private PropertyInfo					propertyInfo;
		private PropertyConstraintAttribute[]	constraints;

		public	PropertyCommand(CommandAttribute attribute, PropertyInfo p, object instance) : base(instance, attribute.name, attribute.description)
		{
			this.propertyInfo = p;
			this.constraints = p.GetCustomAttributes(typeof(PropertyConstraintAttribute), true) as PropertyConstraintAttribute[];

			if (this.propertyInfo.PropertyType.IsClass == true &&
				this.propertyInfo.PropertyType != typeof(string))
				throw new NotSupportedPropertyTypeException(this.propertyInfo);
		}

		/// <summary>
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		/// <exception cref="NGTools.NotSupportedPropertyTypeException">Thrown when a property is of an unsupported type.</exception>
		public override string	GetSetInvoke(params string[] args)
		{
			if (args.Length == 0)
			{
				object	value = this.propertyInfo.GetValue(this.instance, null);

				if (value != null)
					return value.ToString();
				return string.Empty;
			}
			else if (args.Length != 1)
				return "Too many arguments.";

			if (this.propertyInfo.PropertyType == typeof(Int32))
			{
				Int32	n;
				if (Int32.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(Single))
			{
				Single	n;
				if (Single.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(String))
				return this.SetValue(args[0]);
			else if (this.propertyInfo.PropertyType == typeof(Boolean))
			{
				// No constraints possible on boolean.
				if (args[0].Equals("true", StringComparison.CurrentCultureIgnoreCase) == true)
					this.propertyInfo.SetValue(this.instance, true, null);
				else if (args[0].Equals("false", StringComparison.CurrentCultureIgnoreCase) == true)
					this.propertyInfo.SetValue(this.instance, false, null);
				return this.propertyInfo.GetValue(this.instance, null).ToString();
			}
			else if (this.propertyInfo.PropertyType.IsEnum == true)
			{
				try
				{
					return this.SetValue(Enum.Parse(this.propertyInfo.PropertyType, args[0], true));
				}
				catch
				{
					return "Wrong enum. Values available: " + string.Join(", ", Enum.GetNames(this.propertyInfo.PropertyType)) + ".";
				}
			}
			else if (this.propertyInfo.PropertyType == typeof(Char))
			{
				Char	n;
				if (Char.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(UInt32))
			{
				UInt32	n;
				if (UInt32.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(Int16))
			{
				Int16	n;
				if (Int16.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(UInt16))
			{
				UInt16	n;
				if (UInt16.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(Int64))
			{
				Int64	n;
				if (Int64.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(UInt64))
			{
				UInt64	n;
				if (UInt64.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(Double))
			{
				Double	n;
				if (Double.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(Decimal))
			{
				Decimal	n;
				if (Decimal.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(Byte))
			{
				Byte	n;
				if (Byte.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else if (this.propertyInfo.PropertyType == typeof(SByte))
			{
				SByte	n;
				if (SByte.TryParse(args[0], out n) == true)
					return this.SetValue(n);
			}
			else
				throw new NotSupportedPropertyTypeException(this.propertyInfo);

			return "Invalid value.";
		}

		private string	SetValue(object n)
		{
			string	error = this.CheckValueConstraints(n);

			if (error != null)
				return error;
			this.propertyInfo.SetValue(this.instance, n, null);
			return this.propertyInfo.GetValue(this.instance, null).ToString();
		}

		private string	CheckValueConstraints(object value)
		{
			for (int i = 0; i < this.constraints.Length; i++)
			{
				if (this.constraints[i].Check(value) == false)
					return this.constraints[i].GetDescription();
			}

			return null;
		}
	}
}