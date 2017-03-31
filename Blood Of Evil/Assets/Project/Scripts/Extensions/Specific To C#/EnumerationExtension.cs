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
    }
}