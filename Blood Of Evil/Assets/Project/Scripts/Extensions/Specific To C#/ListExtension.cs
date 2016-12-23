using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
    /// <summary>
    /// Cette classe vient de Manzalab.
    /// </summary>
	public static class ListExtension
	{
		public static T GetLast<T>(this List<T> list)
		{
			return list[list.Count - 1];
		}

        public static int GetIndexOf<T>(this IList<T> list, T element)
        {
            int index = 0;

            foreach (T e in list)
            {
                if (e.Equals(element))
                    return index;
                index++;
            }

            throw new UnityException("Element not found in list");
        }

        public static int GetElementIndex<T>(this List<T> list, T element)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Equals(element))
					return i;
			}

			throw new System.Collections.Generic.KeyNotFoundException();
		}

		/// <summary>
		/// Shuffles elements in given list
		/// </summary>
		/// <typeparam name="T">Type of elements to shuffle</typeparam>
		/// <param name="list">list to shuffle</param>
		public static void Shuffle<T>(this List<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = UnityEngine.Random.Range(0, n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}
