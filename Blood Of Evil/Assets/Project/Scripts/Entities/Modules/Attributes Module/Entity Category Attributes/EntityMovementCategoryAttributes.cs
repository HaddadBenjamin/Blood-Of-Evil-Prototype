using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    public class EntityMovementCategoryAttributes : AEntityCategoryAttribute
    {
        #region Fields
        protected Animator animator;
        #endregion

        #region Constructor
        public EntityMovementCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
            this.animator = base.attributeModule.GetComponent<Animator>();
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Movement, "Movement Speed Percentage").Initialize("Percentage Of Movement Speed", 100.0f);
        }

        public override void CreateCallbacksAttributes()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Movement, "Movement Speed Percentage").Current.ValueListener(delegate (float input)
            {
                this.animator.SetFloat("Locomotion Speed Percentage", input * PERCENTAGE_TO_UNIT);
            });
        }
        #endregion
    }
}