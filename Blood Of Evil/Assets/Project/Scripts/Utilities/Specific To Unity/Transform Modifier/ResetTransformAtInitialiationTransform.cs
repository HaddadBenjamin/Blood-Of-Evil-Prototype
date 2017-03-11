using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de remettre un objet à sa position, sa rotation et à son échelle initiale.
    /// </summary>
    public class ResetTransformAtInitialiationTransform : MonoBehaviour
    {
        #region Fields
        private Transform myTransform;
        private Vector3 initialisationPosition, initialisationRotation, initialisationScale;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            this.myTransform = transform;

            // Stoque les informations du transform à son initalisation.
            this.initialisationPosition = this.myTransform.localPosition;
            this.initialisationRotation = this.myTransform.localEulerAngles;
            this.initialisationScale = this.myTransform.localScale;
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Remet à l'échelle un objet à son échelle initial.
        /// </summary>
        public void ResetTransform()
        {
            this.myTransform.localPosition = this.initialisationPosition;
            this.myTransform.localEulerAngles = this.initialisationRotation;
            this.myTransform.localScale = this.initialisationScale;
        }
        #endregion
    }
}
