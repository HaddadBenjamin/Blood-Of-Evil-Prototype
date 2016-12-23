using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    public class EntityExperienceCategoryAttributes : AEntityCategoryAttribute
    {
        #region Constructor
        public EntityExperienceCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Initialize("Experience");
        }

        public override void CreateCallbacksAttributes()
        {
        }
        #endregion
    }
}