using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace BloodOfEvil.Utilities.UI
{
    using Scene;
    using Helpers;
    using Player.Services.Audio;

    [RequireComponent(typeof(Slider))]
    public class ASlider : MonoBehaviour, IPointerDownHandler
    {
        #region Fields
        protected Slider slider;
        [SerializeField]
        private Text percentageText;
        [SerializeField]
        private bool textIsAPercentage = true;
        #endregion

        #region Virtual Public Behaviour
        virtual public void OnSliderValueModified(float value)
        {
            this.percentageText.text = this.textIsAPercentage ?
                StringHelper.NormalizedValuetoPercentageText(value) :
                Mathf.RoundToInt(value).ToString();
        }

        virtual public void InitializeValue() { }
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.slider = GetComponent<Slider>();

            this.slider.onValueChanged.AddListener(this.OnSliderValueModified);

            this.InitializeValue();
        }
        #endregion

        #region Public Behaviour
        public void InitializeValue(float value)
        {
            this.slider.value = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Slider Modified Value");
        }
        #endregion
    }
}