using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class ParticleSystemExtension
    {
      /// <summary>
    /// Permet de jouer un système de particule de façon sécurisé et permet de le jouer plusieurs fois.
    /// </summary>
    public static void SafePlay(
        this ParticleSystem particleSystem, 
        bool canSpamThisParticleSystem = false)
    {
        if (canSpamThisParticleSystem)
        {
            particleSystem.SafeStop();
            particleSystem.SafeClear();
        }

        if (null != particleSystem && 
            false == particleSystem.isPlaying)
            particleSystem.Play();
    }

    /// <summary>
    /// Permet de vider le système de particule en ne risquant pas de crashs.
    /// </summary>
    public static void SafeClear(this ParticleSystem particleSystem)
    {
        if (null != particleSystem)
            particleSystem.Clear();
    }

    /// <summary>
    /// Permet d'arrêter le système de particule en ne risquant pas de crashs.
    /// </summary>
    public static void SafeStop(this ParticleSystem particleSystem)
    {
        if (null != particleSystem)
            particleSystem.Stop();
    }
    }
}
