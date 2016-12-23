using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BloodOfEvil.Utilities.UI
{
    using Player;
    using Player.Services.Language.UI;

    /// <summary>
    /// Tooltip 3D statique, qui se trigger sur du mouse enter et exit.
    /// </summary>
    public class Tooltip3DNode : MonoBehaviour
    {
        // Mode de trigger : on mouse enter et exit ou on hover button et exit ??

        #region Fields
        private GameObject containerGameObject;
        private GameObject textGameObject;
        private Transform myTransform;
        private UpdateLanguageTextAndAddASuit updateLanguageTextAndAddASuit;
        #endregion

        #region Properties
        public UpdateLanguageTextAndAddASuit UpdateLanguageTextAndAddASuit
        {
            get
            {
                return updateLanguageTextAndAddASuit;
            }

            private set
            {
                updateLanguageTextAndAddASuit = value;
            }
        }
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.myTransform = transform;
            this.containerGameObject = this.myTransform.Find("Tooltip").Find("Container").gameObject;
            this.textGameObject = this.containerGameObject.transform.Find("Text").gameObject;
            this.updateLanguageTextAndAddASuit = this.textGameObject.GetComponent<UpdateLanguageTextAndAddASuit>();

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                this.containerGameObject.SetActive(true);
                this.updateLanguageTextAndAddASuit.UpdateText();
                this.containerGameObject.SetActive(false);

            };

            this.containerGameObject.SetActive(false);
        }

        void OnMouseEnter()
        {
            //SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Open Tooltip");
            this.containerGameObject.SetActive(true);
        }

        void OnMouseExit()
        {
            //SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Close Tooltip");
            this.containerGameObject.SetActive(false);
        }
        #endregion
    }
}