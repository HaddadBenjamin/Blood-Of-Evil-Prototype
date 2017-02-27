using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public sealed class HandlesMatrix4x4Restorer : IDisposable
	{
		private static Dictionary<Matrix4x4, HandlesMatrix4x4Restorer>	cached = new Dictionary<Matrix4x4, HandlesMatrix4x4Restorer>();

		private Matrix4x4	last;

		public static HandlesMatrix4x4Restorer	Get(bool condition, Matrix4x4 matrix)
		{
			return condition ? HandlesMatrix4x4Restorer.Get(matrix) : null;
		}

		public static HandlesMatrix4x4Restorer	Get(Matrix4x4 matrix)
		{
			HandlesMatrix4x4Restorer	restorer;

			if (HandlesMatrix4x4Restorer.cached.TryGetValue(matrix, out restorer) == false)
			{
				restorer = new HandlesMatrix4x4Restorer(matrix);

				HandlesMatrix4x4Restorer.cached.Add(matrix, restorer);
			}
			else
				restorer.Set(matrix);

			return restorer;
		}

		public	HandlesMatrix4x4Restorer()
		{
			this.last = Handles.matrix;
		}

		private	HandlesMatrix4x4Restorer(Matrix4x4 matrix)
		{
			this.last = Handles.matrix;
			Handles.matrix = matrix;
		}

		public IDisposable	Set(Matrix4x4 matrix)
		{
			this.last = Handles.matrix;
			Handles.matrix = matrix;

			return this;
		}

		public void	Dispose()
		{
			Handles.matrix = this.last;
		}
	}
}