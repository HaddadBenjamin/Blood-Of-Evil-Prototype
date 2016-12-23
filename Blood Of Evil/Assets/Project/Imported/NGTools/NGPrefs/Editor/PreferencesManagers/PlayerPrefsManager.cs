using NGTools;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class PlayerPrefManager : PrefsManager
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
			try
			{
#if UNITY_EDITOR_WIN
				this.LoadFromRegistrar(@"SOFTWARE\" + PlayerSettings.companyName + @"\" + PlayerSettings.productName);
#elif UNITY_EDITOR_OSX
				this.LoadFromRegistrar("/Users/Apple/Library/Preferences/unity." + Application.companyName + "." + Application.productName + ".plist");
#endif
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}
	}
}