using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Language.Serialization
{
    [System.Serializable]
    public class ELanguageSerializable
    {
        #region Fields
        public ELanguage ELanguage;
        #endregion

        #region Constructor
        public ELanguageSerializable() { }
        #endregion

        #region Constructor
        public ELanguageSerializable(ELanguage ELanguage)
        {
            this.ELanguage = ELanguage;
        }
        #endregion
    }
}