using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Single))]
	public class SingleArgumentDrawer : ArgumentDrawer
	{
		public SingleArgumentDrawer(string name) : base(name, typeof(Single))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.FloatField(this.name, (Single)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetFloat(path, (Single)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (Single)NGEditorPrefs.GetFloat(path);
		}
	}
}