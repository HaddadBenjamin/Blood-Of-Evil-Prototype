using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BloodOfEvil.Helpers
{
    public static class SerializerHelper
    {
        #region Json Serializer
        /// <summary>
        /// Charge le contenu d'un fichier json "jsonData" puis le décrypte puis renvoit sa conversion dans une classe de données sérializable.
        /// </summary>
        public static void JsonSafeDeserializeLoadWithEncryption<TypeToDeserialize>(
            TypeToDeserialize dataToLoad,
            string jsonData)
            where TypeToDeserialize : class, new()
        {
            dataToLoad = JsonUtility.FromJson<TypeToDeserialize>(
                EncryptionHelper.Decrypt(jsonData));

            if (null == dataToLoad)
                dataToLoad = new TypeToDeserialize();
        }

        /// <summary>
        /// Charge le fichier json correspondant au chenmin "jsonFilePath" puis le décrypte puis renvoit sa conversion dans une classe de données sérializable.
        /// </summary>
        public static TypeToDeserialize JsonSafeDeserializeLoadWithEncryption<TypeToDeserialize>(string jsonFilePath) where TypeToDeserialize : new()
        {
            TypeToDeserialize dataToLoad =  JsonUtility.FromJson<TypeToDeserialize>(
                EncryptionHelper.Decrypt(FileSystemHelper.SafeGetFileContent(jsonFilePath)));

            if (null == dataToLoad)
                dataToLoad = new TypeToDeserialize();

            return dataToLoad;
        }

        /// <summary>
        /// Sauvegarde "dataToSave" en json puis l'encrypte dans le fichier correspondant au chemin "jsonFilePath".
        /// </summary>
        public static void JsonSerializeSaveWithEncryption<TypeToSerialize>(
            TypeToSerialize dataToSave,
            string jsonFilePath)
            where TypeToSerialize : class
        {
            FileSystemHelper.CreateFileDirectoryIfDontExists(jsonFilePath);

            using (StreamWriter streamWriter = File.CreateText(jsonFilePath))
            {
                streamWriter.Write(
                    EncryptionHelper.Encrypt(
                        JsonUtility.ToJson(dataToSave)));
            }
        }

        /// <summary>
        /// Charge le contenu d'un fichier json "jsonData" et le convertie dans la classe de données passé en paramètre "dataToLoad".
        /// </summary>
        public static void JsonSafeDeserializeLoad<TypeToDeserialize>(
            TypeToDeserialize dataToLoad,
            string jsonData)
            where TypeToDeserialize : class, new()
        {
            dataToLoad = JsonUtility.FromJson<TypeToDeserialize>(jsonData);

            if (dataToLoad == null)
                dataToLoad = new TypeToDeserialize();
        }

        /// <summary>
        /// Charge le fichier json au "jsonFilePath" dans "dataToLoad".
        /// </summary>
        public static TypeToDeserialize JsonSafeDeserializeLoad<TypeToDeserialize>(string jsonFilePath) where TypeToDeserialize : new()
        {
            TypeToDeserialize dataLoaded = JsonUtility.FromJson<TypeToDeserialize>(FileSystemHelper.SafeGetFileContent(jsonFilePath));

            if (null == dataLoaded)
                dataLoaded = new TypeToDeserialize();

            return dataLoaded;
        }

        /// <summary>
        /// Sauvegarde "dataToSave" en json dans le fichier "jsonFilePath"
        /// </summary>
        public static void JsonSerializeSave<TypeToSerialize>(
            TypeToSerialize dataToSave,
            string jsonFilePath)
            where TypeToSerialize : class
        {
            FileSystemHelper.CreateFileDirectoryIfDontExists(jsonFilePath);

            using (StreamWriter streamWriter = File.CreateText(jsonFilePath))
            {
                streamWriter.Write(JsonUtility.ToJson(dataToSave));
            }
        }
        #endregion

        #region XML Serializer
        public static void XMLDeserializeLoad<TypeToDeserialize>(TypeToDeserialize dataToLoad, string xmlData) where TypeToDeserialize : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TypeToDeserialize));

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlData)))
            {
                dataToLoad = xmlSerializer.Deserialize(stream) as TypeToDeserialize;
            }
        }

        public static TypeToDeserialize XMLDeserializeLoad<TypeToDeserialize>(string path) where TypeToDeserialize : class, new()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TypeToDeserialize));
            TypeToDeserialize data = new TypeToDeserialize();

            using (StreamReader streamReader = File.OpenText(GetCompleteSavePath(path, ".xml")))
            {
                data = xmlSerializer.Deserialize(streamReader) as TypeToDeserialize;
            }

            return data;
        }

        public static void XMLSerializeSave<TypeToSerialize>(TypeToSerialize dataToSave, string path) where TypeToSerialize : class
        {
            string savePath = GetCompleteSavePath(path, ".xml");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TypeToSerialize));

            FileSystemHelper.CreateFileDirectoryIfDontExists(savePath);

            using (StreamWriter streamWriter = File.CreateText(savePath))
            {
                xmlSerializer.Serialize(streamWriter, dataToSave);
            }
        }
        #endregion

        #region Binary Serializer
        public static void BinarySerializeSave<TypeToSerialize>(TypeToSerialize dataToSave, string path) where TypeToSerialize : class
        {
            string savePath = GetCompleteSavePath(path, ".bin");
            BinaryFormatter binarySerializer = new BinaryFormatter();

            FileSystemHelper.CreateFileDirectoryIfDontExists(savePath);

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                binarySerializer.Serialize(stream, dataToSave);
            }
        }

        public static void BinaryDeserializeLoad<TypeToDeserialize>(TypeToDeserialize dataToLoad, string binData) where TypeToDeserialize : class
        {
            BinaryFormatter binarySerializer = new BinaryFormatter();

            using (TextReader textReader = new StringReader(binData))
            {
                dataToLoad = binarySerializer.Deserialize(((StreamReader)textReader).BaseStream) as TypeToDeserialize;
            }
        }

        /// <returns></returns>
        public static TypeToDeserialize BinaryDeserializeLoad<TypeToDeserialize>(string path) where TypeToDeserialize : class, new()
        {
            BinaryFormatter binarySerializer = new BinaryFormatter();
            TypeToDeserialize dataToLoad = new TypeToDeserialize();

            using (FileStream stream = new FileStream(GetCompleteSavePath(path, ".bin"), FileMode.Open))
            {
                dataToLoad = binarySerializer.Deserialize(stream) as TypeToDeserialize;
            }

            return dataToLoad;
        }
        #endregion

        #region Show File Content
        /// <summary>
        /// Affiche le contenu d'un fichier json.
        /// </summary>
        public static void JsonShowSaveFileContent(string path)
        {
            FileSystemHelper.ShowFileContent(GetCompleteSavePath(path, ".json"));
        }

        /// <summary>
        /// Affiche le contenu d'un fichier XML.
        /// </summary>
        public static void XMLShowSaveFileContent(string path)
        {
            FileSystemHelper.ShowFileContent(GetCompleteSavePath(path, ".xml"));
        }

        /// <summary>
        /// Affiche le contenu d'un fichier binaire.
        /// </summary>
        public static void BinaryShowSaveFileContent(string path)
        {
            FileSystemHelper.ShowFileContent(GetCompleteSavePath(path, ".bin"));
        }
        #endregion

        #region Path
        /// <summary>
        /// Permet de récupère ou sera sauvegardé un fichier de nom "path" et d'extension "extension".
        /// </summary>
        public static string GetCompleteSavePath(string path, string extension)
        {
            return Path.Combine(Application.persistentDataPath, string.Format("{0}{1}", path, extension));
        }

        /// <summary>
        /// Détermine si le fichier "path" et d'extension "exntension" éxiste.
        /// </summary>
        public static bool DoesCompletSavePathExists(string path, string extension)
        {
            return File.Exists(GetCompleteSavePath(path, extension));
        }
        #endregion
    }
}
