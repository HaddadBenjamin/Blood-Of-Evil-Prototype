using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de limiter un floatant entre 2 valeurs.
    /// </summary>
    [System.Serializable]
    public class ClampedValue
    {
      #region Fields
      /// <summary>
      /// Tooltip c'est le floatant qui sera limité entre 2 valeurs.
      /// </summary>
      private float value;
      [SerializeField, Tooltip("C'est le nombre minimal possible de la valeur.")]
      private float minimalValue;
      [SerializeField, Tooltip("C'est le nombre maximal possible de la valeur.")]
      private float maximalValue;
      #endregion

      #region Properties
      public float Value
      {
          get { return value; }
      }

      public float MinimalValue
      {
          get { return minimalValue; }
      }

      public float MaximalValue
      {
          get { return maximalValue; }
      }
      #endregion

      #region Constructor
      public ClampedValue() { }

      public ClampedValue(float minimalValue, float maximalValue)
      {
          this.minimalValue = minimalValue;
          this.maximalValue = maximalValue;
      }
      #endregion

      #region Public Behaviour
      /// <summary>
      /// Permet de redéfinir le nombre de la valeur en la limitant entre sa valeur minimal et maximal.
      /// En fait c'est un simple clamp. (Mathf.Clamp).
      /// </summary>
      public void ModifyValue(float newValue)
      {
          this.value = newValue > this.maximalValue ? this.maximalValue :
                       newValue < this.minimalValue ? this.minimalValue :
                       newValue;
      }

      /// <summary>
      /// Rajoute une nombre à la valeur en la limitant entre sa va leur minimal et maximal.
      /// </summary>
      public void AddValue(float valueToAdd)
      {
          this.ModifyValue(this.value + valueToAdd);
      }
      #endregion
    }
}
