using System;
using UnityEngine;

namespace NGToolsEditor
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class SetColorAttribute : Attribute
	{
		public readonly Color	pro;
		public readonly Color	personal;

		public	SetColorAttribute(float proR, float proG, float proB, float proA, float perR, float perG, float perB, float perA)
		{
			this.pro = new Color(proR, proG, proB, proA);
			this.personal = new Color(perR, perG, perB, perA);
		}
	}
}