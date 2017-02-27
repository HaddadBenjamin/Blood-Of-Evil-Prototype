using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(SByteHandler))]
	internal sealed class SByteDrawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	anim;
		private ValueMemorizer<SByte>	drag;

		public	SByteDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.drag = new ValueMemorizer<SByte>();
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.anim == null)
				this.anim = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath()) == true)
			{
				this.drag.NewValue((SByte)data.value);
				this.anim.Start();
			}

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				EditorGUI.BeginChangeCheck();
				SByte	newValue = Convert.ToSByte(EditorGUI.IntField(r, Utility.NicifyVariableName(data.name), Convert.ToInt32(this.drag.Get((SByte)data.value))));
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.drag.Set(newValue);
					this.AsyncUpdateCommand(data.unityData, data.GetPath(), newValue, typeof(SByte));
				}

				this.drag.Draw(r);
			}
		}
	}
}