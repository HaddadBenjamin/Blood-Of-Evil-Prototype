using System;

namespace NGTools.NGGameConsole
{
	using UnityEngine;

	public class GraphicStatsData : DataConsole
	{
		public float	refreshInterval = 5F;

		private float	nextRefresh = 0F;
		private int		totalVertexes = 0;
		private int		totalTriangles = 0;

		public override void	FullGUI()
		{
			if (this.nextRefresh < Time.time)
			{
				this.nextRefresh = Time.time + this.refreshInterval;

				this.totalVertexes= 0;
				this.totalTriangles = 0;

				foreach (MeshFilter mf in Object.FindObjectsOfType<MeshFilter>())
				{
					this.totalVertexes += mf.mesh.vertexCount;
					this.totalTriangles += mf.mesh.triangles.Length;
				}

				this.totalTriangles /= 3;
				this.label.text = "Vertexes: " + totalVertexes + Environment.NewLine + "Triangles: " + totalTriangles;
			}

			GUILayout.TextArea(this.label.text, this.fullStyle);
		}

		public override string	Copy()
		{
			return this.label.text;
		}
	}
}