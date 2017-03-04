using UnityEngine;
using System.Collections;
using BloodOfEvil.Player.Services.TextInformation;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Afficher un tableau dans l'inspecteur à la taille et au noms des index d'une énumération.
    /// </summary>
    public class TestArraySizeAndNameLinkToAEnumeration : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private DisplayIntegerArrayIndexWithEnumeration[] _integerArray;
        #endregion

        #region Unity Behaviour
        private void OnValidate()
        {
            DisplayArrayIndexWithEnumerationHelper.ToCallOnValidate<DisplayIntegerArrayIndexWithEnumeration, ETextInformation>(ref this._integerArray);
        }
        #endregion
    }
}
