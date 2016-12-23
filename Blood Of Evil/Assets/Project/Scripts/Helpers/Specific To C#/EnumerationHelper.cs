using System;
using System.Linq;

namespace BloodOfEvil.Helpers
{
    public static class EnumerationHelper
    {
        /// <summary>
        /// Permet d'obtenir l'index d'une énumération.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static int GetIndex<EnumerationType>(EnumerationType enumeration) where EnumerationType : struct, IConvertible
        {
            return Convert.ToInt32(enumeration);
        }

        /// <summary>
        /// Permet de savoir le nombre d'élements que contiend une énumeration.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <returns></returns>
        public static int Count<EnumerationType>() where EnumerationType : struct, IConvertible
        {
            return System.Enum.GetValues(typeof(EnumerationType)).Length;
        }

        /// <summary>
        /// Converti une énumération en tableau de string.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string[] EnumerationToStringArray<EnumerationType>() where EnumerationType : struct, IConvertible
        {
            return Enum.GetNames(typeof(EnumerationType));
        }

        /// <summary>
        /// Convertie une énumération en string.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <returns></returns>
        public static string EnumerationToString<EnumerationType>(EnumerationType enumeration) where EnumerationType : struct, IConvertible
        {
            return Enum.GetName(typeof(EnumerationType), GetIndex<EnumerationType>(enumeration));
        }

        /// <summary>
        /// Converti l'index d'une énumération vers une string.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <param name="enumerationIntegerIndex"></param>
        /// <returns></returns>
        public static string EnumerationIntegerIndexToString<EnumerationType>(int enumerationIntegerIndex) where EnumerationType : struct, IConvertible
        {
            EnumerationType enumerationIndex = EnumerationHelper.IntegerToEnumeration<EnumerationType>(enumerationIntegerIndex);

            return EnumerationToString(enumerationIndex);
        }

        /// <summary>
        /// Converti une énumération en tableau de valeurs de cette énumération. Ceci permet de la parcourir simplement.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <returns></returns>
        public static EnumerationType[] EnumerationToEnumerationValuesArray<EnumerationType>() where EnumerationType : struct, IConvertible
        {
            return Enum.GetValues(typeof(EnumerationType)).Cast<EnumerationType>().ToArray();
        }

        /// <summary>
        /// Converti une string en énumération.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <param name="enumerationElementString"></param>
        /// <returns></returns>
        public static EnumerationType StringToEnumeration<EnumerationType>(string enumerationElementString) where EnumerationType : struct, IConvertible
        {
            return (EnumerationType)Enum.Parse(typeof(EnumerationType), enumerationElementString);
        }

        /// <summary>
        /// Converti un entier en énumération.
        /// </summary>
        /// <typeparam name="EnumerationType"></typeparam>
        /// <param name="enumerationIndex"></param>
        /// <returns></returns>
        public static EnumerationType IntegerToEnumeration<EnumerationType>(int enumerationIndex) where EnumerationType : struct, IConvertible
        {
            return (EnumerationType)Enum.ToObject(typeof(EnumerationType), enumerationIndex);
        }
    }
}