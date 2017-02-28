using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace BloodOfEvil.Helpers
{
    using Utilities;
    using Scene.Services.Serializer;
    using Extensions;

    public class SerializationService : ASingletonMonoBehaviour<SerializationService>
    {
        /// <summary>
        /// Récupère le contenu d'un fichier de manière multiplateformes et adapdative.
        /// C'est à dire que cela prend en compte si vous souhaitez spécifier si oui ou non votre fichier doit se situer prêt de la build ou non avec le paramètre "isReplicatedNextTheBuild".
        /// Les fichiers qui doivent l'être sont les fichiers de configuration (fichiers de langages, fichiers de difficultés, etc...), ce sont donc des fichiers qui ne devrait pas changer en run-time et qui sont éditable en mode éditeur.
        /// Parcontre les autres fichiers tels que les stastisques du joueurs, les profiles, les objets du joueur devront être vide au premier lancement de l'application et par conséquent le paramètre "isReplicatedNextTheBuild" défini à false.
        /// La gestion de fichier multiplateforme implique d'envoyer à chaque fois une "Action<string>" qui récupéra le contenu du fichier et testera son contenu voir si il n'est pas vide pour pouvoir le traiter.
        /// C'est une façon de faire lourde mais obligatoire en multiplateforme.
        /// </summary>
        public void CallSafeAndCrossPlatformLoadFileContent<TTypeToSave>(
            string path,
            bool isReplicatedNextTheBuild = false,
            bool isEncrypted = false,
            EFileExtension fileExtension = EFileExtension.Json,
            Action<TTypeToSave> onLoadSuccess = null,
            Action onLoadError = null)
            where TTypeToSave : class, new()
        {
            StartCoroutine(this.SafeAndCrossPlatformLoadtFileContent<TTypeToSave>(new object[]
            {
                path,
                isReplicatedNextTheBuild,
                isEncrypted,
                fileExtension,
                onLoadSuccess,
                onLoadError
            }));
        }

        private IEnumerator SafeAndCrossPlatformLoadtFileContent<TTypeToSave>(object[] parameters)
                where TTypeToSave : class, new()
        {
            string path = (string)parameters[0];
            bool isReplicatedNextTheBuild = (bool)parameters[1];
            bool isEncrypted = (bool)parameters[2];
            EFileExtension fileExtension = (EFileExtension)parameters[3];
            Action<TTypeToSave> onLoadSuccess = (Action<TTypeToSave>)parameters[4];
            Action onLoadError = (Action)parameters[5];

            path = UnityFileSystemHelper.GetCrossPlatformAndAdaptativePath(path, isReplicatedNextTheBuild, fileExtension);

            string fileContent = "";

    #if UNITY_ANDROID
            // Sur Android il faut ouvrir les fichiers de façon différente en fonction de si l'on les place près de la build ou non.
            // Dans le cas où l'on la place près de la build on utilise un WWW et autrement un streamWriter.
            if (isReplicatedNextTheBuild)
            {
                WWW www = new WWW(@path);

                yield return www;

                if (string.IsNullOrEmpty(www.error))
                    fileContent = www.text;
                else
                    Debug.LogErrorFormat("Erreur dans le chargement d'un fichier sur mobile et utilisation d'un WWW : {0}", www.error);
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(@path))
                    {
                        string line;

                        while ((line = sr.ReadLine()) != null)
                        {
                            fileContent += line;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogErrorFormat("Erreur dans le chargement d'un fichier sur mobile et utilisation d'un WWW : {0}", exception.Message);
                }
            }
    #else
                fileContent = FileSystemHelper.SafeGetFileContent(path);
    #endif

            yield return null;

            if (isEncrypted)
                fileContent = EncryptionHelper.Decrypt(fileContent);

            //Debug.LogFormat("path : {0}", @path);
            //Debug.Log(fileContent);

            if (!string.IsNullOrEmpty(fileContent))
            {
                if (EFileExtension.Json == fileExtension)
                    onLoadSuccess.SafeCall(JsonUtility.FromJson<TTypeToSave>(fileContent));
                // Il faudrait que je test pour le XML et le binaire mais la méthode en json fonctionne du feu de dieu.
                //else if (EFileExtension.Xml == fileExtension)
                //{
                //    using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
                //    {
                //        onLoadSuccess.SafeCall((new XmlSerializer(typeof(TTypeToSave)).Deserialize(stream)) as TTypeToSave);
                //    }
                //}
                //else if (EFileExtension.Bin == fileExtension)
                //{
                //    using (TextReader textReader = new StringReader(fileContent))
                //    {
                //        onLoadSuccess.SafeCall(
                //            new BinaryFormatter().Deserialize(((StreamReader)textReader).BaseStream) as TTypeToSave);
                //    }
                //}
            }
            else
            {
                if (null == onLoadError)
                    Debug.LogErrorFormat("Impossible de charger le fichier {0} de type <color=red>[{1}]</color>", path, typeof(TTypeToSave).Name);
                else
                    Debug.LogWarningFormat("Impossible de charger le fichier {0} de type <color=red>[{1}]</color>", path, typeof(TTypeToSave).Name);

                onLoadError.SafeCall();
            }

            //onAfterLoadSuccess.SafeCall();
        }
    }
}
