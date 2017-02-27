using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeDrawerFor(typeof(Vector2))]
	internal sealed class Vector2Drawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	animX;
		private BgColorContentAnimator	animY;

		private ValueMemorizer<Single>	dragX;
		private ValueMemorizer<Single>	dragY;

		public	Vector2Drawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.dragX = new ValueMemorizer<Single>();
			this.dragX.labelWidth = 16F;
			this.dragY = new ValueMemorizer<Single>();
			this.dragY.labelWidth = this.dragX.labelWidth;
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.animX == null)
			{
				this.animX = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
				this.animY = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
			}

			Vector2	vector = (Vector2)data.value;
			float	labelWidth;
			float	controlWidth;

			Utility.CalculSubFieldsWidth(r.width, 90F, 2, out labelWidth, out controlWidth);

			r.width = labelWidth;
			EditorGUI.LabelField(r, Utility.NicifyVariableName(data.name));
			r.x += r.width;

			int	iL = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			using (LabelWidthRestorer.Get(12F))
			{
				r.width = controlWidth;

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".x") == true)
				{
					this.dragX.NewValue(vector.x);
					this.animX.Start();
				}

				using (this.animX.Restorer(0F, .8F + this.animX.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	v = EditorGUI.FloatField(r, "X", this.dragX.Get(vector.x));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragX.Set(v);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".x", v, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragX.Draw(r);

					r.x += r.width;
				}

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".y") == true)
				{
					this.dragY.NewValue(vector.y);
					this.animY.Start();
				}

				using (this.animY.Restorer(0F, .8F + this.animY.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	v = EditorGUI.FloatField(r, "Y", this.dragY.Get(vector.y));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragY.Set(v);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".y", v, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragY.Draw(r);
				}
			}
			EditorGUI.indentLevel = iL;
		}
	}
}