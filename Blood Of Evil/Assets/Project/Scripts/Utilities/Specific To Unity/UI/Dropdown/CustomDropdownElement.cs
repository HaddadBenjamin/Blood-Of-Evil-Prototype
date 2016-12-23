using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil
{
    using Player;
    using Player.Services.Language;
    using Extensions;

    namespace Utilities
    {
        namespace UI
        {
            public class CustomDropdownElement : AButton
            {
                #region Fields
                private string text;
                private int index;
                private ELanguageCategory languageCategory;

                public Action<CustomDropdownElement> OnClickCallback;
                [SerializeField]
                private bool updateLanguage;
                #endregion

                #region Properties
                public string Text
                {
                    get { return transform.FindChild("Text").GetComponent<Text>().text; }
                    private set
                    {
                        text = value;
                    }
                }

                public int Index
                {
                    get { return index; }
                    private set
                    {
                        index = value;
                    }
                }

                public ELanguageCategory LanguageCategory
                {
                    get { return languageCategory; }
                    private set
                    {
                        languageCategory = value;
                    }
                }
                #endregion

                #region Abstract Behaviour
                public override void ButtonActionOnClick()
                {
                    this.soundName = "Click Dropdown Element";

                    this.OnClickCallback.SafeCall<CustomDropdownElement>(this);

                    this.transform.parent.parent.GetComponent<CustomDropdown>().ModifyDropdownTitle(this);
                }
                #endregion

                #region Public Behaviour
                public void Initialize(
                    string name, 
                    int index, 
                    ELanguageCategory languageCategory, 
                    Action<CustomDropdownElement> onDropdownElementClickCallback = null)
                {
                    this.text = name;
                    this.index = index;
                    this.languageCategory = languageCategory;

                    this.UpdateTextContent();

                    PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += delegate ()
                    {
                        this.UpdateTextContent();
                    };

                    this.OnClickCallback += onDropdownElementClickCallback;
                }
                #endregion

                #region Intern Behaviour
                private void UpdateTextContent()
                {
                    transform.FindChild("Text").GetComponent<Text>().text = (this.updateLanguage) ?
                        PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(this.languageCategory, this.text) :
                        this.text;
                }
                #endregion
            }
        }
    }
}