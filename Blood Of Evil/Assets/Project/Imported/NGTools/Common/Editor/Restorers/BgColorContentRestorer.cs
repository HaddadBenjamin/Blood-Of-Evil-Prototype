using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public sealed class BgColorContentRestorer : IDisposable
	{
		private static Dictionary<Color, BgColorContentRestorer>	cached = new Dictionary<Color, BgColorContentRestorer>();

		private Color	last;

		public static BgColorContentRestorer	Get(bool condition, Color color)
		{
			return condition ? BgColorContentRestorer.Get(color) : null;
		}

		public static BgColorContentRestorer	Get(Color color)
		{
			BgColorContentRestorer	restorer;

			if (BgColorContentRestorer.cached.TryGetValue(color, out restorer) == false)
			{
				restorer = new BgColorContentRestorer(color);

				BgColorContentRestorer.cached.Add(color, restorer);
			}
			else
				restorer.Set(color);

			return restorer;
		}

		public	BgColorContentRestorer()
		{
			this.last = GUI.backgroundColor;
		}

		private	BgColorContentRestorer(Color color)
		{
			this.last = GUI.backgroundColor;
			GUI.backgroundColor = color;
		}

		public IDisposable	Set(Color color)
		{
			this.last = GUI.backgroundColor;
			GUI.backgroundColor = color;

			return this;
		}

		public IDisposable	Set(float r, float g, float b, float a)
		{
			Color	c = GUI.backgroundColor;

			c.r = r;
			c.g = g;
			c.b = b;
			c.a = a;

			this.last = GUI.backgroundColor;
			GUI.backgroundColor = c;

			return this;
		}

		public void	Dispose()
		{
			GUI.backgroundColor = this.last;
		}
	}
}