using NGTools.NGRemoteScene;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(ColorHandler))]
	internal sealed class ColorDrawer : TypeHandlerDrawer
	{
		private ColorContentAnimator	anim;

		public	ColorDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.anim == null)
				this.anim = new ColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath()) == true)
				this.anim.Start();

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				EditorGUI.BeginChangeCheck();
				Color	newValue = EditorGUI.ColorField(r, Utility.NicifyVariableName(data.name), (Color)data.value);
				if (EditorGUI.EndChangeCheck() == true)
					this.AsyncUpdateCommand(data.unityData, data.GetPath(), newValue, typeof(Color));
			}
		}
	}
}