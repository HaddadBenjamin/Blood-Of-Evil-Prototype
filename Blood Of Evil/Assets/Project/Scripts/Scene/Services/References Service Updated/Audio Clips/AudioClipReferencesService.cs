using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Scene.Services
{
    /// <summary>
    /// Permet de stoquer et de jouer les références sur les audio clips de votre application.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipReferencesService : AObjectReferencesService<AudioClip, AudioClipReferencesService>
    {
        #region Fields
        [SerializeField, Tooltip("C'est l'objet qui contiend l'audio source sur lequel les sons vont être joués.")]
        private AudioSource audioSource;
        #endregion

        #region Override Behaviour
        public override void InitializeSingleton()
        {
            base.InitializeSingleton();

            if (null == this.audioSource)
                this.audioSource = GetComponent<AudioSource>();
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Joue le clip sur l'audio source que l'on a spécifié.
        /// </summary>
        public void PlayAudioClip(string audioClipName)
        {
            AudioClip clip = base.Get(audioClipName);

            if (null != clip &&
                null != this.audioSource)
            {
                this.audioSource.clip = clip;
                this.audioSource.Play();
            }
            else
                Debug.LogWarning("Son ou audiosource non trouvé.");
        }
        #endregion
    }
}