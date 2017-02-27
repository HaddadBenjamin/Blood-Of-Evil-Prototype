using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public sealed class ColorContentRestorer : IDisposable
	{
		private static Dictionary<Color, ColorContentRestorer>	cached = new Dictionary<Color, ColorContentRestorer>();

		private Color	last;

		public static ColorContentRestorer	Get(bool condition, Color color)
		{
			return condition ? ColorContentRestorer.Get(color) : null;
		}

		public static ColorContentRestorer	Get(Color color)
		{
			ColorContentRestorer	restorer;

			if (ColorContentRestorer.cached.TryGetValue(color, out restorer) == false)
			{
				restorer = new ColorContentRestorer(color);

				ColorContentRestorer.cached.Add(color, restorer);
			}
			else
				restorer.Set(color);

			return restorer;
		}

		public	ColorContentRestorer()
		{
			this.last = GUI.contentColor;
		}

		private	ColorContentRestorer(Color color)
		{
			this.last = GUI.contentColor;
			GUI.contentColor = color;
		}

		public IDisposable	Set(Color color)
		{
			this.last = GUI.contentColor;
			GUI.contentColor = color;

			return this;
		}

		public IDisposable	Set(float r, float g, float b, float a)
		{
			Color	c = GUI.contentColor;

			c.r = r;
			c.g = g;
			c.b = b;
			c.a = a;

			this.last = GUI.contentColor;
			GUI.contentColor = c;

			return this;
		}

		public void	Dispose()
		{
			GUI.contentColor = this.last;
		}
	}
}