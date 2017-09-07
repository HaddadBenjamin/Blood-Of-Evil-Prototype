using UnityEngine;
using System.Collections.Generic;

namespace BloodOfEvil.Extensions
{
    public static class ArrayExension
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
    }
}
