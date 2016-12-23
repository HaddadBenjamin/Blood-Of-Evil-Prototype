using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BloodOfEvil.Utilities.UI
{
    using Scene;
    using Player.Services.Audio;

    [RequireComponent(typeof(Button))]
    public abstract class AButton : MonoBehaviour
    {
        #region Fields
        protected Button button;
        protected string soundName = "Click Button";
        #endregion

        #region Abstract Behaviour
        public abstract void ButtonActionOnClick();
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.button = GetComponent<Button>();

            this.button.onClick.AddListener(delegate ()
            {
                this.ButtonActionOnClick();

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, this.soundName);
            });
        }
        #endregion
    }
}