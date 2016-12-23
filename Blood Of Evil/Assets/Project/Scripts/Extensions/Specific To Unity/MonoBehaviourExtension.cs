using UnityEngine;

namespace BloodOfEvil.Extensions
{
	public static class MonoBehaviourExtension
    {
		/// <summary>
		/// Links a local/child component to the given reference.
		/// </summary>
		/// <typeparam name="T">Component type to find</typeparam>
		/// <param name="c">Parent object</param>
		/// <param name="comp">Reference to fill</param>
		/// <param name="names">
		/// Possible paths/names for the transform (child) that contains the component (the order is important)
		/// If names is empty, the component will be searched locally.
		/// If null or an empty string is searched, the component will be searched locally
		/// If no transform has been found, stops the function, comp will be null.
		/// </param>
		public static void Link<T>(this Component c, out T comp, params string[] names) where T : Component
		{
			Transform t = null;
			if (names.Length == 0)
				t = c.transform;
			int i = 0;
			while (t == null && i < names.Length)
			{
				if (string.IsNullOrEmpty(names[i]))
					t = c.transform;
				else
					t = c.transform.FindChild(names[i]);
				i++;
			}
			if (t == null)
			{
				comp = null;
				return;
			}
			comp = t.GetComponent<T>();
		}

		static public T FindInParents<T>(this MonoBehaviour monoBehaviour) where T : Component
		{
			if (monoBehaviour == null) return null;
			var comp = monoBehaviour.GetComponent<T>();

			if (comp != null)
				return comp;

			var t = monoBehaviour.transform.parent;
			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
			return comp;
		}
	}
}
