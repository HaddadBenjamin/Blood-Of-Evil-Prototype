using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace BloodOfEvil.Player.Services.Canvases
{
    public class PanelDraggable : EventTrigger
    {
        #region Fields
        private float offsetX;
        private float offsetY;
        public bool IsDraggable = true;
        private Transform myTransform;
        private Canvas canvas;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.myTransform = transform;
            this.canvas = transform.parent.parent.GetComponent<Canvas>();
        }
        #endregion

        #region Public Behaviour
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            if (this.IsDraggable)
            {
                this.offsetX = this.myTransform.position.x - Input.mousePosition.x;
                this.offsetY = this.myTransform.position.y - Input.mousePosition.y;
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (this.IsDraggable)
                this.myTransform.position = new Vector3(this.offsetX + Input.mousePosition.x, this.offsetY + Input.mousePosition.y);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            // Perrmet de mettre cette fenêtre au dessus de tout autre.
            PlayerServicesAndModulesContainer.Instance.CanvasesService.SetCanvasTransformAtTop(this.canvas);
        }
        #endregion
    }
}