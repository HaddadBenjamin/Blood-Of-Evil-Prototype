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

        //////////////////////////////////////////////

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// Convertit un audio clip en tableau d'octets.
        /// (utilisation directe des samples --> IeeeFloat wave)
        /// </summary>
        /// <returns>Un tableau d'octeToBytests</returns>
        public static byte[] ToBytes(this AudioClip ac)
        {
            byte[] sampleBytes;
            byte[] bytes = new byte[ac.samples * 4];
            float[] samples = new float[ac.samples];

            ac.GetData(samples, 0);

            int offset = 0;
            for (int i = 0; i < ac.samples; i++)
            {
                sampleBytes = BitConverter.GetBytes(samples[i]);

                for (int b = 0; b < sampleBytes.Length; b++, offset++)
                    bytes[offset] = sampleBytes[b];
            }

            return bytes;
        }
    }
}