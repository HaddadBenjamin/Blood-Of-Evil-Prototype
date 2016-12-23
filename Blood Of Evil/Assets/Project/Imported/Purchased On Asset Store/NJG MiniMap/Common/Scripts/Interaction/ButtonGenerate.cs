//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Interaction/Button Generate Map")]
    public class ButtonGenerate : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            NJGMap.GenerateMap();
        }
    }
}
