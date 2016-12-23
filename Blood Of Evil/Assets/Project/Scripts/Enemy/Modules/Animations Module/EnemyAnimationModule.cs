using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Enemies.Modules.Animations
{
    using Scene;
    using Player.Services.Audio;
    using Entities.Modules.Animations;

    public class EnemyAnimationModule : AEntityAnimationModule
    {
        #region Fields
        [SerializeField]
        private Vector2 jumpAttackDamage;
        [SerializeField]
        private Vector2 punchAttackDamage;
        [SerializeField]
        private Vector2 swippingAttackDamage;
        #endregion

        #region Override Behaviour
        public override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Animations Events
        public void DieAnimationIsDone(int mandatoryVariableForItWorks)
        {
            Destroy(gameObject);
        }

        public void JumpAttackDealDamage(int mandatoryVariableForItWorks)
        {
            if (base.CanDealDamageAndPlaySound())
            {
                base.AttributesModule.AttackCategoryAttributes.SetDamagesMinimalAndMaximalThenAttack(this.jumpAttackDamage);

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play3DSound(EAudioCategory.SFX, "Mutant Jump", base.MyTransform);
            }
        }

        public void PunchDealDamage(int mandatoryVariableForItWorks)
        {
            if (base.CanDealDamageAndPlaySound())
            {
                base.AttributesModule.AttackCategoryAttributes.SetDamagesMinimalAndMaximalThenAttack(this.punchAttackDamage);

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play3DSound(EAudioCategory.SFX, "Mutant Punch", base.MyTransform);
            }
        }

        public void SwippingDealDamage(int mandatoryVariableForItWorks)
        {
            if (base.CanDealDamageAndPlaySound())
            {
                base.AttributesModule.AttackCategoryAttributes.SetDamagesMinimalAndMaximalThenAttack(this.swippingAttackDamage);

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play3DSound(EAudioCategory.SFX, "Mutant Swiping", base.MyTransform);
            }
        }

        public void FootstepLeft(int mandatoryVariableForItWorks)
        {
        }

        public void FootstepRight(int mandatoryVariableForItWorks)
        {
        }
        #endregion
    }
}