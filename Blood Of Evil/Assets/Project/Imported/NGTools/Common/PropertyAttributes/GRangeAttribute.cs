using UnityEngine;

namespace NGTools
{
	public class GRangeAttribute : PropertyAttribute
	{
		public readonly float	min;
		public readonly float	max;

		public	GRangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public	GRangeAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}
	}
}