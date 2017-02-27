using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeDrawerFor(typeof(Rect))]
	internal sealed class RectDrawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	animX;
		private BgColorContentAnimator	animY;
		private BgColorContentAnimator	animW;
		private BgColorContentAnimator	animH;

		private ValueMemorizer<Single>	dragX;
		private ValueMemorizer<Single>	dragY;
		private ValueMemorizer<Single>	dragW;
		private ValueMemorizer<Single>	dragH;

		public	RectDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.dragX = new ValueMemorizer<Single>();
			this.dragX.labelWidth = 16F;
			this.dragY = new ValueMemorizer<Single>();
			this.dragY.labelWidth = this.dragX.labelWidth;
			this.dragW = new ValueMemorizer<Single>();
			this.dragW.labelWidth = this.dragX.labelWidth;
			this.dragH = new ValueMemorizer<Single>();
			this.dragH.labelWidth = this.dragX.labelWidth;
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			if (this.animX == null)
			{
				this.animX = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
				this.animY = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
				this.animW = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
				this.animH = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);
			}

			Rect	vector = (Rect)data.value;
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
					Single	newValue = EditorGUI.FloatField(r, "X", this.dragX.Get(vector.x));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragX.Set(newValue);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".x", newValue, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
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
					Single	newValue = EditorGUI.FloatField(r, "Y", this.dragY.Get(vector.y));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragY.Set(newValue);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".y", newValue, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragY.Draw(r);

					r.x += r.width;
				}

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".width") == true)
				{
					this.dragW.NewValue(vector.width);
					this.animW.Start();
				}

				using (this.animW.Restorer(0F, .8F + this.animW.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	newValue = EditorGUI.FloatField(r, "W", this.dragW.Get(vector.width));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragW.Set(newValue);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".width", newValue, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragW.Draw(r);

					r.x += r.width;
				}

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".height") == true)
				{
					this.dragH.NewValue(vector.height);
					this.animH.Start();
				}

				using (this.animH.Restorer(0F, .8F + this.animH.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	newValue = EditorGUI.FloatField(r, "H", this.dragH.Get(vector.height));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragH.Set(newValue);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".height", newValue, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragH.Draw(r);
				}
			}
			EditorGUI.indentLevel = iL;
		}
	}
}