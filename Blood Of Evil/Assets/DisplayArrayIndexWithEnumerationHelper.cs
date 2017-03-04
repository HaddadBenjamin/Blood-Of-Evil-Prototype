using System;
using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities
{
    using Helpers;

    /// <summary>
    /// Permet de lier la longueur d'un tableau et le nom de tous ses index à celle d'une énumération.
    /// </summary>
    public static class DisplayArrayIndexWithEnumerationHelper
    {
        /// <summary>
        /// Méthode à appeler lors du OnValidate de la classe de type ArrayNodeType.
        /// </summary>
        public static void ToCallOnValidate<ArrayNodeType, EnumerationType>(
            ref ArrayNodeType[] array)
            where EnumerationType : struct, IConvertible
            where ArrayNodeType : ADisplayArrayIndexWithEnumeration, new()
        {
            int enumSize = EnumerationHelper.Count<EnumerationType>();

            if (null == array ||
                array.Length != enumSize)
            {
                Debug.LogWarningFormat(
                    "La longueur de votre tableau [<color=red>{0}</color>] n'est pas équivalent la longueur de votre énumération [<color=red>{1}</color>].",
                    null == array ? 0 : array.Length,
                    enumSize);

                array = new ArrayNodeType[enumSize];
            }

            for (int arrayIndex = 0; arrayIndex < enumSize; arrayIndex++)
            {
                if (null == array[arrayIndex])
                    array[arrayIndex] = new ArrayNodeType();

                array[arrayIndex].Name =
                    EnumerationHelper.EnumerationToString(
                        EnumerationHelper.IntegerToEnumeration<EnumerationType>(arrayIndex));
            }
        }
    }
}
