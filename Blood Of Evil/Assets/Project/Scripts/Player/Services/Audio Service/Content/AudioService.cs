using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace BloodOfEvil.Player.Services.Audio
{
    using Scene;
    using Helpers;
    using ObjectInScene;
    using Utilities.Serialization;

    [System.Serializable]
    public class AudioService : ISerializable, IDataInitializable
    {
        #region Fields
        private AudioCategory[] audioCategories;
        #endregion

        #region Properties
        public AudioCategory[] AudioCategories
        {
            get { return audioCategories; }
            private set { audioCategories = value; }
        }
        #endregion

        #region Public Behaviour
        void IDataInitializable.Initialize()
        {
            this.audioCategories = new AudioCategory[EnumerationHelper.Count<EAudioCategory>()];

            for (int audioCategoryIndex = 0; audioCategoryIndex < EnumerationHelper.Count<EAudioCategory>(); audioCategoryIndex++)
                this.audioCategories[audioCategoryIndex] = new AudioCategory();

            ((ISerializable)this).Load();
        }

        public AudioCategory GetAudioCategory(EAudioCategory audioCategory)
        {
            return this.audioCategories[EnumerationHelper.GetIndex(audioCategory)];
        }

        public float GeVolumeOfAnAudioCategory(EAudioCategory audioCategory)
        {
            return  this.GetAudioCategory(EAudioCategory.Overall).Volume *
                    this.GetAudioCategory(audioCategory).Volume;
        }
        #endregion

        #region Interfaces Behaviour
        void ISerializable.Load()
        {
            string audioFileName = this.GetAudioFileName();

            if (SerializerHelper.DoesCompletSavePathExists(audioFileName, ".json"))
            {
                SerializableFloatArray serializableFloatArray = SerializerHelper.JsonDeserializeLoad<SerializableFloatArray>(audioFileName);
                float[] floatArray = serializableFloatArray.floatArray;

                for (int floatArrayIndex = 0; floatArrayIndex < floatArray.Length; floatArrayIndex++)
                    this.audioCategories[floatArrayIndex].Volume = floatArray[floatArrayIndex];
            }
        }

        void ISerializable.Save()
        {
            float[] floatArray = Array.ConvertAll(this.audioCategories, soundCategory => soundCategory.Volume);
            SerializableFloatArray serializableFloatArray = new SerializableFloatArray(floatArray);

            SerializerHelper.JsonSerializeSave(serializableFloatArray, this.GetAudioFileName());
        }
        #endregion

        #region Intern Behaviour
        private string GetAudioFileName()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.AudioSettingsFilename;
        }
        #endregion
    }
}