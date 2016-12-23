using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    public class EntitySkillCategoryAttributes : AEntityCategoryAttribute
    {
        #region Constructor
        public EntitySkillCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            base.GetAttribute(EEntityCategoriesAttributes.Skill, "Skill Effect Percentage").InitializeDefaultPercentage("Percentage Of Skill Effect");
            base.GetAttribute(EEntityCategoriesAttributes.Skill, "Heal Effect Percentage").InitializeDefaultPercentage("Percentage Of Heal Effect");

            base.GetAttribute(EEntityCategoriesAttributes.Skill, "Minion Life Percentage").InitializeDefaultPercentage("Percentage Of Minion Life");
            base.GetAttribute(EEntityCategoriesAttributes.Skill, "Minion Damage Percentage").InitializeDefaultPercentage("Percentage Of Minion Damage");

            base.GetAttribute(EEntityCategoriesAttributes.Skill, "Minimal Heal").Initialize("Minimal Heal", 0.0f, base.GetAttribute(EEntityCategoriesAttributes.Skill, "Heal Effect Percentage"));
            base.GetAttribute(EEntityCategoriesAttributes.Skill, "Maximal Heal").Initialize("Maximal Heal", 0.0f, base.GetAttribute(EEntityCategoriesAttributes.Skill, "Heal Effect Percentage"));
        }

        public override void CreateCallbacksAttributes()
        {
        }
        #endregion
    }
}