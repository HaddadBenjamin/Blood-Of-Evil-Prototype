using UnityEngine;
using System.Collections;
using BloodOfEvil.Player.Services.TextInformation;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Montre comment afficher un tableau dans l'inspecteur de la taille d'une énumération et dont les index sont le nom de chaque index d'une énumération.
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
