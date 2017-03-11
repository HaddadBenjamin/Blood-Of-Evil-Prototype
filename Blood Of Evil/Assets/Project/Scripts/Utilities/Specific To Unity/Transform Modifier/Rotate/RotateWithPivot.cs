using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de tourner un objet autour d'un point de pivot et une vitesse configurable.
    /// </summary>
    public class RotateWithPivot : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est le sens que l'on utilisera pour tourner cet objet 3D")]
        private ERotateDirection rotateDirection;
        [SerializeField, Tooltip("C'est la vitesse de rotation.")]
        private float rotateSpeed = 1.0f;
        [SerializeField, Tooltip("On tournera l'objet autour de ce point de pivot.")]
        private Transform pivot;
        [SerializeField, Tooltip("C'est l'objet que l'on tournera.")]
        private Transform objectToRotate;
        #endregion

        #region Public Behaviour
        public void Rotate()
        {
            this.objectToRotate.RotateAround(this.pivot.position, this.GetRotateAxis(), this.rotateSpeed * Time.deltaTime);
        }

        public void Rotate(float speed)
        {
            this.objectToRotate.RotateAround(this.pivot.position, this.GetRotateAxis(), speed * this.rotateSpeed * Time.deltaTime);
        }
        #endregion

        #region Intern Behaviour
        private Vector3 GetRotateAxis()
        {
            return  ERotateDirection.Right == this.rotateDirection ? Vector3.up :
                    ERotateDirection.Left == this.rotateDirection ? Vector3.down :
                    ERotateDirection.Down == this.rotateDirection ? Vector3.left :
                    ERotateDirection.Up == this.rotateDirection ? Vector3.right :
                    ERotateDirection.Forward == this.rotateDirection ? Vector3.forward :
                    Vector3.back;
        }
        #endregion
    }
}
