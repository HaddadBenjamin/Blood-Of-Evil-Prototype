#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
using System.Collections.Generic;
#endif
using UnityEngine;

namespace NGTools
{
	public static class UnityExtension
	{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		public static void	SetParent(this Transform t, Transform p, bool unused)
		{
			t.parent = p;
		}

		public static T	GetIComponent<T>(this GameObject go)
		{
			Component[]	cs = go.GetComponents<Component>();

			for (int i = 0; i < cs.Length; i++)
			{
				if (cs[i] == null)
					continue;

				if (typeof(T).IsAssignableFrom(cs[i].GetType()) == true)
					return (T)(object)cs[i];
			}

			return default(T);
		}

		public static void	GetComponents<T>(this GameObject go, List<T> result) where T : Component
		{
			result.AddRange(go.GetComponents<T>());
		}

		public static T[]	GetIComponents<T>(this GameObject go)
		{
			List<T>		result = new List<T>();
			Component[]	cs = go.GetComponents<Component>();

			for (int i = 0; i < cs.Length; i++)
			{
				if (cs[i] == null)
					continue;

				if (typeof(T).IsAssignableFrom(cs[i].GetType()) == true)
					result.Add((T)(object)cs[i]);
			}

			return result.ToArray();
		}

		public static void	GetComponentsInChildren<T>(this GameObject go, bool inactive, List<T> result) where T : Component
		{
			result.AddRange(go.GetComponentsInChildren<T>(inactive));
		}
#else
		public static T	GetIComponent<T>(this GameObject go)
		{
			return go.GetComponent<T>();
		}

		public static T[]	GetIComponents<T>(this GameObject go)
		{
			return go.GetComponents<T>();
		}
#endif
	}
}