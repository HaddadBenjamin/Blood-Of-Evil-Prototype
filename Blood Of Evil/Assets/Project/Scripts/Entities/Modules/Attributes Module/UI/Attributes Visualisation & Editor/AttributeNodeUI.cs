using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BloodOfEvil.Entities.Modules.Attributes.UI
{
    using Scene;
    using Helpers;

    using Player;
    using Player.Services.Audio;
    using Player.Services.Language;

    public class AttributeNodeUI : MonoBehaviour
    {
        #region Fields
        public EntityAttributes Attribute { get; private set; }
        public string SubAttributeCategory { get; private set; }
        public ELanguageCategory LanguageCategory { get; set; }
        private Transform attributeUINodeTransform;
        #endregion

        #region Public Behaviour
        public void Initialize(EntityAttributes attribute, string subAttributeCategory, ELanguageCategory languageCategory)
        {
            //this.UnsubscribeToUICallbacks();
            this.Attribute = attribute;
            this.SubAttributeCategory = subAttributeCategory;
            this.LanguageCategory = languageCategory;
            this.attributeUINodeTransform = transform;

            this.ModifiyTitleText();
            this.ModifyAttributeValueText();

            this.AddButtonCallback();
            this.SubscribeToAttributeValueTextCallback();

            this.WithoutThisInputFieldDidntWork();

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                this.ModifiyTitleText();
                this.ModifyAttributeValueText();
            };
        }
        #endregion

        #region Intern Behaviour
        private void WithoutThisInputFieldDidntWork()
        {
            GameObject inputFieldGameobject = this.attributeUINodeTransform.Find("Attribute Value Modified Input Field").gameObject;
            inputFieldGameobject.transform.Find("Text").GetComponent<Text>().text = "";

            inputFieldGameobject.GetComponent<InputField>().Select();
            inputFieldGameobject.GetComponent<InputField>().ActivateInputField();

            EventSystem.current.SetSelectedGameObject(inputFieldGameobject, null);
            inputFieldGameobject.GetComponent<InputField>().OnPointerClick(new PointerEventData(EventSystem.current));

            transform.
                FindChild("Attribute Value Modified Input Field").
                GetComponent<InputField>().text = "";
        }
        private void SubscribeToAttributeValueTextCallback()
        {
            this.GetAttributeOfSubAttributeCategory().ValueListener(this.ModifyAttributeValueText);
        }

        private void UnsubscribeToAttributeValueTextCallback()
        {
            this.GetAttributeOfSubAttributeCategory().ValueUnlistener(this.ModifyAttributeValueText);
        }

        private void ModifyAttributeValueText(float input = 0.0f)
        {
            float attributeValue = this.GetAttributeOfSubAttributeCategory().Value;

            this.attributeUINodeTransform.Find("Attribute Value Text").GetComponent<Text>().text = string.Format("{0} : {1}",
                PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.SubAttributesCategory, this.SubAttributeCategory),
                "Percent" == this.SubAttributeCategory ?
                this.Attribute.GetDefaultUpdateValueWithPercent() - this.Attribute.GetDefaultUpdateValueWithoutPercent() :
                attributeValue);
        }

        private void ModifiyTitleText()
        {
            this.attributeUINodeTransform.Find("Attribute Title").GetComponent<Text>().text =
                this.SubAttributeCategory.Equals("Current") ?
                    PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(this.LanguageCategory, this.Attribute.Current.Title) :
                    "";
        }

        private float GetAttributeModifiedValueInputFieldValue()
        {
            float attributeModifiedValueInputFieldValue;

            float.TryParse(this.attributeUINodeTransform.Find("Attribute Value Modified Input Field").
                Find("Text").
                GetComponent<Text>().text, out attributeModifiedValueInputFieldValue);

            return attributeModifiedValueInputFieldValue;
        }

        private void AddButtonCallback()
        {
            this.attributeUINodeTransform.Find("Apply Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                this.GetAttributeOfSubAttributeCategory().Value = this.GetAttributeModifiedValueInputFieldValue();

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");

                PlayerServicesAndModulesContainer.Instance.
                TextInformationService.AddTextInformation(
                    string.Format("{0} of \"{1}\" is {2}.",
                    PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.TextInformation, "The new attribute value is"),
                    PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.SubAttributesCategory, this.SubAttributeCategory),
                    this.GetAttributeModifiedValueInputFieldValue()));
            });
        }

        private void UnsubscribeToButtonCallback()
        {
            this.attributeUINodeTransform.Find("Apply Button").GetComponent<Button>().onClick.RemoveAllListeners();
        }

        private Attribute GetAttributeOfSubAttributeCategory()
        {
            return AttributeHelper.GetAttributeOfSubAttributeCategory(this.Attribute, this.SubAttributeCategory);
        }

        private Attribute GetAttributeOfSubAttributeCategory(string subAttributeCategory)
        {
            return AttributeHelper.GetAttributeOfSubAttributeCategory(this.Attribute, subAttributeCategory);
        }
        #endregion

        #region Public Behaviour
        public void UnsubscribeToUICallbacks()
        {
            this.UnsubscribeToAttributeValueTextCallback();
            this.UnsubscribeToButtonCallback();
        }
        #endregion
    }
}