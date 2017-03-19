using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet d'activer ou désactiver certain objets et composants.
    /// </summary>
    public class ComponentsAndGameObjectsModifyEnableState : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("Ce sont les objets à activer lorsque le paramètre envoyé est vrai.")]
        private GameObject[] objectsToActive;
        [SerializeField, Tooltip("Ce sont les objets à désactiver lorsque le paramètre envoyé est vrai.")]
        private GameObject[] objectsToDesactive;

        [SerializeField, Tooltip("Ce sont les composants à activer lorsque le paramètre envoyé est vrai.")]
        private MonoBehaviour[] componentsToActive;
        [SerializeField, Tooltip("Ce sont les composants à désactiver lorsque le paramètre envoyé est vrai.")]
        private MonoBehaviour[] componentsToDesactive;
        #endregion

        #region Public Behaviour
        public void ModifyEnableState(bool enableState)
        {
            Array.ForEach(this.objectsToActive, objectToActive => objectToActive.SetActive(enableState));
            Array.ForEach(this.objectsToDesactive, objectToDesactive => objectToDesactive.SetActive(!enableState));

            Array.ForEach(this.componentsToActive, componentToActive => componentToActive.enabled = enableState);
            Array.ForEach(this.componentsToDesactive, componentToDesactive => componentToDesactive.enabled = !enableState);
        }
        #endregion
    }
}
