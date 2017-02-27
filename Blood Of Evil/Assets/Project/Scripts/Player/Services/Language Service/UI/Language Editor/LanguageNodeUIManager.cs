using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.Language.UI
{
    using Scene;
    using Scene.Services.ObjectPool;

    using Audio;
    public class LanguageNodeUIManager : MonoBehaviour
    {
        #region Fields
        private ELanguageCategory languageCategorySelected;
        private ObjectsPool pool;
        private Transform languageUINodeContentTransform;
        private LanguageService languageManager;

        [SerializeField]
        private Button saveButton;
        [SerializeField]
        private Button reloadButton;
        #endregion

        #region Properties
        public ELanguageCategory LanguageCategorySelected
        {
            get { return languageCategorySelected; }
            set
            {
                languageCategorySelected = value;

                this.UpdateLanguageCategory();
            }
        }
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.languageUINodeContentTransform = PlayerServicesAndModulesContainer.Instance.GameObjectInSceneReferencesService.Get("Editor - Node UI Content").transform;
            this.pool = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.GetPool("Language Node UI");
            this.languageManager = PlayerServicesAndModulesContainer.Instance.LanguageService;

            this.LanguageCategorySelected = ELanguageCategory.AttackAttributes;

            this.languageManager.NewLanguageHaveBeenLoaded += delegate ()
            {
                this.UpdateLanguageCategory();
            };

            this.saveButton.onClick.AddListener(() =>
            {
                this.languageManager.SaveLanguage();

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
            });

            this.reloadButton.onClick.AddListener(() =>
            {
                this.languageManager.CurrentLanguage = this.languageManager.CurrentLanguage;

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
            });
        }
        #endregion

        #region Intern Behaviour
        private void UpdateLanguageCategory()
        {
            this.DisableAllObjectsFromPoolAndUnsubscribeApplyButton();

            this.GenerateLanguageUINodes();
        }

        private void DisableAllObjectsFromPoolAndUnsubscribeApplyButton()
        {
            if (this.pool.GetTheNumberOfObjectsInPool() > 0)
            {
                for (int poolIndex = this.pool.GetGameobjects().Length - 1; poolIndex >= 0; poolIndex--)
                {
                    GameObject obj = this.pool.GetGameobjects()[poolIndex];

                    if (null != obj && obj.activeSelf)
                    {
                        obj.transform.FindChild("Apply Button").GetComponent<Button>().onClick.RemoveAllListeners();

                        pool.RemoveObjectInPool(obj);
                    }
                }
            }
        }

        // Devrait juste initializer des classes du type LanguageUINode mais bon... la flemme.
        private void GenerateLanguageUINodes()
        {
            string[] currentLanguageCategoryTexts = this.languageManager.GetCurrentLanguageCategoryTexts(this.LanguageCategorySelected);
            string[] defaultLanguageCategoryTexts = this.languageManager.GetDefaultLanguageCategoryTexts(this.LanguageCategorySelected);

            for (int index = 0; index < defaultLanguageCategoryTexts.Length; index++)
            {
                Transform languageNodeUITransform = this.pool.AddResponsiveObjectInPool(this.languageUINodeContentTransform).transform;

                languageNodeUITransform.FindChild("ID Text").GetComponent<Text>().text = defaultLanguageCategoryTexts[index];

                languageNodeUITransform.FindChild("Content Text").GetComponent<Text>().text =
                    null != currentLanguageCategoryTexts && index < currentLanguageCategoryTexts.Length && !this.languageManager.DoesCurrentLangageIsDefaultLanguage() ?
                    currentLanguageCategoryTexts[index] :
                    defaultLanguageCategoryTexts[index];

                languageNodeUITransform.FindChild("Modify Inputfield").GetComponent<InputField>().text = "";

                bool currentLanguageIsNotDefaultLanguage = !this.languageManager.DoesCurrentLangageIsDefaultLanguage();

                // Désactivation du bouton et du champ de texte pour le langage par défault.
                languageNodeUITransform.FindChild("Modify Inputfield").gameObject.SetActive(currentLanguageIsNotDefaultLanguage);
                languageNodeUITransform.FindChild("Apply Button").gameObject.SetActive(currentLanguageIsNotDefaultLanguage);

                if (currentLanguageIsNotDefaultLanguage)
                    languageNodeUITransform.FindChild("Apply Button").GetComponent<LanguageNodeUIApplyButton>().Initialize(this.LanguageCategorySelected, index, languageNodeUITransform);
            }
        }
        #endregion
    }
}