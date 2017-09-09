using UnityEngine;
using System.Collections;

//11 Attributs :
//Wisdom,           3% energy.                  : V
//Faith,            1-2 heal, 3% heal           : V 
//Resistance,       10 defence, 2% defence      : V
//Chance,           0.5% CC, 3% item find       : V
//Endurance,        25 life, 2% life            : V
//Power,            15 energy, 2% skill power   : V
//Dexterity,        15 accuracy, 2% accuracy    : V
//Spirit,           0.5 all resistances, 1.5% mana : V
//Constitution,     3% pet life and damage      : V
//All Characteristics,
//All Characteristics Percentage
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    [System.Serializable]
    public sealed class PlayerCharacteristicsCategoryAttributes : EntityEmptyCategoryAttributes
    {
        #region Constructor
        public PlayerCharacteristicsCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "All Characteristics Percentage").InitializeDefaultPercentage("Percentage Of All Characteristics");
            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "All Characteristics").Initialize("All Characteristics", 0.0f, base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "All Characteristics Percentage"));

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Remain Characteristics").Initialize("Remain Characteristics", 0.0f);

            //var configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.GetClass(PlayerServicesAndModulesContainer.Instance.Classe).CharacteristicAttributes;

            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Strength").Initialize("Strength", configuration.StrengthAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Power").Initialize("Power", configuration.PowerAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Dexterity").Initialize("Dexterity", configuration.DexterityAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Endurance").Initialize("Endurance", configuration.EnduranceAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Chance").Initialize("Chance", configuration.ChanceAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Faith").Initialize("Faith", configuration.FaithAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Resistance").Initialize("Resistance", configuration.ResistanceAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Wisdom").Initialize("Wisdom", configuration.WisdomAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").Initialize("Spirit", configuration.SpiritAtStart);
            //base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Constitution").Initialize("Constitution", configuration.ConstitutionAtStart);
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "All Characteristics").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Strength").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Power").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Dexterity").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Endurance").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Chance").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Faith").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Resistance").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Wisdom").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Spirit").SetOtherAttribute("Characteristic", input);
                base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Constitution").SetOtherAttribute("Characteristic", input);
            });
        }
        #endregion
    }
}