using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefString : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(String);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetString(path, (String)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetString(path, (String)instance);
		}
	}
}