using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Helpers
{
    using BloodOfEvil.Scene.Services.Serializer;

    public static class UnityFileSystemHelper
    {
        /// <summary>
        /// Renvoie un chemin qui s'adapte à la fois à toutes les plateformes et met les fichiers proche ou non de la build.
        /// Il faut mettre les fichiers proche de la build pour les fichiers de configuration qui ne s'en changent qu'en mode éditeur : fichiers de langages, de difficultés, etc..
        /// Autrement pour les fichiers qui sont sauvegardable mais qui ne sont pas rempli au premier lancement de l'application mettre le paramètre isReplicatedNextTheBuild à false.
        /// Par exemple pour les fichiers du types : profiles du joueur, stats du joueur, etc...
        /// </summary>
        public static string GetCrossPlatformAndAdaptativePath(
            string fileName,
            bool isReplicatedNextTheBuild = false,
            EFileExtension fileExtension = EFileExtension.Json)
        {
            string directory = "";

            if (isReplicatedNextTheBuild)
    #if UNITY_ANDROID
            directory = "jar:file://" + Application.dataPath + "!/assets";
    #else
                directory = Application.streamingAssetsPath;
    #endif
            else
                directory = Application.persistentDataPath;

            return string.Format("{0}/{1}{2}",
                                    directory,
                                    fileName,
                                    GetFileExtension(fileExtension));
        }

        /// <summary>
        /// Retourne l'extension correspondant à l'énumération de type "EFileExtension".
        /// </summary>
        public static string GetFileExtension(EFileExtension fileExtension)
        {
            return EFileExtension.Json == fileExtension ? ".json" :
                    EFileExtension.Xml == fileExtension ? ".xml" :
                    ".bin";
        }
    }
}