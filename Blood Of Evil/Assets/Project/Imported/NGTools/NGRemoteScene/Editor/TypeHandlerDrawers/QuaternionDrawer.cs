using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeDrawerFor(typeof(Quaternion))]
	internal sealed class QuaternionDrawer : TypeHandlerDrawer
	{
		private BgColorContentAnimator	animX;
		private BgColorContentAnimator	animY;
		private BgColorContentAnimator	animZ;
		private BgColorContentAnimator	animW;

		private ValueMemorizer<Single>	dragX;
		private ValueMemorizer<Single>	dragY;
		private ValueMemorizer<Single>	dragZ;
		private ValueMemorizer<Single>	dragW;

		public	QuaternionDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
			this.dragX = new ValueMemorizer<Single>();
			this.dragX.labelWidth = 16F;
			this.dragY = new ValueMemorizer<Single>();
			this.dragY.labelWidth = this.dragX.labelWidth;
			this.dragZ = new ValueMemorizer<Single>();
			this.dragZ.labelWidth = this.dragX.labelWidth;
			this.dragW = new ValueMemorizer<Single>();
			this.dragW.labelWidth = this.dragX.labelWidth;
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

			Quaternion	quaternion = (Quaternion)data.value;
			float		labelWidth;
			float		controlWidth;

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
					this.dragX.NewValue(quaternion.x);
					this.animX.Start();
				}

				using (this.animX.Restorer(0F, .8F + this.animX.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	newValue = EditorGUI.FloatField(r, "X", this.dragX.Get(quaternion.x));
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
					this.dragY.NewValue(quaternion.y);
					this.animY.Start();
				}

				using (this.animY.Restorer(0F, .8F + this.animY.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	newValue = EditorGUI.FloatField(r, "Y", this.dragY.Get(quaternion.y));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragY.Set(newValue);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".y", newValue, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragY.Draw(r);

					r.x += r.width;
				}

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".z") == true)
				{
					this.dragZ.NewValue(quaternion.z);
					this.animZ.Start();
				}

				using (this.animZ.Restorer(0F, .8F + this.animZ.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	newValue = EditorGUI.FloatField(r, "Z", this.dragZ.Get(quaternion.z));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragZ.Set(newValue);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".z", newValue, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragZ.Draw(r);

					r.x += r.width;
				}

				if (data.inspector.Hierarchy.GetUpdateNotification(data.GetPath() + ".w") == true)
				{
					this.dragW.NewValue(quaternion.w);
					this.animW.Start();
				}

				using (this.animW.Restorer(0F, .8F + this.animW.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					Single	newValue = EditorGUI.FloatField(r, "W", this.dragW.Get(quaternion.w));
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.dragW.Set(newValue);
						this.AsyncUpdateCommand(data.unityData, data.GetPath() + ".w", newValue, typeof(Single), TypeHandlersManager.GetTypeHandler(typeof(Single)));
					}

					this.dragW.Draw(r);
				}
			}
			EditorGUI.indentLevel = iL;
		}
	}
}