using UnityEditor;

namespace NGToolsEditor
{
	public sealed class NGEditorPrefs
	{
		public static string	GetPerProjectPrefix()
		{
			return PlayerSettings.productName + '.';
		}

		public static bool		GetBool(string key, bool defaultValue = false, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			return EditorPrefs.GetBool(key, defaultValue);
		}

		public static int		GetInt(string key, int defaultValue = 0, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			return EditorPrefs.GetInt(key, defaultValue);
		}

		public static float		GetFloat(string key, float defaultValue = 0F, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			return EditorPrefs.GetFloat(key, defaultValue);
		}

		public static string	GetString(string key, string defaultValue = "", bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			return EditorPrefs.GetString(key, defaultValue);
		}

		public static void		SetBool(string key, bool value, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			EditorPrefs.SetBool(key, value);
		}

		public static void		SetInt(string key, int value, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			EditorPrefs.SetInt(key, value);
		}

		public static void		SetFloat(string key, float value, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			EditorPrefs.SetFloat(key, value);
		}

		public static void		SetString(string key, string value, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			EditorPrefs.SetString(key, value);
		}

		public static void	DeleteKey(string key, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			EditorPrefs.DeleteKey(key);
		}

		public static bool	HasKey(string key, bool perProject = false)
		{
			if (perProject == true)
				key = NGEditorPrefs.GetPerProjectPrefix() + key;
			return EditorPrefs.HasKey(key);
		}
	}
}