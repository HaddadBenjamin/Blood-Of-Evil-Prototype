using UnityEngine;

namespace NGTools.NGGameConsole
{
	public class FourCornersActivator : MonoBehaviour
	{
		public enum Corners
		{
			TopLeft,
			TopRight,
			BottomRight,
			BottomLeft
		}

		public NGGameConsole	gameConsole;
		public Corners[]		password;
		public float			cornerSizeInPixel = 100F;
		public float			cooldownInSecond = 5F;

		private int	currentStep;

		protected virtual void	Awake()
		{
			if (this.gameConsole == null)
			{
				InternalNGDebug.LogWarning("Game Console is required.", this);
				this.enabled = false;
			}
			if (this.password.Length == 0)
			{
				InternalNGDebug.LogWarning("Password is required.", this);
				this.enabled = false;
			}
		}

		protected virtual void	FixedUpdate()
		{
			if (this.currentStep == this.password.Length)
			{
				this.currentStep = 0;
				this.gameConsole.visible = true;
				this.CancelInvoke("ResetCooldown");
			}

			Corners	target = this.password[this.currentStep];

			if (target == Corners.TopLeft)
			{
				if (Input.mousePosition.x <= this.cornerSizeInPixel && Input.mousePosition.y >= Screen.height - this.cornerSizeInPixel)
					this.Increment();
			}
			else if (target == Corners.TopRight)
			{
				if (Input.mousePosition.x >= Screen.width - this.cornerSizeInPixel && Input.mousePosition.y >= Screen.height - this.cornerSizeInPixel)
					this.Increment();
			}
			else if (target == Corners.BottomLeft)
			{
				if (Input.mousePosition.x <= this.cornerSizeInPixel && Input.mousePosition.y <= this.cornerSizeInPixel)
					this.Increment();
			}
			else if (target == Corners.BottomRight)
			{
				if (Input.mousePosition.x >= Screen.width - this.cornerSizeInPixel && Input.mousePosition.y <= this.cornerSizeInPixel)
					this.Increment();
			}
		}

		private void	ResetCooldown()
		{
			this.currentStep = 0;
		}

		private void	Increment()
		{
			++this.currentStep;
			if (this.currentStep == 1)
				this.Invoke("ResetCooldown", this.cooldownInSecond);
		}
	}
}