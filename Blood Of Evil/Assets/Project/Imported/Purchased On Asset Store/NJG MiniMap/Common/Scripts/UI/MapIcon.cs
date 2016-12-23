//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game map can have icons on it -- this class takes care of animating them when needed.
/// </summary>

namespace NJG
{
    [RequireComponent(typeof(CanvasGroup))]
	public class MapIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
	{
        static List<MapIcon> selected = new List<MapIcon>();

		public MapItem item;
		public bool isValid = false;
		public bool isMapIcon = true;
		public bool isVisible = false;
        public Image icon;
        public Image border;

		public float alpha { get { return mAlpha; } set { mAlpha = value; } }

		bool isLooping;
		bool isScaling;
		Vector3 onHoverScale = new Vector3(1.3f, 1.3f, 1.3f);
		//TweenParms tweenParms = new TweenParms();

		//Tweener mLoop;
		float mAlpha = 1;
		bool mFadingOut;
		bool mSelected;
        Color mColor;
        //TweenColor mTweenColor;
        //TweenScale mLoop;
        CanvasGroup group;

		/// <summary>
		/// Triggered when the icon is visible on the map.
		/// </summary>

		void Start() 
        {
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
            UnSelect();
            if (item.fadeOutAfterDelay == 0) group.alpha = 1;

            if (Application.isPlaying) { CheckAnimations(); } 
        }

		void Update() 
		{
			if (mSelected != item.isSelected)
			{
				mSelected = item.isSelected;
				if (mSelected) Select();
				else UnSelect();
			}

			if (item.showIcon && item.showOnAction)
			{
				OnVisible();
				item.showIcon = false;
			}
		}

        /// <summary>
        /// Display a tooltip with the appropiate content.
        /// </summary>

        void OnTooltip(bool show)
        {
            if (!string.IsNullOrEmpty(item.content))
            {
                if (show)
                    Tooltip.Show(item.content);
                else
                    Tooltip.Hide();
            }
        }

        /// <summary>
        /// Triggered when mouse is over this icon.
        /// </summary>

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isLooping)
            {
                //tweenParms.Prop("localScale", onHoverScale).Ease(EaseType.EaseOutExpo);
                //HOTween.To(cachedTransform, 0.1f, tweenParms);

                //TweenScale.Begin(sprite.cachedGameObject, 0.1f, onHoverScale);
                LeanTween.scale(icon.gameObject, onHoverScale, 0.1f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isLooping)
            {
                //tweenParms.Prop("localScale", Vector3.one).Ease(EaseType.EaseOutExpo);
                //HOTween.To(cachedTransform, 0.3f, tweenParms);

                //TweenScale.Begin(sprite.cachedGameObject, 0.3f, Vector3.one);
                LeanTween.scale(icon.gameObject, Vector3.one, 0.3f);
            }
        }

        public void OnPointerClick(PointerEventData eventData) { Select(); }

        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
        }

        /*public void OnDeselect(BaseEventData eventData)
        {
            if (!Input.GetKey(KeyCode.LeftShift) && !item.forceSelection) UnSelectAll();
        }*/

        /// <summary>
        /// React to key-based input.
        /// </summary>

        /*void OnKey(KeyCode key)
        {
            if (enabled && gameObject.activeInHierarchy)
            {
                if (key == KeyCode.Escape)
                {
                    OnDeselect(null);
                }
            }
        }*/

		public void Select()
		{
			if (!Input.GetKey(KeyCode.LeftShift) && !item.forceSelection)
				UnSelectAll();

            if (border != null) border.enabled = true;

			item.isSelected = true;			
			if (!selected.Contains(this)) selected.Add(this);
		}

		public void UnSelect() 
        {
            if (border != null) border.enabled = false;
        }

		static public void UnSelectAll()
		{
			for (int i = 0, imax = selected.Count; i < imax; i++)
			{
				MapIcon ic = selected[i];
				ic.UnSelect();
			}
			selected.Clear();
		}

		protected void CheckAnimations()
		{
			alpha = 1;
			if (item != null)
			{				
				if (item.showOnAction)
					transform.localScale = Vector3.zero;
				else if (item.fadeOutAfterDelay > 0)
				{
					if (!mFadingOut)
					{
						mFadingOut = true;
						StartCoroutine(DelayedFadeOut());
					}
				}
				else if (item.loopAnimation)
					OnLoop();
				else if (item.animateOnVisible && !isMapIcon && item.fadeOutAfterDelay == 0)
					OnVisible();				
			}
		}

		/// <summary>
		/// Add this unit to the list of in-game units.
		/// </summary>

		void OnEnable()
		{
			if (Application.isPlaying)
			{				
				//if (mLoop != null && !item.loopAnimation) mLoop.Kill();
				transform.localScale = Vector3.one;
				CheckAnimations();
			}
		}

		/// <summary>
		/// Remove this unit from the list.
		/// </summary>

		void OnDisable()
		{
			if (mFadingOut)
			{
				mFadingOut = false;
				StopAllCoroutines();
			}
			if (isVisible) isVisible = false;
		}

		/// <summary>
		/// Triggered when the icon is visible on the map.
		/// </summary>

		void OnVisible()
		{
            if (!isVisible)
            {
                if (item.fadeOutAfterDelay > 0)
                {
                    if (!mFadingOut)
                    {
                        mFadingOut = true;
                        StartCoroutine(DelayedFadeOut());
                    }
                }

                if(icon != null) LeanTween.alpha(icon.gameObject, 1f, 0.3f).setFrom(0).setEase(LeanTweenType.linear);

                /*TweenAlpha ta = TweenAlpha.Begin(sprite.cachedGameObject, 0.3f, 1f);
                ta.from = 0;
                ta.method = UITweener.Method.Linear;*/

                if (!item.loopAnimation)
                {
                    //icon.transform.localScale = Vector3.zero;
                    //TweenScale ts = TweenScale.Begin(sprite.cachedGameObject, 0.5f, Vector3.one);
                    //ts.method = UITweener.Method.BounceIn;
                }
                isVisible = true;
            }		
		}

		protected virtual void OnLoop()
		{
            if (item.loopAnimation)
            {
                isLooping = true;

                LeanTween.scale(icon.gameObject, Vector3.one, 1).setFrom(Vector3.one * 1.5f).setLoopPingPong().setEase(LeanTweenType.linear);

                /*if (mLoop == null)
                {
                    mLoop = TweenScale.Begin(sprite.cachedGameObject, 1, Vector3.one);
                    mLoop.from = Vector3.one * 1.5f;
                    mLoop.style = UITweener.Style.PingPong;
                    mLoop.method = UITweener.Method.Linear;
                }*/
            }
		}

		protected IEnumerator DelayedFadeOut()
		{
			yield return new WaitForSeconds(item.fadeOutAfterDelay);

			OnFadeOut();
		}

		protected virtual void OnFadeOut()
		{
            LeanTween.alpha(icon.gameObject, 0f, 1f).setEase(LeanTweenType.linear);
            //TweenAlpha ta = TweenAlpha.Begin(sprite.cachedGameObject, 1f, 0f);
            //ta.method = UITweener.Method.Linear;

            /*if (mTweenColor == null)
            {
                mColor.a = 0;
                mTweenColor = TweenColor.Begin(sprite.cachedGameObject, 1, mColor);
                mColor.a = 1;
                mTweenColor.from = mColor;
                mTweenColor.method = UITweener.Method.Linear;
            }
            else
            {
                mTweenColor.Play(true);
            }*/
            mFadingOut = false;
		}
	}
}