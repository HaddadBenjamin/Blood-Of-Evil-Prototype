using UnityEngine;
using System.Collections;

//6 Attributs :
//Life,
//Maximum Life
//Life Percentage,
//Life Regenerate Per Second,
//Life Regenerate Per Second Percentage,
//Percentage Of Life Regenerate Per Second : ex pour 5; j'ai 1000 vie, je regenerate 1000 * 0.05 vie.

namespace BloodOfEvil.Player.Modules.Attributes
{
    using Scene;
    using Utilities;
    using Entities.Modules.Attributes;

    using Services.Audio;

    using Configuration;

    [System.Serializable]
    public sealed class PlayerLifeCategoryAttributes : EntityLifeCategoryAttributes
    {
        #region Fields
        private PlayerCharacteristicCategoryAttributeConfiguration configuration = PlayerServicesAndModulesContainer.Instance.ConfigurationService.CharacteristicAttributes;
        private Timer backToLifeTimer = new Timer(3.0f);

        private EntityAttributes lifeRegeneratePerSecondsPercentageAttributes;
        private EntityAttributes lifeRegeneratePerSecondsAttributes;
        private EntityAttributes percentageOfLifeRegeneratePerSecondAttributes;
        #endregion

        #region Constructor
        public PlayerLifeCategoryAttributes(PlayerAttributesModule attributeModule) : base(attributeModule)
        {
        }
        #endregion

        #region Override Behaviour
        public override void InitialzeAttributes()
        {
            base.InitialzeAttributes();

            this.lifeRegeneratePerSecondsPercentageAttributes = base.GetAttribute(EEntityCategoriesAttributes.Life, "Life Regenerate Per Second Percentage");
            this.lifeRegeneratePerSecondsAttributes = base.GetAttribute(EEntityCategoriesAttributes.Life, "Life Regenerate Per Second");
            this.percentageOfLifeRegeneratePerSecondAttributes = base.GetAttribute(EEntityCategoriesAttributes.Life, "Percentage Of Life Regenerate Per Second");

            this.lifeRegeneratePerSecondsPercentageAttributes.InitializeDefaultPercentage("Percentage Of Life Regenerated Per Second");
            this.lifeRegeneratePerSecondsAttributes.Initialize("Life Regenerated Per Second", 1.0f, base.GetAttribute(EEntityCategoriesAttributes.Life, "Life Regenerate Per Second Percentage"));
            this.percentageOfLifeRegeneratePerSecondAttributes.Initialize("Percentage Of Life Regenerated Per Second", 1.0f, base.GetAttribute(EEntityCategoriesAttributes.Life, "Life Regenerate Per Second Percentage"));

            // Doit être présent ou bien la vie repop à son maximum, aucune idée du comment du pourquoi.
            base.IsDeath = false;
            SceneServicesContainer.Instance.SceneStateModule.RegisterPlayer(base.attributeModule.transform);
        }

        public override void CreateCallbacksAttributes()
        {
            base.CreateCallbacksAttributes();

            base.GetAttribute(EEntityCategoriesAttributes.Characteristics, "Endurance").Current.ValueListener(delegate (float input)
            {
                base.MaximumLifettributes.SetOtherAttribute("Characteristic", input * configuration.LifePerEndurance);
                base.LifePercentageAttributes.SetOtherAttribute("Characteristic", input * configuration.LifePercentagePerEndurance);
            });
        }

        public override void Update()
        {
            if (this.IsDeath &&
                this.backToLifeTimer.IsRingingUpdated())
                this.ComeBackToLife();

            if (!base.IsDeath)
                this.RegenerateLife();
        }
        #endregion

        #region Intern Behaviour
        protected override void ComeBackToLife()
        {
            base.ComeBackToLife();

            SceneServicesContainer.Instance.SceneStateModule.RegisterPlayer(base.attributeModule.transform);

            base.IsDeath = false;

            base.ResetLifeToMaximumLife();

            this.Respawn();
        }

        protected override void Death()
        {
            PlayerServicesAndModulesContainer.Instance.MovementModule.OnDeath();

            base.Death();

            SceneServicesContainer.Instance.SceneStateModule.UnregisterPlayer();

            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Player Death");
            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Player Game Over");


            this.backToLifeTimer.Reset();
            base.IsDeath = true;
        }

        private void RegenerateLife()
        {
            float currentLife = base.LifeAttributes.Current.Value;

            if (currentLife < base.MaximumLifettributes.Current.Value)
                base.LifeAttributes.Current.Value +=
                    this.lifeRegeneratePerSecondsAttributes.Current.Value * Time.deltaTime +
                    this.percentageOfLifeRegeneratePerSecondAttributes.Current.Value
                    * Time.deltaTime;
        }

        private void Respawn()
        {
            // Respawn en dur, changera par la suite.

            base.attributeModule.transform.position = Vector3.zero;
        }
        #endregion
    }
}