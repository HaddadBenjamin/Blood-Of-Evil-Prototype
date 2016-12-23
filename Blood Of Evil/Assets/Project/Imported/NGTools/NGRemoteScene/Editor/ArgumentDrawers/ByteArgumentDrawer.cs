using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Byte))]
	public class ByteArgumentDrawer : ArgumentDrawer
	{
		public ByteArgumentDrawer(string name) : base(name, typeof(Byte))
		{
		}

		public override void	OnGUI()
		{
			this.value = (Byte)EditorGUILayout.IntField(this.name, (Byte)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetInt(path, (Byte)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (Byte)NGEditorPrefs.GetInt(path);
		}
	}
}