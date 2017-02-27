using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Single))]
	internal sealed class SingleArgumentDrawer : ArgumentDrawer
	{
		public SingleArgumentDrawer(string name, Type type) : base(name, typeof(Single))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.FloatField(this.name, (Single)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetFloat(path, (Single)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (Single)NGEditorPrefs.GetFloat(path);
		}
	}
}