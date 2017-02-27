using System;

namespace NGTools.NGRemoteScene
{
	/// <summary>
	/// <para>Represents a custom way to override monitors and implement a specific behaviour for types.</para>
	/// <para>Any class inheriting from CustomMonitorData must implement:.</para>
	/// <para>private static bool	{<see cref="CustomMonitorData.StaticCanHandleMethodName"/>}(Type)</para>
	/// </summary>
	internal abstract class CustomMonitorData : MonitorData
	{
		public const string	StaticCanHandleMethodName = "CanMonitorType";

		protected	CustomMonitorData(string path, Func<object> getInstance) : base(path, getInstance)
		{
		}
	}
}