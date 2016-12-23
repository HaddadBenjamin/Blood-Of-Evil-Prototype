using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Enemies.Modules.Attributes
{
    using Entities.Modules.Attributes;

    public class EnemyExperienceategoryAttributes : EntityExperienceCategoryAttributes
    {
        #region Constructor
        public EnemyExperienceategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            // En dur pour l'instant, mais sera configurable.
            base.GetAttribute(EEntityCategoriesAttributes.Experience, "Experience").Current.Value = 150.0f;
        }

        public override void CreateCallbacksAttributes()
        {
        }
        #endregion
    }
}