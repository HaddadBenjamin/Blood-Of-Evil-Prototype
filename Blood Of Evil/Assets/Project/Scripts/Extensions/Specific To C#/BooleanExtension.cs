using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class BooleanExtension
    {
      /// <summary>
      /// Inverse la valeur duCe s boolean.
      /// </summary>
      public static void Inverse(this bool boolean)
      {
          boolean = !boolean;
      }
    }
}
