using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities.UI
{
    /// <summary>
    /// Permet d'activer certain composant ou objets et dans désactiver lorsque l'on clic sur notre bouton.
    /// </summary>
    public class ComponentsAndGameObjectsToEnableOrDisableButton : AButtonAction
    {
        #region Fields
        [SerializeField, Tooltip("Ce sont les objets à activer lorsque l'on clic sur notre bouton.")]
        private GameObject[] objectsToActive;
        [SerializeField, Tooltip("Ce sont les objets à désactiver lorsque l'on clic sur notre bouton.")]
        private GameObject[] objectsToDesactive;

        [SerializeField, Tooltip("Ce sont les composants à activer lorsque l'on clic sur notre bouton.")]
        private MonoBehaviour[] componentsToActive;
        [SerializeField, Tooltip("Ce sont les composants à désactiver lorsque l'on clic sur notre bouton.")]
        private MonoBehaviour[] componentsToDesactive;
        #endregion

        #region Unity Behaviour
        protected override void ButtonAction()
        {
            Array.ForEach(this.objectsToActive, objectToActive => objectToActive.SetActive(true));
            Array.ForEach(this.objectsToDesactive, objectToDesactive => objectToDesactive.SetActive(false));

            Array.ForEach(this.componentsToActive, componentToActive => componentToActive.enabled = true);
            Array.ForEach(this.componentsToDesactive, componentToDesactive => componentToDesactive.enabled = false);
        }
        #endregion
    }
}
