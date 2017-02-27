using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(Vector4Handler))]
	internal sealed class Vector4Drawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	animX;
		private BgColorContentAnimator	animY;
		private BgColorContentAnimator	animZ;
		private BgColorContentAnimator	animW;

		private ValueMemorizer<Single>	dragX;
		private ValueMemorizer<Single>	dragY;
		private ValueMemorizer<Single>	dragZ;
		private ValueMemorizer<Single>	dragW;

		public	Vector4Drawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.dragX = new ValueMemorizer<Single>();
			this.dragY = new ValueMemorizer<Single>();
			this.dragZ = new ValueMemorizer<Single>();
			this.dragW = new ValueMemorizer<Single>();
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.animX == null)
			{
				this.animX = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
				this.animY = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
				this.animZ = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
				this.animW = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
			}

			Vector4	vector = (Vector4)data.value;
			float	labelWidth;
			float	controlWidth;

			Utility.CalculSubFieldsWidth(r.width, 44F, 4, out labelWidth, out controlWidth);

			r.width = labelWidth;
			EditorGUI.LabelField(r, Utility.NicifyVariableName(data.name));
			r.x += r.width;

			int	iL = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			using (LabelWidthRestorer.Get(14F))
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
					r.x += r.width;
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragX.Set(v);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".x", v, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragX.Draw(r);
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
					r.x += r.width;
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragY.Set(v);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".y", v, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragY.Draw(r);
				}

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".z") == true)
				{
					this.dragZ.NewValue(vector.z);
					this.animZ.Start();
				}

				using (this.animZ.Restorer(0F, .8F + this.animZ.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	v = EditorGUI.FloatField(r, "Z", this.dragZ.Get(vector.z));
					r.x += r.width;
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragZ.Set(v);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".z", v, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragZ.Draw(r);
				}

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".w") == true)
				{
					this.dragW.NewValue(vector.w);
					this.animW.Start();
				}

				using (this.animW.Restorer(0F, .8F + this.animW.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	v = EditorGUI.FloatField(r, "W", this.dragW.Get(vector.w));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragW.Set(v);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".w", v, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragW.Draw(r);
				}
			}
			EditorGUI.indentLevel = iL;
		}
	}
}