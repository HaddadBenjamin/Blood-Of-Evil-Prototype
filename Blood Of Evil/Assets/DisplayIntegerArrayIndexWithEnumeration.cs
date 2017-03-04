using UnityEngine;
using System.Collections;


namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// C'est un entier qui est affiché dans l'inspecteur. 
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