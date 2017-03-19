using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities.UI
{
    /// <summary>
    /// Permet d'activer certain composant ou objets et dans désactiver lorsque l'état de notre bouton type case à coché change.
    /// </summary>
    public class ComponentsAndGameObjectsToEnableOrDisableToggleButton : AToggleButtonAction
    {
        #region Fields
        [SerializeField, Tooltip("Ce sont les objets à activer lorsque l'état de notre case à coché est allumé.")]
        private GameObject[] objectsToActiveOnOnButtonState;
        [SerializeField, Tooltip("Ce sont les objets à activer lorsque l'état de notre case à coché est éteind.")]
        private GameObject[] objectsToActiveOnOffButtonState;

        [SerializeField, Tooltip("Ce sont les composants à activer lorsque l'état de notre case à coché est allumé.")]
        private MonoBehaviour[] componentsToActiveOnOnButtonState;
        [SerializeField, Tooltip("Ce sont les composants à activer lorsque l'état de notre case à coché est éteind.")]
        private MonoBehaviour[] componentsToActiveOnOffButtonState;
        #endregion

        #region Override Behaviour
        protected override void ButtonToggleAction(bool isOn)
        {
            Array.ForEach(this.objectsToActiveOnOnButtonState, objectToActive => objectToActive.SetActive(isOn));
            Array.ForEach(this.objectsToActiveOnOffButtonState, objectToDesactive => objectToDesactive.SetActive(!isOn));

            Array.ForEach(this.componentsToActiveOnOnButtonState, componentToActive => componentToActive.enabled = isOn);
            Array.ForEach(this.componentsToActiveOnOffButtonState, componentToDesactive => componentToDesactive.enabled = !isOn);
        }
        #endregion
    }
}
