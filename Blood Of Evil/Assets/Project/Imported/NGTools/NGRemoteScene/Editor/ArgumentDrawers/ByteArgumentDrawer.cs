using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Byte))]
	internal sealed class ByteArgumentDrawer : ArgumentDrawer
	{
		public ByteArgumentDrawer(string name, Type type) : base(name, typeof(Byte))
		{
		}

		public override void	OnGUI()
		{
			this.value = (Byte)EditorGUILayout.IntField(this.name, (Byte)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (Byte)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (Byte)NGEditorPrefs.GetInt(path);
		}
	}
}