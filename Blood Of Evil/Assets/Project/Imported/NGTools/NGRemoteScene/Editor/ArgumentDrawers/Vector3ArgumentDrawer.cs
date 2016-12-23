using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Vector3))]
	public class Vector3ArgumentDrawer : ArgumentDrawer
	{
		public Vector3ArgumentDrawer(string name) : base(name, typeof(Vector3))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.Vector3Field(this.name, (Vector3)this.value);
		}

		public override void	Serialize(string path)
		{
			Vector3	c = (Vector3)this.value;

			NGEditorPrefs.SetFloat(path + ".x", (float)c.x);
			NGEditorPrefs.SetFloat(path + ".y", (float)c.y);
			NGEditorPrefs.SetFloat(path + ".z", (float)c.z);
		}

		public override void	Deserialize(string path)
		{
			Vector3	v = (Vector3)this.value;

			v.x = NGEditorPrefs.GetFloat(path + ".x");
			v.y = NGEditorPrefs.GetFloat(path + ".y");
			v.z = NGEditorPrefs.GetFloat(path + ".z");

			this.value = v;
		}
	}
}