using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.ObjectInScene.Modules.Portal
{
    using Scene;
    using Player;
    using Helpers;
    using Utilities.UI;
    using Player.Services.Audio;

    public class PortalToChangeArea : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private string sceneToLoad;
        [SerializeField]
        private float timeToWaitBeforeLoadScene;

        //private bool isClicked;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            // Doit-être dans le start pour ne pas avoir de conflit avec l'awake du script ci-dessous.
            GetComponent<Tooltip3DNode>().UpdateLanguageTextAndAddASuit.UpdateDefaultText(sceneToLoad);
        }

        //    void Update()
        //    {
        //        if (PlayerServicesAndModulesContainer.Instance.InputService.IsDown(EPlayerInput.Select))
        //        { 
        //}
        //    }

        void OnTriggerEnter(Collider other)
        {
            if ("Player" == other.tag)
                StartCoroutine(this.LoadDiffered());
        }
        #endregion

        #region Intern Behaviour
        private IEnumerator LoadDiffered()
        {
            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Portal");
            yield return new WaitForSeconds(this.timeToWaitBeforeLoadScene);

            SceneManagerHelper.LoadScene(this.sceneToLoad);
        }
        #endregion
    }
}