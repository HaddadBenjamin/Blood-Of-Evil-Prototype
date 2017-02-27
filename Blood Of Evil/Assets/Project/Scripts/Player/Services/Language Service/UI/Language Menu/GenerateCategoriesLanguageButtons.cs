using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Player.Services.Language.UI
{
    using Helpers;

    using Audio;

    public class GenerateCategoriesLanguageButtons : MonoBehaviour
    {
        #region Fields
        private Transform myTransform;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.myTransform = transform;

            this.Generate();
        }
        #endregion

        #region Intern Behaviour
        private void Generate()
        {
            Array.ForEach(EnumerationHelper.EnumerationToEnumerationValuesArray<ELanguageCategory>(), languageCategory =>
            {
                PlayerServicesAndModulesContainer.Instance.PrefabReferencesService.Instantiate("Language Category Button", this.myTransform, responsive:true).
                    GetComponent<LanguageCategoryButton>().
                    Initialize(languageCategory);
            });
        }
        #endregion
    }
}