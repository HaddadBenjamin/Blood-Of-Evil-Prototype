using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Int32))]
	public class Int32ArgumentDrawer : ArgumentDrawer
	{
		public Int32ArgumentDrawer(string name) : base(name, typeof(Int32))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.IntField(this.name, (Int32)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetInt(path, (Int32)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (Int32)NGEditorPrefs.GetInt(path);
		}
	}
}