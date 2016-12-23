using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Helpers
{
    using Entities.Modules.Attributes;
    using Player.Services.Language;

    public static class AttributeHelper
    {
        public static Attribute GetAttributeOfSubAttributeCategory(EntityAttributes entityAttributes, string subAttributeCategory)
        {
            return subAttributeCategory.Equals("Current") ? entityAttributes.Current :
                    subAttributeCategory.Equals("At Start") ? entityAttributes.AtStart :
                    subAttributeCategory.Equals("Percent") ? entityAttributes.Percent.Current :
                    entityAttributes.GetOtherAttribute(subAttributeCategory);
        }

        public static ELanguageCategory EEntityCategoryAttributesToELanguageCategory(EEntityCategoriesAttributes entityCategoryAttributes)
        {
            switch (entityCategoryAttributes)
            {
                case EEntityCategoriesAttributes.Attack:
                    return ELanguageCategory.AttackAttributes;

                case EEntityCategoriesAttributes.Characteristics:
                    return ELanguageCategory.CharacteristicsAttributes;

                case EEntityCategoriesAttributes.Defence:
                    return ELanguageCategory.DefenceAttributes;

                case EEntityCategoriesAttributes.Experience:
                    return ELanguageCategory.ExperienceAttributes;

                case EEntityCategoriesAttributes.Life:
                    return ELanguageCategory.LifeAttributes;

                case EEntityCategoriesAttributes.Loot:
                    return ELanguageCategory.LootAttributes;

                case EEntityCategoriesAttributes.Mana:
                    return ELanguageCategory.ManaAttributes;

                case EEntityCategoriesAttributes.Movement:
                    return ELanguageCategory.MovementAttributes;

                case EEntityCategoriesAttributes.Resistances:
                    return ELanguageCategory.ResistancesAttributes;

                case EEntityCategoriesAttributes.Skill:
                    return ELanguageCategory.SkillAttributes;
            }

            return ELanguageCategory.AttackAttributes;
        }
    }
}