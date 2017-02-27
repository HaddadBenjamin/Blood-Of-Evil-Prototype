using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(SingleHandler))]
	internal sealed class SingleDrawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	anim;
		private ValueMemorizer<Single>	drag;

		public	SingleDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.drag = new ValueMemorizer<Single>();
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.anim == null)
				this.anim = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath()) == true)
			{
				this.drag.NewValue((Single)data.value);
				this.anim.Start();
			}

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				EditorGUI.BeginChangeCheck();
				Single	newValue = EditorGUI.FloatField(r, Utility.NicifyVariableName(data.name), this.drag.Get((Single)data.value));
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.drag.Set(newValue);
					this.AsyncUpdateCommand(data.unityData, data.GetPath(), newValue, typeof(Single));
				}

				this.drag.Draw(r);
			}
		}
	}
}