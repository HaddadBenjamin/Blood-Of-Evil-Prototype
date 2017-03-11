using System;
using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Extensions;
using BloodOfEvil.Player.Services;
using UnityEngine;
using UnityEngine.Events;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de faire une intéraction lorsque l'on déplace un doight ou le curseur de la souris.
    /// Les classes enfantes ne feront que redéfinir lorsque l'intéraction est active.
    /// </summary>
    public abstract class AMoveUserInput : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est dans cette direction que l'utilisateur doit déplacer son doight ou la souris pour renvoyé une action positive.")]
        private EMoveDirection direction;
        [Tooltip("C'est l'action qui sera appelé lorsque l'utilisateur bougera son doight.")]
        public Action<float> MoveEvent;
        [SerializeField, Tooltip("C'est le facteur de vitesse de déplacement.")]
        private float moveFactor = 10.0f;
        /// <summary>
        /// Permet de spécifier si l'intéraction de déplacement est active ou non.
        /// </summary>
        protected bool DoesInteractionIsActive;
        #endregion

        #region Properties
        public EMoveDirection Direction
        {
            get
            {
                return direction;
            }
        }
        #endregion

        #region Unity Behaviour
        private void LateUpdate()
        {
            if (this.DoesInteractionIsActive)
                this.MoveEvent.SafeCall(this.GetMoveOffset() * this.moveFactor * Time.deltaTime);
        }
        #endregion

        #region Intern Behaviour
        private float GetMoveOffset()
        {
            // C'est le vecteur de déplacement de la frame précédente à courante.
            Vector2 currentMoveUserInputRelativeToPreviousFrame = InputService.Instance.GetMoveUserInputPosition();

            return  EMoveDirection.LeftToRight == this.direction ? -(currentMoveUserInputRelativeToPreviousFrame.x) :
                    EMoveDirection.RightToLeft == this.direction ? currentMoveUserInputRelativeToPreviousFrame.x :
                    EMoveDirection.BottomToTop == this.direction ? -(currentMoveUserInputRelativeToPreviousFrame.y) :
                    -currentMoveUserInputRelativeToPreviousFrame.y;
        }
        #endregion
    }
}
