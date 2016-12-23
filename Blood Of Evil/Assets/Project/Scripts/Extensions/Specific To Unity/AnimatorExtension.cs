using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Extensions
{
    public static class AnimatorExtension
    {
        /// <summary>
        /// Renvoie le temps que prendra l'animation "animationName".
        /// </summary>
        public static float GetAnimationTime(this Animator animator, string animationName)
        {
            return animator.GetAnimationTime(animator.GetAnimation(animationName));
        }


        /// <summary>
        /// Récupère le temps que prendra une animation en fonction de la vitesse de son animator.
        /// </summary>
        public static float GetAnimationTime(this Animator animator, AnimationClip clip)
        {
            return clip.length * animator.speed;
        }

        /// <summary>
        /// Renvoit l'animation ayant pour nom "animationName".
        /// </summary>
        public static AnimationClip GetAnimation(this Animator animator, string animationName)
        {
            return Array.Find(animator.runtimeAnimatorController.animationClips, animationClip => animationClip.name == animationName);
        }

        /// <summary>
        /// Renvoit toutes les animations de l'animator entrain de se jouer.
        /// </summary>
        public static AnimationClip[] GetAnimations(this Animator animator)
        {
            return animator.runtimeAnimatorController.animationClips;
        }

        /// <summary>
        /// DoesCurrentAnimationNameIs("Base Layer.Run") : renvoit si l'animation courante qui est joué dans l'animator est bien "Base Layer.Run".
        /// Cette méthode n'est pas safe, il se peut que l'animator est moins de layer d'animations que animationLayerIndex + 1.
        /// </summary>
        public static bool DoesCurrentAnimationNameIs(
            this Animator animator,
            string animationName,
            int animationLayerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(animationLayerIndex).IsName(animationName);
        }
    }
}