using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Scene.Services
{
    /// <summary>
    /// Permet de jouer une musique au lancement de l'application.
    /// </summary>
    public class PlayMusic : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("Permet de spécifier si la musique doit tourner en boucle ou non.")]
        private bool isLooped;
        [SerializeField, Tooltip("C'est l'audio source sur lequel les sons vont se jouer.")]
        private AudioSource audioSource;
        [SerializeField, Tooltip("C'est le son à jouer.")]
        private AudioClip audioClip;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            if (null != this.audioSource &&
                null != this.audioClip)
            {
                this.audioSource.loop = this.isLooped;
                this.audioSource.clip = this.audioClip;

                this.audioSource.Play();
            }
            else
                Debug.LogWarning("La musique ou l'audio source vaut null.");
        }
        #endregion
    }
}