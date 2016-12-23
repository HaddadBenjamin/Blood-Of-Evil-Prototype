using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities.UI
{
    using Player;
    using Helpers;


    [System.Serializable]
    [RequireComponent(typeof(EventTrigger))]
    public class ATooltilpHolder : MonoBehaviour
    {
        #region Fields
        protected GameObject tooltilpGameObject;
        protected Transform tooltilpTransform;
        [SerializeField]
        private string tooltipGameObjectName;
        private bool updatePosition = true;

        protected bool onPointerEnter;
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
        public void Initialize(UICallbackData[] UICallbacksData)
        {
            this.tooltilpGameObject = PlayerServicesAndModulesContainer.Instance.TooltipsService.Get(this.tooltipGameObjectName).gameObject;
            this.tooltilpTransform = this.tooltilpGameObject.transform;

            UICallbackHelper.AddCallbacksToEventTrigger(GetComponent<EventTrigger>(), UICallbacksData);
        }
        #endregion

        #region Unity Methods
        protected void UpdateTooltipPosition()
        {
            if (this.onPointerEnter &&
                this.UpdatePosition)
                this.tooltilpTransform.position = Input.mousePosition;
        }
        #endregion

        #region Behaviour Methods
        protected void EnterPointerButtonAction(BaseEventData data)
        {
            //SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Open Tooltip");
            this.tooltilpGameObject.SetActive(true);

            this.SetTooltilContent();

            this.onPointerEnter = true;
        }

        protected void ExitPointerButtonAction(BaseEventData data)
        {
            //SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Close Tooltip");
            this.tooltilpGameObject.SetActive(false);

            this.onPointerEnter = false;
        }

        protected virtual void SetTooltilContent() { }
        #endregion
    }
}