using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class BooleanExtension
    {
        /// <summary>
        /// Retour la valeur inverse de ce boolean.
        /// </summary>
        public static bool Inverse(this bool boolean)
        {
            return boolean ^ true;
        }
    }
}
