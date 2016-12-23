using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Player.Services.Language.UI
{
    using Scene;

    using Audio;

    // Ne peut pas hériter de AButton car si la méthode n'est pas dans le start elle ne marche pas.
    public class LanguageNodeUIApplyButton : MonoBehaviour
    {
        #region Fields
        private ELanguageCategory languageCategory;
        private int index;
        private Transform languageNodeUITransform;
        #endregion

        #region Public Behaviour
        public void Initialize(ELanguageCategory languageCategory, int index, Transform languageNodeUITransform)
        {
            this.languageCategory = languageCategory;
            this.index = index;
            this.languageNodeUITransform = languageNodeUITransform;

            this.RemoveAndCreateApplyButtonAction();
        }

        private void RemoveAndCreateApplyButtonAction()
        {
            Button button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate ()
            {
                string text = this.languageNodeUITransform.FindChild("Modify Inputfield").FindChild("Text").GetComponent<Text>().text;

                PlayerServicesAndModulesContainer.Instance.LanguageService.ModifyText(this.languageCategory, this.index, text);

                this.languageNodeUITransform.FindChild("Content Text").GetComponent<Text>().text = text;

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
            });
        }
        #endregion
    }
}