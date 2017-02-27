using System;
using UnityEngine;

namespace NGToolsEditor
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class LocaleHeaderAttribute : PropertyAttribute
	{
		public readonly string	key;
		public readonly float	height;

		public	LocaleHeaderAttribute(string header, float height = 24F)
		{
			this.key = header;
			this.height = height;
		}
	}
}