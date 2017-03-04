using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using BloodOfEvil.Utilities.Serialization;


// <<<<<<<<<<- A mettre dans à savoir de mon trello ->>>>>>>

// Penser à mettre à jour : GetAttributesNamesFromACategory.
// Lorsqu'il y a un crash tenter cette procédure :
// Vérifier que la catégorie du GetAttribute est conforme : exemple GetAttribute(Life, "Mana") n'est pas conforme.
// Vérifier que le nom de l'attributs est bien répertorié GetAttributesNamesFromACategory et qu'il est dans la bonne catégorie.
// Afficher les noms des attributs dans la méthode GetAttribute avec un Debug.Log

// Pour la sérialization des catégories d'attributs, pour le moment elle n'est pas pertinente car il n'y a pas de données de type non float qui ont un intérêt à être sauvegarder.
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Scene;
    using Helpers;
    using ObjectInScene;

    using Entities.Modules.Attributes;
    using Entities.Modules.Attributes.Serialization;

    public sealed class PlayerAttributesModule : AEntityAttributesModule
    {
        #region Fields
        private EPlayerClass classe;
        public const float PERCENTAGE_TO_UNIT = 0.01f;
        private SerializableFloatArray characteristicsPointsAddedButNotApplied = new SerializableFloatArray(
            EnumerationHelper.Count<EAttributeCharacteristics>());
        #endregion

        #region Properties
        public EPlayerClass Classe
        {
            get { return classe; }
            private set { classe = value; }
        }
        #endregion

        #region Unity Behaviour
        void Update()
        {
            base.MyUpdate();
        }
        #endregion

        #region Abstract Override Behaviour
        protected override string GetFileName()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.AttributesFilename;
        }
        #endregion

        #region Public Behaviour
        public void ReloadDefaultConfiguration()
        {
            // Je fais 2 fois la même chose mais sans ça cela ne marche pas, pourquoi ?
            this.ClearOtherAttributesFromAllAttributesCategories();
            this.InitializeAttributesValues();

            this.ClearOtherAttributesFromAllAttributesCategories();
            this.InitializeAttributesValues();
        }
        #endregion

        #region Get Attributes

        #endregion

        #region Initialize
        public void Initialize(EPlayerClass classe)
        {
            this.classe = classe;

            base.Initialize();

            //this.SpecificLoadForPlayer();
        }


        protected override void CreateAttributes()
        {
            base.CreateAttributes();

            base.entityCategoriesAttributes = new AEntityCategoryAttribute[]
            {
            (base.LifeCategoryAttributes = new PlayerLifeCategoryAttributes(this)),
            new PlayerManaCategoryAttributes(this),
            new PlayerExperienceCategoryAttributes(this),
            (base.AttackCategoryAttributes = new PlayerAttackCategoryAttributes(this)),
            new PlayerMovementSpeedCategoryAttributes(this),
            new PlayerResistanceCategoryAttributes(this),
            new PlayerDefenceCategoryAttributes(this),
            new PlayerLootCategoryAttributes(this),
            new PlayerSkillCategoryAttributes(this),

            new PlayerCharacteristicsCategoryAttributes(this), // doit-être en dernier pour pouvoir initializer les autres catégories d'attributs en fonction des charactéristiques du joueur.
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
                        "Strength",
                        "Wisdom",
                        "Faith",
                        "Resistance",
                        "Chance",
                        "Endurance",
                        "Power",
                        "Dexterity",
                        "Spirit",
                        "Constitution",
                        "All Characteristics",
                        "All Characteristics Percentage",
                        "Remain Characteristics",
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
                        "Experience Percentage",
                        "Experience Additional Per Kill",
                        "Add Experience",
                        "Maximum Experience",
                        "Total Experience",
                        "Level"
                        });

                case EEntityCategoriesAttributes.Life:
                    return StringHelper.FusionStringArray(
                       baseAttributesNames,
                       new string[]
                        {
                        "Life Regenerate Per Second",
                        "Life Regenerate Per Second Percentage",
                        "Percentage Of Life Regenerate Per Second"
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
                        "Mana Regenerate Per Second",
                        "Mana Regenerate Per Second Percentage",
                        "Percentage Of Mana Regenerate Per Second"
                        });

                case EEntityCategoriesAttributes.Movement:
                    return StringHelper.FusionStringArray(
                       baseAttributesNames,
                       new string[]
                       {
                        "Steps"
                        });

                case EEntityCategoriesAttributes.Resistances:
                    return StringHelper.FusionStringArray(
                       baseAttributesNames,
                       new string[]
                       {
                        "Fire Resistance Limitation",
                        "Cold Resistance Limitation",
                        "Lighting Resistance Limitation",
                        "Faith Resistance Limitation",
                        "Wind Resistance Limitation",
                        "Earth Resistance Limitation",
                        "Poison Resistance Limitation",
                        "All Resistances",
                        "All Resistances Percentage"
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

        #region Public Behaviour
        public void AddCharacteristicsPointsAddedButNotApplied(EAttributeCharacteristics characteristic)
        {
            this.characteristicsPointsAddedButNotApplied.floatArray[
                EnumerationHelper.GetIndex<EAttributeCharacteristics>(characteristic)] += 1;
        }

        public void RemoveCharacteristicsPointsAddedButNotApplied(EAttributeCharacteristics characteristic)
        {
            this.characteristicsPointsAddedButNotApplied.floatArray[
                EnumerationHelper.GetIndex<EAttributeCharacteristics>(characteristic)] -= 1;
        }

        public float GetCharacteristicsPointsAddedButNotApplied(EAttributeCharacteristics characteristic)
        {
            return this.characteristicsPointsAddedButNotApplied.floatArray[
                EnumerationHelper.GetIndex<EAttributeCharacteristics>(characteristic)];
        }

        /// <summary>
        /// Sauvegarde les points de charactéristiques ajoutés mais non appliquer.
        /// </summary>
        public void SaveCharacteristicsPointsAddedButNotApplied()
        {
            SerializerHelper.Save<SerializableFloatArray>(
                filename: this.GetCharacteristicsPointsAddedTemporaryPointsAddedFilename(),
                dataToSave: this.characteristicsPointsAddedButNotApplied,
                isReplicatedNextTheBuild: false,
                isEncrypted: true);
        }

        public void LoadCharacteristicsPointsAddedButNotApplied()
        {
            SerializerHelper.Load<SerializableFloatArray>(
                filename: this.GetCharacteristicsPointsAddedTemporaryPointsAddedFilename(),
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                onLoadSuccess: (SerializableFloatArray playerCharactericticAdded) =>
                {
                    this.characteristicsPointsAddedButNotApplied = playerCharactericticAdded;
                });
        }

        // Ce code est sal et non maintenable mais il permet d'éviter d'avoir des charactéristiques et des sorts en trop et d'avoir une expérience correctement setté.
        // C'est sal !
        public void SpecificLoadForPlayer()
        {
            this.LoadCharacteristicsPointsAddedButNotApplied();

            SerializerHelper.Load<EntityAttributesArrayOfArraySerializable>(
                filename: this.GetFileName(),
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                onLoadSuccess: (EntityAttributesArrayOfArraySerializable attributesSerializable) =>
                {
                    this.ClearOtherAttributesFromAllAttributesCategories();

                    int experienceCategoryIndex =
                        EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(EEntityCategoriesAttributes.Experience);
                    int lifeCategoryIndex =
                        EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(EEntityCategoriesAttributes.Life);
                    int manaCategoryIndex =
                        EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(EEntityCategoriesAttributes.Mana);

                    float totalExperienceOfSerialization =
                        attributesSerializable.EntityAttributesArrayOfArray[experienceCategoryIndex].
                            EntityAttributeArray[
                                ObjectContainerHelper.GetHashCodeIndex("Total Experience",
                                    base.attributeHashIds[experienceCategoryIndex])].Current.Value;
                    float currentLifeOfSerialization =
                        attributesSerializable.EntityAttributesArrayOfArray[lifeCategoryIndex].
                            EntityAttributeArray[
                                ObjectContainerHelper.GetHashCodeIndex("Life", base.attributeHashIds[lifeCategoryIndex])
                            ].Current.Value;
                    float currentManaOfSerialization =
                        attributesSerializable.EntityAttributesArrayOfArray[manaCategoryIndex].
                            EntityAttributeArray[
                                ObjectContainerHelper.GetHashCodeIndex("Mana", base.attributeHashIds[manaCategoryIndex])
                            ].Current.Value;

                    attributesSerializable.Load(this.attributes);

                    GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value =
                        PlayerExperienceCategoryAttributes.EXPERIENCE_AT_START;
                    GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").Current.Value =
                        PlayerExperienceCategoryAttributes.MAXIMUM_EXPERIENCE_AT_START;
                    GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").AtStart.Value =
                        PlayerExperienceCategoryAttributes.EXPERIENCE_AT_START;
                    GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").AtStart.Value =
                        PlayerExperienceCategoryAttributes.MAXIMUM_EXPERIENCE_AT_START;

                    PlayerExperienceCategoryAttributes experienceCategory =
                        (PlayerExperienceCategoryAttributes) this.entityCategoriesAttributes[2];

                    while ((int) GetAttribute(EEntityCategoriesAttributes.Experience, "Level").Current.Value >
                           (int) PlayerExperienceCategoryAttributes.LEVEL_AT_START)
                        experienceCategory.UnLevelUp();

                    GetAttribute(EEntityCategoriesAttributes.Experience, "Add Experience").Current.Value =
                        totalExperienceOfSerialization;
                    GetAttribute(EEntityCategoriesAttributes.Experience, "Total Experience").Current.Value =
                        totalExperienceOfSerialization;
                    GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.Value = currentLifeOfSerialization;
                    GetAttribute(EEntityCategoriesAttributes.Mana, "Mana").Current.Value = currentManaOfSerialization;

                    // Ici il faudrait reload les charactéristiques et les sorts pour éviter de redevoir mettre les points restant suite à une monté de niveau.
                    // Il faudrait aussi fermer ces fenêtres si on les ouvre dans le script de level up ou sinon le faire dans le unlevel up.

                });
        }
        #endregion

        #region Intern Behaviour
        private string GetCharacteristicsPointsAddedTemporaryPointsAddedFilename()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.CharacteristicsPointsAddedButNotApplyedFilename;
        }
        #endregion
    }
}       