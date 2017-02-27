using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefInt64 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Int64);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetString(path, ((Int64)instance).ToString());
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Int64	result;

			if (Int64.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				return result;
			return instance;
		}
	}
}