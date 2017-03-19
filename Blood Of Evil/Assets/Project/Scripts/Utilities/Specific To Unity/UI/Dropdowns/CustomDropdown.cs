using System;
using System.Collections.Generic;
using BloodOfEvil.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities.UI
{
    using Scene;
    using Helpers;

    using Player;
    using Player.Services.Audio;
    using Player.Services.Language;

    public class CustomDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields
        [SerializeField]
        private RectTransform contentRectTransform;
        [SerializeField]
        private bool isOpen;

        [SerializeField]
        private float animationSpeed = 12.0f;

        [SerializeField]
        private string dropdownElementPrefabName;
        [SerializeField]
        private float dropdownElementMinSize = 25.0f;
        [SerializeField]
        private ELanguageCategory languageCategory;
        //[SerializeField]
        //private bool updateLanguage = true;
        #endregion

        #region Properties
        public ELanguageCategory LanguageCategory
        {
            get { return languageCategory; }
            private set { languageCategory = value; }
        }
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.contentRectTransform = transform.FindChild("Content").GetComponent<RectTransform>();
            this.isOpen = false;
        }

        void Update()
        {
            Vector3 scale = this.contentRectTransform.localScale;

            scale.y = Mathf.Lerp(scale.y, this.isOpen ? 1.0f : 0.0f, Time.deltaTime * this.animationSpeed);

            contentRectTransform.localScale = scale;
        }
        #endregion

        #region Interfaces Implementaiton
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            this.isOpen = true;

            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Open Dropdown");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            this.isOpen = false;

            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Close Dropdown");
        }
        #endregion

        #region Public Behaviour
        public CustomDropdownElement AddDropdownElement(string dropdownElementName, int index, ELanguageCategory languageCategory, Action<CustomDropdownElement> onDropdownElementClickCallback = null)
        {
            GameObject dropdownElementGameObject = PlayerServicesAndModulesContainer.Instance.PrefabReferencesService.Instantiate(this.dropdownElementPrefabName, contentRectTransform);

            dropdownElementGameObject.GetComponent<RectTransform>().ResetPositionAndScaleForResponsivity();
            dropdownElementGameObject.GetComponent<UnityEngine.UI.LayoutElement>().minHeight = this.dropdownElementMinSize;

            CustomDropdownElement customDropdownElement = dropdownElementGameObject.GetComponent<CustomDropdownElement>();

            customDropdownElement.Initialize(dropdownElementName, index, languageCategory, onDropdownElementClickCallback);

            return customDropdownElement;
        }

        public void AddDropdownElements<EnumerationType>(ELanguageCategory languageCategory, Action<CustomDropdownElement> onDropdownElementClickCallback = null) where EnumerationType : struct, IConvertible
        {
            this.AddDropdownElements(EnumerationHelper.EnumerationToStringArray<EnumerationType>(), languageCategory, onDropdownElementClickCallback);
        }

        public void AddDropdownElements(string[] dropdownElementsNames, ELanguageCategory languageCategory, Action<CustomDropdownElement> onDropdownElementClickCallback = null)
        {
            for (int dropdownElementIndex = 0; dropdownElementIndex < dropdownElementsNames.Length; dropdownElementIndex++)
                this.AddDropdownElement(dropdownElementsNames[dropdownElementIndex], dropdownElementIndex, languageCategory, onDropdownElementClickCallback);
        }

        public void ModifyDropdownTitle(CustomDropdownElement customDropdownElement)
        {
            this.transform.FindChild("Text").GetComponent<Text>().text =
                    //this.updateLanguage ?
                    //    PlayerModulesContainer.Instance.LanguageManager.GetText(this.languageCategory, customDropdownElement.Text) :
                    customDropdownElement.Text;
        }

        public void ModifyDropdownTitle(string newTitle)
        {
            this.transform.FindChild("Text").GetComponent<Text>().text = newTitle;
        }
        #endregion
    }
}