using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(SByte))]
	internal sealed class SByteArgumentDrawer : ArgumentDrawer
	{
		public SByteArgumentDrawer(string name, Type type) : base(name, typeof(SByte))
		{
		}

		public override void	OnGUI()
		{
			this.value = (SByte)EditorGUILayout.IntField(this.name, (SByte)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (SByte)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (SByte)NGEditorPrefs.GetInt(path);
		}
	}
}