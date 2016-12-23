//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using NJG;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Text World Name (Dynamic)")]
[RequireComponent(typeof(Text))]
public class DynamicWorldName : MonoBehaviour 
{
    public LeanTweenType easing = LeanTweenType.easeOutCirc;
	public bool showOnStart = true;
	public float fadeInDuration = 0.5f;
	public float fadeOutDuration = 1f;
	public float hideDelay = 3;

    Text label;
    RectTransform rect;

	void Awake() 
	{
        label = GetComponent<Text>();
        rect = label.GetComponent<RectTransform>();
        NJGMap.onWorldZoneChange += OnNameChanged;
    }

	/// <summary>
	/// Start with fade in effect.
	/// </summary>

	void Start()
	{
		
		if(showOnStart) FadeIn();
	}

	/// <summary>
	/// Fades in the label wait few seconds then fade out.
	/// </summary>

	void FadeIn()
	{
        /*TweenColor tc = TweenColor.Begin(gameObject, fadeInDuration, ec);
		tc.from = sc;
		tc.method = method;*/

        if (LeanTween.isTweening(rect.gameObject)) LeanTween.cancel(rect.gameObject);

        LTDescr t = LeanTween.textAlpha(rect, 1, fadeInDuration);
        t.setFrom(0);
        t.setEase(easing);

        LeanTween.delayedCall(hideDelay, FadeOut);
	}

	/// <summary>
	/// Fades out the label.
	/// </summary>

	void FadeOut()
	{
        /*TweenColor tc = TweenColor.Begin(gameObject, fadeOutDuration, ec);
		tc.from = sc;
		tc.method = method;*/

        LTDescr t = LeanTween.textAlpha(rect, 0, fadeInDuration);
        t.setFrom(1);
        t.setEase(easing);
	}

	/// <summary>
	/// The name changed so lets fade in and change the label. 
	/// </summary>

	void OnNameChanged(string worldName)
	{
        label.color = NJGMap.instance.zoneColor;
        label.text = worldName;
		FadeIn();		
	}
}
