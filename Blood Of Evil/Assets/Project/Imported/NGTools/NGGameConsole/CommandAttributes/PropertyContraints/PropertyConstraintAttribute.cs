using System;

namespace NGTools
{
	public abstract class PropertyConstraintAttribute : Attribute
	{
		public abstract bool	Check(object value);
		public abstract string	GetDescription();
	}
}