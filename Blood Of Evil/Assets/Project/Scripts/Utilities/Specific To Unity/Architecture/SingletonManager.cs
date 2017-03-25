using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Extensions;
using UnityEngine.SceneManagement;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Cette classe permet de relancer votre application sans avoir des problèmes d'instances, d'initialisation ou d'évênements sur vos singletons de type de données ou héritant de MonoBehaviour.
    /// Elle doit se situer dans votre scène de base et ne dois jamais être détruite à part au moment où vous relancez votre application.
    /// </summary>
    public class SingletonManager : ASingletonMonoBehaviour<SingletonManager>
    {
        #region Fields
        public static Action OnReinitializeAllSingletons;
        private const string defaultSceneName = "Persistent";
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Public Behaviour
        public static void ReinitializeAllSingletons()
        {
            OnReinitializeAllSingletons.SafeCall();
        }
        
        /// <summary>
        /// Cette méthode est appelé lorsque l'on doit relancer notre jeu.
        /// </summary>
        public void RestartApplication()
        {
            ReinitializeAllSingletons();

            // Charge la scène par défault.
            SceneManager.LoadScene(defaultSceneName);

            // S'auto-détruit.
            base.Reinitialize();
            Destroy(gameObject);
        }
        #endregion
    }
}
