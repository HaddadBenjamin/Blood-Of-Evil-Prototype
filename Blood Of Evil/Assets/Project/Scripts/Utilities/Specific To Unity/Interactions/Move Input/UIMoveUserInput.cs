using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Gère du mouvement multiplateforme fait par l'utilisateur sur de l'UI.
    /// </summary>
    public class UIMoveUserInput : AMoveUserInput, IPointerDownHandler, IPointerUpHandler
    {
        #region Interface Behaviour
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            base.DoesInteractionIsActive = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            base.DoesInteractionIsActive = false;
        }
        #endregion
    }
}
