using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Modules.Attributes.UI
{
    using Entities.Modules.Attributes;

    using Services.Language;
    
    [RequireComponent(typeof(Text))]
    public sealed class LevelText : MonoBehaviour
    {
        #region Fields
        private Text text;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.text = GetComponent<Text>();

            PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Level").Current.ValueListener(delegate (float value)
            {
                this.UdpateText();
            });

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += this.UdpateText;
        }
        #endregion

        #region Intern Behaviour
        private void UdpateText()
        {
            this.text.text =
                string.Format("{0} : {1}",
                    PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.ExperienceAttributes, "Level"),
                    PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Level").Current.Value);
        }
        #endregion
    }
}