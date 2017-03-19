using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Extensions;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de lancer un évênement lorsque l'utilisateur sur :
    /// - PC : tourne la molette de sa souris.
    /// - Mobile : place 2 doigts sur son mobile ou sur sa tablette et les déplace. (intéraction pour zoomer ou dézoomer sur une photo).
    /// </summary>
    public class ZoomInZoomOutCrossPlatformInteraction : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est la vitesse de cette intéraction sur PC")]
        private float pcZoomInZoomOutSpeed = 1;
        [SerializeField, Tooltip("C'est la vitesse de cette intéraction sur PC")]
        private float mobileZoomInZoomOutSpeed = 1;

        [Tooltip("C'est l'évênement qui reçoit le zoom in et le zoom out.")]
        public UnityFloatEvent OnZoomInZoomOutAction;

        /// <summary>
        /// Les données spécifiques sur du mobile, la position du premier et deuxième doigt à la frame précédente et si il y a eu 2 doights appuyés à la frame précédente.
        /// </summary>
        private Vector2 firstFingerPositionAtPreviousFrame, secondtFingerPositionAtPreviousFrame;
        private bool doesTwoFingerHaveBeenPressedAtPreviousFrame;
        #endregion

        #region Unity Behaviour
        private void Update()
        {
            // Intéraction sur Mobile.
            #if UNITY_ANDROID || UNITY_IPHONE 
                if (Input.touchCount >= 2 &&
                    this.doesTwoFingerHaveBeenPressedAtPreviousFrame)
                {
                    float distanceBetweenFirstAndSecondFingerAtPreviousFrame = Vector2.Distance(this.firstFingerPositionAtPreviousFrame, this.secondtFingerPositionAtPreviousFrame);
                    float distanceBetweenFirstAndSecondFingerAtCurrentFrame = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

                    this.OnZoomInZoomOutAction.SafeInvoke(this.pcZoomInZoomOutSpeed * (distanceBetweenFirstAndSecondFingerAtCurrentFrame - distanceBetweenFirstAndSecondFingerAtPreviousFrame));
                }

                this.doesTwoFingerHaveBeenPressedAtPreviousFrame = Input.touchCount >= 2;

                if (this.doesTwoFingerHaveBeenPressedAtPreviousFrame)
                {
                    this.firstFingerPositionAtPreviousFrame = Input.touches[0].position;
                    this.firstFingerPositionAtPreviousFrame = Input.touches[1].position;
                }
            #else // Intéraction sur PC.
                float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

                if (mouseWheel != 0)
                    this.OnZoomInZoomOutAction.SafeInvoke(this.pcZoomInZoomOutSpeed * mouseWheel);
            #endif

        }
        #endregion
    }
}
