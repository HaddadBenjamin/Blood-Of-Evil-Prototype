using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Char))]
	internal sealed class CharArgumentDrawer : ArgumentDrawer
	{
		public CharArgumentDrawer(string name, Type type) : base(name, typeof(Char))
		{
		}

		public override void	OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			string	v = EditorGUILayout.TextField(this.name, ((char)this.value).ToString());
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (string.IsNullOrEmpty(v) == true)
					this.value = (char)0;
				else
					this.value = v[0];
			}
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (char)this.value);
		}

		public override void	Load(string path)
		{
			this.value = (char)NGEditorPrefs.GetInt(path);
		}
	}
}