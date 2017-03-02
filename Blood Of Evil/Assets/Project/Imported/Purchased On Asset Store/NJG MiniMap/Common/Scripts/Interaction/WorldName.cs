//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BloodOfEvil.Player;
using BloodOfEvil.Player.Services.Language;

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

            PlayerServicesAndModulesContainer.Instance.LanguageService.NewLanguageHaveBeenLoaded += () =>OnNameChanged(label.text);
        }

        void OnNameChanged(string worldName)
        {
            label.color = NJGMap.instance.zoneColor;
            label.text = PlayerServicesAndModulesContainer.Instance.LanguageService.GetText(ELanguageCategory.MapAreas, worldName);
        }
    }
}
