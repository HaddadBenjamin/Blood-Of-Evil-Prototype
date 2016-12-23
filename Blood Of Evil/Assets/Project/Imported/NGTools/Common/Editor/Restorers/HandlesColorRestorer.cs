using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class HandlesColorRestorer : IDisposable
	{
		private static Dictionary<Color, HandlesColorRestorer>	cached = new Dictionary<Color, HandlesColorRestorer>();

		private Color	last;

		public static HandlesColorRestorer	Get(Color color)
		{
			HandlesColorRestorer	restorer;

			if (HandlesColorRestorer.cached.TryGetValue(color, out restorer) == false)
			{
				restorer = new HandlesColorRestorer(color);

				HandlesColorRestorer.cached.Add(color, restorer);
			}
			else
				restorer.Set(color);

			return restorer;
		}

		public	HandlesColorRestorer()
		{
			this.last = Handles.color;
		}

		private	HandlesColorRestorer(Color color)
		{
			this.last = Handles.color;
			Handles.color = color;
		}

		public IDisposable	Set(Color color)
		{
			Handles.color = color;

			return this;
		}

		public IDisposable	Set(float r, float g, float b, float a)
		{
			Color	c = Handles.color;

			c.r = r;
			c.g = g;
			c.b = b;
			c.a = a;

			Handles.color = c;

			return this;
		}

		public void Dispose()
		{
			Handles.color = this.last;
		}
	}
}