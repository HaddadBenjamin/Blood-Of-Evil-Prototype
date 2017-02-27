using System;
using UnityEngine;
using UnityEngine.Events;

namespace NGToolsEditor.NGRemoteScene
{
	public sealed class BgColorContentAnimator : GUITimer
	{
		private BgColorContentRestorer	restorer;

		public	BgColorContentAnimator(UnityAction update, float value, float target) : base(update, value, target)
		{
			this.restorer = new BgColorContentRestorer();
		}

		public IDisposable	Restorer(Color color)
		{
			if (this.Value > 0)
				return this.restorer.Set(color);
			return this.restorer;
		}

		public IDisposable	Restorer(float r, float g, float b, float a)
		{
			if (this.Value > 0)
				return this.restorer.Set(r, g, b, a);
			return this.restorer;
		}
	}
}