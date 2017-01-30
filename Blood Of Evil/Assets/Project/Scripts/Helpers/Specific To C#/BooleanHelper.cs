using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Helpers
{
    public static class BooleanHelper
    {
        /// <summary>
        /// Renvoie si tous les booleans passés en paramètre sont vrais.
        /// </summary>
        public static bool AreAllTrue(params bool[] allBooleans)
        {
            foreach (bool boolean in allBooleans)
            {
                if (!boolean)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Renvoie si un des booleans passés en paramètre est vrai.
        /// </summary>
        public static bool ContainsOneTrue(params bool[] allBooleans)
        {
            foreach (bool boolean in allBooleans)
            {
                if (boolean)
                    return true;
            }

            return false;
        }
    }
}
