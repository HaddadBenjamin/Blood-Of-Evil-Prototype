using UnityEngine;
using System.Collections;

// 3 Attributs :
//Find Item Percentage,
//Magic Find Item Percentage,
//Find Gold Percentage,
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    using Configuration;

    [System.Serializable]
    public sealed class PlayerLootCategoryAttributes : EntityLootCategoryAttributes
    {
        #region Constructor
        public PlayerLootCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
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

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Chance").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Item Percentage").SetOtherAttribute("Characteristic", input * configuration.ChanceToFindRareItemPerChance);
            });
        }
        #endregion
    }
}