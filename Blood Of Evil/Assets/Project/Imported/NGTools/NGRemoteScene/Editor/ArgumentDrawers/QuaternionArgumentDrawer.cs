using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Quaternion))]
	internal sealed class QuaternionArgumentDrawer : ArgumentDrawer
	{
		private Vector4	v;

		public QuaternionArgumentDrawer(string name, Type type) : base(name, typeof(Quaternion))
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

		public override void	Save(string path)
		{
			Quaternion	c = (Quaternion)this.value;

			NGEditorPrefs.SetFloat(path + ".x", (float)c.x);
			NGEditorPrefs.SetFloat(path + ".y", (float)c.y);
			NGEditorPrefs.SetFloat(path + ".z", (float)c.z);
			NGEditorPrefs.SetFloat(path + ".w", (float)c.w);
		}

		public override void	Load(string path)
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