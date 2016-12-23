//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using UnityEngine.UI;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Interaction/Button Zoom Map")]
    public class ButtonZoom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Map map;
        public bool zoomIn;
        public float amount = 0.5f;
        private bool highlight = false;

        void Start()
        {
            if (map == null) map = GetComponentInParent<Map>();
        }

        void Update()
        {
            if (highlight)
                this.Zoom();
        }

        void Zoom()
        {
            if (map == null)
            {
                Debug.LogError("There is no Map instance set.", gameObject);
                return;
            }

            if (zoomIn) map.ZoomIn(amount);
            else map.ZoomOut(amount);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.highlight = false;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            this.highlight = true;
        }
    }
}
