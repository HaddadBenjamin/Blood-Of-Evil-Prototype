using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Language.UI
{
    using Helpers;
    using ObjectInScene;
    using Utilities.UI;

    public class AddLanguageDropdownElements : ADropdownElementAdder
    {
        #region Unity Behaviour
        void Start()
        {
            base.customDropdown.AddDropdownElements<ELanguage>(base.GetLanguageCategory(), delegate (CustomDropdownElement customDropdownElement)
            {
                PlayerServicesAndModulesContainer.Instance.LanguageService.CurrentLanguage = EnumerationHelper.IntegerToEnumeration<ELanguage>(customDropdownElement.Index);

                ((ISerializable)PlayerServicesAndModulesContainer.Instance.LanguageService).Save();
            });

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                base.customDropdown.ModifyDropdownTitle(
                    PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.Language,
                        EnumerationHelper.EnumerationToString(PlayerServicesAndModulesContainer.Instance.LanguageService.CurrentLanguage)));

            };
        }
        #endregion
    }
}