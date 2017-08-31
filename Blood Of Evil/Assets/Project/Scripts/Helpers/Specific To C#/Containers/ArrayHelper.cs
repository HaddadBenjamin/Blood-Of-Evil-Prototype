using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Helpers
{
    using Utilities;
    using Helpers;

    public static class ArrayHelper
    {
        /// <summary>
        /// Récupère l'index d'un élément de façon rapide.
        /// </summary>
        public static int FastIndexOf<TArrayElement>(TArrayElement[] array, TArrayElement element, bool doesArrayIsSorted = false) where
            TArrayElement : class
        {
            if (null == array || 
                null == element ||
                0 == array.Length)
                return -1;
                
            if (doesArrayIsSorted)
            {
                int index = Array.BinarySearch(array, element);
                
                return index < 0 ? Array.IndexOf(array, element) : index;
            }
               
            return Array.IndexOf(array, element);
        }
        
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
        
        /// <summary>
        /// Mélange tous les éléments d'un tableau de données.
        /// </summary>
        public static void Shuffle<TArrayElement>(ref TArrayElement[] array)
            //TArrayElement : class
        {
            if (null == array || 0 == array.Length)
                return;

            List<int> shuffleIndexes = ListHelper.GetAShuffleIntList(array.Length);

            for (int arrayIndex = 0; arrayIndex < array.Length; arrayIndex++)
            {
                TArrayElement leftValue = array[shuffleIndexes[arrayIndex]];
                TArrayElement rightValue = array[arrayIndex];
                TArrayElement tmpValue = leftValue;

                array[shuffleIndexes[arrayIndex]] = rightValue;
                array[arrayIndex] = tmpValue;
            }
        }
    }
}
