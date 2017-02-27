using System;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefRect : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Rect);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Rect	value = (Rect)instance;

			NGEditorPrefs.SetFloat(path + ".x", value.x);
			NGEditorPrefs.SetFloat(path + ".y", value.y);
			NGEditorPrefs.SetFloat(path + ".w", value.width);
			NGEditorPrefs.SetFloat(path + ".h", value.height);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Rect	value = (Rect)instance;

			value.x = NGEditorPrefs.GetFloat(path + ".x", value.x);
			value.y = NGEditorPrefs.GetFloat(path + ".y", value.y);
			value.width = NGEditorPrefs.GetFloat(path + ".w", value.width);
			value.height = NGEditorPrefs.GetFloat(path + ".h", value.height);

			return value;
		}
	}
}