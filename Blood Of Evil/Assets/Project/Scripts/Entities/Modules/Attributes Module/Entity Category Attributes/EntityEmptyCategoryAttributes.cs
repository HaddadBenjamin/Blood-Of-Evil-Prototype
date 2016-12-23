using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    public class EntityEmptyCategoryAttributes : AEntityCategoryAttribute
    {
        #region Constructor
        public EntityEmptyCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
        }

        public override void CreateCallbacksAttributes()
        {
        }
        #endregion
    }
}