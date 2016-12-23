using UnityEngine;
using System.Collections;

// 2 Attributs :
// Defence
// Defence Percentage
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    using Configuration;

    [System.Serializable]
    public sealed class PlayerDefenceCategoryAttributes : EntityDefenceCategoryAttributes
    {
        #region Constructor
        public PlayerDefenceCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Fields
        private PlayerCharacteristicCategoryAttributeConfiguration configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.CharacteristicAttributes;
        #endregion

        // GetRes(ERes)
        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Resistance").Current.ValueListener(delegate (float input)
            {
                base.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").SetOtherAttribute("Characteristic", input * configuration.DefencePerResistance);
                base.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence Percentage").SetOtherAttribute("Characteristic", input * configuration.DefencePercentagePerResistance);
            });
        }
        #endregion
    }
}