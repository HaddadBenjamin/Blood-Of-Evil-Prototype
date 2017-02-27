using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Color))]
	internal sealed class ColorArgumentDrawer : ArgumentDrawer
	{
		public ColorArgumentDrawer(string name, Type type) : base(name, typeof(Color))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.ColorField(this.name, (Color)this.value);
		}

		public override void	Save(string path)
		{
			Color	c = (Color)this.value;

			NGEditorPrefs.SetFloat(path + ".r", (float)c.r);
			NGEditorPrefs.SetFloat(path + ".g", (float)c.g);
			NGEditorPrefs.SetFloat(path + ".b", (float)c.b);
			NGEditorPrefs.SetFloat(path + ".a", (float)c.a);
		}

		public override void	Load(string path)
		{
			Color	v = (Color)this.value;

			v.r = NGEditorPrefs.GetFloat(path + ".r");
			v.g = NGEditorPrefs.GetFloat(path + ".g");
			v.b = NGEditorPrefs.GetFloat(path + ".b");
			v.a = NGEditorPrefs.GetFloat(path + ".a");

			this.value = v;
		}
	}
}