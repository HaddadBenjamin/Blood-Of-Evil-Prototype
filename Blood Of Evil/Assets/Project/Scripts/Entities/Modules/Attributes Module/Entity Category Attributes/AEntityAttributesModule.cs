using UnityEngine;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    using Helpers;
    using ObjectInScene;

    using Serialization;

    public abstract class AEntityAttributesModule : MonoBehaviour, ISerializable
    {
        #region Fields
        // Ce n'est pas une hashmap pour raison de performance avec 2 ID, je vais suffisament vite même si ce n'est pas très pratique.
        // En effet la hashmap est un tableau de liste, ce qui est plus long qu'un tableau de tableau.
        // [Index est un EEntityAttributes, l'accès est rapide][Index est une string hashé, demande une conversion en int puis une recherhe]
        protected EntityAttributes[][] attributes;
        protected int[][] attributeHashIds;
        protected AEntityCategoryAttribute[] entityCategoriesAttributes;

        private EntityLifeCategoryAttributes lifeCategoryAttributes;
        private EntityAttackCategoryAttributes attackCategoryAttributes;

        [SerializeField]
        private EEntity entityType;
        [SerializeField]
        private EEntity targetType;
        #endregion

        #region Properties
        public EntityLifeCategoryAttributes LifeCategoryAttributes
        {
            get
            {
                return lifeCategoryAttributes;
            }

            protected set
            {
                lifeCategoryAttributes = value;
            }
        }

        public EntityAttackCategoryAttributes AttackCategoryAttributes
        {
            get
            {
                return attackCategoryAttributes;
            }

            protected set
            {
                attackCategoryAttributes = value;
            }
        }

        public EEntity EntityType
        {
            get
            {
                return entityType;
            }

            private set
            {
                entityType = value;
            }
        }

        public EEntity TargetType
        {
            get
            {
                return targetType;
            }

            private set
            {
                targetType = value;
            }
        }
        #endregion

        #region Interfaces Behaviour
        void ISerializable.Load()
        {
            SerializerHelper.Load<EntityAttributesArrayOfArraySerializable>(
                filename: this.GetFileName(),
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                onLoadSuccess: (EntityAttributesArrayOfArraySerializable attributesSerialized) =>
                {
                    this.ClearOtherAttributesFromAllAttributesCategories();

                    int lifeCategoryIndex = EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(EEntityCategoriesAttributes.Life);
                    float currentLifeOfSerialization = attributesSerialized.EntityAttributesArrayOfArray[lifeCategoryIndex].
                        EntityAttributeArray[ObjectContainerHelper.GetHashCodeIndex("Life", this.attributeHashIds[lifeCategoryIndex])].Current.Value;

                    attributesSerialized.Load(this.attributes);

                    GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.Value = currentLifeOfSerialization;
                });
        }

        void ISerializable.Save()
        {
            SerializerHelper.Save< EntityAttributesArrayOfArraySerializable>(
                filename: this.GetFileName(),
                dataToSave: new EntityAttributesArrayOfArraySerializable(this.attributes),
                isReplicatedNextTheBuild: false,
                isEncrypted: true);
        }
        #endregion

        #region Abstract Behaviour
        protected abstract string GetFileName();
        protected abstract string[] GetAttributesNamesFromACategory(EEntityCategoriesAttributes category);
        #endregion

        #region Virtual Behaviour
        protected virtual void CreateAttributes()
        {
            this.attributeHashIds = new int[EnumerationHelper.Count<EEntityCategoriesAttributes>()][];
            this.attributes = new EntityAttributes[EnumerationHelper.Count<EEntityCategoriesAttributes>()][];

            for (int i = 0; i < EnumerationHelper.EnumerationToEnumerationValuesArray<EEntityCategoriesAttributes>().Length; i++)
            {
                EEntityCategoriesAttributes categoryEnumeration = EnumerationHelper.EnumerationToEnumerationValuesArray<EEntityCategoriesAttributes>()[i];

                // Penser à mettre à jour : GetAttributesNamesFromACategory dès que le vous rajouter un attribut.
                ObjectContainerHelper.InitializeHashIds(this.GetAttributesNamesFromACategory(categoryEnumeration),
                    ref this.attributeHashIds[EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(categoryEnumeration)]);

                int numberOfAttributesPerCategory = this.GetAttributesNamesFromACategory(categoryEnumeration).Length;

                this.attributes[EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(categoryEnumeration)] = new EntityAttributes[numberOfAttributesPerCategory];

                for (int attributeIndex = 0; attributeIndex < numberOfAttributesPerCategory; attributeIndex++)
                    this.attributes[EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(categoryEnumeration)][attributeIndex] = new EntityAttributes();
            }
        }

        protected virtual void InitializeAttributesValues()
        {
            for (int categoryAttributeIndex = 0; categoryAttributeIndex < this.entityCategoriesAttributes.Length; categoryAttributeIndex++)
                this.entityCategoriesAttributes[categoryAttributeIndex].InitialzeAttributes();
        }

        protected virtual void InitializeCallbacksAttributes()
        {
            for (int categoryAttributeIndex = 0; categoryAttributeIndex < this.entityCategoriesAttributes.Length; categoryAttributeIndex++)
                this.entityCategoriesAttributes[categoryAttributeIndex].CreateCallbacksAttributes();
        }
        #endregion

        #region Public Behaviour
        public EntityAttributes[] GetCategoryAttribute(EEntityCategoriesAttributes category)
        {
            return this.attributes[EnumerationHelper.GetIndex<EEntityCategoriesAttributes>(category)];
        }

        public EntityAttributes GetAttribute(EEntityCategoriesAttributes category, string attributeID)
        {
            //Debug.LogFormat("ID : {0}, Category : {1}", attributeID, category);

            return this.GetCategoryAttribute(category)
                [ObjectContainerHelper.GetHashCodeIndex(attributeID, this.attributeHashIds[EnumerationHelper.GetIndex(category)])];
        }

        public void ClearOtherAttributesFromAllAttributesCategories()
        {
            for (int categoryIndex = 0; categoryIndex < this.attributes.Length; categoryIndex++)
            {
                EntityAttributes[] category = this.attributes[categoryIndex];

                for (int attributeIndex = 0; attributeIndex < category.Length; attributeIndex++)
                    category[attributeIndex].ClearOtherAttributes();
            }
        }
        #endregion

        #region Unherited Behaviour
        protected void MyUpdate()
        {
            for (int categoryAttributeIndex = 0; categoryAttributeIndex < this.entityCategoriesAttributes.Length; categoryAttributeIndex++)
                this.entityCategoriesAttributes[categoryAttributeIndex].Update();
        }

        /// <summary>
        ///  Contient les attributs commums entre le joueur et tous les enemmies.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        protected string[] GetCommumAttributesNamesFromACategory(EEntityCategoriesAttributes category)
        {
            switch (category)
            {
                case EEntityCategoriesAttributes.Attack:
                    return new string[]
                    {
                   "Damage Percentage",
                   "Attack Speed Percentage",
                   "Critical Chance Percentage",
                   "Critical Damage Percentage",
                   "Dexterity Percentage",
                   "Minimal Damage",
                   "Maximal Damage",
                   "Dexterity",
                   "Attack Range",
                   "Attack Speed"
                    };

                case EEntityCategoriesAttributes.Characteristics:
                    return new string[]
                    {
                    };

                case EEntityCategoriesAttributes.Defence:
                    return new string[]
                    {
                    "Defence",
                    "Defence Percentage"
                    };

                case EEntityCategoriesAttributes.Experience:
                    return new string[]
                    {
                    "Experience",
                    };

                case EEntityCategoriesAttributes.Life:
                    return new string[]
                    {
                    "Life",
                    "Maximum Life",
                    "Life Percentage",
                    };

                case EEntityCategoriesAttributes.Loot:
                    return new string[]
                    {
                    "Find Item Percentage",
                    "Magic Find Item Percentage",
                    "Find Gold Percentage",
                    "Gold"
                    };

                case EEntityCategoriesAttributes.Mana:
                    return new string[]
                    {
                    "Mana",
                    "Maximum Mana",
                    "Mana Percentage",
                    };

                case EEntityCategoriesAttributes.Movement:
                    return new string[]
                    {
                    "Movement Speed Percentage",
                    };

                case EEntityCategoriesAttributes.Resistances:
                    return new string[]
                    {
                    "Fire Resistance Percentage",
                    "Cold Resistance Percentage",
                    "Lighting Resistance Percentage",
                    "Faith Resistance Percentage",
                    "Wind Resistance Percentage",
                    "Earth Resistance Percentage",
                    "Poison Resistance Percentage",
                    "Fire Resistance",
                    "Cold Resistance",
                    "Lighting Resistance",
                    "Faith Resistance",
                    "Wind Resistance",
                    "Earth Resistance",
                    "Poison Resistance",
                    };

                case EEntityCategoriesAttributes.Skill:
                    return new string[]
                    {
                    "Skill Effect Percentage",
                    "Heal Effect Percentage",
                    "Minion Life Percentage",
                    "Minion Damage Percentage",
                    "Minimal Heal",
                    "Maximal Heal"
                    };

                default:
                    Debug.LogErrorFormat("The strings of category {} are not initialized", category);
                    return null;
            }
        }

        protected void Initialize()
        {
            if (null == this.attributes)
            {
                this.CreateAttributes();

                this.InitializeCallbacksAttributes();

                this.InitializeAttributesValues();
            }
        }
        #endregion
    }
}