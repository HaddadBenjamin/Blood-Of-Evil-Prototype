using System;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	public class FPSCountersData : DataConsole
	{
		public const float	RefreshPeriod = 0.5f;

		private float	nextTime;
		private int		updateCount;
		private int		onGUICount;
		private int		updatePerSecond;
		private int		onGUIPerSecond;

		private string	shortString = string.Empty;
		private string	fullString = string.Empty;

		protected virtual void	Start()
		{
			this.nextTime = Time.realtimeSinceStartup + FPSCountersData.RefreshPeriod;
		}

		protected virtual void	OnGUI()
		{
			++this.onGUICount;
		}

		protected virtual void	Update()
		{
			if (this.nextTime <= Time.realtimeSinceStartup)
			{
				this.updatePerSecond = (int)(this.updateCount / FPSCountersData.RefreshPeriod);
				this.onGUIPerSecond = (int)(this.onGUICount / FPSCountersData.RefreshPeriod);
				this.onGUICount = 0;
				this.updateCount = 0;
				this.shortString = this.onGUIPerSecond + " OnGUI/s " + this.updatePerSecond + " Update/s";
				this.fullString = "OnGUI: " + this.onGUIPerSecond + " per second" + Environment.NewLine + "Update: " + this.updatePerSecond + " per second";
				this.nextTime += FPSCountersData.RefreshPeriod;
			}
			else
				++this.updateCount;
		}

		public override void	ShortGUI()
		{
			this.label.text = this.shortString;
			this.DrawSimpleShortGUI();
		}

		public override void	FullGUI()
		{
			GUILayout.TextArea(this.fullString, this.fullStyle);
		}

		public override string	Copy()
		{
			return this.fullString;
		}
	}
}