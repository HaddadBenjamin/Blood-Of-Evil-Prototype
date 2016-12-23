using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    public class EntityResistanceCategoryAttributes : AEntityCategoryAttribute
    {
        #region Fields
        public static string[] ResistancesString = new string[]
        {
        "Fire",
        "Cold",
        "Lighting",
        "Faith",
        "Wind",
        "Earth",
        "Poison",
        };
        #endregion

        #region Constructor
        public EntityResistanceCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            for (int resistanceIndex = 0; resistanceIndex < ResistancesString.Length; resistanceIndex++)
            {
                string resistanceString = ResistancesString[resistanceIndex];

                base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance Percentage", resistanceString)).InitializeDefaultPercentage(string.Format("Percentage Of {0} Resistance", resistanceString));
                base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance", resistanceString)).Initialize(string.Format("{0} Resistance", resistanceString), 0.0f, base.GetAttribute(EEntityCategoriesAttributes.Resistances, string.Format("{0} Resistance Percentage", resistanceString)));
            }
        }

        public override void CreateCallbacksAttributes()
        {
        }
        #endregion
    }
}