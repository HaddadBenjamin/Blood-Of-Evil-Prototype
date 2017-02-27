using System;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefVector2 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Vector2);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Vector2	value = (Vector2)instance;

			NGEditorPrefs.SetFloat(path + ".x", value.x);
			NGEditorPrefs.SetFloat(path + ".y", value.y);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Vector2	value = (Vector2)instance;

			value.x = NGEditorPrefs.GetFloat(path + ".x", value.x);
			value.y = NGEditorPrefs.GetFloat(path + ".y", value.y);

			return value;
		}
	}
}