//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Interaction/Button Close Map")]
    public class ButtonCloseMap : MonoBehaviour, IPointerClickHandler
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
            map.Hide();
        }
    }
}


