using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Vector4))]
	internal sealed class Vector4ArgumentDrawer : ArgumentDrawer
	{
		public Vector4ArgumentDrawer(string name, Type type) : base(name, typeof(Vector4))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.Vector4Field(this.name, (Vector4)this.value);
		}

		public override void	Save(string path)
		{
			Vector4	c = (Vector4)this.value;

			NGEditorPrefs.SetFloat(path + ".x", (float)c.x);
			NGEditorPrefs.SetFloat(path + ".y", (float)c.y);
			NGEditorPrefs.SetFloat(path + ".z", (float)c.z);
			NGEditorPrefs.SetFloat(path + ".w", (float)c.w);
		}

		public override void	Load(string path)
		{
			Vector4	v = (Vector4)this.value;

			v.x = NGEditorPrefs.GetFloat(path + ".x");
			v.y = NGEditorPrefs.GetFloat(path + ".y");
			v.z = NGEditorPrefs.GetFloat(path + ".z");
			v.w = NGEditorPrefs.GetFloat(path + ".w");

			this.value = v;
		}
	}
}