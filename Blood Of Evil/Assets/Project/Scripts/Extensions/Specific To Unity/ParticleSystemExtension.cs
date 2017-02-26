using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class ParticleSystemExtension
    {
      public static void SafePlay(this ParticleSystem particleSystem)
      {
          if (null != particleSystem && 
              false == particleSystem.isPlaying)
              particleSystem.Play();
      }
    }
}
