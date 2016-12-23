//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NJG
{

    /// <summary>
    /// Example script that can be used to show tooltips.
    /// </summary>

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("NJG MiniMap/Interaction/Tooltip")]
    public class Tooltip : MonoBehaviour
    {
        static Tooltip mInstance;

        Camera uiCamera;

        public Text text;
        public Image background;
        //public UISprite arrow;	

        public float appearSpeed = 10f;
        public bool scalingTransitions = true;
        public bool followMousePosition = true;
        public Vector2 positionOffset = new Vector2(0f, 30f);
        public Vector2 padding = new Vector2(40f, 25f);

        //private bool _xShifted, _yShifted = false;
        private float width, height, canvasWidth, canvasHeight;
        //private int screenWidth, screenHeight;
        private float YShift, xShift;
        private RenderMode guiMode;

        float mTarget = 0f;
        float mCurrent = 0f;
        Vector3 mPos;
        Vector3 mSize;
        bool inside;


        RectTransform rectTransform;
        CanvasGroup mGroup;

        void OnDestroy() { mInstance = null; }

        /// <summary>
        /// Get a list of widgets underneath the tooltip.
        /// </summary>

        void Awake()
        {
            mInstance = this;
            rectTransform = GetComponent<RectTransform>();
            Canvas canvas = GetComponentInParent<Canvas>();
            mGroup = GetComponent<CanvasGroup>();
            mPos = transform.localPosition;
            mSize = transform.localScale;
            uiCamera = canvas.worldCamera;
            guiMode = canvas.renderMode;

            //size of the screen
            //screenWidth = Screen.width;
            //screenHeight = Screen.height;

            xShift = 0f;
            YShift = -30f;

            //_xShifted = _yShifted = false;

            gameObject.SetActive(false);

            SetAlpha(0f);
        }

        /// <summary>
        /// Update the tooltip's alpha based on the target value.
        /// </summary>

        void Update()
        {
            if (mCurrent != mTarget)
            {
                mCurrent = Mathf.Lerp(mCurrent, mTarget, Time.deltaTime * appearSpeed);
                if (Mathf.Abs(mCurrent - mTarget) < 0.001f) mCurrent = mTarget;
                SetAlpha(mCurrent * mCurrent);

                if (scalingTransitions)
                {
                    Vector3 offset = mSize * 0.25f;
                    offset.y = -offset.y;

                    Vector3 size = Vector3.one * (1.5f - mCurrent * 0.5f);
                    Vector3 pos = Vector3.Lerp(mPos - offset, mPos, mCurrent);

                    transform.localPosition = pos;
                    transform.localScale = size;
                }
            }

            if (inside)
            {
                if (guiMode == RenderMode.ScreenSpaceCamera)
                {
                    UpdatePosition();
                }
            }


            if (followMousePosition && mCurrent > 0) UpdatePosition();
        }

        /// <summary>
        /// Set the alpha of all widgets.
        /// </summary>

        void SetAlpha(float val)
        {
            mGroup.alpha = val;
        }

        /// <summary>
        /// Set the tooltip's text to the specified string.
        /// </summary>

        void SetText(string content, Color bgColor)
        {
            gameObject.SetActive(false);

            if (content != null && !string.IsNullOrEmpty(content))
            {
                mTarget = 1f;
                text.text = content;

                rectTransform.sizeDelta = new Vector2(text.preferredWidth + padding.x, text.preferredHeight + padding.y);
                //background.color = bgColor;

                UpdatePosition();
            }
            else mTarget = 0f;
            SetAlpha(mCurrent * mCurrent);
        }

        /// <summary>
        /// Update Tooltip position.
        /// </summary>

        void UpdatePosition()
        {
            Vector3 newPos = uiCamera.ScreenToViewportPoint(Input.mousePosition - new Vector3(xShift, YShift, 0f));
            Vector3 newPosWVP = uiCamera.ViewportToWorldPoint(newPos);

            width = rectTransform.sizeDelta[0];
            height = rectTransform.sizeDelta[1];

            // check and solve problems for the tooltip that goes out of the screen on the horizontal axis
            float val;

            Vector3 lowerLeft = uiCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
            Vector3 upperRight = uiCamera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));

            //check for right edge of screen
            val = (newPosWVP.x + width / 2);
            if (val > upperRight.x)
            {
                Vector3 shifter = new Vector3(val - upperRight.x, 0f, 0f);
                Vector3 newWorldPos = new Vector3(newPosWVP.x - shifter.x, newPos.y, 0f);
                newPos.x = uiCamera.WorldToViewportPoint(newWorldPos).x;
            }
            //check for left edge of screen
            val = (newPosWVP.x - width / 2);
            if (val < lowerLeft.x)
            {
                Vector3 shifter = new Vector3(lowerLeft.x - val, 0f, 0f);
                Vector3 newWorldPos = new Vector3(newPosWVP.x + shifter.x, newPos.y, 0f);
                newPos.x = uiCamera.WorldToViewportPoint(newWorldPos).x;
            }

            // check and solve problems for the tooltip that goes out of the screen on the vertical axis

            //check for upper edge of the screen
            val = (newPosWVP.y + height / 2);
            if (val > upperRight.y)
            {
                Vector3 shifter = new Vector3(0f, 35f + height / 2, 0f);
                Vector3 newWorldPos = new Vector3(newPos.x, newPosWVP.y - shifter.y, 0f);
                newPos.y = uiCamera.WorldToViewportPoint(newWorldPos).y;
            }

            //check for lower edge of the screen (if the shifts of the tooltip are kept as in this code, no need for this as the tooltip always appears above the mouse bu default)
            val = (newPosWVP.y - height / 2);
            if (val < lowerLeft.y)
            {
                Vector3 shifter = new Vector3(0f, 35f + height / 2, 0f);
                Vector3 newWorldPos = new Vector3(newPos.x, newPosWVP.y + shifter.y, 0f);
                newPos.y = uiCamera.WorldToViewportPoint(newWorldPos).y;
            }

            this.transform.position = new Vector3(newPosWVP.x, newPosWVP.y, 0f);
            this.gameObject.SetActive(true);
            inside = true;
        }

        /// <summary>
        /// Hide the tooltip.
        /// </summary>

        static public void Hide() { if (mInstance != null) mInstance.mTarget = 0f; }

        /// <summary>
        /// Show a tooltip with the specified text.
        /// </summary>

        static public void Show(string contentText)
        {
            if (mInstance != null)
            {
                mInstance.SetText(contentText, Color.white);
            }
        }

        /// <summary>
        /// Show a tooltip with the specified text.
        /// </summary>

        static public void Show(string contentText, Color color)
        {
            if (mInstance != null)
            {
                mInstance.SetText(contentText, color);
            }
        }
    }
}