using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Int16))]
	internal sealed class Int16ArgumentDrawer : ArgumentDrawer
	{
		public Int16ArgumentDrawer(string name, Type type) : base(name, typeof(Int16))
		{
		}

		public override void	OnGUI()
		{
			this.value = (Int16)EditorGUILayout.IntField(this.name, (Int32)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (Int16)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (Int16)NGEditorPrefs.GetInt(path);
		}
	}
}