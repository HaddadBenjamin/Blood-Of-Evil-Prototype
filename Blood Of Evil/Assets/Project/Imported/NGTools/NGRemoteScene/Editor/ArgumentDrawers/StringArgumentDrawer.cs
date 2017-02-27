using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(String))]
	internal sealed class StringArgumentDrawer : ArgumentDrawer
	{
		public StringArgumentDrawer(string name, Type type) : base(name, typeof(String))
		{
			this.value = string.Empty;
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.TextField(this.name, (String)this.value);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetString(path, (String)this.value);
		}

		public override void	Load(string path)
		{
			this.value = NGEditorPrefs.GetString(path);
		}
	}
}