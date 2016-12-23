using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Quaternion))]
	public class QuaternionArgumentDrawer : ArgumentDrawer
	{
		private Vector4	v;

		public QuaternionArgumentDrawer(string name) : base(name, typeof(Quaternion))
		{
		}

		public override void	OnGUI()
		{
			Quaternion	q = (Quaternion)this.value;

			this.v.x = q.x;
			this.v.y = q.y;
			this.v.z = q.z;
			this.v.w = q.w;

			EditorGUI.BeginChangeCheck();
			this.v = EditorGUILayout.Vector4Field(this.name, this.v);
			if (EditorGUI.EndChangeCheck() == true)
			{
				q.x = this.v.x;
				q.y = this.v.y;
				q.z = this.v.z;
				q.w = this.v.w;

				this.value = q;
			}
		}

		public override void	Serialize(string path)
		{
			Quaternion	c = (Quaternion)this.value;

			NGEditorPrefs.SetFloat(path + ".x", (float)c.x);
			NGEditorPrefs.SetFloat(path + ".y", (float)c.y);
			NGEditorPrefs.SetFloat(path + ".z", (float)c.z);
			NGEditorPrefs.SetFloat(path + ".w", (float)c.w);
		}

		public override void	Deserialize(string path)
		{
			Quaternion	v = (Quaternion)this.value;

			v.x = NGEditorPrefs.GetFloat(path + ".x");
			v.y = NGEditorPrefs.GetFloat(path + ".y");
			v.z = NGEditorPrefs.GetFloat(path + ".z");
			v.w = NGEditorPrefs.GetFloat(path + ".w");

			this.value = v;
		}
	}
}