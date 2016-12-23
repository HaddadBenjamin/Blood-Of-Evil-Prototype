#if UNITY_5
using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Double))]
	public class DoubleArgumentDrawer : ArgumentDrawer
	{
		public DoubleArgumentDrawer(string name) : base(name, typeof(Double))
		{
		}

		public override void	OnGUI()
		{
			this.value = (Double)EditorGUILayout.DoubleField(this.name, (Double)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetString(path, ((Double)this.value).ToString());
		}

		public override void	Deserialize(string path)
		{
			Double	result;

			if (Double.TryParse(NGEditorPrefs.GetString(path), out result) == true)
				this.value = result;
		}
	}
}
#endif