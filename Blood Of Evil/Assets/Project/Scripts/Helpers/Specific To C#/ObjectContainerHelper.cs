using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Helpers
{
    public static class ObjectContainerHelper
    {
        /// <summary>
        /// Initialize un tableau de hash avec un tableau de string.
        /// </summary>
        public static void InitializeHashIds(string[] names, ref int[] hashIDs)
        {
            hashIDs = new int[names.Length];

            for (ushort index = 0; index < names.Length; index++)
            {
                if (null == names[index])
                    Debug.LogErrorFormat("ObjectContainerHelper : Index {0} is null", names[index]);
                else
                    hashIDs[index] = names[index].GetHashCode();
            }
        }

        /// <summary>
        /// Renvoit l'index d'un tableau de hash correspondant au hash de "name".
        /// </summary>
        public static int GetHashCodeIndex(string name, int[] hashIDs)
        {
            int hashCodeID = name.GetHashCode();
            int hashIndex = Array.FindIndex(hashIDs, hashId => hashId == hashCodeID);

            if (-1 == hashIndex)
                Debug.LogError("ObjectContainerHelper:  The reference of name \"" + name + "\" don't exist");

            return hashIndex;
        }
    }
}