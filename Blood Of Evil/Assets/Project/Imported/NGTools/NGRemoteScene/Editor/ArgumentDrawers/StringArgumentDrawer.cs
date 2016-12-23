using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(String))]
	public class StringArgumentDrawer : ArgumentDrawer
	{
		public StringArgumentDrawer(string name) : base(name, typeof(String))
		{
			this.value = string.Empty;
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.TextField(this.name, (String)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetString(path, (String)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = NGEditorPrefs.GetString(path);
		}
	}
}