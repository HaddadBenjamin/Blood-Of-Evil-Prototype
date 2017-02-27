#if UNITY_5
using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(UInt32))]
	internal sealed class UInt32ArgumentDrawer : ArgumentDrawer
	{
		public UInt32ArgumentDrawer(string name, Type type) : base(name, typeof(UInt32))
		{
		}

		public override void	OnGUI()
		{
			this.value = (UInt32)EditorGUILayout.LongField(this.name, (UInt32)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (Int32)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (UInt32)NGEditorPrefs.GetInt(path);
		}
	}
}
#endif