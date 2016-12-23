using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(UInt16))]
	public class UInt16ArgumentDrawer : ArgumentDrawer
	{
		public UInt16ArgumentDrawer(string name) : base(name, typeof(UInt16))
		{
		}

		public override void	OnGUI()
		{
			this.value = (UInt16)EditorGUILayout.IntField(this.name, (UInt16)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetInt(path, (UInt16)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (UInt16)NGEditorPrefs.GetInt(path);
		}
	}
}