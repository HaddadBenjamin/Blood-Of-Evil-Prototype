//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace NJG 
{
    [AddComponentMenu("NJG MiniMap/Interaction/Button Full Screen")]
    public class ButtonFullscreen : MonoBehaviour, IPointerClickHandler 
    {
	    public Image normalState;
        public Image exitState;
        public Image widget;
	    //public UIDragObject drag;
	    //public UIDragResize resize;
	    public float speed = 1;
	    //public UITweener.Method ease = UITweener.Method.BounceOut;
	    public int defaultWidth = 750;
	    public int defaultHeight = 400;

	    bool mToggle;

	    void Awake()
	    {
		    //NGUITools.SetActive(normalState.gameObject, true);
		    //NGUITools.SetActive(exitState.gameObject, false);
	    }

	    public void OnPointerClick(PointerEventData eventData)
	    {
		    mToggle = !mToggle;

		    if (mToggle)
		    {
			    /*widget.cachedTransform.localPosition = Vector3.zero;
			    NGUITools.SetActive(normalState.gameObject, false);
			    NGUITools.SetActive(exitState.gameObject, true);
			    TweenWidth tw = TweenWidth.Begin(widget, speed, Screen.width);
			    tw.method = ease;
			    EventDelegate.Add(tw.onFinished, OnFullScreen, true);

			    TweenHeight th = TweenHeight.Begin(widget, speed, Screen.height);
			    th.method = ease;

			    if (drag != null) NGUITools.SetActive(drag.gameObject, false);
			    if (resize != null) NGUITools.SetActive(resize.gameObject, false);*/
		    }
		    else
		    {
                /*NGUITools.SetActive(normalState.gameObject, true);
                NGUITools.SetActive(exitState.gameObject, false);
                stretch.style = UIStretch.Style.None;
                stretch.enabled = false;

                TweenWidth tw = TweenWidth.Begin(widget, speed, defaultWidth);
                tw.method = ease;

                TweenHeight th = TweenHeight.Begin(widget, speed, defaultHeight);
                th.method = ease;

			    if (drag != null) NGUITools.SetActive(drag.gameObject, true);
			    if (resize != null) NGUITools.SetActive(resize.gameObject, true);*/
            }
	    }

	    void OnFullScreen()
	    {
		    //stretch.enabled = true;
		    //stretch.style = UIStretch.Style.Both;
	    }
    }
}
