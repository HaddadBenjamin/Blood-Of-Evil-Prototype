using System;
using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Utilities;
using UnityEngine;

// Tester ce code sur mobile.

namespace BloodOfEvil.Scene.Services
{
    /// <summary>
    /// Permet d'obtenir des informations sur les évênements produit par l'utilisateur à la frame courante et précédente sur les plateformes PC et mobiles.
    /// 1) Récupérer les touches appuyées, restés appuyées, levées à la frame courante ou précédente.
    /// 2) Récupérer la position de l'utilisateur de façon multiplateforme à la frame courante et précédente.
    /// 3) Récupérer le vecteur de déplacement de la frame précédente à la frame courante.
    /// 4) Gérer et mettre à jour les objets cliquables.
    /// Je souhaiterais améliorer ce code plus tard.
    /// </summary>
    public class InputService : ASingletonMonoBehaviour<InputService>
    {
        #region Fields
        [HideInInspector]
        /// <summary>
        /// Informations des évênements à la frame précédente.
        /// </summary>
        public InputFrame PreviousInputFrame { get; private set; }
        [HideInInspector]
        /// <summary>
        /// Informations des évênements à la frame courante.
        /// </summary>
        public InputFrame CurrentInputFrame { get; private set; }

        [SerializeField, Tooltip("C'est le layer sur lequel les objets sont clicable par ce service.")]
        private LayerMask clickableObjectLayer;
        public List<ClickableCollider> ClickableColliders { get; private set; }
        #endregion

        #region Properties
        /// <summary>
        /// C'est un simple getteur de la variable clickableObjectLayer.
        /// </summary>
        public LayerMask GetClickableObjetLayer()
        {
            return this.clickableObjectLayer;
        }
        #endregion

        #region Override Behaviour
        public override void InitializeSingletons()
        {
            this.PreviousInputFrame = new InputFrame();
            this.CurrentInputFrame = new InputFrame();
            this.ClickableColliders = new List<ClickableCollider>();
        }
        #endregion

        #region Unity Behaviour
        public void Awake()
        {
            this.InitializeSingletons();
        }

        /// <summary>
        /// Met à jour les événênements utilisateur de la frame courante et précédente.
        /// </summary>
        private void Update()
        {
            this.UpdateInputsFrames();
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Renvoit le vectuer de déplacement de la position la frame précédnte à la frame courante.
        /// </summary>
        public Vector2 GetMoveUserInputPosition()
        {
            return  this.PreviousInputFrame.UserInputPosition -
                    this.CurrentInputFrame.UserInputPosition;
        }

        /// <summary>
        /// Permet de rajouter un collider cliquable à la liste de ceux clickable.
        /// </summary>
        public void RegisterClickableCollider(ClickableCollider clickableCollider)
        {
            this.ClickableColliders.Add(clickableCollider);
        }

        /// <summary>
        /// Permet de enlever un collider cliquable à la liste de ceux clickable.
        /// </summary>
        public void UnregisterClickableCollider(ClickableCollider clickableCollider)
        {
            this.ClickableColliders.Remove(clickableCollider);
        }

        /// <summary>
        /// Permet de récupérer la position de l'utilisateur sur les plateformes PC et mobiles.
        /// </summary>
        public static Vector2 GetCrossPlatformUserInputPosition()
        {
            return  DoesMobileMoveUserInputIsPressed() ? Input.GetTouch(0).position :
                    new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        /// <summary>
        /// Récupère la valeur de l'énumération KeyCode pour un clic gauche.
        /// </summary>
        public static KeyCode GetMoveUserInputKeyCode()
        {
            return KeyCode.Mouse0;
        }

        /// <summary>
        /// Détermiine si l'utilisateur met un doight sur l'écran de son mobile ou de sa tablette.
        /// </summary>
        public static bool DoesMobileMoveUserInputIsPressed()
        {
            return Input.touchCount > 0;
        }
        #endregion

        #region Intern Behaviour
        private void UpdateInputsFrames()
        {
            this.PreviousInputFrame.Copy(this.CurrentInputFrame);

            this.CurrentInputFrame.UpdateCurrentFrame(this.PreviousInputFrame);

            //Debug.LogFormat("frame courante is down : {0}, is up {1}, is pressed {2}",
            //    this.CurrentInputFrame.DoesKeyIsDown(GetMoveUserInputKeyCode()),
            //    this.CurrentInputFrame.DoesKeyIsUp(GetMoveUserInputKeyCode()),
            //    this.CurrentInputFrame.DoesKeyIsPress(GetMoveUserInputKeyCode()));
        }
        #endregion
    }
}
