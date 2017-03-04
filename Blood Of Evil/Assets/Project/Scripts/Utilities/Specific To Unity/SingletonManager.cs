using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        private const string defaultSceneName = "Persistent";
        #endregion

        #region Unity Behaviour
        /// <summary>
        /// Initialize tous les singletons de la scène de base.
        /// </summary>
        private void Awake()
        {
            //var d = DataAndDifficultyController.Instance;
    //
            //var e = LanguageService.Instance;
            //var f = SingletonManager.Instance;

            //DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Cette méthode est appelé lorsque l'on doit relancer notre jeu.
        /// </summary>
        public void RestartApplication()
        {
            //// Réinitialise tous les singletons héritant de MonoBehaviour en leur disant qu'ils peuvent être de nouveau initializer et en désabonnant tous leurs évênements.
            //GameController.Instance.Reinitialize();
            //ScoreModule.Instance.Reinitialize();
            //ScoreInterfaceModule.Instance.Reinitialize();
            //DataAndDifficultyController.Instance.Reinitialize();
            //
            //// Réinitialise tous les singletons héritant de données héritant d'aucune classe en leur disant qu'ils peuvent être de nouveau initializer et en désabonnant tous leurs évênements.
            //LanguageService.Instance.Reinitialize();
    //
            //// Détruit tous les singletons héritant de MonoBehaviour.
            //Destroy(GameController.Instance.gameObject);
            //Destroy(ScoreModule.Instance.gameObject);
            //Destroy(ScoreInterfaceModule.Instance.gameObject);
            //Destroy(DataAndDifficultyController.Instance.gameObject);

            // Charge la scène par défault.
            SceneManager.LoadScene(defaultSceneName);

            // S'auto-détruit.
            base.Reinitialize();
            Destroy(gameObject);
        }
        #endregion
    }
}
