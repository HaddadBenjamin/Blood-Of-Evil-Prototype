using System;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefQuaternion : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(Quaternion);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Quaternion	value = (Quaternion)instance;

			NGEditorPrefs.SetFloat(path + ".x", value.x);
			NGEditorPrefs.SetFloat(path + ".y", value.y);
			NGEditorPrefs.SetFloat(path + ".z", value.z);
			NGEditorPrefs.SetFloat(path + ".w", value.w);
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			Quaternion	value = (Quaternion)instance;

			value.x = NGEditorPrefs.GetFloat(path + ".x", value.x);
			value.y = NGEditorPrefs.GetFloat(path + ".y", value.y);
			value.z = NGEditorPrefs.GetFloat(path + ".z", value.z);
			value.w = NGEditorPrefs.GetFloat(path + ".w", value.w);

			return value;
		}
	}
}