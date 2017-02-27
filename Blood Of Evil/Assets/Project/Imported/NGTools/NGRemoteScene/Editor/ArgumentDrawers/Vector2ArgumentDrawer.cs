using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Vector2))]
	internal sealed class Vector2ArgumentDrawer : ArgumentDrawer
	{
		public Vector2ArgumentDrawer(string name, Type type) : base(name, typeof(Vector2))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.Vector2Field(this.name, (Vector2)this.value);
		}

		public override void	Save(string path)
		{
			Vector2	c = (Vector2)this.value;

			NGEditorPrefs.SetFloat(path + ".x", (float)c.x);
			NGEditorPrefs.SetFloat(path + ".y", (float)c.y);
		}

		public override void	Load(string path)
		{
			Vector2	v = (Vector2)this.value;

			v.x = NGEditorPrefs.GetFloat(path + ".x");
			v.y = NGEditorPrefs.GetFloat(path + ".y");

			this.value = v;
		}
	}
}