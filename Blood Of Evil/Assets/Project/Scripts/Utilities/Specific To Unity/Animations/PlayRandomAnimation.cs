using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

namespace BloodOfEvil.Utilities.Animations
{
    using Helpers;

    public class PlayRandomAnimation : StateMachineBehaviour
    {
        #region Fields
        [SerializeField]
        private string animationParameter;
        [SerializeField]
        private int numberOfDifferentAnimations;
        #endregion

        #region Unity Behaviour
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(this.animationParameter, MathHelper.GenerateRandomBeetweenTwoInts(0, this.numberOfDifferentAnimations - 1));

        }
        #endregion
    }
}