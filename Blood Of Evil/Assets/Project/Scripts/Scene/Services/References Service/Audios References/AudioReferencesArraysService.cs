using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

namespace BloodOfEvil.Scene.Services.References
{
    using Helpers;
    using ObjectInScene;
    using Scene.Services.ObjectPool;

    using Player;
    using Player.Services.Audio;

    public sealed class AudioReferencesArraysService : AInitializableComponent
    {
        #region Fields
        private AudioReferences[] allAudioSources;

        [SerializeField]
        private AudioReferences SFX;
        [SerializeField]
        private AudioReferences Music;
        [SerializeField]
        private AudioReferences Dialog;
        [SerializeField, Header("Must contain only one reference to don't crash")]
        private AudioReferences Overall;

        private ObjectsPoolService poolManager;
        private ObjectsPool pool2DSound;
        private ObjectsPool pool3DSound;
        private static int nameIndex = 0;
        #endregion

        #region Abstract Initializer
        public override void Initialize()
        {
            this.allAudioSources = new AudioReferences[EnumerationHelper.Count<EAudioCategory>()];

            this.allAudioSources[EnumerationHelper.GetIndex(EAudioCategory.Overall)] = this.Overall;
            this.allAudioSources[EnumerationHelper.GetIndex(EAudioCategory.SFX)] = this.SFX;
            this.allAudioSources[EnumerationHelper.GetIndex(EAudioCategory.Music)] = this.Music;
            this.allAudioSources[EnumerationHelper.GetIndex(EAudioCategory.Dialog)] = this.Dialog;

            Array.ForEach(this.allAudioSources, audioSources => audioSources.Initialize());

            this.poolManager = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService;
            this.pool2DSound = this.poolManager.GetPool("Audio Node 2D");
            this.pool3DSound = this.poolManager.GetPool("Audio Node 3D");
        }
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.pool2DSound.RemoveAllObjectInPool();
            this.pool3DSound.RemoveAllObjectInPool();

            SceneManager.sceneLoaded += delegate (UnityEngine.SceneManagement.Scene scene, LoadSceneMode m)
            {
                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.Music, "Music");
            };
        }
        #endregion

        #region Public Behaviour
        public void Play2DSound(EAudioCategory audioCategory, string audioName)
        {
            GameObject audioNode = this.pool2DSound.AddObjectInPool();
            AudioClip clip = this.Get(audioCategory, audioName);

            audioNode.GetComponent<AudioNode>().Initialize(audioCategory, clip);

            if (EAudioCategory.Music == audioCategory)
                this.poolManager.RemoveObjectInPool("Audio Node 2D", audioNode, clip.length);

            audioNode.name = string.Format("Audio Node 2D : {0}", nameIndex++);
        }

        public void Play3DSound(EAudioCategory audioCategory, string audioName, Transform parent)
        {
            GameObject audioNode = this.pool3DSound.AddObjectInPool(parent.position);
            AudioClip clip = this.Get(audioCategory, audioName);

            audioNode.GetComponent<AudioNode>().Initialize(audioCategory, clip);

            if (EAudioCategory.Music == audioCategory)
                this.poolManager.RemoveObjectInPool("Audio Node 3D", audioNode, clip.length);

            audioNode.name = string.Format("Audio Node 3D : {0}", nameIndex++);
        }

        public AudioClip Get(EAudioCategory audioCategory, string audioName)
        {
            return this.allAudioSources[EnumerationHelper.GetIndex(audioCategory)].Get(audioName);
        }

        public void DisalbleAllSoundFromMusicCategory()
        {
            GameObject[] soundToDisable = Array.FindAll(this.pool2DSound.GetGameobjects(), soundNode => EAudioCategory.Music == soundNode.GetComponent<AudioNode>().SoundCategory);

            Array.ForEach(soundToDisable, soundNode => soundNode.SetActive(false));

            soundToDisable = Array.FindAll(this.pool3DSound.GetGameobjects(), soundNode => EAudioCategory.Music == soundNode.GetComponent<AudioNode>().SoundCategory);

            Array.ForEach(soundToDisable, soundNode => soundNode.SetActive(false));
        }
        #endregion
    }
}