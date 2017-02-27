#if UNITY_5
using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Double))]
	internal sealed class DoubleArgumentDrawer : ArgumentDrawer
	{
		public DoubleArgumentDrawer(string name, Type type) : base(name, typeof(Double))
		{
		}

		public override void	OnGUI()
		{
			this.value = (Double)EditorGUILayout.DoubleField(this.name, (Double)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetString(path, ((Double)this.value).ToString());
		}

		public override void	Load(string path)
		{
			Double	result;

			if (Double.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				this.value = result;
		}
	}
}
#endif