using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Int32))]
	internal sealed class Int32ArgumentDrawer : ArgumentDrawer
	{
		public Int32ArgumentDrawer(string name, Type type) : base(name, typeof(Int32))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.IntField(this.name, (Int32)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (Int32)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (Int32)NGEditorPrefs.GetInt(path);
		}
	}
}