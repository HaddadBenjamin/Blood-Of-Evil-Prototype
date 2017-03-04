using UnityEngine;
using System.Collections;

// Ce script fait beaucoup trop de choses : gestion d'UI et de comportement, je devrai le découper en MVC, il faudrait que j'utilise des délégates.
namespace BloodOfEvil.Enemies
{
    using Modules.Attributes;
    using Entities.Modules.Attributes;

    public class EnemyCategoryGenerationModule : MonoBehaviour
    {
        #region Fields
        private EnemyAttributesModule attributeModule;

        [SerializeField]
        private bool categoryIsForced = false;
        [SerializeField]
        private EEnemyCategory enemyCategoryForce;
        [SerializeField]
        private GameObject iconNormal = null, iconBoss = null, iconGobelin = null;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.attributeModule = GetComponent<EnemyServicesAndModulesContainer>().Instance.AttributesModule;
            this.UpdateMinimapICon();

            if (!this.attributeModule.EnemyCategoryHaveBeenLoad)
                this.Generate();
        }
        #endregion

        #region Intern Behaviour
        private void Generate()
        {
            this.attributeModule.Category = this.categoryIsForced ?
                                            this.enemyCategoryForce :
                                            EnemyAttributesModule.GetEnemyCategory();

            this.UpdateMinimapICon();

            this.attributeModule.ReloadDefaultConfiguration();

            this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Range").Current.Value *= this.attributeModule.EnemySize;

            this.GenerateEnemyAttributes();
            this.attributeModule.LifeCategoryAttributes.ResetLifeToMaximumLife();
        }

        private void GenerateEnemyAttributes()
        {
            this.UpdateMinimapICon();

            switch (this.attributeModule.Category)
            {
                case EEnemyCategory.Champion:
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").Current.Value *= 1.25f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage").Current.Value *= 2.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value *= 3.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage").Current.Value *= 1.4f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").Current.Value *= 1.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Current.Value *= 1.25f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Gold Percentage").Current.Value *= 3.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Item Percentage").Current.Value *= 3.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Magic Find Item Percentage").Current.Value *= 1.2f;
                    break;

                case EEnemyCategory.Gozu:
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").Current.Value *= 1.85f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage").Current.Value *= 1.75f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value *= 4.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage").Current.Value *= 1.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").Current.Value *= 1.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Current.Value *= 1.25f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Movement, "Movement Speed Percentage").Current.Value *= 1.25f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Gold Percentage").Current.Value *= 4.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Item Percentage").Current.Value *= 1.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Magic Find Item Percentage").Current.Value *= 3.5f;
                    break;

                case EEnemyCategory.Boss:
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").Current.Value *= 1.35f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage").Current.Value *= 5.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value *= 12.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage").Current.Value *= 2.4f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").Current.Value *= 1.75f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Current.Value *= 1.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Gold Percentage").Current.Value *= 12.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Item Percentage").Current.Value *= 5.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Magic Find Item Percentage").Current.Value *= 4.0f;
                    break;

                case EEnemyCategory.WorldBoss:
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").Current.Value *= 1.8f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage").Current.Value *= 20.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value *= 40.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage").Current.Value *= 4.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").Current.Value *= 2.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Current.Value *= 2.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Gold Percentage").Current.Value *= 50.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Item Percentage").Current.Value *= 15.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Magic Find Item Percentage").Current.Value *= 8.0f;
                    break;

                case EEnemyCategory.Gobelin:
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage").Current.Value *= 5.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage").Current.Value *= 10.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value *= 20.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Damage Percentage").Current.Value *= 1.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Dexterity").Current.Value *= 2.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Current.Value *= 1.5f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Movement, "Movement Speed Percentage").Current.Value *= 8.00f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Gold Percentage").Current.Value *= 40.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Item Percentage").Current.Value *= 10.0f;
                    this.attributeModule.GetAttribute(EEntityCategoriesAttributes.Loot, "Magic Find Item Percentage").Current.Value *= 8.0f;
                    //Gold Amount Gold
                    break;
            }
        }

        /// <summary>
        /// Active la bonne icône à afficher sur la minimap.
        /// </summary>
        private void UpdateMinimapICon()
        {
            this.iconGobelin.SetActive(false);
            this.iconBoss.SetActive(false);
            this.iconNormal.SetActive(false);

            if (EEnemyCategory.Normal == this.attributeModule.Category ||
                EEnemyCategory.Champion == this.attributeModule.Category ||
                EEnemyCategory.Gozu == this.attributeModule.Category)
                this.iconBoss.SetActive(true);
            else if (EEnemyCategory.Boss == this.attributeModule.Category ||
                     EEnemyCategory.WorldBoss == this.attributeModule.Category)
                this.iconBoss.SetActive(true);
            else if (EEnemyCategory.Gobelin == this.attributeModule.Category)
                this.iconGobelin.SetActive(true);
        }
        #endregion
    }
}