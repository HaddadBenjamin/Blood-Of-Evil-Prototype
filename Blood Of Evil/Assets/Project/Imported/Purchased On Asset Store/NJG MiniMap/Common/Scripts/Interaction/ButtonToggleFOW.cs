//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Interaction/Button Toggle FOW")]
    public class ButtonToggleFOW : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (NJGFOW.instance == null)
            {
                Debug.LogError("There is no FOW instance set.", gameObject);
                return;
            }

            NJGMap.instance.fow.enabled = !NJGMap.instance.fow.enabled;
        }
    }
}
