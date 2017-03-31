using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Helpers
{
    public static class RandomHelper
    {
        /// <summary>
        /// Renvoit une valeur aléatoire comprise entre 2 entiers.
        /// </summary>
        public static int GetARandomValueBetweenTwoInts(int minimumValue, int maximumValue)
        {
            return Random.Range(minimumValue, maximumValue + 1);
        }

        /// <summary>
        /// Renvoit une valeur aléatoire comprise entre 2 floatants.
        /// </summary>
        public static float GetARandomValueBetweenTwoFloats(float minimumValue, float maximumValue)
        {
            return Random.Range(minimumValue, maximumValue);
        }

        /// <summary>
        /// Retourne un index du tableau choisi de façon aléatoire.
        /// </summary>
        public static TArrayType SafeGetARandomElement<TArrayType>(TArrayType[] array) 
                where TArrayType : class
        {
            return (null == array || 0 == array.Length) ?
                    null :
                    array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Retourne un index de la liste choisi de façon aléatoire.
        /// Devrait être dans ListExtension plutôt. !!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public static TListType SafeGetARandomElement<TListType>(List<TListType> list)
                where TListType : class
        {
            return (null == list || 0 == list.Count) ?
                    null :
                    list[Random.Range(0, list.Count)];
        }
    }
}
