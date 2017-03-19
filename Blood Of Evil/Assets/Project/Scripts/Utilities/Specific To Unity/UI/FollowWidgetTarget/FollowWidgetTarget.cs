using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities.UI
{
    /// <summary>
    /// Permet de placer un widget (position 2D) à la position 2D correspondant à un point 3D.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class FollowWidgetTarget : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est la cible 3D à suivre.")]
        private Transform target;
        private RectTransform rectTransform;
        #endregion

        #region Properties
        public Transform Target
        {
            get { return target; }
        }
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            this.rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (null != this.target)
                this.rectTransform.position = Camera.main.WorldToScreenPoint(this.target.position);

        }
        #endregion

        #region Public Behaviour
        public void SetWidgetTarget(Transform newTarget)
        {
            this.target = newTarget;
        }
        #endregion
    }
}
