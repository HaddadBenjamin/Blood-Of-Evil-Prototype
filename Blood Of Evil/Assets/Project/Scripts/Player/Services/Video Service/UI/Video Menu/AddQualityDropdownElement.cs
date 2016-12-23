using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using Utilities.UI;
    using ObjectInScene;

    using Language;

    public sealed class AddQualityDropdownElement : ADropdownElementAdder
    {
        #region Unity Behaviour
        void Start()
        {
            string[] qualities = QualitySettings.names;

            for (int qualityIndex = 0; qualityIndex < qualities.Length; qualityIndex++)
            {
                base.customDropdown.ModifyDropdownTitle(
                    base.customDropdown.AddDropdownElement(qualities[qualityIndex], qualityIndex, base.GetLanguageCategory(), delegate (CustomDropdownElement customDropdownElement)
                    {
                        PlayerServicesAndModulesContainer.Instance.VideoService.QualityIndex = customDropdownElement.Index;

                        ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
                    })
                );
            }

            PlayerServicesAndModulesContainer.Instance.VideoService.QualityIndexListener += this.UpdateQualityString;

            this.UpdateQualityString(PlayerServicesAndModulesContainer.Instance.VideoService.QualityIndex);
            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                this.UpdateQualityString(PlayerServicesAndModulesContainer.Instance.VideoService.QualityIndex);
            };
        }
        #endregion

        private void UpdateQualityString(int qualityIndex)
        {
            base.customDropdown.ModifyDropdownTitle(
                    PlayerServicesAndModulesContainer.Instance.
                        LanguageService.GetText(ELanguageCategory.MainMenuAndSubMenus, QualitySettings.names[qualityIndex]));
        }
    }
}