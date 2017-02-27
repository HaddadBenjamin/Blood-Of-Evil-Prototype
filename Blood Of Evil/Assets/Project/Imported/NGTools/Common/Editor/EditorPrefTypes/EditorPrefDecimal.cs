using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefDecimal : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Decimal);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetString(path, ((Decimal)instance).ToString());
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Decimal	result;

			if (Decimal.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				return result;
			return instance;
		}
	}
}