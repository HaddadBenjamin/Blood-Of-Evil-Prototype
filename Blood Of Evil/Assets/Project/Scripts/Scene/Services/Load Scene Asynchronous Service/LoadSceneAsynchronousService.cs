using System;
using UnityEngine;
using System.Collections;
using BloodOfEvil.Utilities;
using UnityEngine.SceneManagement;

namespace BloodOfEvil.Scene.Services
{
    public class LoadSceneAsynchronousService : ASingletonMonoBehaviour<LoadSceneAsynchronousService>
    {
        #region Fields
        [SerializeField, Tooltip("C'est le menu de chargement asynchrone.")]
        private AsynchroneLoadSceneMenu loadMenu;

        private AsyncOperation async;
        public Action OnSceneLoaded;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(loadMenu);
        }
        #endregion

        #region Public Behaviour
        public void LoadSceneAsynchrone(string sceneName)
        {
            StartCoroutine(LoadSceneAsynchroneCoroutine(sceneName));
        }

        #endregion

        #region Intern Behaviour
        private IEnumerator LoadSceneAsynchroneCoroutine(string sceneName)
        {
            this.loadMenu.EnableDownload = true;
            SingletonManager.ReinitializeAllSingletons();

            float timer = 0;
            while (timer < 2.0f)
            {
                timer += UnityEngine.Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!async.isDone)
                yield return null;
            yield return null;
        }

        //private void ResetAllSingletons()
        //{
        //    InputsManager.Instance.Reinitialize();
        //    NARFingersManager.Instance.Reinitialize();

        //    if (SceneManager.GetActiveScene().name.Equals("Gameplay Base"))
        //    {
        //        SerializationService.Instance.Reinitialize();
        //        SubtitlesManager.Instance.Reinitialize();
        //    }
        //}
        #endregion
    }
}