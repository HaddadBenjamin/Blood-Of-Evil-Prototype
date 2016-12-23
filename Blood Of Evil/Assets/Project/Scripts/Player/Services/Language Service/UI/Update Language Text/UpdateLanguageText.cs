using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.Language.UI
{
    public class UpdateLanguageText : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private ELanguageCategory languageCategory;
        private string defaultText;
        private Text text;
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.text = GetComponent<Text>();
            this.UpdateDefaultText(this.text.text);

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                this.UpdateText();
            };
        }
        #endregion

        #region Public Behaviour
        public void UpdateDefaultText(string text)
        {
            this.defaultText = text;
        }

        public void Reinitialize(string defaultText, ELanguageCategory languageCategory)
        {
            this.defaultText = defaultText;
            this.languageCategory = languageCategory;
        }

        public void UpdateText()
        {
            //Debug.LogFormat("Category {0}, defaultText : {1}", this.languageCategory, this.defaultText);
            this.text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(this.languageCategory, this.defaultText);
        }
        #endregion
    }
}