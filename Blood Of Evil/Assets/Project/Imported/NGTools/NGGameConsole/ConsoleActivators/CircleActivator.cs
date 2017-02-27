using UnityEngine;

namespace NGTools.NGGameConsole
{
	public class CircleActivator : MonoBehaviour
	{
		public NGGameConsole	gameConsole;
		[Header("Minimum degrees before activating the console.")]
		public float			minDegrees = 720F;
		[Header("Point where you must circle.")]
		public Vector2			centerRelativeToScreen = new Vector2(.5F, .5F);

		private float	degrees;
		private Vector2	lastDirection;
		private float	nextCheck;

		protected virtual void	Awake()
		{
			if (this.gameConsole == null)
			{
				InternalNGDebug.LogWarning("Game Console is required.", this);
				this.enabled = false;
			}
			if (this.minDegrees < 360)
			{
				InternalNGDebug.LogWarning("Min Degrees is lower than 36O.", this);
				this.enabled = false;
			}
			else
				this.nextCheck = Time.realtimeSinceStartup;
		}

		protected virtual void	Update()
		{
			if (this.nextCheck <= Time.realtimeSinceStartup)
			{
				Vector2	center = new Vector2(Screen.width * this.centerRelativeToScreen.x, Screen.height * this.centerRelativeToScreen.y);
				Vector2	b = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - center).normalized;
				float	d = Utility.RelativeAngle(this.lastDirection, b, Vector3.up);

				if (d < 0)
					this.degrees = 0F;

				this.degrees += d;

				if (this.degrees >= this.minDegrees)
				{
					this.degrees = 0F;
					this.gameConsole.visible = true;
				}

				this.lastDirection = b;
				this.nextCheck = Time.realtimeSinceStartup + .1F;
			}
		}
	}
}