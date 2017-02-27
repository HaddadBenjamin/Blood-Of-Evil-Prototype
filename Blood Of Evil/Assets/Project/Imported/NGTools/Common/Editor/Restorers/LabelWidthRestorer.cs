using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor
{
	public sealed class LabelWidthRestorer : IDisposable
	{
		private static Dictionary<float, LabelWidthRestorer>	cached = new Dictionary<float, LabelWidthRestorer>();

		private float	lastWidth;

		public static LabelWidthRestorer	Get(bool condition, float width)
		{
			return condition ? LabelWidthRestorer.Get(width) : null;
		}

		public static LabelWidthRestorer	Get(float width)
		{
			LabelWidthRestorer	restorer;

			if (LabelWidthRestorer.cached.TryGetValue(width, out restorer) == false)
			{
				restorer = new LabelWidthRestorer(width);

				LabelWidthRestorer.cached.Add(width, restorer);
			}
			else
			{
				restorer.lastWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = width;
			}

			return restorer;
		}

		private	LabelWidthRestorer(float width)
		{
			this.lastWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = width;
		}

		public void	Dispose()
		{
			EditorGUIUtility.labelWidth = this.lastWidth;
		}
	}
}