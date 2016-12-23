using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Animation
{
    using Enemies;
    using Player;

    using Modules.Attributes;

    /// <summary>
    /// Ce script permet de stopper l'animation d'attaque si le charactère meurt ou n'attaque plus.
    /// </summary>
    public class StopAttackAnimationIfNotAttackingOrIsDying : StateMachineBehaviour
    {
        #region Fields
        private int isAttackingHashPercentage = Animator.StringToHash("Attack Speed Percentage");
        private int isAttackingHash = Animator.StringToHash("Is Attacking");
        private int isDyingHash = Animator.StringToHash("Is Dying");
        [SerializeField]
        private EEntity entityType;
        private EntityAttributes attackSpeedPercentageAttribute;
        #endregion

        #region Unity Behaviour
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AEntityAttributesModule entityAttributeModule;

            if (this.entityType == EEntity.Player)
                entityAttributeModule = PlayerServicesAndModulesContainer.Instance.AttributesModule;
            else
                entityAttributeModule = animator.GetComponent<EnemyServicesAndModulesContainer>().AttributesModule;

            this.attackSpeedPercentageAttribute = entityAttributeModule.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Speed Percentage");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool(this.isDyingHash) || !animator.GetBool(this.isAttackingHash))
                animator.SetFloat(this.isAttackingHashPercentage, float.MaxValue);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(this.isAttackingHashPercentage, this.attackSpeedPercentageAttribute.GetDefaultUpdateValueWithPercent() * 0.01f);
        }
        #endregion
    }
}