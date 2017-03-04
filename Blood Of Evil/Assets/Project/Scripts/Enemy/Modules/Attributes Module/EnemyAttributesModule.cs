using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Enemies.Modules.Attributes
{
    using Scene;
    using Helpers;
    using Entities.Modules.Attributes;
    using Extensions;
    using Serialization;

    public sealed class EnemyAttributesModule : AEntityAttributesModule
    {
        #region Fields
        public const float PERCENTAGE_TO_UNIT = 0.01f;
        private EEnemyCategory category = EEnemyCategory.Normal;
        private float enemySize;

        public Action<float> EnemySizeListener;
        public bool EnemyCategoryHaveBeenLoad = false;
        #endregion

        #region Properties
        public Action<EEnemyCategory> EnemyCategoryListener;
        public EEnemyCategory Category
        {
            get
            {
                return category;
            }

            set
            {
                category = value;

                EnemyCategoryListener.SafeCall(category);

                this.EnemySize = GetEnemyCategorySize(this.category);

                this.EnemySizeListener.SafeCall(this.EnemySize);
            }
        }

        public float EnemySize
        {
            get
            {
                return enemySize;
            }

            set
            {
                enemySize = value;

                transform.localScale = Vector3.one * enemySize;
            }
        }
        #endregion

        #region Unity Behaviour
        void Update()
        {
            base.MyUpdate();
        }
        #endregion

        // Revoir
        #region Abstract Override Behaviour
        protected override string GetFileName()
        {
            return SceneServicesContainer.Instance.
                    FileSystemConfiguration.EnemyAttributesFilename(
                        GetComponent<EnemyServicesAndModulesContainer>().SaveIndex);
        }
        #endregion

        #region Public Behaviour
        public void LoadEEnemyCategory()
        {
            SerializerHelper.Load<SerializableEEnemyCategory>(
                filename: this.GetCategoryFilename(), 
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                onLoadSuccess: (SerializableEEnemyCategory data) =>
                {
                    this.EnemyCategoryHaveBeenLoad = true;
                    this.Category = data.Data;
                });
        }

        public void SaveEEnemyCategory()
        {
            SerializerHelper.Save<SerializableEEnemyCategory>(
                filename: this.GetCategoryFilename(),
                dataToSave: new SerializableEEnemyCategory(this.Category),
                isReplicatedNextTheBuild: false,
                isEncrypted: true);
        }

        public string GetCategoryFilename()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.EnemyCategoryFilename(
                GetComponent<EnemyServicesAndModulesContainer>().SaveIndex);
        }

        public void ReloadDefaultConfiguration()
        {
            this.InitializeAttributesValues();
        }
        #endregion

        #region Initialize
        public void InitializeAttributesModules()
        {
            base.Initialize();

            //((ISerializable)this).Load();
        }

        protected override void CreateAttributes()
        {
            base.CreateAttributes();

            base.entityCategoriesAttributes = new AEntityCategoryAttribute[]
            {
        (base.LifeCategoryAttributes = new EnemyLifeCategoryAttributes(this)),
        new EntityManaCategoryAttributes(this),
        new EnemyExperienceategoryAttributes(this),
        (base.AttackCategoryAttributes = new EntityAttackCategoryAttributes(this)),
        new EntityMovementCategoryAttributes(this),
        new EntityResistanceCategoryAttributes(this),
        new EntityDefenceCategoryAttributes(this),
        new EntityLootCategoryAttributes(this),
        new EntitySkillCategoryAttributes(this),

        new EntityEmptyCategoryAttributes(this), // doit-être en dernier pour pouvoir initializer les autres catégories d'attributs en fonction des charactéristiques du joueur.
            };
        }

        protected override void InitializeAttributesValues()
        {
            base.InitializeAttributesValues();
        }

        protected override void InitializeCallbacksAttributes()
        {
            base.InitializeCallbacksAttributes();
        }
        #endregion

        #region Intern Behaviour
        protected override string[] GetAttributesNamesFromACategory(EEntityCategoriesAttributes category)
        {
            string[] emptyStringArray = new string[] { };
            string[] baseAttributesNames = base.GetCommumAttributesNamesFromACategory(category);

            switch (category)
            {
                case EEntityCategoriesAttributes.Attack:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        emptyStringArray);

                case EEntityCategoriesAttributes.Characteristics:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        new string[]
                        {
                        });

                case EEntityCategoriesAttributes.Defence:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        emptyStringArray);

                case EEntityCategoriesAttributes.Experience:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        new string[]
                        {
                        });

                case EEntityCategoriesAttributes.Life:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        new string[]
                        {
                        });

                case EEntityCategoriesAttributes.Loot:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        emptyStringArray);

                case EEntityCategoriesAttributes.Mana:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        new string[]
                        {
                        });

                case EEntityCategoriesAttributes.Movement:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        new string[]
                        {
                        });

                case EEntityCategoriesAttributes.Resistances:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        new string[]
                        {
                        });

                case EEntityCategoriesAttributes.Skill:
                    return StringHelper.FusionStringArray(
                        baseAttributesNames,
                        emptyStringArray);

                default:
                    Debug.LogErrorFormat("The strings of category {} are not initialized", category);
                    return null;
            }
        }
        #endregion

        #region Helpers
        public static float GetEnemyCategorySize(EEnemyCategory enemyCategory)
        {
            return
                EEnemyCategory.Normal == enemyCategory ? 1.0f :
                EEnemyCategory.Champion == enemyCategory ? 1.5f :
                EEnemyCategory.Gozu == enemyCategory ? 1.35f :
                EEnemyCategory.Boss == enemyCategory ? 2.5f :
                EEnemyCategory.WorldBoss == enemyCategory ? 3.5f :
                0.5f; // gobelin
        }

        public static EEnemyCategory GetEnemyCategory()
        {
            float[] probabilities = new float[EnumerationHelper.Count<EEnemyCategory>()];

            probabilities[EnumerationHelper.GetIndex(EEnemyCategory.Normal)] = 1000.0f;
            probabilities[EnumerationHelper.GetIndex(EEnemyCategory.Champion)] = 200.0f;
            probabilities[EnumerationHelper.GetIndex(EEnemyCategory.Gozu)] = 120.0f;
            probabilities[EnumerationHelper.GetIndex(EEnemyCategory.Boss)] = 25.0f;
            probabilities[EnumerationHelper.GetIndex(EEnemyCategory.WorldBoss)] = 5.0f;
            probabilities[EnumerationHelper.GetIndex(EEnemyCategory.Gobelin)] = 12.5f;

            return ProbabilitHelper.ProbabilitiesToEnumeration<EEnemyCategory>(probabilities);
        }
        #endregion
    }
}