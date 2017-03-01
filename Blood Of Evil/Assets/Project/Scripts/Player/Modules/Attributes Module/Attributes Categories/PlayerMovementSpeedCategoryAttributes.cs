using UnityEngine;
using System.Collections;
using BloodOfEvil.Player.Modules.Movements;

//2 Attribut :
//Movement Speed Percentage
//Steps
namespace BloodOfEvil.Player.Modules.Attributes
{
    using Entities.Modules.Attributes;

    [System.Serializable]
    public sealed class PlayerMovementSpeedCategoryAttributes : EntityMovementCategoryAttributes
    {
        #region Constructor
        public PlayerMovementSpeedCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Movement, "Steps").Initialize("Number Of Steps");
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Movement, "Movement Speed Percentage").Current.ValueListener(
                delegate(float input)
                {
                    base.attributeModule.GetComponent<PlayerMovementModule>().AnimationMovementSpeedRatio = input * PERCENTAGE_TO_UNIT;
                });
        }
        #endregion
    }
}