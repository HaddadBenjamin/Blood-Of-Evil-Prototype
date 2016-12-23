using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Enemies.Modules.Attributes
{
    using Scene;
    using Player.Services.Audio;
    using Entities.Modules.Attributes;

    [System.Serializable]
    public sealed class EnemyLifeCategoryAttributes : EntityLifeCategoryAttributes
    {
        #region Fields
        private HealthBar healthBar;
        private bool saveDeadState = true;
        private bool raiseEnemyIndex = true;
        #endregion

        #region Properties
        public bool SaveDeadState
        {
            get
            {
                return saveDeadState;
            }

            set
            {
                saveDeadState = value;
            }
        }
        #endregion

        #region Constructor
        public EnemyLifeCategoryAttributes(EnemyAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Override Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();

            this.ComeBackToLife();
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.ValueListener(delegate (float input)
            {
                float maximumLife = base.GetAttribute(EEntityCategoriesAttributes.Life, "Maximum Life").Current.Value;

                if (input > maximumLife)
                    base.ResetLifeToMaximumLife();
            });
        }

        public override void Update()
        {
            base.Update();
        }
        #endregion

        #region Intern Behaviour
        protected override void ComeBackToLife()
        {
            base.ComeBackToLife();

            SceneServicesContainer.Instance.SceneStateModule.RegisterEnemy(base.attributeModule.transform, this.raiseEnemyIndex);

            this.raiseEnemyIndex = false;
            base.IsDeath = false;

            base.ResetLifeToMaximumLife();
            this.CreateHealthBar();
            base.attributeModule.GetComponent<BoxCollider>().enabled = true;

        }

        private void CreateHealthBar()
        {
            if (null == this.healthBar)
                this.healthBar = base.attributeModule.GetComponent<HealthBar>();

            this.healthBar.EnableHealthBar();

            base.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Maximum Life").Current.ValueUnlistener(this.UpdateLifeMaximumHealthBar);
            base.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.ValueUnlistener(this.UpdateLifeHealthBar);

            this.healthBar.SetMaximumLife(attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Maximum Life").Current.ValueListenerAndGetValue(delegate (float value)
            {
                this.UpdateLifeMaximumHealthBar(value);
            }));

            this.healthBar.SetMinimumLife(attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.ValueListenerAndGetValue(delegate (float value)
            {
                this.healthBar.SetMinimumLife(value);
            }));

            this.healthBar.Initialize(
                    base.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Life").Current.Value,
                    base.attributeModule.GetAttribute(EEntityCategoriesAttributes.Life, "Maximum Life").Current.Value);
        }

        protected override void Death()
        {
            SceneServicesContainer.Instance.SceneStateModule.UnRegisterEnemy(base.attributeModule.transform);

            base.Death();
            base.attributeModule.GetComponent<shaderGlow>().outlined = false;
            base.attributeModule.GetComponent<BoxCollider>().enabled = false;

            SceneServicesContainer.Instance.AudioReferencesArraysService.Play3DSound(EAudioCategory.SFX, "Mutant Death", base.attributeModule.transform);

            base.IsDeath = true;

            if (null != this.healthBar)
                this.healthBar.DisableHealthBar();
        }

        private void UpdateLifeMaximumHealthBar(float maximumLife)
        {
            this.healthBar.SetMaximumLife((int)maximumLife);
        }

        private void UpdateLifeHealthBar(float life)
        {
            this.healthBar.SetMinimumLife(life);
        }
        #endregion
    }
}
