using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    public class EntityLootCategoryAttributes : AEntityCategoryAttribute
    {
        #region Constructor
        public EntityLootCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Loot, "Gold").InitializeDefaultPercentage("Gold");
            base.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Gold Percentage").InitializeDefaultPercentage("Percentage Of Gold Find");
            base.GetAttribute(EEntityCategoriesAttributes.Loot, "Find Item Percentage").InitializeDefaultPercentage("Percentage Of Item Find");
            base.GetAttribute(EEntityCategoriesAttributes.Loot, "Magic Find Item Percentage").InitializeDefaultPercentage("Percentage Of Magic Find");
        }

        public override void CreateCallbacksAttributes()
        {
        }
        #endregion
    }
}