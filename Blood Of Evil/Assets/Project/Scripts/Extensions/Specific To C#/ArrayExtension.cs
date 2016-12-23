using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
namespace BloodOfEvil.Extensions
{
    public static class ArrayExension
    {
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
    }
}
