using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Helpers
{
    public static class ArrayHelper
    {
        /// <summary>
        /// Retourne un index de la liste choisi de façon aléatoire.
        /// Devrait être dans ListExtension plutôt. !!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public static TArrayElement SafeGetARandomElement<TArrayElement>(TArrayElement[] array) where
            TArrayElement : class
        {
            return (null == array || 0 == array.Length) ?
                    null :
                    array[Random.Range(0, array.Length)];
        }
    }
}
