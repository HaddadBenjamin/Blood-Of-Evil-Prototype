using System;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[ArgumentDrawerFor(typeof(Enum))]
	internal sealed class EnumArgumentDrawer : ArgumentDrawer
	{
		private string[]	names;
		private int[]		values;

		public EnumArgumentDrawer(string name, Type type) : base(name, typeof(Enum))
		{
			this.value = 0;
			this.names = Enum.GetNames(type);

			Array	array = Enum.GetValues(type);
			this.values = new int[array.Length];

			for (int i = 0; i < array.Length; i++)
				this.values[i] = (int)array.GetValue(i);
		}

		public override void	OnGUI()
		{
			this.value = EditorGUILayout.IntPopup(this.name, (int)this.value, this.names, this.values);
		}

		public override void	Save(string path)
		{
			NGEditorPrefs.SetInt(path, (int)this.value);
		}

		public override void	Load(string path)
		{
			this.value = NGEditorPrefs.GetInt(path);
		}
	}
}