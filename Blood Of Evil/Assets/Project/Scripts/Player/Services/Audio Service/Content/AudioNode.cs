using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioNode : MonoBehaviour
    {
        #region Fields
        private AudioSource audioSource;
        [SerializeField]
        private EAudioCategory soundCategory;
        private float volumeAtInitialization;
        #endregion

        #region Properties
        public EAudioCategory SoundCategory
        {
            get
            {
                return soundCategory;
            }

            private set
            {
                soundCategory = value;
            }
        }

        public float VolumeAtInitialization
        {
            get
            {
                return volumeAtInitialization;
            }

            private set
            {
                volumeAtInitialization = value;
            }
        }

        public AudioSource AudioSource
        {
            get
            {
                return audioSource;
            }

            private set
            {
                audioSource = value;
            }
        }
        #endregion

        #region Unity Behaviour
        void OnDestroy()
        {
            //PlayerModulesContainer.Instance.SoundManagerService.GetSoundCategory(this.soundCategory).VolumeHaveBeenModified -= this.UpdateVolume;
        }
        #endregion

        #region Public Behaviour
        public void Initialize(EAudioCategory soundCategory, AudioClip clip = null)
        {
            this.SoundCategory = soundCategory;

            if (null == this.AudioSource)
            {
                this.AudioSource = GetComponent<AudioSource>();
                this.VolumeAtInitialization = this.AudioSource.volume;
            }

            this.AudioSource.clip = clip;
            this.AudioSource.Play();
            this.AudioSource.loop = soundCategory == EAudioCategory.Music;

            AudioService audioService = PlayerServicesAndModulesContainer.Instance.AudioService;

            this.UpdateVolume(audioService.GetAudioCategory(this.SoundCategory).Volume);

            audioService.GetAudioCategory(this.SoundCategory).VolumeHaveBeenModified += this.UpdateVolume;
            audioService.GetAudioCategory(EAudioCategory.Overall).VolumeHaveBeenModified += this.UpdateVolume;
        }
        #endregion

        #region Intern Behaviour
        private void UpdateVolume(float volume)
        {
            this.AudioSource.volume =
                this.VolumeAtInitialization *
                PlayerServicesAndModulesContainer.Instance.AudioService.GetAudioCategory(this.SoundCategory).Volume *
                PlayerServicesAndModulesContainer.Instance.AudioService.GetAudioCategory(EAudioCategory.Overall).Volume;
        }
        #endregion
    }
}