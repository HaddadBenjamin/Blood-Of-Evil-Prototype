using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefUInt16 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(UInt16);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetInt(path, (UInt16)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetInt(path, (UInt16)instance);
		}
	}
}