using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(ByteHandler))]
	internal sealed class ByteDrawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	anim;
		private ValueMemorizer<Byte>	drag;

		public	ByteDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.drag = new ValueMemorizer<Byte>();
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.anim == null)
				this.anim = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath()) == true)
			{
				this.drag.NewValue((Byte)data.value);
				this.anim.Start();
			}

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				EditorGUI.BeginChangeCheck();
				Byte	newValue = (Byte)EditorGUI.IntField(r, Utility.NicifyVariableName(data.name), this.drag.Get((Byte)data.value));
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.drag.Set(newValue);
					this.AsyncUpdateCommand(data.unityData, data.GetPath(), newValue, typeof(Byte));
				}

				this.drag.Draw(r);
			}
		}
	}
}