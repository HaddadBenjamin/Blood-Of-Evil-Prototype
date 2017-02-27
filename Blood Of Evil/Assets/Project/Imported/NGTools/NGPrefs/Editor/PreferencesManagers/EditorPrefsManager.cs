using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGPrefs
{
	internal sealed class EditorPrefManager : PrefsManager
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
			if (Application.platform == RuntimePlatform.WindowsEditor)
				this.LoadFromRegistrar(@"SOFTWARE\Unity Technologies\Unity Editor 5.x");
			else if (Application.platform == RuntimePlatform.OSXEditor)
				this.LoadFromRegistrar("/Users/Apple/Library/Preferences/com.unity3d.UnityEditor" + Application.unityVersion.Substring(0, Application.unityVersion.IndexOf('.')) + ".x.plist");
			else
				Debug.LogError("EditorPrefsManager does not support the current platform " + Application.platform + ".");
		}
	}
}