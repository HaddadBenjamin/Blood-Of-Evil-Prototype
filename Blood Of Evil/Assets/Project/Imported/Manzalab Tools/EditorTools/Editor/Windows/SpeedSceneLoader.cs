using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cette classe venant de Manzalab.
/// </summary>
namespace BloodOfEvil.Extensions
{
    public static class ExtensionsArray
    {
        // List

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

        // Array
        public static T[] Add<T>(T[] array, T element)
        {
            List<T> newList = new List<T>();

            if (array != null)
                newList.AddRange(array);

            newList.Add(element);

            return newList.ToArray();
        }

        public static T[] AddRange<T>(T[] array, IEnumerable<T> arrayRange)
        {
            List<T> newList = new List<T>();

            if (array != null)
                newList.AddRange(array);

            newList.AddRange(arrayRange);

            return newList.ToArray();
        }

        public static T[] Remove<T>(T[] array, T item)
        {
            List<T> newList = new List<T>();

            if (array != null)
                newList.AddRange(array);

            newList.Remove(item);

            return newList.ToArray();
        }

        public static T[] RemoveAt<T>(T[] array, int index)
        {
            List<T> newList = new List<T>();

            if (array != null)
                newList.AddRange(array);

            newList.RemoveAt(index);

            return newList.ToArray();
        }

        public static T[] RemoveAll<T>(T[] array, System.Predicate<T> predicate)
        {
            List<T> newList = new List<T>();

            if (array != null)
                newList.AddRange(array);

            newList.RemoveAll(predicate);
            return newList.ToArray();
        }

        public static T[] Sort<T>(T[] array, System.Comparison<T> compare)
        {
            List<T> newList = new List<T>();

            if (array != null)
                newList.AddRange(array);

            newList.Sort(compare);

            return newList.ToArray();
        }
        public static T[] InsertAfter<T>(T[] array, int index, T o)
        {
            List<T> newList = new List<T>();

            if (array != null)
                newList.AddRange(array);

            if (index + 1 >= newList.Count)
                newList.Add(o);
            else
                newList.Insert(index + 1, o);
            return newList.ToArray();
        }

        public static T Find<T>(T[] array, System.Predicate<T> predicate)
        {
            return (new List<T>(array)).Find(predicate);
        }

        public static T[] Resize<T>(T[] array, int size)
        {
            while (array.Length > size)
                array = RemoveAt(array, array.Length - 1);
            while (array.Length < size)
                array = Add(array, default(T));

            return array;
        }
    }
}