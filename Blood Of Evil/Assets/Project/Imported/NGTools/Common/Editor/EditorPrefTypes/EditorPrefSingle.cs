using System;

namespace NGToolsEditor
{
	internal sealed class EditorPrefSingle : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Single);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			NGEditorPrefs.SetFloat(path, (Single)instance);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			return NGEditorPrefs.GetFloat(path, (Single)instance);
		}
	}
}