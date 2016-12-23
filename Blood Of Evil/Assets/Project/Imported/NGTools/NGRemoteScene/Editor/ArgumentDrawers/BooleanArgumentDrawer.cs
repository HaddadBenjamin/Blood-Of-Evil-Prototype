using System;
using UnityEditor;

namespace NGToolsEditor
{
	[ArgumentDrawerFor(typeof(Boolean))]
	public class BooleanArgumentDrawer : ArgumentDrawer
	{
		public BooleanArgumentDrawer(string name) : base(name, typeof(Boolean))
		{
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.Toggle(this.name, (Boolean)this.value);
		}

		public override void	Serialize(string path)
		{
			NGEditorPrefs.SetBool(path, (bool)this.value);
		}

		public override void	Deserialize(string path)
		{
			this.value = (bool)NGEditorPrefs.GetBool(path);
		}
	}
}