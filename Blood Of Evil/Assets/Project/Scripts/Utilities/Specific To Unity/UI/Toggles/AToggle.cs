using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.UI
{
    using Scene;
    using Player.Services.Audio;

    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public abstract class AToggle : MonoBehaviour
    {
        #region Fields
        protected string soundName = "Click Toggle";
        protected UnityEngine.UI.Toggle toggle;
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.toggle = GetComponent<UnityEngine.UI.Toggle>();

            this.toggle.onValueChanged.AddListener(delegate (bool valueChanged)
            {
                this.OnValueChangedListener(valueChanged);

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, this.soundName);
            });

            this.InitializeValue();
        }
        #endregion

        #region Abstract Behaviour
        public abstract void OnValueChangedListener(bool state);
        public abstract void InitializeValue();
        #endregion

        #region Public Behaviour
        public void InitializeValue(bool value)
        {
            this.toggle.isOn = value;
        }
        #endregion
    }
}