using NGTools;
using System;
using UnityEditor;
#if UNITY_EDITOR_OSX
using UnityEngine;
#endif

namespace NGToolsEditor
{
	public class EditorPrefManager : PrefsManager
	{
		public override void	DeleteKey(string key)
		{
			EditorPrefs.DeleteKey(key);
		}

		public override void	DeleteAll()
		{
			EditorPrefs.DeleteAll();
		}

		public override bool	HasKey(string key)
		{
			return EditorPrefs.HasKey(key);
		}

		public override float	GetFloat(string key, float defaultValue = 0F)
		{
			return EditorPrefs.GetFloat(key, defaultValue);
		}

		public override int		GetInt(string key, int defaultValue = 0)
		{
			return EditorPrefs.GetInt(key, defaultValue);
		}

		public override string	GetString(string key, string defaultValue = null)
		{
			return EditorPrefs.GetString(key, defaultValue);
		}

		public override void	SetFloat(string key, float value)
		{
			EditorPrefs.SetFloat(key, value);
		}

		public override void	SetInt(string key, int value)
		{
			EditorPrefs.SetInt(key, value);
		}

		public override void	SetString(string key, string value)
		{
			EditorPrefs.SetString(key, value);
		}

		public override void	LoadPreferences()
		{
			try
			{
#if UNITY_EDITOR_WIN
				this.LoadFromRegistrar(@"SOFTWARE\Unity Technologies\Unity Editor 5.x");
#elif UNITY_EDITOR_OSX
				this.LoadFromRegistrar("/Users/Apple/Library/Preferences/com.unity3d.UnityEditor" + Application.unityVersion.Substring(0, Application.unityVersion.IndexOf('.')) + ".x.plist");
#endif
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}
	}
}