using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(CharHandler))]
	internal sealed class CharDrawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	anim;
		private ValueMemorizer<Char>	drag;

		public	CharDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.drag = new ValueMemorizer<Char>();
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.anim == null)
				this.anim = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath()) == true)
			{
				this.drag.NewValue((Char)data.value);
				this.anim.Start();
			}

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				EditorGUI.BeginChangeCheck();
				string	newValue = EditorGUI.TextField(r, Utility.NicifyVariableName(data.name), this.drag.Get((Char)data.value).ToString());
				if (EditorGUI.EndChangeCheck() == true)
				{
					if (string.IsNullOrEmpty(newValue) == false)
					{
						this.drag.Set(newValue[0]);
						this.AsyncUpdateCommand(data.unityData, data.GetPath(), newValue[0], typeof(Char));
					}
				}

				this.drag.Draw(r);
			}
		}
	}
}