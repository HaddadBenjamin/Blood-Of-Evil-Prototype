using System;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefVector4 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Vector4);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Vector4	value = (Vector4)instance;

			NGEditorPrefs.SetFloat(path + ".x", value.x);
			NGEditorPrefs.SetFloat(path + ".y", value.y);
			NGEditorPrefs.SetFloat(path + ".z", value.z);
			NGEditorPrefs.SetFloat(path + ".w", value.w);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Vector4	value = (Vector4)instance;

			value.x = NGEditorPrefs.GetFloat(path + ".x", value.x);
			value.y = NGEditorPrefs.GetFloat(path + ".y", value.y);
			value.z = NGEditorPrefs.GetFloat(path + ".z", value.z);
			value.w = NGEditorPrefs.GetFloat(path + ".w", value.w);

			return value;
		}
	}
}