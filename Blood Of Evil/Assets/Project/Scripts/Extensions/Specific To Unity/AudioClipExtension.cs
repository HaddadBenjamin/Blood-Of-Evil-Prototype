using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Extensions
{
    public static class AudioClipExtension
    {
        /// <summary>
        /// Lance un son sur l'objet "audioClipOwner" si il contient un AudioSource.
        /// </summary>
        public static void SafePlay(
            this AudioClip clip, 
            GameObject audioClipOwner)
        {
            var audioSource = audioClipOwner.GetComponent<AudioSource>();

            if (null != audioSource)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
