using UnityEngine;
using System.Collections;

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
        }
        #endregion
    }
}