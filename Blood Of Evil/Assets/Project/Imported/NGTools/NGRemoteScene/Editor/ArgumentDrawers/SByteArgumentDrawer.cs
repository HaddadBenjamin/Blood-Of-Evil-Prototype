using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(SByte))]
	public class SByteArgumentDrawer : ArgumentDrawer
	{
		public SByteArgumentDrawer(string name) : base(name, typeof(SByte))
		{
		}

		public override void	OnGUI()
		{
			this.value = (SByte)EditorGUILayout.IntField(this.name, (SByte)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetInt(path, (SByte)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (SByte)NGEditorPrefs.GetInt(path);
		}
	}
}