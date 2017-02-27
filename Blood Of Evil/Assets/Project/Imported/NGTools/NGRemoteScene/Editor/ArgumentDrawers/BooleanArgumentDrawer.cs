using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Boolean))]
	internal sealed class BooleanArgumentDrawer : ArgumentDrawer
	{
		public BooleanArgumentDrawer(string name, Type type) : base(name, typeof(Boolean))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.Toggle(this.name, (Boolean)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetBool(path, (bool)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (bool)NGEditorPrefs.GetBool(path);
		}
	}
}