using System;
using System.Reflection;

namespace NGToolsEditor
{
	/// <summary>
	/// <para>Any class having this attribute must implement:</para>
	/// <para>private static bool {<see cref="RowLogHandleAttribute.StaticMethodName"/>}(UnityLogEntry).</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class RowLogHandlerAttribute : Attribute
	{
		public const string	StaticMethodName = "CanDealWithIt";

		public readonly int	priority;

		public MethodBase	handler;

		/// <param name="priority">The higher the bigger priority.</param>
		public	RowLogHandlerAttribute(int priority)
		{
			this.priority = priority;
		}
	}
}