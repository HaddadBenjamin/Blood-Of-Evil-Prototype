using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Helpers
{
    public static class GenericContainerHelper
    {
        /// <summary>
        /// Retourne un index de la liste choisi de façon aléatoire.
        /// Devrait être dans ListExtension plutôt. !!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public static TListType SafeGetARandomElement<TContainerType, TListType>(TContainerType containerType) where
                TContainerType : IList<TListType>, ICollection<TListType>, IEnumerable<TListType>, IList, IEnumerable
                where TListType : class
        {
            return (null == containerType || 0 == (containerType as ICollection).Count) ?
                    null :
                    (containerType as IList<TListType>)[Random.Range(0, (containerType as ICollection).Count)];
        }
        
        /// <summary>
        /// Mélange tous les éléments d'un conteneur de données.
        /// </summary>
        public static void Shuffle<TContainerType, TListType>(ref TContainerType containerType) where
                TContainerType : IList<TListType>, ICollection<TListType>, IEnumerable<TListType>, IList, IEnumerable
                where TListType : class
            //TArrayElement : class
        {
            if (null == containerType || 0 == (containerType as ICollection).Count)
                return;

            List<int> shuffleIndexes = RandomHelper.GetAShuffleIntList((containerType as ICollection).Count);

            for (int arrayIndex = 0; arrayIndex < (containerType as ICollection).Count; arrayIndex++)
            {
                TListType leftValue = (containerType as IList<TListType>)[shuffleIndexes[arrayIndex]];
                TListType rightValue = (containerType as IList<TListType>)[arrayIndex];

                //Debug.LogFormat("leftValue : {0}, rightValue : {1}", leftValue, rightValue);
                Utilities.Swap(ref leftValue, ref rightValue);
                //Debug.LogFormat("leftValue : {0}, rightValue : {1}", leftValue, rightValue);
            }

            //for (int arrayIndex = 0; arrayIndex < (containerType as ICollection).Count; arrayIndex++)
            //{
            //    TListType value = (containerType as IList<TListType>)[arrayIndex];
            //    Debug.LogFormat("value : {0}", value);
            //}
        }
    }
}
