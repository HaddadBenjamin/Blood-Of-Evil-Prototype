using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Int16))]
	public class Int16ArgumentDrawer : ArgumentDrawer
	{
		public Int16ArgumentDrawer(string name) : base(name, typeof(Int16))
		{
		}

		public override void	OnGUI()
		{
			this.value = (Int16)EditorGUILayout.IntField(this.name, (Int32)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetInt(path, (Int16)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (Int16)NGEditorPrefs.GetInt(path);
		}
	}
}