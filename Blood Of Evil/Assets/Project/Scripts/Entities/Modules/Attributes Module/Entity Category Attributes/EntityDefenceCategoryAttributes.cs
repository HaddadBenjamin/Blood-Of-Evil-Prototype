using UnityEngine;
using System.Collections;

// 2 Attributs :
// Defence
// Defence Percentage
namespace BloodOfEvil.Entities.Modules.Attributes
{
    [System.Serializable]
    public class EntityDefenceCategoryAttributes : AEntityCategoryAttribute
    {
        #region Constructor
        public EntityDefenceCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence Percentage").InitializeDefaultPercentage("Percentage Of Defence");
            base.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence").Initialize("Defence", 10.0f, base.GetAttribute(EEntityCategoriesAttributes.Defence, "Defence Percentage"));
        }

        public override void CreateCallbacksAttributes()
        {
        }
        #endregion
    }
}