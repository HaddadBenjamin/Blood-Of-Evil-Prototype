using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    public class EntityManaCategoryAttributes : AEntityCategoryAttribute
    {
        #region Fields
        private EntityAttributes manaPercentageAttributes;
        private EntityAttributes maximumManaAttributes;
        private EntityAttributes manaAttributes;
        #endregion

        #region Properties
        public EntityAttributes ManaPercentageAttributes
        {
            get
            {
                return manaPercentageAttributes;
            }

            set
            {
                manaPercentageAttributes = value;
            }
        }

        public EntityAttributes MaximumManaAttributes
        {
            get
            {
                return maximumManaAttributes;
            }

            set
            {
                maximumManaAttributes = value;
            }
        }

        public EntityAttributes ManaAttributes
        {
            get
            {
                return manaAttributes;
            }

            set
            {
                manaAttributes = value;
            }
        }
        #endregion

        #region Constructor
        public EntityManaCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            this.ManaPercentageAttributes.InitializeDefaultPercentage("Percentage Of Mana");

            this.MaximumManaAttributes.Initialize("Maximum Mana", 100.0f, base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana Percentage"));
            this.ManaAttributes.Initialize("Mana", 80.0f, base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana Percentage"), false);
        }

        public override void CreateCallbacksAttributes()
        {
            this.ManaAttributes = base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana");
            this.MaximumManaAttributes = base.GetAttribute(EEntityCategoriesAttributes.Mana, "Maximum Mana");
            this.ManaPercentageAttributes = base.GetAttribute(EEntityCategoriesAttributes.Mana, "Mana Percentage");

            this.ManaAttributes.Current.ValueListener(delegate (float input)
            {
                if (input < 0.0f)
                    this.ManaAttributes.Current.Value = 0.0f;

                if (input > this.MaximumManaAttributes.Current.Value)
                    this.ResetManaToMaximumMana();
            });
        }
        #endregion

        #region Unherited Behaviour
        protected void ResetManaToMaximumMana()
        {
            this.ManaAttributes.Current.Value = this.MaximumManaAttributes.Current.Value;
        }
        #endregion
    }
}