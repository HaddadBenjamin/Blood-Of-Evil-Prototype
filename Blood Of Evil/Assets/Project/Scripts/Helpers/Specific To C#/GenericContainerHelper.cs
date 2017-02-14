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
    }
}
