#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HelpURLAttribute : Attribute
	{
		public	HelpURLAttribute(string url)
		{
		}
	}
}
#endif