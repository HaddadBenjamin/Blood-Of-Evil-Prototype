using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(UInt16))]
	internal sealed class UInt16ArgumentDrawer : ArgumentDrawer
	{
		public UInt16ArgumentDrawer(string name, Type type) : base(name, typeof(UInt16))
		{
		}

		public override void	OnGUI()
		{
			this.value = (UInt16)EditorGUILayout.IntField(this.name, (UInt16)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (UInt16)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (UInt16)NGEditorPrefs.GetInt(path);
		}
	}
}