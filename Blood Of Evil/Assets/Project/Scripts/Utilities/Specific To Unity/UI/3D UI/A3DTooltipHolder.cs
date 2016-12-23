using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.UI
{
    using Player;
    public abstract class A3DTooltipHolder : MonoBehaviour
    {
        #region Fields
        protected GameObject tooltilpGameObject;
        protected Transform tooltilpTransform;
        [SerializeField]
        private string tooltipGameObjectName;
        private bool updatePosition = true;
        protected bool onPointerEnter;
        protected AButton3D button3D;
        #endregion

        #region Properties
        public bool UpdatePosition
        {
            get
            {
                return updatePosition;
            }

            set
            {
                updatePosition = value;
            }
        }
        #endregion

        #region Initialize
        public void Initialize()
        {
            this.tooltilpGameObject = PlayerServicesAndModulesContainer.Instance.TooltipsService.Get(this.tooltipGameObjectName).gameObject;
            this.tooltilpTransform = this.tooltilpGameObject.transform;
            this.button3D = GetComponent<AButton3D>();

            this.button3D.IsHoverListener += delegate (bool isHover)
            {
                if (isHover)
                    this.ShowTooltip();
                else
                    this.UnshowTooltip();
            };
        }
        #endregion

        #region Unity Methods
        protected void UpdateTooltipPosition()
        {
            if (this.button3D.IsHover &&
                this.UpdatePosition)
                this.tooltilpTransform.position = Input.mousePosition;
        }
        #endregion

        #region Behaviour Methods
        protected void ShowTooltip()
        {
            //SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Open Tooltip");
            this.tooltilpGameObject.SetActive(true);

            this.SetTooltilContent();

            this.onPointerEnter = true;
        }

        protected void UnshowTooltip()
        {
            //SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Close Tooltip");
            this.tooltilpGameObject.SetActive(false);

            this.onPointerEnter = false;
        }

        protected virtual void SetTooltilContent() { }
        #endregion
    }
}