#if UNITY_5
using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Int64))]
	public class Int64ArgumentDrawer : ArgumentDrawer
	{
		public Int64ArgumentDrawer(string name) : base(name, typeof(Int64))
		{
		}

		public override void	OnGUI()
		{
			this.value = (Int64)EditorGUILayout.LongField(this.name, (Int64)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetString(path, ((Int64)this.value).ToString());
		}

		public override void	Deserialize(string path)
		{
			Int64	result;

			if (Int64.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				this.value = result;
		}
	}
}
#endif