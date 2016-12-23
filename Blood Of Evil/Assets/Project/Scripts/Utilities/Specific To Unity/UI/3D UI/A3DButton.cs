using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Utilities.UI
{
    using Scene;
    using Player;
    using Player.Services.Audio;
    using Extensions;

    public abstract class AButton3D : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private TextMesh text;
        protected string soundName = "Click Button";
        public Action OnClickListener;
        public Action OnHoverListener;
        public Action<bool> IsHoverListener;
        private bool isHover;
        #endregion

        #region Properties
        public TextMesh Text
        {
            get
            {
                return text;
            }

            private set
            {
                text = value;
            }
        }

        public bool IsHover
        {
            get
            {
                return isHover;
            }

            set
            {
                isHover = value;

                this.IsHoverListener.SafeCall<bool>(this.IsHover);
            }
        }
        #endregion

        #region Untiy Behaviour
        void OnMouseDown()
        {
            this.OnClickListener.SafeCall();

            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, this.soundName);
        }

        void OnMouseOver()
        {
            this.OnHoverListener.SafeCall();
        }

        void OnMouseEnter()
        {
            this.IsHover = true;
        }

        void OnMouseExit()
        {
            this.IsHover = false;
        }
        #endregion
    }
}
   