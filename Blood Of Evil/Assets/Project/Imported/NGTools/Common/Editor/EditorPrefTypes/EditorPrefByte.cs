using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefByte : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Byte);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetInt(path, (Byte)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetInt(path, (Byte)instance);
		}
	}
}