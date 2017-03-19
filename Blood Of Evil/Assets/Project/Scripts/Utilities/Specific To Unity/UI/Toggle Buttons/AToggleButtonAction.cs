using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities.UI
{
    /// <summary>
    /// Permet de de rajouter une action a appelé au moment du clic sur notre bouton type case à coché.
    /// </summary>
    [RequireComponent(typeof(ToggleButton))]
    public abstract class AToggleButtonAction : MonoBehaviour
    {
        #region Unity Behaviour
        private void Awake()
        {
            GetComponent<ToggleButton>().OnIsOnChanged.AddListener((isOn) =>
            {
                this.ButtonToggleAction(isOn);
            });

            this.OnAwake();
        }
        #endregion

        #region Virtual Behaviour
        /// <summary>
        /// Cette méthode est appelé lorsque notre bouton type case à coché change d'état.
        /// </summary>
        protected abstract void ButtonToggleAction(bool isOn);

        /// <summary>
        /// Cette méthode est appelé au moment de l'Awake d'Unity, on peut la redéfinir pour initialiser une classe fille.
        /// </summary>
        protected virtual void OnAwake() { }
        #endregion
    }
}
