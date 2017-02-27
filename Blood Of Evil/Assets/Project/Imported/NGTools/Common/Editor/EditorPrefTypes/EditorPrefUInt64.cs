using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefUInt64 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(UInt64);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetString(path, ((UInt64)instance).ToString());
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			UInt64	result;

			if (UInt64.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				return result;
			return instance;
		}
	}
}