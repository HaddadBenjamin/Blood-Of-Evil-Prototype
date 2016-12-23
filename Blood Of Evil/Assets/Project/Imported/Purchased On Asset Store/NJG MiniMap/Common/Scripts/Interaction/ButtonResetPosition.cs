//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Interaction/Button Reset Panning Position")]
    public class ButtonResetPosition : MonoBehaviour, IPointerClickHandler
    {
        public Map map;

        void Start()
        {
            if (map == null) map = GetComponentInParent<Map>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (map == null)
            {
                Debug.LogError("There is no Map instance set.", gameObject);
                return;
            }

            map.ResetPanning();
        }
    }
}
