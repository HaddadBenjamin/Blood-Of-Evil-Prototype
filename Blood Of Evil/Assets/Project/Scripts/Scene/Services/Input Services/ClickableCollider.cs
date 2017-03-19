using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BloodOfEvil.Scene.Services
{
    /// <summary>
    /// Appelle des actions si l'utiisateur clic (gère le multiplateforme) sur ce collider, reste clicé ou relève son doigh.
    /// </summary>
    [   SerializeField,
        RequireComponent(typeof(Collider))]
    public class ClickableCollider : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est l'évênement appelé lorsque l'utilisateur clic cet objet.")]
        public UnityBoolEvent OnClicked;
        [SerializeField, Tooltip("C'est l'évênement appelé lorsque l'utilisateur à le curseur de sa souris appuyé sur cet objet.")]
        public UnityBoolEvent OnClickedDown;
        [SerializeField, Tooltip("C'est l'évênement appelé lorsque l'utilisateur ne clic plus sur cet objet.")]
        public UnityBoolEvent OnClickedUp;

        public bool isClicked;
        public bool isClickedDown;
        #endregion

        #region Properties
        public bool IsClickedUp
        {
            get
            {
                return !IsClicked;
            }

            set
            {
                IsClicked = !value;

                this.OnClickedUp.SafeInvoke(!IsClicked);
            }
        }

        public bool IsClickedDown
        {
            get
            {
                return isClickedDown;
            }

            set
            {
                isClickedDown = value;

                OnClickedDown.SafeInvoke(IsClickedDown);
            }
        }

        public bool IsClicked
        {
            get
            {
                return isClicked;
            }

            set
            {
                isClicked = value;

                OnClicked.SafeInvoke(IsClicked);
            }
        }
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            InputService.Instance.RegisterClickableCollider(this);
        }

        private void OnEnable()
        {
            if (null != InputService.Instance)
            InputService.Instance.RegisterClickableCollider(this);
        }

        private void OnDestroy()
        {
            if (null != InputService.Instance)
                InputService.Instance.UnregisterClickableCollider(this);
        }

        private void OnDisable()
        {
            if (null != InputService.Instance)
                InputService.Instance.UnregisterClickableCollider(this);
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Permet de dire que ce collider n'a pas reçu de clic.
        /// </summary>
        public void Reset()
        {
            this.IsClicked = false;
            this.IsClickedDown = false;
        }
        #endregion
    }

    public static class ClickableColliderExtension
    {
        public static void Copy(this ClickableCollider thisObject, ClickableCollider clickableCollider)
        {
            if (null == thisObject)
                thisObject = clickableCollider;

            if (null != clickableCollider)
            {
                thisObject.IsClicked = clickableCollider.IsClicked;
                thisObject.IsClickedDown = clickableCollider.IsClickedDown;
            }
        }
    }
}
