using System;

namespace NGTools.NGGameConsole
{
	public abstract class PropertyConstraintAttribute : Attribute
	{
		public abstract bool	Check(object value);
		public abstract string	GetDescription();
	}
}