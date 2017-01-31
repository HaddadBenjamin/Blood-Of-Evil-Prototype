using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class BooleanExtension
    {
      /// <summary>
      /// Inverse la valeur de ce boolean.
      /// </summary>
      public static void Inverse(this bool boolean)
      {
          boolean = !boolean;
      }
    }
}
