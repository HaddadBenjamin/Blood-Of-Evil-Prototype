using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

namespace BloodOfEvil.Player
{
    using Helpers;
    using Extensions;
    using ObjectInScene;

    using Utilities.UI;
    using Utilities.Serialization;

    using Scene;
    using Scene.Services.References;
    using Scene.Services.ObjectPool;
    using Scene.Services.DontDestroy;

    using Services.Keys;
    using Services.Game;
    using Services.Audio;
    using Services.Video;
    using Services.Language;
    using Services.Canvases;
    using Services.Configuration;
    using Services.TextInformation;

    using Modules.Cursors;
    using Modules.Portails;
    using Modules.Movements;
    using Modules.Animations;
    using Modules.Attributes;

    public sealed class PlayerServicesAndModulesContainer :
        Utilities.ASingletonMonoBehaviour<PlayerServicesAndModulesContainer>,
        ISerializable
    {
        #region Fields
        [SerializeField]
        private bool debugMode = true;
        [SerializeField]
        private EResolutionType resolutionType = EResolutionType.Development;
        [SerializeField]
        private EPlayerClass classe = EPlayerClass.Cro_Magnon;
        #region Modules
        public PlayerAnimationModule AnimationModule { get; private set; }
        public PlayerAttributesModule AttributesModule { get; private set; }
        public PlayerMovementModule MovementModule { get; private set; }
        public PortalModule PortalModule { get; private set; }

        private IDataUpdatable[] Updatables;
        #endregion

        #region Services
        [SerializeField]
        private PlayerConfigurationService configurationService;
        public KeysService InputService { get; private set; }
        public LanguageService LanguageService { get; private set; }
        public AudioService AudioService { get; private set; }
        public GameService GameService { get; private set; }
        public VideoService VideoService { get; private set; }
        public ObjectsPoolService ObjectsPoolService { get; private set; }
        public CanvasesManagerService CanvasesService { get; private set; }
        public CursorModule CursorModule { get; private set; }
        [SerializeField]
        private TooltipsService tooltipsService;
        public TextInformationService TextInformationService { get; private set; }
        public DontDestroyOnLoadService DontDestroyOnLoadService { get; private set; }
        public GameObjectInSceneReferencesService GameObjectInSceneReferencesService { get; private set; }
        public PrefabReferencesService PrefabReferencesService { get; private set; }
        #endregion

        public Action<bool> DebugModeListener;
        public Action<EResolutionType> ResolutionTypeListener;
        #endregion

        #region Properties
        public bool DebugMode
        {
            get { return debugMode; }
            private set
            {
                debugMode = value;

                DebugModeListener.SafeCall(debugMode);
            }
        }

        public EResolutionType ResolutionType
        {
            get { return resolutionType; }
            private set
            {
                resolutionType = value;

                ResolutionTypeListener.SafeCall(resolutionType);
            }
        }

        public PlayerConfigurationService ConfigurationService
        {
            get { return configurationService; }
            private set { configurationService = value; }
        }

        public TooltipsService TooltipsService
        {
            get { return tooltipsService; }
            private set { tooltipsService = value; }
        }

        public EPlayerClass Classe
        {
            get
            {
                return classe;
            }

            private set
            {
                classe = value;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Le service de langage semble être initialiser plusieurs fois, vérifier si il y a des crashs si je ne le met qu'une seule fois.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // C'est un vrai bordel de gérer les dépendances entre-elles.

            IDataInitializable languageService = this.LanguageService = new LanguageService();
            languageService.Initialize();

            this.GameObjectInSceneReferencesService = GetComponent<GameObjectInSceneReferencesService>();
            this.MovementModule = GetComponent<PlayerMovementModule>();
            //this.CursorModule = GetComponent<CursorModule>();
            //this.PortalModule = GetComponent<PortalModule>();
            //this.CanvasesService = GetComponent<CanvasesManagerService>();
            //this.TextInformationService = GetComponent<TextInformationService>();
            //this.DontDestroyOnLoadService = GetComponent<DontDestroyOnLoadService>();
            this.PrefabReferencesService = GetComponent<PrefabReferencesService>();
            this.ObjectsPoolService = gameObject.GetComponent<ObjectsPoolService>();
            this.AnimationModule = GetComponent<PlayerAnimationModule>();
            this.AttributesModule = GetComponent<PlayerAttributesModule>();

            this.GameObjectInSceneReferencesService.Initialize();
            this.PrefabReferencesService.Initialize();
            this.ObjectsPoolService.Initialize();

            IDataInitializable[] dataServicesToInitialize = new IDataInitializable[]
            {
            this.ConfigurationService,
            (this.VideoService = new VideoService()),
            (this.InputService = new KeysService()),
            (this.LanguageService = new LanguageService()),
            (this.AudioService = new AudioService()),
            (this.GameService = new GameService()),
            };

            foreach (IDataInitializable dataService in dataServicesToInitialize)
                dataService.Initialize();

            this.AnimationModule.Initialize();

            this.AttributesModule.Initialize(this.Classe);

            AInitializableComponent[] servicesAndModulesComponentToInitialize =
            {
            this.MovementModule,
            this.AnimationModule,
            (this.CursorModule = GetComponent<CursorModule>()),
            (this.PortalModule = GetComponent<PortalModule>()),
            (this.CanvasesService = GetComponent<CanvasesManagerService>()),
            (this.TextInformationService = GetComponent<TextInformationService>()),
            (this.DontDestroyOnLoadService = GetComponent<DontDestroyOnLoadService>())
        };

            foreach (AInitializableComponent serviceOrModules in servicesAndModulesComponentToInitialize)
                serviceOrModules.Initialize();

            // Je réinitialise ici mon service d'attributs car le problème c'est que mes composants ont des dépendances entre-eux,
            // il faudrait que je rajoute une interface : initialize dependance.
            this.AttributesModule.Initialize(this.Classe);

            this.Updatables = new IDataUpdatable[]
            {
            this.GameService,
            };

            StartCoroutine(this.InitializeAtEndFrame());
            StartCoroutine(this.LoadAtFirstApplication());
            //  (entityAttributeSerializable, "attribute");
            //SerializerHelper.JsonDeserializeLoad<EntityAttributeSerializable>("attribute").Load(this.AttributesModule.GetAttribute(EEntityCategoriesAttributes.Life, "Maximum Life"));

            //DontDestroyOnLoad(base.Instan);
        }

        private IEnumerator InitializeAtEndFrame()
        {
            yield return new WaitForEndOfFrame();

            this.LanguageService.InitializeAtEndFrame();

            SceneManagerHelper.FirstApplicationLoad();
        }
        #endregion

        #region Tests
        void Update()
        {
            foreach (IDataUpdatable updatable in this.Updatables)
                updatable.Update();
        }

        private IEnumerator LoadAtFirstApplication()
        {
            yield return new WaitForSeconds(0.5f);

            SceneManagerHelper.FirstApplicationLoad();
        }
        #endregion

        #region Interface Behaviour
        // Charge seulement les fichiers spécifique à CHAQUE scène.
        void ISerializable.Load()
        {
            string positionAndRotationFilename = SceneServicesContainer.Instance.FileSystemConfiguration.PositionAndRotationFilename;

            if (SerializerHelper.DoesCompletSavePathExists(positionAndRotationFilename, ".json"))
                SerializerHelper.JsonDeserializeLoadWithEncryption<SerializablePositionAndRotation>(positionAndRotationFilename).Load(this.transform);
        }

        void ISerializable.Save()
        {
            SerializerHelper.JsonSerializeSaveWithEncryption<SerializablePositionAndRotation>(new SerializablePositionAndRotation(this.transform),
                SceneServicesContainer.Instance.FileSystemConfiguration.PositionAndRotationFilename);

            ((ISerializable)this.AttributesModule).Save();

            // Potentiellement les portails (on devra les instancier) je pense.
            // Waypoints, jobs.
            // Les données spécifiques des données d'attributs. : pour le moment juste le timer de mort.
        }

        public void FirstLoadApplication()
        {
            this.AttributesModule.SpecificLoadForPlayer();
            // Potentiellement les portails.
            // Waypoints, jobs.
            // Les données spécifiques des données d'attributs. : pour le moment juste le timer de mort.
        }
        #endregion
    }
}