using System;

namespace NGToolsEditor
{
	public class EditorPrefBoolean : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Boolean);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetBool(path, (Boolean)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetBool(path, (Boolean)instance);
		}
	}
}