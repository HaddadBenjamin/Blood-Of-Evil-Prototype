using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// C'est un bouton dont l'action est appelé à chaque fois que l'on reste appuyé dessus.
    /// </summary>
    public class ButtonRepeater : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Fields
        /// <summary>
        /// Permet de savoir si l'on doit appelé ou non l'action du bouton.
        /// </summary>
        private bool canCallRepeatEvent = false;
        [SerializeField, Tooltip("C'est l'action du bouton à appeler.")]
        private UnityEvent repeatEvent;
        #endregion

        #region Interface Behaviour
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            this.canCallRepeatEvent = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.canCallRepeatEvent = false;
        }
        #endregion

        #region Unity Behaviour
        private void LateUpdate()
        {
            if (this.canCallRepeatEvent)
                this.repeatEvent.SafeInvoke();
        }
        #endregion
    }
}
