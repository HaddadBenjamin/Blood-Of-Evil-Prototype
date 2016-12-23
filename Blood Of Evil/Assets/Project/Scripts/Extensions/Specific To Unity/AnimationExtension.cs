using System;
using System.Collections;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
    public static class AnimationExtension
    {
        /// <summary>
        /// Joue une animation à l'envers.
        /// </summary>
        public static void PlayReverse(
            this Animation animation,
            string animationName,
            float animatorSpeed = 1.0f)
        {
            animation[animationName].time = animation[animationName].length * animatorSpeed;
            animation[animationName].speed = -animatorSpeed;

            animation.Play();
        }
    }
}