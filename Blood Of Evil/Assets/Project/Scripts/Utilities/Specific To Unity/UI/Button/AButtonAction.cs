using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities.UI
{
    /// <summary>
    /// Permet de de rajouter une action a appelé lorsque l'on clic notre bouton.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class AButtonAction : MonoBehaviour
    {
        #region Unity Behaviour
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                this.ButtonAction();
            });

            this.OnAwake();
        }
        #endregion

        #region Virtual Behaviour
        /// <summary>
        /// Cette méthode est appelé lorsque l'on clic sur notre bouton.
        /// </summary>
        protected abstract void ButtonAction();

        /// <summary>
        /// Cette méthode est appelé au moment de l'Awake d'Unity, on peut la redéfinir pour initialiser une classe fille.
        /// </summary>
        protected virtual void OnAwake() { }
        #endregion
    }
}
