using System;

namespace NGTools
{
	/// <summary>Provides property and method to get a type and a value.</summary>
	public interface IValueGetter
	{
		Type	Type { get; }

		T			GetValue<T>(object instance);
		bool		IsDefined(Type type, bool inherit);
		object[]	GetCustomAttributes(Type type, bool inherit);
	}
}