using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefUInt32 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(UInt32);
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