//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Map Item")]
    public class MapItem : MonoBehaviour
    {
        static public Action onListChanged;
        static public List<MapItem> list = new List<MapItem>();

        #region Public Properties

        public bool isRevealed = false;
        public bool revealFOW = false;
        public bool drawDirection = false;
        public string content = "";
        public int type = 0;
        public int revealDistance = 0;
        public MapArrow arrow;
        public Map map;

        public Action<bool> onSelect;
        public Color color { get { if (!mColorSet) mColor = NJGMap.instance.GetColor(type); mColorSet = true; return mColor; } }
        public bool rotate { get { if (!mRotateSet) { mRotateSet = true; mRotate = NJGMap.instance.GetRotate(type); } return mRotate; } }
        public bool interaction { get { if (!mInteractionSet) { mInteractionSet = true; mInteraction = NJGMap.instance.GetInteraction(type); } return mInteraction; } }
        //public bool alwaysVisible { get { if (!mAlwaysSet) { mAlwaysSet = true; mAlways = NJGMap.instance.GetAlwaysVisible(type); } return mAlways; } }
        public bool arrowRotate { get { if (!mArrowRotateSet) { mArrowRotateSet = true; mArrowRotate = NJGMap.instance.GetArrowRotate(type); } return mArrowRotate; } }
        public bool updatePosition { get { if (!mUpdatePosSet) { mUpdatePosSet = true; mUpdatePos = NJGMap.instance.GetUpdatePosition(type); } return mUpdatePos; } }
        public bool smoothPosition { get { if (!mSmoothPosSet) { mSmoothPosSet = true; mSmoothPos = NJGMap.instance.GetSmoothPosition(type); } return mSmoothPos; } }
        public bool animateOnVisible { get { if (!mAnimOnVisibleSet) { mAnimOnVisibleSet = true; mAnimOnVisible = NJGMap.instance.GetAnimateOnVisible(type); } return mAnimOnVisible; } }
        public bool showOnAction { get { if (!mAnimOnActionSet) { mAnimOnActionSet = true; mAnimOnAction = NJGMap.instance.GetAnimateOnAction(type); } return mAnimOnAction; } }
        public bool loopAnimation { get { if (!mLoopSet) { mLoopSet = true; mLoop = NJGMap.instance.GetLoopAnimation(type); } return mLoop; } }
        public bool haveArrow { get { if (!mArrowSet) { mArrowSet = true; mArrow = NJGMap.instance.GetHaveArrow(type); } return mArrow; } }
        //public bool arrowAlwaysVisible { get { if (!mArrowAlwaysSet) { mArrowAlwaysSet = true; mArrowAlways = NJGMap.instance.GetArrowAlwaysVisible(type); } return mArrowAlways; } }
        //public bool revealFOW { get { if (!mFOWSet) { mFOWSet = true; mFOW = NJGMap.instance.GetRevealFOW(type); } return mFOW; } }
        public float fadeOutAfterDelay { get { if (mFadeOut == -1) mFadeOut = NJGMap.instance.GetFadeOutAfter(type); return mFadeOut; } }
        public int size
        {
            get
            {
                if (NJGMap.instance.GetCustom(type))
                    mSize = NJGMap.instance.GetSize(type);
                else mSize = NJGMap.instance.iconSize;
                return mSize;
            }
        }
        public int borderSize
        {
            get
            {
                if (NJGMap.instance.GetCustomBorder(type))
                    mBSize = NJGMap.instance.GetBorderSize(type);
                else mBSize = NJGMap.instance.borderSize;
                return mBSize;
            }
        }
        public virtual Vector3 iconScale
        {
            get
            {
                if (mIconSize != size)
                {
                    mIconSize = size;
                    mIconScale.x = mIconScale.y = size;
                }
                return mIconScale;
            }
        }
        public virtual Vector3 borderScale
        {
            get
            {
                if (mBorderSize != borderSize)
                {
                    mBorderSize = borderSize;
                    mBorderScale.x = mBorderScale.y = borderSize;
                }
                return mBorderScale;
            }
        }
        //public int depth { get { if (mDepth == int.MaxValue) mDepth = NJGMap.instance.GetDepth(type); return mDepth; } }
        //public int arrowDepth { get { if (mArrowDepth == int.MaxValue) mArrowDepth = NJGMap.instance.GetArrowDepth(type); return mArrowDepth; } }
        public int arrowOffset { get { if (mArrowOffset == int.MaxValue) mArrowOffset = NJGMap.instance.GetArrowOffset(type); return mArrowOffset; } }

        public bool isSelected { get { return mSelected; } set { mSelected = value; if (onSelect != null) onSelect(mSelected); } }
        public bool forceSelection { get { return mForceSelect; } set { mForceSelect = value; } }

        public bool showIcon;

        /// <summary>
        /// Whether the revealer is actually active or not. If you wanted additional checks such as "is the unit dead?",
        /// then simply derive from this class and change the "isActive" value accordingly.
        /// </summary>

        public bool isActive = true;


        public MapIcon icon;
        #endregion

        #region Private Properties

        Color mColor = Color.clear;
        bool mInteraction;
        bool mRotate;
        bool mArrowRotate;
        bool mUpdatePos;
        bool mSmoothPos;
        bool mAnimOnVisible;
        bool mAnimOnAction;
        bool mLoop;
        bool mArrow;
        //bool mAlways;
        //bool mArrowAlways;
        bool mFOW;
        float mFadeOut = -1;
        Vector2 mIconScale;
        Vector2 mBorderScale;
        int mIconSize = int.MaxValue;
        int mSize = int.MaxValue;
        int mBSize = int.MaxValue;
        int mBorderSize = int.MaxValue;
        //int mDepth = int.MaxValue;
        //int mArrowDepth = int.MaxValue;
        int mArrowOffset = int.MaxValue;

        bool mInteractionSet;
        public bool mColorSet = false;
        bool mRotateSet;
        bool mArrowRotateSet;
        bool mUpdatePosSet;
        bool mSmoothPosSet;
        bool mAnimOnVisibleSet;
        bool mAnimOnActionSet;
        bool mLoopSet;
        bool mArrowSet;
        //bool mAlwaysSet;
        //bool mArrowAlwaysSet;
        bool mFOWSet;

        bool mForceSelect;
        bool mSelected;
        NJGFOW.Revealer mRevealer;
        Map miniMap;
        Map worldMap;

        #endregion

        #region Monobehaviour Methods

        void Awake()
        {
            miniMap = Map.miniMap;
            worldMap = Map.miniMap;
        }

        void Start()
        {
            if (NJGMap.instance != null)
            {
                if (revealFOW && NJGMap.instance.fow.enabled) mRevealer = NJGFOW.CreateRevealer();
            }

            if (miniMap != null)
            {
                icon = miniMap.GetEntry(this);
                arrow = miniMap.GetArrow(this);
            }
            if (worldMap != null)
            {
                icon = worldMap.GetEntry(this);
                arrow = worldMap.GetArrow(this);
            }

            mColorSet = false;
            //enabled = revealFOW;
        }

        /// <summary>
        /// Add this unit to the list of in-game units.
        /// </summary>

        void OnEnable()
        {
            if (!list.Contains(this)) { list.Add(this); if (onListChanged != null) onListChanged(); }

            if (miniMap != null)
            {
                icon = miniMap.GetEntry(this);
                arrow = miniMap.GetArrow(this);
            }
            if (worldMap != null)
            {
                icon = worldMap.GetEntry(this);
                arrow = worldMap.GetArrow(this);
            }



            if (icon != null) icon.gameObject.SetActive(true);
        }

        /// <summary>
        /// Remove this unit from the list.
        /// </summary>

        void OnDestroy()
        {
            if (Application.isPlaying)
            {
                if (NJGMap.instance != null && NJGMap.instance.fow.enabled) NJGFOW.DeleteRevealer(mRevealer);
            }
            
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                if (mRevealer != null) mRevealer.isActive = false;
                if (arrow != null && map != null)
                {
                    map.DeleteArrow(arrow);
                    arrow = null;
                }
            }

            if (miniMap != null)
            {
                if(icon != null) miniMap.Delete(icon);
                if (arrow != null) miniMap.DeleteArrow(arrow);
            }

            if (worldMap != null)
            {
                if (icon != null) worldMap.Delete(icon);
                if (arrow != null) worldMap.DeleteArrow(arrow);
            }

            if (list.Contains(this)) { list.Remove(this); if (onListChanged != null) onListChanged(); }         
        }

        void Update()
        {
            if (revealFOW)
            {
                if (NJGMap.instance == null || miniMap == null) return;

                if (mRevealer == null) mRevealer = NJGFOW.CreateRevealer();

                if (isActive)
                {
                    mRevealer.pos = miniMap.WorldToMap(transform.position, false);
                    mRevealer.revealDistance = revealDistance > 0 ? revealDistance : NJGMap.instance.fow.revealDistance;
                    mRevealer.isActive = true;
                }
                else
                {
                    mRevealer.isActive = false;
                }
            }

            /*if (transform.hasChanged)
            {
                if (worldMap != null) worldMap.UpdateItem(this);
                if (miniMap != null) miniMap.UpdateItem(this);
                transform.hasChanged = false;
            }*/
        }

        void OnDrawGizmosSelected()
        {
            if (revealFOW)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, revealDistance);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Manually select current icon.
        /// </summary>

        public void Select() { mSelected = true; }

        /// <summary>
        /// Manually force icon selection.
        /// </summary>

        public void Select(bool forceSelect) { mSelected = true; mForceSelect = forceSelect; }

        /// <summary>
        /// Unselects the current icon.
        /// </summary>

        public void UnSelect() { mSelected = false; }

        /// <summary>
        /// Shows the icon.
        /// </summary>

        public void Show() { showIcon = true; }

        #endregion
    }
}
