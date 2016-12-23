#if UNITY_5
using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(UInt64))]
	public class UInt64ArgumentDrawer : ArgumentDrawer
	{
		public UInt64ArgumentDrawer(string name) : base(name, typeof(UInt64))
		{
		}

		public override void	OnGUI()
		{
			this.value = (UInt64)EditorGUILayout.LongField(this.name, (Int64)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetString(path, ((UInt64)this.value).ToString());
		}

		public override void	Deserialize(string path)
		{
			UInt64	result;

			if (UInt64.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				this.value = result;
		}
	}
}
#endif