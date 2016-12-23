using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Services.Audio
{
    using Scene;
    using Scene.Services.References;

    using ObjectInScene;

    public class AudioCategoryNodeUI : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private EAudioCategory soundCategory;
        private Transform myTransform;
        private Image volumeImage;
        private Slider volumeSlider;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.myTransform = transform;
            this.volumeSlider = this.myTransform.FindChild("Volume Slider").GetComponent<Slider>();
            this.volumeImage = this.myTransform.FindChild("Speaker Image").GetComponent<Image>();
            this.volumeSlider.onValueChanged.AddListener(this.OnModifiedSliderVolume);

            this.volumeSlider.value = PlayerServicesAndModulesContainer.Instance.AudioService.GetAudioCategory(this.soundCategory).Volume;

        }
        #endregion

        #region Intern Behaviour
        private void OnModifiedSliderVolume(float volume)
        {
            //this.percentageText.text = StringHelper.NormalizedValuetoPercentageText(volume);

            PlayerServicesAndModulesContainer.Instance.AudioService.GetAudioCategory(this.soundCategory).Volume = volume;

            this.volumeImage.sprite =
                SceneServicesContainer.Instance.SpriteReferencesArraysService.Get(ESpriteCategory.MainMenu,
                volume < 0.01f ? "Sound Volume 0" :
                volume < 0.4f ? "Sound Volume 1" :
                volume < 0.7f ? "Sound Volume 2" :
                "Sound Volume 3");

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.AudioService).Save();

            //SceneServicesContainer.Instance.AudioReferencesArrays.Play2DSound(ESoundCategory.SFX, "Slider Modified Value");
        }
        #endregion
        // sound modified : percentage, soundManager.getcateogry.setvolume, volume image
    }
}