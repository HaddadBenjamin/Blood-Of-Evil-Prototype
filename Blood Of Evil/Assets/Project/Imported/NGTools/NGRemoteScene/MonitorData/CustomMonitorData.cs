using System;

namespace NGTools
{
	/// <summary>
	/// <para>Represents a custom way to override monitors and implement a specific behaviour for types.</para>
	/// <para>Any class inheriting from CustomMonitorData must implement:.</para>
	/// <para>private static bool	{<see cref="CustomMonitorData.StaticCanHandleMethodName"/>}(Type)</para>
	/// </summary>
	public abstract class CustomMonitorData : MonitorData
	{
		public const string	StaticCanHandleMethodName = "CanHandle";

		protected	CustomMonitorData(string path, Func<object> getInstance) : base(path, getInstance)
		{
		}
	}
}