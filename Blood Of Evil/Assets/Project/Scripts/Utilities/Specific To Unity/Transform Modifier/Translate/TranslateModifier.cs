using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Extensions;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de modifier la position d'un objet dans une direction et une vitesse configurable.
    /// </summary>
    public class TranslateModifier : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est la direction dans laquel on va déplacer notre objet.")]
        private ERotateDirection translateDirection;
        [SerializeField, Tooltip("C'est la vitesse de déplacement de l'objet.")]
        private float translateSpeed = 1.0f;
        [SerializeField, Tooltip("C'est la valeur de position minimal et maximale que peut avoir notre objet.")]
        private ClampedValue clampedScale = new ClampedValue();
        [SerializeField, Tooltip("C'est l'objet qui contiend l'échelle que l'on modifiera.")]
        private Transform translateTransform;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            // Permet de définir la valeur de base.
            this.clampedScale.AddValue(this.GetPositionValue());
        }

        private void LateUpdate()
        {
            float positionValue = this.GetPositionValue();

            if (this.clampedScale.Value != positionValue)
                this.clampedScale.ModifyValue(positionValue);
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Modifie la position de l'objet dans la direction et l'axe et la vitesse choisie.
        /// </summary>
        public void ModifyPosition()
        {
            this.clampedScale.AddValue(this.translateSpeed * Time.deltaTime);

            this.ModifyPosition(this.clampedScale.Value);
        }

        /// <summary>
        /// Modifie la taille de l'objet sur l'axe et la vitesse choisie.
        /// </summary>
        public void ModifyPositionWithOtherSpeed(float otherSpeed)
        {
            float addedValue = this.translateSpeed * otherSpeed * Time.deltaTime *
                               (ERotateDirection.Left == this.translateDirection || ERotateDirection.Down == this.translateDirection || ERotateDirection.Backward == this.translateDirection ?
                               1 : -1);

            this.clampedScale.AddValue(addedValue);

            this.ModifyPosition(this.clampedScale.Value);
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Renvoit la valeur de la position par rapport à la direction choisit.
        /// </summary>
        private float GetPositionValue()
        {
            return  ERotateDirection.Left == this.translateDirection || ERotateDirection.Right == this.translateDirection ? this.translateTransform.localPosition.x :
                    ERotateDirection.Down == this.translateDirection || ERotateDirection.Up == this.translateDirection ? this.translateTransform.localPosition.y :
                    this.translateTransform.localPosition.z;
        }

        private void ModifyPosition(float newPosition)
        {
            if (null != this.translateTransform)
            {
                if (ERotateDirection.Left == this.translateDirection || ERotateDirection.Right == translateDirection)
                    this.translateTransform.SetLocalPositionX(newPosition);
                else if (ERotateDirection.Down == this.translateDirection || ERotateDirection.Up == translateDirection)
                    this.translateTransform.SetLocalPositionY(newPosition);
                else
                    this.translateTransform.SetLocalPositionZ(newPosition);
            }
        }
        #endregion
    }
}
