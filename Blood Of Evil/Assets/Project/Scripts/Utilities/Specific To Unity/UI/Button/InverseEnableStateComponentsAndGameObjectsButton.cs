using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities.UI
{
    /// <summary>
    /// Permet d'inverser l'état d'activation de composant et de gameobject lorsque l'on clic sur notre bouton.
    /// </summary>
    public class InverseEnableStateComponentsAndGameObjectsButton : AButtonAction
    {
        #region Fields
        [SerializeField, Tooltip("Ce sont les objets dont on va inversé l'état d'activation lorsque l'on clic sur notre bouton.")]
        private GameObject[] objectsToInverseEnableState;
        [SerializeField, Tooltip("Ce sont les composants dont on va inversé l'état d'activation lorsque l'on clic sur notre bouton.")]
        private MonoBehaviour[] componentsToInverseEnableState;
        #endregion

        #region Unity Behaviour
        protected override void ButtonAction()
        {
            Array.ForEach(this.objectsToInverseEnableState, objectToInverseEnableState => objectToInverseEnableState.SetActive(objectToInverseEnableState.activeSelf.Inverse()));
            Array.ForEach(this.componentsToInverseEnableState, componentsToInverseEnableState => componentsToInverseEnableState.enabled = componentsToInverseEnableState.enabled.Inverse());
        }
        #endregion
    }
}
