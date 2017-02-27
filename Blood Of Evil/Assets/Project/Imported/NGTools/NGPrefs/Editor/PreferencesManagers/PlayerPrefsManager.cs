using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGPrefs
{
	internal sealed class PlayerPrefManager : PrefsManager
	{
		public override void	DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}

		public override void	DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public override bool	HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public override float	GetFloat(string key, float defaultValue = 0F)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}

		public override int		GetInt(string key, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}

		public override string	GetString(string key, string defaultValue = null)
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}

		public override void	SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public override void	SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public override void	SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public override void	LoadPreferences()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
				this.LoadFromRegistrar(@"SOFTWARE\" + PlayerSettings.companyName + @"\" + PlayerSettings.productName);
			else if (Application.platform == RuntimePlatform.OSXEditor)
				this.LoadFromRegistrar("/Users/Apple/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist");
			else
				Debug.LogError("PlayerPrefsManager does not support the current platform " + Application.platform + ".");
		}
	}
}