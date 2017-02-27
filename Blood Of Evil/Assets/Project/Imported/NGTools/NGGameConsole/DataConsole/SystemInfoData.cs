using System;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	public class SystemInfoData : DataConsole
	{
		private string	content;

		protected virtual void	Awake()
		{
			this.content = "Unity" + Environment.NewLine;

#if UNITY_EDITOR
			this.content += "ActiveBuildTarget: " + UnityEditor.EditorUserBuildSettings.activeBuildTarget + Environment.NewLine;
#endif

			this.content += "Platform: " + Application.platform + Environment.NewLine +
				"RunInBackground: " + Application.runInBackground + Environment.NewLine +
				"SystemLanguage: " + Application.systemLanguage + Environment.NewLine +
				"UnityVersion: " + Application.unityVersion + Environment.NewLine +
#if UNITY_5
				"Version: " + Application.version + Environment.NewLine + Environment.NewLine +
#endif
				"System" + Environment.NewLine +
				"OperatingSystem: " + SystemInfo.operatingSystem + Environment.NewLine +
				"ProcessorCount: " + SystemInfo.processorCount + Environment.NewLine +
				"ProcessorType: " + SystemInfo.processorType + Environment.NewLine +
				"DeviceModel: " + SystemInfo.deviceModel + Environment.NewLine +
				"DeviceName: " + SystemInfo.deviceName + Environment.NewLine +
				"DeviceType: " + SystemInfo.deviceType + Environment.NewLine +
				"GraphicsDeviceName: " + SystemInfo.graphicsDeviceName + Environment.NewLine +
				"GraphicsDeviceVendor: " + SystemInfo.graphicsDeviceVendor + Environment.NewLine +
				"GraphicsDeviceVendorID: " + SystemInfo.graphicsDeviceVendorID + Environment.NewLine +
				"GraphicsDeviceVersion: " + SystemInfo.graphicsDeviceVersion + Environment.NewLine +
				"GraphicsMemorySize: " + SystemInfo.graphicsMemorySize + Environment.NewLine +
#if UNITY_5
				"GraphicsMultiThreaded: " + SystemInfo.graphicsMultiThreaded + Environment.NewLine +
#endif
				"GraphicsShaderLevel: " + SystemInfo.graphicsShaderLevel + Environment.NewLine +
				"SystemMemorySize: " + SystemInfo.systemMemorySize;
		}

		public override void	FullGUI()
		{
			GUILayout.TextArea(this.content, this.fullStyle);
		}

		public override string	Copy()
		{
			return this.content;
		}
	}
}