#if UNITY_5
using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(UInt64))]
	internal sealed class UInt64ArgumentDrawer : ArgumentDrawer
	{
		public UInt64ArgumentDrawer(string name, Type type) : base(name, typeof(UInt64))
		{
		}

		public override void	OnGUI()
		{
			this.value = (UInt64)EditorGUILayout.LongField(this.name, (Int64)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetString(path, ((UInt64)this.value).ToString());
		}

		public override void	Load(string path)
		{
			UInt64	result;

			if (UInt64.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				this.value = result;
		}
	}
}
#endif