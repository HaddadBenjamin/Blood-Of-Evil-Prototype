using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Scene.Services
{
    /// <summary>
    /// Contiend des informations d'évênements à une frame spécifique.
    /// Je souhaiterais améliorer ce code plus tard.
    /// </summary>
    [System.Serializable]
    public class InputFrame
    {
        #region Fields
        /// <summary>
        /// C'est la position du doight de l'utilisateur sur mobile et la position du curseur de la souris sur PC.
        /// </summary>
        public Vector2 UserInputPosition { get; private set; }
        /// <summary>
        /// C'est l'ensemble des touches en état "Down".
        /// </summary>
        public List<KeyCode> KeysDown { get; private set; }
        /// <summary>
        /// C'est l'ensemble des touches en état "Press".
        /// </summary>
        public List<KeyCode> KeysPressed { get; private set; }
        /// <summary>
        /// C'est l'ensemble des colliders qui ont été clickés.
        /// </summary>
        public ClickableCollider ClickableCollider;
        #endregion

        #region Constructor
        public InputFrame()
        {
            this.KeysDown = new List<KeyCode>();
            this.KeysPressed = new List<KeyCode>();
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Permet de savoir si la touche sélectionné est resté appuyé.
        /// </summary>
        public bool DoesKeyIsDown(KeyCode keyCode)
        {
            return this.KeysDown.Contains(keyCode);
        }

        /// <summary>
        /// Permet de savoir si la touche sélectionné est appuyé.
        /// </summary>
        public bool DoesKeyIsPress(KeyCode keyCode)
        {
            return this.KeysPressed.Contains(keyCode);
        }

        /// <summary>
        /// Permet de savoir si la touche sélectionné est appuyé.
        /// </summary>
        public bool DoesKeyIsUp(KeyCode keyCode)
        {
            return !this.DoesKeyIsDown(keyCode);
        }

        /// <summary>
        /// Met à jour les données de la frame courante.
        /// </summary>
        public void UpdateCurrentFrame(InputFrame previousInputFrame)
        {
            this.Clear();

            this.UpdateCurrentFrameInputs(previousInputFrame);
            this.UpdateCurrentFrameClickObjects(previousInputFrame);
        }

        /// <summary>
        /// C'est clairement dégueulasse.
        /// </summary>
        public void Copy(InputFrame currentInputFrame)
        {
            this.Clear();

            foreach (var keyDown in currentInputFrame.KeysDown)
                this.KeysDown.Add(keyDown);

            foreach (var keyPress in currentInputFrame.KeysPressed)
                this.KeysPressed.Add(keyPress);

            this.ClickableCollider.Copy(currentInputFrame.ClickableCollider);

            this.UserInputPosition = new Vector2(currentInputFrame.UserInputPosition.x, currentInputFrame.UserInputPosition.y);
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Met à jour la touche de déplacement de l'utilisateur pour mobile.
        /// </summary>
        private void UpdateMoveUserPositionForMobile(InputFrame previousInputFrame)
        {
            KeyCode moveUserInputKeyCode = InputService.GetMoveUserInputKeyCode();

            if (InputService.DoesMobileMoveUserInputIsPressed())
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                    this.KeysPressed.Add(moveUserInputKeyCode);

                this.KeysDown.Add(moveUserInputKeyCode);

                this.UserInputPosition = Input.touches[0].position;
            }
        }

        /// <summary>
        /// Nettoie les listes d'évênements et des colliders cliquables.
        /// </summary>
        private void Clear()
        {
            this.KeysDown.Clear();
            this.KeysPressed.Clear();
        }

        /// <summary>
        /// Met à jour les données relative au input de la frame courante.
        /// </summary>
        private void UpdateCurrentFrameInputs(InputFrame previousInputFrame)
        {
            foreach (KeyCode keyCode in EnumerationHelper.GetAllEnumerationValues<KeyCode>())
            {
                if (Input.GetKeyDown(keyCode))
                    KeysPressed.Add(keyCode);

                if (Input.GetKey(keyCode))
                    this.KeysDown.Add(keyCode);
            }

            this.UpdateMoveUserPositionForMobile(previousInputFrame);

            this.UserInputPosition = InputService.GetCrossPlatformUserInputPosition();
        }

        /// <summary>
        /// Met à jour les objets clickable à la frame courante.
        /// </summary>
        private void UpdateCurrentFrameClickObjects(InputFrame previousInputFrame)
        {
            if (this.DoesKeyIsDown(InputService.GetMoveUserInputKeyCode()))
            {
                RaycastHit raycastHit;
                Ray ray = Camera.main.ScreenPointToRay(InputService.GetCrossPlatformUserInputPosition());
                LayerMask clickableLayerMask = InputService.Instance.GetClickableObjetLayer();

                if (Physics.Raycast(ray, out raycastHit, clickableLayerMask))
                {
                    ClickableCollider clickableColliderTemporary = raycastHit.collider.GetComponent<ClickableCollider>();

                    if (null != clickableColliderTemporary)
                    {
                        if (null == this.ClickableCollider)
                            ClickableCollider = clickableColliderTemporary;

                        ClickableCollider.IsClickedDown = true;

                        ClickableCollider.IsClicked = (null == previousInputFrame.ClickableCollider ||
                                                      !previousInputFrame.ClickableCollider.IsClickedDown);
                    }

                    foreach (var clickableColliderNode in InputService.Instance.ClickableColliders)
                            clickableColliderNode.Reset();
                }
            }
            else
                this.ResetAllCollider();
        }

        private void ResetAllCollider()
        {
            if (null != ClickableCollider)
                ClickableCollider.Reset();

            ClickableCollider = null;

            foreach (var clickableColliderNode in InputService.Instance.ClickableColliders)
                clickableColliderNode.Reset();
        }
        #endregion
    }
}
