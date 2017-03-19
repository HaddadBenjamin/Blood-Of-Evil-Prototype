using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.UI
{
    using Player.Services.Language;

    public abstract class ADropdownElementAdder : MonoBehaviour
    {
        #region Fields
        protected CustomDropdown customDropdown;
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.customDropdown = GetComponent<CustomDropdown>();
        }
        #endregion

        #region Public Behaviour
        public ELanguageCategory GetLanguageCategory()
        {
            return this.customDropdown.LanguageCategory;
        }
        #endregion
    }
}