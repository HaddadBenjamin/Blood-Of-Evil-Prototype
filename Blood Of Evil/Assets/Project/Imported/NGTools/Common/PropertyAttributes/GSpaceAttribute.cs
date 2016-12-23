using UnityEngine;

namespace NGTools
{
	public class GSpaceAttribute : PropertyAttribute
	{
		public readonly float	space;

		public	GSpaceAttribute(float space)
		{
			this.space = space;
		}
	}
}