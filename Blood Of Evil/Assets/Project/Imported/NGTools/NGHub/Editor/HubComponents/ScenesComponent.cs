using NGToolsEditor.NGScenes;
using System;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	[Serializable, Category("Scene")]
	internal sealed class ScenesComponent : HubComponent
	{
		private const float	WindowWidth = 600F;
		private const float	WindowHeight = 400F;
		
		[NonSerialized]
		private GUIContent		content;
		[NonSerialized]
		private GUIStyle		dropDownButton;
		[NonSerialized]
		private NGScenesWindow	window;

		public	ScenesComponent() : base("Scenes")
		{
		}

		public override void	Init(NGHubWindow hub)
		{
			base.Init(hub);

			this.content = new GUIContent("Scenes", "Toggle scenes manager.");
		}

		public override void	OnGUI()
		{
			if (this.dropDownButton == null)
				this.dropDownButton = new GUIStyle("DropDownButton");

			this.dropDownButton.fixedHeight = this.hub.height;

			Rect	r = new Rect(this.hub.position.x, this.hub.position.y, 0F, this.hub.height);

			if (GUILayout.Button(this.content, this.dropDownButton) == true)
			{
				if (this.window != null)
				{
					this.window.Close();
					this.window = null;
				}
				else
				{
					window = ScriptableObject.CreateInstance<NGScenesWindow>();
					window.position = new Rect(r.x, r.y + r.height + 4F, ScenesComponent.WindowWidth, ScenesComponent.WindowHeight);
					Vector2	vector = new Vector2(window.position.width, window.position.height);
					window.maxSize = vector;
					window.minSize = vector;
					window.ShowPopup();
				}
			}
		}
	}
}