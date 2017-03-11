using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de modifier l'échelle sur un axe et à une vitesse configurable.
    /// </summary>
    public class ScaleModifier : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est la composante du vecteur qui modifiera l'échelle.")]
        private EScaleComponent componentToScale;
        [SerializeField, Tooltip("C'est la vitesse de modification de l'échelle.")]
        private float scaleSpeed = 1.0f;
        [SerializeField, Tooltip("C'est l'échelle maximale et minimale.")]
        private ClampedValue clampedScale = new ClampedValue();
        [SerializeField, Tooltip("C'est l'objet qui contiend l'échelle que l'on modifiera.")]
        private Transform scaleTransform;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            // Permet de définir la valeur de base.
            this.clampedScale.AddValue(this.GetScaleValue());
        }

        private void LateUpdate()
        {
            float scaleValue = this.GetScaleValue();

            if (this.clampedScale.Value != scaleValue)
                this.clampedScale.ModifyValue(scaleValue);
        }
        #endregion

        #region Public Behaviour]
        /// <summary>
        /// Modifie la taille de l'objet sur l'axe et la vitesse choisie.
        /// </summary>
        public void ModifyScale()
        {
            this.clampedScale.AddValue(this.scaleSpeed * Time.deltaTime);

            this.ModifyScale(this.clampedScale.Value);
        }

        /// <summary>
        /// Modifie la taille de l'objet sur l'axe et la vitesse choisie.
        /// </summary>
        public void ModifyScaleWithOtherSpeed(float otherSpeed)
        {
            this.clampedScale.AddValue(this.scaleSpeed * otherSpeed * Time.deltaTime);

            this.ModifyScale(this.clampedScale.Value);
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Renvoit la valeur d'échelle de la composante d'échelle choisit.
        /// </summary>
        private float GetScaleValue()
        {
            return  EScaleComponent.Height == this.componentToScale ? this.scaleTransform.localScale.y :
                    EScaleComponent.Length == this.componentToScale ? this.scaleTransform.localScale.z :
                    this.scaleTransform.localScale.x;
        }

        private void ModifyScale(float newScale)
        {
            if (null != this.scaleTransform)
            {
                if (EScaleComponent.Height == this.componentToScale)
                    this.scaleTransform.SetYLocalScale(newScale);
                else if (EScaleComponent.Length == this.componentToScale)
                    this.scaleTransform.SetZLocalScale(newScale);
                else
                    this.scaleTransform.SetXLocalScale(newScale);
            }
        }
        #endregion
    }
}
