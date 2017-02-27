using NGToolsEditor.NGNavSelection;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	[Serializable, Category("Misc")]
	internal sealed class NavSelectionComponent : HubComponent
	{
		[NonSerialized]
		private GUIContent	leftContent;
		[NonSerialized]
		private GUIContent	rightContent;
		[NonSerialized]
		private GUIStyle	buttonLeft;
		[NonSerialized]
		private GUIStyle	buttonRight;

		public	NavSelectionComponent() : base("Navigate Selection")
		{
		}

		public override void	Init(NGHubWindow hub)
		{
			base.Init(hub);

			this.leftContent = new GUIContent("<", "Select previous selection");
			this.rightContent = new GUIContent(">", "Select next selection");

			NGNavSelectionWindow.SelectionChanged += this.hub.Repaint;
		}

		public override void	Uninit()
		{
			base.Uninit();

			NGNavSelectionWindow.SelectionChanged -= this.hub.Repaint;
		}

		public override void	OnGUI()
		{
			if (this.buttonLeft == null)
			{
				this.buttonLeft = "ButtonLeft";
				this.buttonRight = "ButtonRight";
			}

			EditorGUI.BeginDisabledGroup(!NGNavSelectionWindow.CanSelectPrevious);
			{
				if (GUILayout.Button(this.leftContent, this.buttonLeft, GUILayout.Height(this.hub.height)) == true)
					NGNavSelectionWindow.SelectPreviousSelection();
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(!NGNavSelectionWindow.CanSelectNext);
			{
				if (GUILayout.Button(this.rightContent, this.buttonRight, GUILayout.Height(this.hub.height)) == true)
					NGNavSelectionWindow.SelectNextSelection();
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}