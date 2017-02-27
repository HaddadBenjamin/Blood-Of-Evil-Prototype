using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefChar : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Char);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetInt(path, (Char)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetInt(path, (Char)instance);
		}
	}
}