using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefDouble : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Double);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetString(path, ((Double)instance).ToString());
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Double	result;

			if (Double.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				return result;
			return instance;
		}
	}
}