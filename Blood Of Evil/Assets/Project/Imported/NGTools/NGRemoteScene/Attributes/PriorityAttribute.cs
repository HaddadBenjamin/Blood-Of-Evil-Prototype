using System;

namespace NGTools
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PriorityAttribute : Attribute
	{
		public readonly int	priority;

		/// <param name="priority">Priority nearest to 0 has higher priority.</param>
		public	PriorityAttribute(int priority)
		{
			this.priority = priority;
		}
	}
}