using System;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class MainModuleSettings
		{
			[LocaleHeader("NGSettings_MainModule_AlertOnWarning")]
			public bool		alertOnWarning;
			[LocaleHeader("NGSettings_MainModule_WarningColor")]
			public Color	warningColor = new Color(90F / 255F, 255F / 255F, 0F / 255F);
			[LocaleHeader("NGSettings_MainModule_ErrorColor")]
			public Color	errorColor = new Color(236F / 255F, 0F / 255F, 0F / 255F);
		}
		public MainModuleSettings	mainModule = new MainModuleSettings();
	}
}