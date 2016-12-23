using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public static class UnityExtension
	{
#if UNITY_4_5
		public static void	GetComponents<T>(this GameObject go, List<T> result) where T : Component
		{
			result.AddRange(go.GetComponents<T>());
		}
#endif
	}
}