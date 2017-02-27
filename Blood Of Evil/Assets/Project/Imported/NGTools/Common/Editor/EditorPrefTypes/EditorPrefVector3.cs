using System;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefVector3 : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Vector3);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Vector3	value = (Vector3)instance;

			NGEditorPrefs.SetFloat(path + ".x", value.x);
			NGEditorPrefs.SetFloat(path + ".y", value.y);
			NGEditorPrefs.SetFloat(path + ".z", value.z);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Vector3	value = (Vector3)instance;

			value.x = NGEditorPrefs.GetFloat(path + ".x", value.x);
			value.y = NGEditorPrefs.GetFloat(path + ".y", value.y);
			value.z = NGEditorPrefs.GetFloat(path + ".z", value.z);

			return value;
		}
	}
}