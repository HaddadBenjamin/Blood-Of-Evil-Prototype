//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Interaction/Text World Name")]
    [RequireComponent(typeof(Text))]
    public class WorldName : MonoBehaviour
    {
        Text label;

        void Awake()
        {
            label = GetComponent<Text>();
            if (NJGMap.instance != null) NJGMap.onWorldZoneChange += OnNameChanged;
        }

        void OnNameChanged(string worldName)
        {
            label.color = NJGMap.instance.zoneColor;
            label.text = worldName;
        }
    }
}
