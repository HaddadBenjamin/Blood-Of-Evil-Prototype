using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Player.Services.Language.UI
{
    using Helpers;
    using Utilities.UI;
    using Extensions;

    public class LanguageCategoryButton : AButton
    {
        #region Fields
        public ELanguageCategory LanguageCategory { get; private set; }
        private string text;
        #endregion

        #region Public Behaviour
        public void Initialize(ELanguageCategory languageCategory)
        {
            this.LanguageCategory = languageCategory;
            this.text = EnumerationHelper.EnumerationToString(this.LanguageCategory).ReplaceUppercaseBySpaceAndUppercase();

            this.UpdateTextLanguage();

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += this.UpdateTextLanguage;
        }
        #endregion

        #region Abstract Behaviour
        public override void ButtonActionOnClick()
        {
            transform.parent.GetComponent<LanguageNodeUIManager>().LanguageCategorySelected = this.LanguageCategory;
        }
        #endregion

        #region Private Behaviour
        private void UpdateTextLanguage()
        {
            transform.Find("Text").
                GetComponent<Text>().
                text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.LanguageCategory, this.text);
        }
        #endregion
    }
}