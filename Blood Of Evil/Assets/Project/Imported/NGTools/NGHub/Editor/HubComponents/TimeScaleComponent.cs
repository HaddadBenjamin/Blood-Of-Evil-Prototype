using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	[Serializable, Category("Misc")]
	internal sealed class TimeScaleComponent : HubComponent
	{
		public const float	MinTime = 0F;
		public const float	MaxTime = 100F;
		public static Color	BorderColor = Color.grey * .8F;

		[Exportable]
		public float	presets1 = TimeScaleComponent.MinTime - 1F;
		[Exportable]
		public float	presets2 = TimeScaleComponent.MinTime - 1F;
		[Exportable]
		public float	presets3 = TimeScaleComponent.MinTime - 1F;

		[NonSerialized]
		private GUIContent	presetContent1;
		[NonSerialized]
		private GUIContent	presetContent2;
		[NonSerialized]
		private GUIContent	presetContent3;
		[NonSerialized]
		private GUIStyle	buttonLeft;
		[NonSerialized]
		private GUIStyle	buttonMid;
		[NonSerialized]
		private GUIStyle	buttonRight;

		public	TimeScaleComponent() : base("Time Scale", true, true)
		{
		}

		public override void	Init(NGHubWindow hub)
		{
			base.Init(hub);

			this.presetContent1 = new GUIContent("P1", this.presets1.ToString());
			this.presetContent2 = new GUIContent("P2", this.presets2.ToString());
			this.presetContent3 = new GUIContent("P3", this.presets3.ToString());
		}

		public override void	OnEditionGUI()
		{
			EditorGUI.BeginChangeCheck();
			this.presets1 = EditorGUILayout.FloatField("Preset 1", this.presets1);
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.presets1 = Mathf.Clamp(this.presets1, TimeScaleComponent.MinTime - 1F, TimeScaleComponent.MaxTime);
				this.presetContent1.tooltip = this.presets1.ToString();
			}

			EditorGUI.BeginChangeCheck();
			this.presets2 = EditorGUILayout.FloatField("Preset 2", this.presets2);
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.presets2 = Mathf.Clamp(this.presets2, TimeScaleComponent.MinTime - 1F, TimeScaleComponent.MaxTime);
				this.presetContent2.tooltip = this.presets2.ToString();
			}

			EditorGUI.BeginChangeCheck();
			this.presets3 = EditorGUILayout.FloatField("Preset 3", this.presets3);
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.presets3 = Mathf.Clamp(this.presets3, TimeScaleComponent.MinTime - 1F, TimeScaleComponent.MaxTime);
				this.presetContent3.tooltip = this.presets3.ToString();
			}
		}

		public override void	OnGUI()
		{
			if (this.buttonLeft == null)
			{
				this.buttonLeft = "ButtonLeft";
				this.buttonMid = "ButtonMid";
				this.buttonRight = "ButtonRight";
			}

			Utility.content.text = "R";
			Utility.content.tooltip = "Reset time to 1";
			if (GUILayout.Button(Utility.content, this.buttonLeft, GUILayout.Height(this.hub.height)) == true)
			{
				Time.timeScale = 1F;
				GUI.FocusControl(null);
			}
			Utility.content.tooltip = string.Empty;

			int		last = 0;

			if (this.presets1 >= TimeScaleComponent.MinTime)
				++last;
			if (this.presets2 >= TimeScaleComponent.MinTime)
				++last;
			if (this.presets3 >= TimeScaleComponent.MinTime)
				++last;

			if (this.presets1 >= TimeScaleComponent.MinTime && GUILayout.Button(this.presetContent1, (--last != 0) ? this.buttonMid : this.buttonRight, GUILayout.Height(this.hub.height)) == true)
			{
				Time.timeScale = this.presets1;
				GUI.FocusControl(null);
			}

			if (this.presets2 >= TimeScaleComponent.MinTime && GUILayout.Button(this.presetContent2, (--last != 0) ? this.buttonMid : this.buttonRight, GUILayout.Height(this.hub.height)) == true)
			{
				Time.timeScale = this.presets2;
				GUI.FocusControl(null);
			}

			if (this.presets3 >= TimeScaleComponent.MinTime && GUILayout.Button(this.presetContent3, (--last != 0) ? this.buttonMid : this.buttonRight, GUILayout.Height(this.hub.height)) == true)
			{
				Time.timeScale = this.presets3;
				GUI.FocusControl(null);
			}

			using (LabelWidthRestorer.Get(15F))
			{
				EditorGUI.BeginChangeCheck();
				Utility.content.text = Time.timeScale.ToString();
				float	v = EditorGUILayout.FloatField(" ", Time.timeScale, GeneralStyles.VerticalCenterTextField, GUILayout.Height(this.hub.height), GUILayout.Width(20F + GUI.skin.label.CalcSize(Utility.content).x));
				if (EditorGUI.EndChangeCheck() == true)
					Time.timeScale = Mathf.Clamp(v, TimeScaleComponent.MinTime, TimeScaleComponent.MaxTime);

				Rect	r = GUILayoutUtility.GetLastRect();
				r.width = 15F;
				GUI.Label(r, "T", GeneralStyles.CenterText);
			}
		}
	}
}