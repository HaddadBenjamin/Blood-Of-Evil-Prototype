using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefInt32Enum : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Int32) || type.IsEnum == true;
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetInt(path, (Int32)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetInt(path, (Int32)instance);
		}
	}
}