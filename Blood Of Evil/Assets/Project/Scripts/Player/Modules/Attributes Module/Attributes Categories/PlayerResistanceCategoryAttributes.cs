using UnityEngine;
using System.Collections;
using System;

// 22 Attributs ?
// Fire Resistance Percentage,     
// Cold Resistance Percentage,       
// Lighting Resistance Percentage,    
// Faith Resistance Percentage,       
// Wind Resistance Percentage,         
// Earth Resistance Percentage,       
// Poison Resistance Percentage,    
// Fire Resistance,     
// Cold Resistance,       
// Lighting Resistance,    
// Faith Resistance,       
// Wind Resistance,         
// Earth Resistance,       
// Poison Resistance,
// Fire Resistance Limitation,     
// Cold Resistance Limitation,       
// Lighting Resistance Limitation,    
// Faith Resistance Limitation,       
// Wind Resistance Limitation,         
// Earth Resistance Limitation,       
// Poison Resistance Limitation,    
// All Resistances
// All Resistances Percentage 
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    using Configuration;

    public sealed class PlayerResistanceCategoryAttributes : EntityResistanceCategoryAttributes
    {
        #region Fields
        private PlayerCharacteristicCategoryAttributeConfiguration configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.CharacteristicAttributes;

        private const float RESISTANCE_LIMITATION_AT_START = 75.0f;
        private const float RESISTANCE_LIMITATION_MAXIMUM = 95.0f; // IL FAUT LES IMPLEMENTER LORSQUE LE NOUVEAU SYSTEME DATTRIBUT SERA MODIFIABLE.
        #endregion

        #region Constructor
        public PlayerResistanceCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();

            for (int resistanceIndex = 0; resistanceIndex < ResistancesString.Length; resistanceIndex++)
            {
                string resistanceString = ResistancesString[resistanceIndex];

                base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance Limitation", resistanceString)).Initialize(string.Format("{0} Resistance Limitation", resistanceString), RESISTANCE_LIMITATION_AT_START);
            }

            base.GetAttribute(EEntityCategoriesAttributes.Resistances, "All Resistances Percentage").InitializeDefaultPercentage("All Resistances Percentage");
            base.GetAttribute(EEntityCategoriesAttributes.Resistances, "All Resistances").Initialize("All Resistances Percentage", 0.0f, base.GetAttribute(EEntityCategoriesAttributes.Resistances, "All Resistances Percentage"));
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Resistances, "All Resistances").SetOtherAttribute("Characteristic", input * configuration.AllResistancePerSpirit);
            });

            for (int resistanceIndex = 0; resistanceIndex < ResistancesString.Length; resistanceIndex++)
            {
                string resistanceString = ResistancesString[resistanceIndex];

                base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance Limitation", resistanceString)).Current.ValueListener(delegate (float input)
                {
                    if (input > RESISTANCE_LIMITATION_MAXIMUM)
                        base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance Limitation", resistanceString)).Current.Value = RESISTANCE_LIMITATION_MAXIMUM;
                });

                base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance", resistanceString)).Current.ValueListener(delegate (float input)
                {
                    if (input > base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance Limitation", resistanceString)).Current.Value)
                        base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance", resistanceString)).Current.Value = base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance Limitation")).Current.Value;
                });
            }

            base.GetAttribute(EEntityCategoriesAttributes.Resistances, "All Resistances").Current.ValueListener(delegate (float input)
            {
                for (int resistanceIndex = 0; resistanceIndex < ResistancesString.Length; resistanceIndex++)
                    base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance", ResistancesString[resistanceIndex])).SetOtherAttribute("All Resistances", input);
            });
        }
        #endregion
    }
}