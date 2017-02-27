using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefInt16 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Int16);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetInt(path, (Int16)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetInt(path, (Int16)instance);
		}
	}
}