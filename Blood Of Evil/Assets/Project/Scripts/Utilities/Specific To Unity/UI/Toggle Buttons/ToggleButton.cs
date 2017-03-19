using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BloodOfEvil.Utilities.UI
{
    /// <summary>
    /// Appele une action lorsque ce "bouton type case à coché" est activé ou désactivé.
    /// </summary>
    public class ToggleButton : MonoBehaviour, IPointerClickHandler
    {
        #region Fields
        /// <summary>
        /// Détermine si ce "bouton type case à coché" est en état d'activation ou non.
        /// </summary>
        private bool isOn;
        [SerializeField, Tooltip("Appelé  lorsque le bouton change d'état.")]
        public UnityBoolEvent OnIsOnChanged;
        #endregion

        #region Properties
        public bool IsOn
        {
            get
            {
                return isOn;
            }

            set
            {
                if (isOn != value)
                {
                    isOn = value;
                    OnIsOnChanged.SafeInvoke(IsOn);
                }
            }
        }
        #endregion

        #region Interface Behaviour
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            this.IsOn = this.IsOn.Inverse();
        }
        #endregion
    }
}
