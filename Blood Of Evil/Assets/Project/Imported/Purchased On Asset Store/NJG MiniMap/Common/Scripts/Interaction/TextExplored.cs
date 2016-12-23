//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Interaction/Text Explored")]
    [RequireComponent(typeof(Text))]
    public class TextExplored : MonoBehaviour
    {
        public string format = "Explored:{0}";
        Text label;
        float lastRatio;

        void Awake() 
        { 
            label = GetComponent<Text>();

            NJGMap.onFOWEnabled += OnFOWEnabled;
            NJGMap.onFOWDisabled += OnFOWDisabled;

            gameObject.SetActive(NJGMap.instance.fow.enabled);
        }

        void OnDestroy()
        {
            NJGMap.onFOWEnabled -= OnFOWEnabled;
            NJGMap.onFOWDisabled -= OnFOWDisabled;
        }

        void OnFOWEnabled()
        {
            gameObject.SetActive(true);
        }

        void OnFOWDisabled()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (NJGFOW.instance == null)
            {
                Debug.LogError("NJGFOW.instance is not set", gameObject);
                return;
            }

            if (lastRatio != NJGFOW.instance.exploredRatio)
            {
                label.text = string.Format(format, (int)(NJGFOW.instance.exploredRatio * 100));
                lastRatio = NJGFOW.instance.exploredRatio;
            }
        }
    }
}
