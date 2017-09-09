using UnityEngine;
using System.Collections;

//4 Attributs :
//Experience.
//Experience Percentage.
//Experience Additional Per Kill.
//Add Experience.
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Scene;
    using Entities.Modules.Attributes;

    using Services.Audio;

    [System.Serializable]
    public sealed class PlayerExperienceCategoryAttributes : EntityExperienceCategoryAttributes
    {
        #region Fields
        private const float maximumLevel = 150;
        private const float XP_ADDITIONAL_PER_LEVEL = 500.0f;
        private const float FACTOR_OF_XP_ADDITIONAL_PER_LEVEL = 1.2f;

        public static readonly float MAXIMUM_EXPERIENCE_AT_START = 200.0f;
        public static readonly float LEVEL_AT_START = 1;
        public static readonly float EXPERIENCE_AT_START = 0;
        #endregion

        #region Constructor
        public PlayerExperienceCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value = EXPERIENCE_AT_START;

            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience Percentage").InitializeDefaultPercentage("Percentage Of Experience");

            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Level").Initialize("Level", LEVEL_AT_START);
            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").Initialize("Maximum Experience", MAXIMUM_EXPERIENCE_AT_START);
            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Total Experience").Initialize("Total Experience");
            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience Additional Per Kill").Initialize("Experience Additionnal Per Kill");

            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Add Experience").Initialize("Add Experience", 0.0f,
                base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience Percentage"));
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Add Experience").Current.ValueListener(delegate (float input)
            {
                if (input != 0)
                {
                    float XPGain = (input + base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience Additional Per Kill").Current.Value);

                    base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").AtStart.Value += XPGain;
                    base.GetAttribute(EEntityCategoriesAttributes.Experience, "Total Experience").Current.Value += XPGain;

                    while (base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value >=
                            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").Current.Value)
                    {
                        if (base.GetAttribute(EEntityCategoriesAttributes.Experience, "Level").Current.Value >= maximumLevel)
                            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").AtStart.Value = base.GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").Current.Value - 0.1f;
                        else
                            this.LevelUp();
                    }

                    base.GetAttribute(EEntityCategoriesAttributes.Experience, "Add Experience").AtStart.Value = 0.0f;
                }
            });
        }
        #endregion

        #region Intern Behaviour
        private void LevelUp()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").AtStart.Value -= base.GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").Current.Value;
            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").Current.Value = base.GetAttribute(EEntityCategoriesAttributes.Experience, "Maximum Experience").Current.Value * FACTOR_OF_XP_ADDITIONAL_PER_LEVEL + XP_ADDITIONAL_PER_LEVEL;

            ++base.GetAttribute(EEntityCategoriesAttributes.Experience, "Level").Current.Value;
            //var configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.GetClass(PlayerServicesAndModulesContainer.Instance.Classe).CharacteristicAttributes;

            //// GIVE DEFINE ATTRIBUTE
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Strength").SetOtherAttributePlusEqual("Level Up", configuration.StrengthAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Power").SetOtherAttributePlusEqual("Level Up", configuration.PowerAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Dexterity").SetOtherAttributePlusEqual("Level Up", configuration.DexterityAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Endurance").SetOtherAttributePlusEqual("Level Up", configuration.EnduranceAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Chance").SetOtherAttributePlusEqual("Level Up", configuration.ChanceAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Faith").SetOtherAttributePlusEqual("Level Up", configuration.FaithAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Resistance").SetOtherAttributePlusEqual("Level Up", configuration.ResistanceAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Wisdom").SetOtherAttributePlusEqual("Level Up", configuration.WisdomAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").SetOtherAttributePlusEqual("Level Up", configuration.SpiritAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Constitution").SetOtherAttributePlusEqual("Level Up", configuration.ConstitutionAtLevelUp);

            // GIVE ATTRIBUTES REMAIN.
            // GIVE 1 SKILL REMAIN.
            base.attributeModule.LifeCategoryAttributes.ResetLifeToMaximumLife();

            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Player Level Up");
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Utile seulement pour la sérialisation.
        /// </summary>
        public void UnLevelUp()
        {
            //var configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.GetClass(PlayerServicesAndModulesContainer.Instance.Classe).CharacteristicAttributes;

            //--base.GetAttribute(EEntityCategoriesAttributes.Experience, "Level").Current.Value;

            //// GIVE DEFINE ATTRIBUTE
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Strength").SetOtherAttributeLessEqual("Level Up", configuration.StrengthAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Power").SetOtherAttributeLessEqual("Level Up", configuration.PowerAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Dexterity").SetOtherAttributeLessEqual("Level Up", configuration.DexterityAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Endurance").SetOtherAttributeLessEqual("Level Up", configuration.EnduranceAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Chance").SetOtherAttributeLessEqual("Level Up", configuration.ChanceAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Faith").SetOtherAttributeLessEqual("Level Up", configuration.FaithAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Resistance").SetOtherAttributeLessEqual("Level Up", configuration.ResistanceAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Wisdom").SetOtherAttributeLessEqual("Level Up", configuration.WisdomAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").SetOtherAttributeLessEqual("Level Up", configuration.SpiritAtLevelUp);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Constitution").SetOtherAttributeLessEqual("Level Up", configuration.ConstitutionAtLevelUp);
        }
        #endregion
    }
}