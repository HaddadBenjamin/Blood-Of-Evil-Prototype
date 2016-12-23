using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Animations
{
    using Enemies;
    using ObjectInScene;

    using Modules.Attributes;
    using Player;

    public abstract class AEntityAnimationModule : AInitializableComponent
    {
        #region Fields
        private Transform myTransform;
        private Animator myAnimator;
        private AEntityAttributesModule attributesModule;

        private readonly int isAttackingHash = Animator.StringToHash("Is Attacking");
        private readonly int isDyingHash = Animator.StringToHash("Is Dying");
        private readonly int locomotionSpeedHash = Animator.StringToHash("Locomotion Speed");
        private readonly int locomotionSpeedPercentageHash = Animator.StringToHash("Locomotion Speed Percentage");

        private bool dieAnimationIsPlaying;

        [SerializeField]
        private EEntity entityType = EEntity.Player;
        #endregion

        #region Properties
        protected AEntityAttributesModule AttributesModule
        {
            get
            {
                return attributesModule;
            }

            private set
            {
                attributesModule = value;
            }
        }

        protected Animator MyAnimator
        {
            get
            {
                return myAnimator;
            }

            set
            {
                myAnimator = value;
            }
        }

        protected int IsAttackingHash
        {
            get
            {
                return isAttackingHash;
            }
        }

        protected int IsDyingHash
        {
            get
            {
                return isDyingHash;
            }
        }

        public EEntity EntityType
        {
            get
            {
                return entityType;
            }

            set
            {
                entityType = value;
            }
        }

        protected Transform MyTransform
        {
            get
            {
                return myTransform;
            }

            set
            {
                myTransform = value;
            }
        }

        public int LocomotionSpeedHash
        {
            get
            {
                return locomotionSpeedHash;
            }
        }

        public int LocomotionSpeedPercentageHash
        {
            get
            {
                return locomotionSpeedPercentageHash;
            }
        }

        public bool DieAnimationIsPlaying
        {
            get
            {
                return dieAnimationIsPlaying;
            }

            set
            {
                dieAnimationIsPlaying = value;
            }
        }
        #endregion

        #region Override Behaviour
        public override void Initialize()
        {
            this.MyTransform = transform;
            this.MyAnimator = GetComponent<Animator>();

            this.AttributesModule =
                EEntity.Player == this.EntityType ?
                this.AttributesModule = PlayerServicesAndModulesContainer.Instance.AttributesModule :
                this.AttributesModule = GetComponent<EnemyServicesAndModulesContainer>().AttributesModule;
        }
        #endregion

        #region Public Behaviour
        public bool GetIsAttackingParameter()
        {
            return this.MyAnimator.GetBool(this.IsAttackingHash);
        }

        public bool GetIsDyingParameter()
        {
            return this.MyAnimator.GetBool(this.IsDyingHash) ||
                    this.DieAnimationIsPlaying;
        }

        public float GetLocomotionSpeedParameter()
        {
            return this.MyAnimator.GetFloat(this.LocomotionSpeedHash);
        }

        public float GetLocomotionSpeedPercentageParameter()
        {
            return this.MyAnimator.GetFloat(this.LocomotionSpeedPercentageHash);
        }

        public void SetIsAttackingParameter(bool isAttacking)
        {
            this.MyAnimator.SetBool(this.IsAttackingHash, isAttacking);
        }

        public void SetIsDyingParameter(bool isDying)
        {
            this.MyAnimator.SetBool(this.IsDyingHash, isDying);
        }

        public void SetLocomotionSpeedParameter(float locomotionSpeed)
        {
            this.MyAnimator.SetFloat(this.locomotionSpeedHash, locomotionSpeed, 0.1f, Time.deltaTime);
        }

        public void SetLocomotionSpeedPercentageParameter(float locomotionSpeedPercentage)
        {
            this.MyAnimator.SetFloat(this.locomotionSpeedPercentageHash, locomotionSpeedPercentage);
        }
        #endregion

        #region Protected Behaviour
        protected bool CanDealDamageAndPlaySound()
        {
            return this.GetIsAttackingParameter() &&
                    !this.GetIsDyingParameter();
        }
        #endregion

        #region Animation Events
        public void DieAnimationStartPlaying(int mandatoryVariableForItWorks)
        {
            this.DieAnimationIsPlaying = true;
        }

        public void DieAnimationEndPlaying(int mandatoryVariableForItWorks)
        {
            this.DieAnimationIsPlaying = false;
        }
        #endregion
    }
}