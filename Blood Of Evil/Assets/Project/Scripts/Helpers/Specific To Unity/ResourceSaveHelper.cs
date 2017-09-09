using System.IO;
using UnityEngine;

namespace BloodOfEvil.Helpers
{
    using Extensions;

    public static class ResourceSaveHelper
    {
        ///// <summary>
        ///// Sauvegarde un audioClip dans le fichier "path".
        ///// </summary>
        //public static void SaveAudio(AudioClip audioClip, string path)
        //{
        //    SaveBytes(audioClip.ToBytes(), path);
        //}

        /// <summary>
        /// Sauvegarde une texture 2D PNG dans le fichier "path".
        /// </summary>
        public static void SavePNGImage(Texture2D PNGImage, string path)
        {
            SaveBytes(PNGImage.GetPNGBytes(), path);
        }

        /// <summary>
        /// Sauvegarde une texture 2D JPG dans le fichier "path".
        /// </summary>
        public static void SaveJPGImage(Texture2D PNGImage, string path)
        {
            SaveBytes(PNGImage.GetJPGBytes(), path);
        }

        /// <summary>
        /// Sauvegarde un WWW dans le fichier "path".
        /// </summary>
        public static void SaveWWW(WWW resourceFromWebsite, string path)
        {
            SaveBytes(resourceFromWebsite.bytes, path);
        }

        /// <summary>
        /// Sauvegarde des bytes dans le fichier "path.
        /// </summary>
        public static void SaveBytes(byte[] bytesToSave, string path)
        {
            File.WriteAllBytes(path, bytesToSave);
        }
    }

}