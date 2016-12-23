using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    using Extensions;

    public class EntityLifeCategoryAttributes : AEntityCategoryAttribute
    {
        #region Fields
        private bool isDeath = false;
        protected Animator myAnimator;

        private EntityAttributes lifePercentageAttributes;
        private EntityAttributes lifeAttributes;
        private EntityAttributes maximumLifettributes;

        public Action ReduceLifeListener;
        #endregion

        #region Properties
        public bool IsDeath
        {
            get
            {
                return isDeath;
            }

            protected set
            {
                isDeath = value;
            }
        }

        public EntityAttributes LifePercentageAttributes
        {
            get
            {
                return lifePercentageAttributes;
            }

            set
            {
                lifePercentageAttributes = value;
            }
        }

        public EntityAttributes LifeAttributes
        {
            get
            {
                return lifeAttributes;
            }

            set
            {
                lifeAttributes = value;
            }
        }

        public EntityAttributes MaximumLifettributes
        {
            get
            {
                return maximumLifettributes;
            }

            set
            {
                maximumLifettributes = value;
            }
        }
        #endregion

        #region Constructor
        public EntityLifeCategoryAttributes(AEntityAttributesModule attributeModule) : base(attributeModule)
        {
            //base.GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.Value = 1.0f;
        }
        #endregion

        #region Virtual Behaviour
        public override void InitialzeAttributes()
        {
            this.LifeAttributes = base.GetAttribute(EEntityCategoriesAttributes.Life, "Life");
            this.LifePercentageAttributes = base.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage");
            this.MaximumLifettributes = base.GetAttribute(EEntityCategoriesAttributes.Life, "Maximum Life");

            this.LifePercentageAttributes.InitializeDefaultPercentage("Percentage Of Life");

            this.LifeAttributes.Initialize("Life", 100.0f, base.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage"));
            this.MaximumLifettributes.Initialize("Maximum Life", 100.0f, base.GetAttribute(EEntityCategoriesAttributes.Life, "Life Percentage"));
        }

        public override void CreateCallbacksAttributes()
        {
            this.myAnimator = base.attributeModule.GetComponent<Animator>();

            //base.GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.SubscribeToOnValueModified(delegate (float input)
            //{
            //    if (input <= 0.1f)
            //        this.Death();
            //});
        }


        protected virtual void ComeBackToLife()
        {
            this.myAnimator.SetBool(Animator.StringToHash("Is Dying"), false);
        }
        protected virtual void Death()
        {
            this.myAnimator.SetFloat(Animator.StringToHash("Locomotion Speed"), 0.0f);
            this.myAnimator.SetBool(Animator.StringToHash("Is Dying"), true);
        }
        #endregion

        #region Public Behaviour
        public bool EndureDamage(float damage)
        {
            if (damage > this.LifeAttributes.Current.Value)
            {
                this.Death();
                this.LifeAttributes.Current.Value = -0.1f;

                return true;
            }
            else
                this.LifeAttributes.Current.Value -= damage;

            this.ReduceLifeListener.SafeCall();

            return false;
        }

        public void ResetLifeToMaximumLife()
        {
            this.LifeAttributes.Current.Value = this.MaximumLifettributes.Current.Value;
        }
        #endregion
    }
}