using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Modules.Animations
{
    using Scene;
    using Scene.Services.Footsteps;

    using Entities.Modules.Animations;
    using Entities.Modules.Attributes;
    using Entities.Modules.Footsteps;

    using Services.Keys;
    using Services.Audio;

    using Attributes;

    [RequireComponent(typeof(FootstepsModule))]
    public class PlayerAnimationModule : AEntityAnimationModule
    {
        #region Fields
        [SerializeField]
        private Vector2 kickPunchDamage;
        [SerializeField]
        private Vector2 punchAttackDamage;

        private KeysService keysServices;
        private GameObject pathfindingParticuleGameObject;
        private FootstepsModule footstepsTrigger;

        private const float MINIMUM_LOCOMOTION_SPEED_TO_PLAY_FOOTSTEPS = 0.3f;
        #endregion

        #region Override Behaviour
        public override void Initialize()
        {
            this.footstepsTrigger = GetComponent<FootstepsModule>();

            this.keysServices = PlayerServicesAndModulesContainer.Instance.InputService;
            this.pathfindingParticuleGameObject = PlayerServicesAndModulesContainer.Instance.GameObjectInSceneReferencesService.Get("Pathfinding Particle");

            base.Initialize();
        }
        #endregion

        #region Unity Behaviour
        void Update()
        {
            bool wasAttacking = base.GetIsAttackingParameter();
            bool isAttacking = this.keysServices.IsContiniouslyDown(EPlayerInput.AttackWithoutMove);

            if (wasAttacking != isAttacking)
            {
                base.SetIsAttackingParameter(isAttacking);

                if (isAttacking)
                    this.pathfindingParticuleGameObject.SetActive(false);

                // Devrait être dans un script de behaviour animation.
                base.SetLocomotionSpeedPercentageParameter(
                    isAttacking ?
                    1000.0f :
                    base.AttributesModule.GetAttribute(EEntityCategoriesAttributes.Movement, "Movement Speed Percentage").Current.Value * 0.01f);
            }
        }
        #endregion

        #region Animations Events
        public void PunchAttackDealDamage(int mandatoryVariableForItWorks)
        {
            if (this.CanDealDamageAndPlaySound())
            {
                base.AttributesModule.AttackCategoryAttributes.SetDamagesMinimalAndMaximalThenAttack(this.punchAttackDamage);

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play3DSound(EAudioCategory.SFX, "Player Punch", base.MyTransform);
            }
        }

        public void KickAttackDealDamage(int mandatoryVariableForItWorks)
        {
            if (base.CanDealDamageAndPlaySound())
            {
                base.AttributesModule.AttackCategoryAttributes.SetDamagesMinimalAndMaximalThenAttack(this.kickPunchDamage);

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play3DSound(EAudioCategory.SFX, "Player Kick", base.MyTransform);
            }
        }

        public void FootstepLeft(int mandatoryVariableForItWorks)
        {
            if (this.CanPlayFoosteps())
                this.footstepsTrigger.LeftFootStepAction();
        }

        public void FootstepRight(int mandatoryVariableForItWorks)
        {
            if (this.CanPlayFoosteps())
                this.footstepsTrigger.RightFootStepAction();
        }
        #endregion

        #region Intern Behaviour
        private bool CanPlayFoosteps()
        {
            return this.MyAnimator.GetFloat("Locomotion Speed") > MINIMUM_LOCOMOTION_SPEED_TO_PLAY_FOOTSTEPS;
        }
        #endregion
    }
}