using System;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefColor : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Color);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Color	value = (Color)instance;

			NGEditorPrefs.SetFloat(path + ".r", value.r);
			NGEditorPrefs.SetFloat(path + ".g", value.g);
			NGEditorPrefs.SetFloat(path + ".b", value.b);
			NGEditorPrefs.SetFloat(path + ".a", value.a);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Color	value = (Color)instance;

			value.r = NGEditorPrefs.GetFloat(path + ".r", value.r);
			value.g = NGEditorPrefs.GetFloat(path + ".g", value.g);
			value.b = NGEditorPrefs.GetFloat(path + ".b", value.b);
			value.a = NGEditorPrefs.GetFloat(path + ".a", value.a);

			return value;
		}
	}
}