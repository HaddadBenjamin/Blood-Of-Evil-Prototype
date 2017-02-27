using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Rect))]
	internal sealed class RectArgumentDrawer : ArgumentDrawer
	{
		public RectArgumentDrawer(string name, Type type) : base(name, typeof(Rect))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.RectField(this.name, (Rect)this.value);
		}

		public override void	Save(string path)
		{
			Rect	c = (Rect)this.value;

			NGEditorPrefs.SetFloat(path + ".x", (float)c.x);
			NGEditorPrefs.SetFloat(path + ".y", (float)c.y);
			NGEditorPrefs.SetFloat(path + ".w", (float)c.width);
			NGEditorPrefs.SetFloat(path + ".h", (float)c.height);
		}

		public override void	Load(string path)
		{
			Rect	v = (Rect)this.value;

			v.x = NGEditorPrefs.GetFloat(path + ".x");
			v.y = NGEditorPrefs.GetFloat(path + ".y");
			v.width = NGEditorPrefs.GetFloat(path + ".w");
			v.height = NGEditorPrefs.GetFloat(path + ".h");

			this.value = v;
		}
	}
}