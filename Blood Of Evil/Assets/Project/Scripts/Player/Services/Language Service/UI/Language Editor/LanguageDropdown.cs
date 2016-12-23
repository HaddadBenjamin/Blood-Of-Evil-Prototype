using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Player.Services.Language.UI
{
    using Helpers;

    using Scene;
    using Scene.Services.References;

    public class LanguageDropdown : MonoBehaviour
    {
        #region Fields
        UnityEngine.UI.Dropdown dropdown;
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.dropdown = GetComponent<UnityEngine.UI.Dropdown>();
            this.InitializeDropdownContent();

            this.dropdown.options.Clear();

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                this.UpdateDropdownTexts();
            };
            //ce dernier doit faire appeler au langauge manager.
        }
        #endregion

        #region Intern Behaviour
        private void InitializeDropdownContent()
        {
            string[] languageCategoriesStrings = EnumerationHelper.EnumerationToStringArray<ELanguage>();

            for (int languageCategoryIndex = 0; languageCategoryIndex < languageCategoriesStrings.Length; languageCategoryIndex++)
            {
                string languageCategoryString = languageCategoriesStrings[languageCategoryIndex];

                this.dropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData(
                    PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.Language, languageCategoryString),
                    SceneServicesContainer.Instance.SpriteReferencesArraysService.Get(ESpriteCategory.LanguageFlag, string.Format("{0} Flag", languageCategoryString)
                    )));
            }

            this.dropdown.value = 0;
        }

        private void UpdateDropdownTexts()
        {
            for (int languageCategoryIndex = 0; languageCategoryIndex < EnumerationHelper.Count<ELanguageCategory>(); languageCategoryIndex++)
            {
                ELanguageCategory languageCategory = EnumerationHelper.IntegerToEnumeration<ELanguageCategory>(languageCategoryIndex);
                string languageCategoryString = EnumerationHelper.EnumerationToString<ELanguageCategory>(languageCategory);

                this.dropdown.options[languageCategoryIndex].text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.Language, languageCategoryString);
            }
        }
        #endregion
    }
}