using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Scene
{
    using Utilities.UI;
    using ObjectInScene;

    using Player;
    using Player.Services.Audio;
    using Player.Services.Configuration;

    using Modules.State;
    using Services.Time;
    using Services.References;
    using Services.ObjectPool;
    using Services.Footsteps;

    public sealed class SceneServicesContainer : Utilities.ASingletonMonoBehaviour<SceneServicesContainer>
    {
        #region Attributes & Properties
        public TimeService TimeService { get; private set; }

        [SerializeField]
        private TooltipsService tooltipsService;

        [SerializeField, Header("File System Configuration")]
        private FileSystemConfiguration fileSystemConfiguration;

        public AudioReferencesArraysService AudioReferencesArraysService { get; private set; }
        public SpriteReferencesArraysService SpriteReferencesArraysService { get; private set; }
        public GameObjectInSceneReferencesService GameObjectInSceneReferencesService { get; private set; }
        public ObjectsPoolService ObjectsPoolService { get; private set; }
        public PrefabReferencesService PrefabReferencesService { get; private set; }
        public TextureReferencesService TextureReferencesService { get; private set; }
        public FootstepsService FootstepService { get; private set; }

        public SceneStateModule SceneStateModule { get; private set; }
        #endregion

        #region Properties
        public FileSystemConfiguration FileSystemConfiguration
        {
            get
            {
                return fileSystemConfiguration;
            }

            private set
            {
                fileSystemConfiguration = value;
            }
        }

        public TooltipsService TooltipsService
        {
            get
            {
                return tooltipsService;
            }

            private set
            {
                tooltipsService = value;
            }
        }
        #endregion


        #region Initialisation
        public override void InitializeSingleton()
        {
            this.FootstepService = GetComponent<FootstepsService>();

            this.SceneStateModule = GetComponent<SceneStateModule>();
            this.SceneStateModule.Reset();

            AInitializableComponent[] servicesComponent =
            {
                    (this.SpriteReferencesArraysService = GetComponent<SpriteReferencesArraysService>()),
                    (this.AudioReferencesArraysService = GetComponent<AudioReferencesArraysService>()),
                    (this.GameObjectInSceneReferencesService = GetComponent<GameObjectInSceneReferencesService>()),
                    (this.ObjectsPoolService = GetComponent<ObjectsPoolService>()),
                    (this.PrefabReferencesService = GetComponent<PrefabReferencesService>()),
                    (this.TextureReferencesService = GetComponent<TextureReferencesService>())
                };

            foreach (AInitializableComponent serviceComponent in servicesComponent)
                serviceComponent.Initialize();

            this.TimeService = new TimeService();
        }
        #endregion
    }
}