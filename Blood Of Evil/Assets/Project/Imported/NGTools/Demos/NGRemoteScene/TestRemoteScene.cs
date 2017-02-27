using UnityEngine;

namespace NGTools.Tests
{
	public class TestRemoteScene : MonoBehaviour
	{
		private const float	Spacing = 10F;
		private const float	XOffset = 20F;
		private const float	YOffset = 20F;
		private const float	Height = 30F;

		private GUIStyle	labelStyle;
		private Rect		r = new Rect(0F, 0F, 0, 30F);
		private Vector2		scrollPosition;
		private Rect		viewRect = default(Rect);

		protected virtual void	OnGUI()
		{
			if (this.labelStyle == null)
			{
				this.labelStyle = new GUIStyle(GUI.skin.label);
				this.labelStyle.wordWrap = false;
			}

#if UNITY_EDITOR
			if (TestUtility.RequireUnmaximized() == true)
				return;
#endif

			this.r.x = TestRemoteScene.XOffset;
			this.r.y = TestRemoteScene.YOffset;
			this.r.width = Screen.width - this.r.x - this.r.x;
			this.r.height = Screen.height - this.r.y - this.r.y;
			this.viewRect.height = 14 * TestRemoteScene.Height + 8 * TestRemoteScene.Spacing;

			GUI.Box(this.r, "Tutorial NG Remote Scene");

			this.scrollPosition = GUI.BeginScrollView(this.r, this.scrollPosition, this.viewRect);
			{
				r.height = 30F;

				GUI.Label(r, "1. Generate a standalone build with this current scene.", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "2. Start the build.", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "3. Open a <b>NG Remote Hierarchy</b> window. You will find it in the menu at <b>Window/NG Remote Hierarchy</b>.", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "4. In <b>NG Remote Hierarchy</b>, type \"<i>127.0.0.1</i>\" in <b>Address</b> and \"<i>17257</i>\" in <b>Port</b>. Now press the button \"<b>Connect</b>\".", this.labelStyle);
				r.y += r.height;
				GUI.Label(r, "   You should see the same Game Object as those in this current scene.", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "5. Open a <b>NG Remote Inspector</b> window. You will find it in the menu at <b>Window/NG Remote Inspector</b>.", this.labelStyle);
				r.y += r.height;
				GUI.Label(r, "   This is the equivalent of Unity's <b>Inspector</b> window", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "6. Open a <b>NG Remote Project</b> window. You will find it in the menu at <b>Window/NG Remote Project</b>.", this.labelStyle);
				r.y += r.height;
				GUI.Label(r, "   This is the equivalent of Unity's <b>Project</b> window", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "7. To choose which assets appear in <b>NG Remote Project</b>, you need to check the assets in <b>Embedded Resources</b> in the component <b>NG Server Scene</b>.", this.labelStyle);
				r.y += r.height;
				GUI.Label(r, "   This way you can avoid embedding unwanted assets and keep your test build lighter and faster to generate.", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "Tips. When testing on device, make sure the port is open in both sides and the device is reachable from your network.", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;

				GUI.Label(r, "Tips. When working on Game Object, DO NEVER DISABLE the Game Object containing <b>NG Server Scene</b>!", this.labelStyle);
				r.y += r.height + TestRemoteScene.Spacing;
			}
			GUI.EndScrollView();
		}
	}
}