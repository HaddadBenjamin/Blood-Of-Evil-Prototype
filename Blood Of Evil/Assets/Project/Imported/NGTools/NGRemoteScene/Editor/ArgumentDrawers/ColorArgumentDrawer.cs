using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Color))]
	public class ColorArgumentDrawer : ArgumentDrawer
	{
		public ColorArgumentDrawer(string name) : base(name, typeof(Color))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.ColorField(this.name, (Color)this.value);
		}

		public override void	Serialize(string path)
		{
			Color	c = (Color)this.value;

			NGEditorPrefs.SetFloat(path + ".r", (float)c.r);
			NGEditorPrefs.SetFloat(path + ".g", (float)c.g);
			NGEditorPrefs.SetFloat(path + ".b", (float)c.b);
			NGEditorPrefs.SetFloat(path + ".a", (float)c.a);
		}

		public override void	Deserialize(string path)
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