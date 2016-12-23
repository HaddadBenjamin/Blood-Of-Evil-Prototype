#if UNITY_5
using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(UInt32))]
	public class UInt32ArgumentDrawer : ArgumentDrawer
	{
		public UInt32ArgumentDrawer(string name) : base(name, typeof(UInt32))
		{
		}

		public override void	OnGUI()
		{
			this.value = (UInt32)EditorGUILayout.LongField(this.name, (UInt32)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetInt(path, (Int32)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (UInt32)NGEditorPrefs.GetInt(path);
		}
	}
}
#endif