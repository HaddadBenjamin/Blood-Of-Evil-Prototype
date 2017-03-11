using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities
{
    public static class Utilities
    {
        /// <summary>
        /// Échange la valeur de 2 objets.
        /// </summary>
        public static void Swap<TSwapType>(ref TSwapType leftValue,
            ref TSwapType rightValue)
        {
            TSwapType tempValue = leftValue;

            leftValue = rightValue;
            rightValue = tempValue;
        }
    }
}