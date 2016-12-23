using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using BloodOfEvil.Extensions;
using BloodOfEvil.Utilities;

namespace BloodOfEvil.Utilities.Animations
{
    using Extensions;

    public class ShakeObject : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float shakeSpeed = 1.0f, shakerTime = 1.0f;

        private Timer shakeTimer;
        private Transform myTransform;
        private Vector3 shakePosition, initialPosition; // La position initiale doit être modifié si l'utilisateur bouge son objet.

        public UnityEvent beforeShakeListener;
        public UnityEvent duringShakeListener;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            this.myTransform = transform;
            this.shakeTimer = new Timer(this.shakerTime);
            this.initialPosition = this.myTransform.position;
        }

        private void Update()
        {
            this.shakeTimer.Update();

            if (!this.shakeTimer.IsRinging())
                this.Shake();
            else
                this.shakePosition = this.initialPosition;

            this.myTransform.position = Vector3.Lerp(this.myTransform.position, this.shakePosition, 0.5f);
        }
        #endregion

        #region Public Behaviour
        private void Shake()
        {
            this.duringShakeListener.SafeCall();

            this.shakePosition = this.myTransform.GetRandomLocationAroundPosition(this.shakeSpeed);
        }
        #endregion

        #region Intern Behaviour
        public void EnableShake()
        {
            this.beforeShakeListener.SafeCall();

            this.shakeTimer.Reset();
        }
        #endregion
    }
}