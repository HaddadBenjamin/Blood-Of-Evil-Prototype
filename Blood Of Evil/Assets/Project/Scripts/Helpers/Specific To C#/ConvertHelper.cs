using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Helpers
{
    /// <summary>
    /// Devrait être dans des extensions : double, float, int.
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// Convertit un entier vers un double.
        /// </summary>
        public static double IntToDouble(int value)
        {
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Convertit un floatant vers un entier.
        /// </summary>
        public static int FloatToInt(float value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Convertit un floatant vers un double.
        /// </summary>
        public static long FloatToLong(float value)
        {
            return Convert.ToInt64(value);
        }
    }
}