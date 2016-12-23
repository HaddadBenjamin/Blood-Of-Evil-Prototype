using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor
{
	public class LabelWidthRestorer : IDisposable
	{
		private static Dictionary<float, LabelWidthRestorer>	cached = new Dictionary<float, LabelWidthRestorer>();

		private float	width;
		private float	lastWidth;

		public static LabelWidthRestorer	Get(float width)
		{
			LabelWidthRestorer	restorer;

			if (LabelWidthRestorer.cached.TryGetValue(width, out restorer) == false)
			{
				restorer = new LabelWidthRestorer(width);

				LabelWidthRestorer.cached.Add(width, restorer);
			}
			else
				EditorGUIUtility.labelWidth = restorer.width;

			return restorer;
		}

		private	LabelWidthRestorer(float width)
		{
			this.width = width;
			this.lastWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = width;
		}

		public void Dispose()
		{
			EditorGUIUtility.labelWidth = this.lastWidth;
		}
	}
}