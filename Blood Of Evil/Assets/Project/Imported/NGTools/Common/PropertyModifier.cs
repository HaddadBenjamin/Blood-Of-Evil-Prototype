using System;
using System.Reflection;

namespace NGTools
{
	public class PropertyModifier : IFieldModifier
	{
		public Type		Type { get { return this.propertyInfo.PropertyType; } }
		public string	Name { get { return this.propertyInfo.Name; } }
		public bool		IsPublic { get { return this.propertyInfo.CanRead; } }

		private readonly PropertyInfo	propertyInfo;

		public	PropertyModifier(PropertyInfo propertyInfo)
		{
			this.propertyInfo = propertyInfo;
		}

		public void		SetValue(object instance, object value)
		{
			this.propertyInfo.SetValue(instance, value, null);
		}

		public object	GetValue(object instance)
		{
			return this.propertyInfo.GetValue(instance, null);
		}

		public T		GetValue<T>(object instance)
		{
			return (T)this.propertyInfo.GetValue(instance, null);
		}

		public bool		IsDefined(Type type, bool inherit)
		{
			return this.propertyInfo.IsDefined(type, inherit);
		}

		public object[]	GetCustomAttributes(Type type, bool inherit)
		{
			return this.propertyInfo.GetCustomAttributes(type, inherit);
		}
	}
}