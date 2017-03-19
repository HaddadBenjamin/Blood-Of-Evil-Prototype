using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Helpers
{
    public static class RandomHelper
    {
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
