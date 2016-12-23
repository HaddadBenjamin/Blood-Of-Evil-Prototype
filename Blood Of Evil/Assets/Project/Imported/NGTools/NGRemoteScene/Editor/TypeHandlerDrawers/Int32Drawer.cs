using NGTools;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[TypeHandlerDrawerFor(typeof(Int32Handler))]
	public class Int32Drawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	anim;
		private ValueMemorizer<Int32>	drag;

		public	Int32Drawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.drag = new ValueMemorizer<Int32>();
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.anim == null)
				this.anim = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath()) == true)
			{
				this.drag.NewValue((Int32)data.value);
				this.anim.Start();
			}

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				EditorGUI.BeginChangeCheck();
				Int32	newValue = EditorGUI.IntField(r, Utility.NicifyVariableName(data.name), this.drag.Get((Int32)data.value));
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.drag.Set(newValue);
					this.AsyncUpdateCommand(data.unityData, data.GetPath(), newValue, typeof(Int32));
				}

				this.drag.Draw(r);
			}
		}
	}
}