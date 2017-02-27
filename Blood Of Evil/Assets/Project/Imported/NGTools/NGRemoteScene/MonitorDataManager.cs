using System;
using System.Collections.Generic;
using System.Reflection;

namespace NGTools.NGRemoteScene
{
	internal static class MonitorDataManager
	{
		private static readonly Dictionary<MethodInfo, Type>	types;
		private static readonly object[]						cachedCanHandleArgument;

		static	MonitorDataManager()
		{
			MonitorDataManager.types = new Dictionary<MethodInfo, Type>();

			foreach (Type t in Utility.EachSubClassesOf(typeof(CustomMonitorData)))
				MonitorDataManager.types.Add(t.GetMethod(CustomMonitorData.StaticCanHandleMethodName, BindingFlags.Static | BindingFlags.NonPublic), t);

			MonitorDataManager.cachedCanHandleArgument = new object[1];
		}

		/// <summary>Creates an instance of a CustomMonitorData if one handles <paramref name="targetType"/>.</summary>
		/// <param name="targetType"></param>
		/// <param name="path"></param>
		/// <param name="instance"></param>
		/// <param name="fieldInfo"></param>
		/// <returns>An instance of CustomMonitorData when available, otherwise null.</returns>
		public static CustomMonitorData	CreateMonitorData(Type targetType, string path, Func<object> getInstance, IValueGetter fieldInfo)
		{
			MonitorDataManager.cachedCanHandleArgument[0] = targetType;

			try
			{
				foreach (var item in MonitorDataManager.types)
				{
					if ((bool)item.Key.Invoke(null, MonitorDataManager.cachedCanHandleArgument) == true)
						return Activator.CreateInstance(item.Value, path, getInstance, fieldInfo) as CustomMonitorData;
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("TargetType=" + targetType + Environment.NewLine + "Path=" + path, ex);
			}

			return null;
		}
	}
}