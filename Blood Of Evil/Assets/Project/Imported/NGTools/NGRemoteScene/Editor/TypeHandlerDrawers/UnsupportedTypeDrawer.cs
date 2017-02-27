using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class UnsupportedTypeDrawer : TypeHandlerDrawer
	{
		public	UnsupportedTypeDrawer() : base(null)
		{
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			GUI.enabled = false;
			EditorGUI.LabelField(r, Utility.NicifyVariableName(data.name), "Unsupported type");
			GUI.enabled = true;
		}
	}
}