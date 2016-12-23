using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

namespace BloodOfEvil.Entities.Animations
{
    using Entities;
    using Entities.Modules.Attributes;
    using Enemies;
    using Player;

    /// <summary>
    /// Ce script permet de ne pouvoir attaquer sans attendre que l'animation d'idle se finisse.
    /// Permet de mettre à jour la vitesse de déplacement. Si le joueur est mort ou attaque et qu'il est en animation d'idle alors la vitesse de changement d'animation est au maximum.
    /// L'état exit permet juste de réinitialiser la valeur de vitesse de déplacement.
    /// </summary>
    public class PlayAnimationAttackIfAttackingAndIdleState : StateMachineBehaviour
    {

        #region Fields
        private int locomotionSpeedPercentageHash = Animator.StringToHash("Locomotion Speed Percentage");
        private int locomotionSpeedHash = Animator.StringToHash("Locomotion Speed");
        private int isDyingHash = Animator.StringToHash("Is Dying");
        private int isAttackingHash = Animator.StringToHash("Is Attacking");
        [SerializeField]
        private EEntity entityType;
        private EntityAttributes locomotionSpeedPercentageAttribute;
        #endregion

        #region Unity Behaviour
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AEntityAttributesModule entityAttributeModule;

            if (this.entityType == EEntity.Player)
                entityAttributeModule = PlayerServicesAndModulesContainer.Instance.AttributesModule;
            else
                entityAttributeModule = animator.GetComponent<EnemyServicesAndModulesContainer>().AttributesModule;

            this.locomotionSpeedPercentageAttribute = entityAttributeModule.GetAttribute(EEntityCategoriesAttributes.Movement, "Movement Speed Percentage");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bool isIdleState = animator.GetFloat(this.locomotionSpeedHash) < 0.1f;
            bool isAttackingOrDying = animator.GetBool(this.isDyingHash) || animator.GetBool(this.isAttackingHash);

            if (isIdleState && isAttackingOrDying)
                animator.SetFloat(this.locomotionSpeedPercentageHash, float.MaxValue);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(this.locomotionSpeedPercentageHash, this.locomotionSpeedPercentageAttribute.GetDefaultUpdateValueWithPercent() * 0.01f);
        }
        #endregion
    }
}