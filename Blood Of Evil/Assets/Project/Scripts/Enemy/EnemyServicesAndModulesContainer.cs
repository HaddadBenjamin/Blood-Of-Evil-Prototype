using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Enemies
{
    using Scene;
    using Helpers;
    using ObjectInScene;
    using Utilities.Serialization;

    using Modules.Animations;
    using Modules.Attributes;
    using Modules.IA;

    public sealed class EnemyServicesAndModulesContainer : MonoBehaviour, ISerializable
    {
        #region Fields
        /// <summary>
        /// Méthode vérifiant que tous les services et modules de l'enemi sont initializées.
        /// </summary>
        public EnemyServicesAndModulesContainer Instance
        {
            get
            {
                if (null == this.AttributesModule)
                    this.Initialize();

                return this;
            }
        }

        public EnemyAnimationModule AnimationModule { get; private set; }
        public EnemyAttributesModule AttributesModule { get; private set; }
        public EnemyCategoryGenerationModule CategoryGenerationModule { get; private set; }
        public EnemyIAModule IAModule { get; private set; }

        //private IDataUpdatable[] Updatables;

        private Transform myTransform;
        private int saveIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Seul le SceneState doit pouvoir y toucher.
        /// </summary>
        public int SaveIndex
        {
            get
            {
                return saveIndex;
            }

            set
            {
                saveIndex = value;
            }
        }
        #endregion

        #region Intern Behaviour
        private void Initialize()
        {
            this.myTransform = transform;
            this.AttributesModule = GetComponent<EnemyAttributesModule>();
            this.AttributesModule.InitializeAttributesModules();

            AInitializableComponent[] initializableComponents = new AInitializableComponent[]
            {
            (this.AnimationModule = GetComponent<EnemyAnimationModule>()),
            (this.IAModule = GetComponent<EnemyIAModule>()),
            };

            foreach (AInitializableComponent initializableComponent in initializableComponents)
                initializableComponent.Initialize();

            this.CategoryGenerationModule = GetComponent<EnemyCategoryGenerationModule>();

            //this.Updatables = new IDataUpdatable[]
            //{
            //    this.BehaviourTree,
            //};
        }

        public string GetPositionAndRotationFilename()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.EnemyPositionAndRotationFilename(this.SaveIndex);
        }
        #endregion

        #region Unity Behaviour
        //void Update()
        //{
        //    foreach (IDataUpdatable updatable in this.Updatables)
        //        updatable.Update();
        //}
        #endregion

        #region Interface Behaviour
        void ISerializable.Load()
        {
            this.AttributesModule.LoadEEnemyCategory(); // Charge juste la catégorie de l'ennemi.

            ((ISerializable)this.AttributesModule).Load(); // Charge tous les attributs.

            SerializerHelper.Load<SerializablePositionAndRotation>(
                filename: this.GetPositionAndRotationFilename(),
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                onLoadSuccess: (SerializablePositionAndRotation data) =>
                {
                    data.Load(this.myTransform);
                });

            // Il ne faut pas load son IA ici, c'est son initialisation qui doit l'appeler.
            //((ISerializable)this.IAModule).Load();
        }

        void ISerializable.Save()
        {
            this.AttributesModule.SaveEEnemyCategory(); // Sauvegarde juste la catégorie de l'ennemi.
            ((ISerializable)this.AttributesModule).Save(); // Sauvegarde tous les attributs.

            SerializerHelper.Save<SerializablePositionAndRotation>(
                filename: this.GetPositionAndRotationFilename(),
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                dataToSave: new SerializablePositionAndRotation(this.myTransform));

            ((ISerializable)this.IAModule).Save();
        }
        #endregion
    }
}