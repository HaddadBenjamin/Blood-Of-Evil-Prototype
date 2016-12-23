using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    using Configuration;

    //6 Attributs :
    //Mana,
    //Maximum Mana
    //Mana Percentage,
    //Mana Regenerate Per Second,
    //Mana Regenerate Per Second Percentage,
    //Percentage Of Mana Regenerate Per Second : ex pour 5; j'ai 1000 vie, je regenerate 1000 * 0.05 vie.
    [System.Serializable]
    public sealed class PlayerManaCategoryAttributes : EntityManaCategoryAttributes
    {
        #region Fields
        private PlayerCharacteristicCategoryAttributeConfiguration configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.CharacteristicAttributes;

        private EntityAttributes manaRegeneratePerSecondPercentageAttributes;
        private EntityAttributes manaRegeneratePerSecondAttributes;
        private EntityAttributes percentageOfManaRegeneratePerSecondAttributes;
        #endregion

        #region Constructor
        public PlayerManaCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();

            this.manaRegeneratePerSecondPercentageAttributes.InitializeDefaultPercentage("Percentage Of Mana Regenerated Per Second");
            this.manaRegeneratePerSecondAttributes.Initialize("Mana Regenerated Per Second", 1.0f, base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana Regenerate Per Second Percentage"));
            this.percentageOfManaRegeneratePerSecondAttributes.Initialize("Percentage Of Mana Regenerated Per Second", 1.0f, base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana Regenerate Per Second Percentage"));

            this.ResetManaToMaximumMana();
        }

        public override void CreateCallbacksAttributes()
        {
            this.manaRegeneratePerSecondAttributes = base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana Regenerate Per Second");
            this.manaRegeneratePerSecondPercentageAttributes = base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana Regenerate Per Second Percentage");
            this.percentageOfManaRegeneratePerSecondAttributes = base.GetAttribute(EEntityCategoriesAttributes.Mana, "Percentage Of Mana Regenerate Per Second");

            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Power").Current.ValueListener(delegate (float input)
            {
                base.MaximumManaAttributes.SetOtherAttribute("Characteristic", input * configuration.EnergyPerPower);
            });

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").Current.ValueListener(delegate (float input)
            {
                this.UpdateValueToAddToManaPercentage();
            });

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Wisdom").Current.ValueListener(delegate (float input)
            {
                this.UpdateValueToAddToManaPercentage();
            });
        }
        #endregion

        #region Unity Behaviour
        public override void Update()
        {
            this.RegenerateMana();
        }
        #endregion

        #region Intern Behabiour 
        private void UpdateValueToAddToManaPercentage()
        {
            base.ManaPercentageAttributes.SetOtherAttribute("Characteristic",
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").Current.Value * configuration.ManaPercentagePerSpirit +
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Wisdom").Current.Value * configuration.EnergyPerWisdom);
        }

        private void RegenerateMana()
        {
            float currentMana = base.ManaAttributes.Current.Value;

            if (currentMana < base.MaximumManaAttributes.Current.Value)
                base.ManaAttributes.Current.Value +=
                    this.manaRegeneratePerSecondAttributes.Current.Value * Time.deltaTime +
                    this.percentageOfManaRegeneratePerSecondAttributes.Current.Value * Time.deltaTime;
        }
        #endregion
    }
}