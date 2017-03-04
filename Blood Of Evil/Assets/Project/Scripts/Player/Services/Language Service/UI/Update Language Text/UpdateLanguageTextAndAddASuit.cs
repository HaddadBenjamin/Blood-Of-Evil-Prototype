using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.Language.UI
{
    public class UpdateLanguageTextAndAddASuit : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private ELanguageCategory languageCategory;
        private string defaultText;
        private Text text;
        [SerializeField]
        private string suit;
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.text = GetComponent<Text>();
            this.UpdateDefaultText(this.text.text);

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
            {
                string textTranslated = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(this.languageCategory, this.defaultText);

                if (null == textTranslated)
                    Debug.LogFormat("Object with null text : {0}, language category : {1}, text to translate : {2}", name, this.languageCategory, this.defaultText);
                else
                    this.text.text = string.Format("{0}{1}", textTranslated, this.suit);
            };
        }
        #endregion

        #region Public Behaviour
        public void UpdateDefaultText(string text)
        {
            this.defaultText = text;
        }

        public void UpdateDefaultTextThenUpdateText(string text)
        {
            this.UpdateDefaultText(text);
            this.UpdateText();
        }

        public void Reinitialize(string defaultText, ELanguageCategory languageCategory)
        {
            this.defaultText = defaultText;
            this.languageCategory = languageCategory;
        }

        public void ReinitializeAndUpdateText(string defaultText, ELanguageCategory languageCategory)
        {
            this.Reinitialize(defaultText, languageCategory);
            this.UpdateText();
        }

        public void UpdateText()
        {
            //Debug.LogFormat("Category {0}, defaultText : {1}", this.languageCategory, this.defaultText);
            this.text.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(this.languageCategory, this.defaultText);
        }
        #endregion
    }
}