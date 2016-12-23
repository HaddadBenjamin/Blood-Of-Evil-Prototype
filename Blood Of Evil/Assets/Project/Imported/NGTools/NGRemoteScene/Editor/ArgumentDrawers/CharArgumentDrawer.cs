using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Char))]
	public class CharArgumentDrawer : ArgumentDrawer
	{
		public CharArgumentDrawer(string name) : base(name, typeof(Char))
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

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetInt(path, (char)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (char)NGEditorPrefs.GetInt(path);
		}
	}
}