using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Vector2))]
	public class Vector2ArgumentDrawer : ArgumentDrawer
	{
		public Vector2ArgumentDrawer(string name) : base(name, typeof(Vector2))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.Vector2Field(this.name, (Vector2)this.value);
		}

		public override void	Serialize(string path)
		{
			Vector2	c = (Vector2)this.value;

			NGEditorPrefs.SetFloat(path + ".x", (float)c.x);
			NGEditorPrefs.SetFloat(path + ".y", (float)c.y);
		}

		public override void	Deserialize(string path)
		{
			Vector2	v = (Vector2)this.value;

			v.x = NGEditorPrefs.GetFloat(path + ".x");
			v.y = NGEditorPrefs.GetFloat(path + ".y");

			this.value = v;
		}
	}
}