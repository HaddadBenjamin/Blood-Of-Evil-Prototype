using UnityEngine;
using System.Collections;


namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// C'est la classe d'index d'un tableau avec un entier qui est affiché dans l'inspecteur. 
    /// </summary>
    [System.Serializable]
    public class DisplayIntegerArrayIndexWithEnumeration : ADisplayArrayIndexWithEnumeration
    {
        #region Fields
        public int integer;
        #endregion

        #region Constructor
        public DisplayIntegerArrayIndexWithEnumeration()
        {
        }
        #endregion
}
}