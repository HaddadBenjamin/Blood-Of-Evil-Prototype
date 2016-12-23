using System;

namespace BloodOfEvil.Extensions
{
    public static class EnumerationExtension
    {
        /// <summary>
        /// Affiche le nombre d'éléments que contient une énumération.
        /// </summary>
        public static int Count(this Enum enumeration)
        {
            return Enum.GetNames(enumeration.GetType()).Length;
        }

        //////////////////////////////////////////////////////////////
        // Ce qui suit sont des méthodes de Manzalab.
        public static bool Contains(this Enum val, Enum val2)
        {
            return (Convert.ToInt32(val) & Convert.ToInt32(val2)) != 0;
        }

        public static bool Contains(this TransformExtension.TransformPosition val, TransformExtension.TransformPosition val2)
        {
            return (val & val2) != 0;
        }

        public static T Add<T>(this T me, Enum toAdd) where T : struct
        {
            return (T)(object)(Convert.ToInt32(me) | Convert.ToInt32(toAdd));
        }

        public static T Remove<T>(this T me, Enum toAdd) where T : struct
        {
            return (T)(object)(Convert.ToInt32(me) & ~Convert.ToInt32(toAdd));
        }
    }
}