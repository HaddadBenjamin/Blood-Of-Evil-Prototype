using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefSByte : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(SByte);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetInt(path, (SByte)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetInt(path, (SByte)instance);
		}
	}
}