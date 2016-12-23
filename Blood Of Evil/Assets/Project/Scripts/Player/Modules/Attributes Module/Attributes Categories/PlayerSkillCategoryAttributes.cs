   using UnityEngine;
using System.Collections;

// 6 Attributs : 
//Skill Effect Percentage
//Heal Effect Percentage
//Minion Life Percentage
//Minion Damage Percentage
//Minimal Heal,
//Maximal Heal,
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    using Configuration;

    [System.Serializable]
    public sealed class PlayerSkillCategoryAttributes : EntitySkillCategoryAttributes
    {
        #region Constructor
        public PlayerSkillCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Fields
        private PlayerCharacteristicCategoryAttributeConfiguration configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.CharacteristicAttributes;
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Faith").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Skill, "Heal Effect Percentage").SetOtherAttribute("Characteristic", input * configuration.HealEffectPercentageFaith);
                base.GetAttribute(EEntityCategoriesAttributes.Skill, "Minimal Heal").SetOtherAttribute("Characteristic", input * configuration.MinimalHealPerFaith);
                base.GetAttribute(EEntityCategoriesAttributes.Skill, "Maximal Heal").SetOtherAttribute("Characteristic", input * configuration.MaximalHealPerFaith);
            });

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Power").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Skill, "Skill Effect Percentage").SetOtherAttribute("Characteristic", input * configuration.SkillEffectPerPower);
            });

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Constitution").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Skill, "Minion Life Percentage").SetOtherAttribute("Characteristic", input * configuration.PetLifePercentagePerConstitution);
                base.GetAttribute(EEntityCategoriesAttributes.Skill, "Minion Damage Percentage").SetOtherAttribute("Characteristic", input * configuration.PetDamagePercentagePerConstitution);
            });
        }
        #endregion
    }
}