//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Player;

/// <summary>
/// Map base class.
/// </summary>

namespace NJG
{
    //[ExecuteInEditMode]
	public class Map : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        #region Enums

        public enum ZoomType
        {
            ZoomIn,
            ZoomOut
        }

		public enum Pivot
		{
			BottomLeft,
			Left,
			TopLeft,
			Top,
			TopRight,
			Right,
			BottomRight,
			Bottom,
			Center,
		}

        public enum Type
        {
            MiniMap,
            WorldMap
        }

        #endregion

        static Map mMiniInst;
        static Map mWorldInst;

        static public Map miniMap
        {
            get
            {
                if (mMiniInst == null)
                {
                    Map[] mps = GameObject.FindObjectsOfType<Map>();
                    for (int i = 0, imax = mps.Length; i < imax; i++)
                    {
                        Map mp = mps[i];
                        if (mp.mapType == Type.MiniMap) mMiniInst = mp;
                    }
                }

                return mMiniInst;
            }
        }

        static public Map worldMap
        {
            get
            {
                if (mWorldInst == null)
                {
                    Map[] mps = GameObject.FindObjectsOfType<Map>();
                    for (int i = 0, imax = mps.Length; i < imax; i++)
                    {
                        Map mp = mps[i];
                        if (mp.mapType == Type.WorldMap) mWorldInst = mp;
                    }
                }

                return mWorldInst;
            }
        }

        #region Public Variables

        public Type mapType = Type.MiniMap;        

		/// <summary>
		/// Map margin.
		/// </summary>

		public Vector2 margin = Vector2.zero;

		/// <summary>
		/// Game map's pivot point.
		/// </summary>

		public Pivot pivot = Pivot.Center;

		/// <summary>
		/// Current level of zoom of the Minimap.
		/// </summary>

		[SerializeField]
		public float zoom = 1;

		/// <summary>
		/// How much zoom in/out.
		/// </summary>

		[SerializeField]
		public float zoomAmount = 0.5f;

		/// <summary>
		/// The target can be found using this tag.
		/// </summary>

		public string targetTag = "Player";

		/// <summary>
		/// Minimun level of zoom.
		/// </summary>

		[SerializeField]
		public float minZoom = 1;

		/// <summary>
		/// Maximun level of zoom.
		/// </summary>

		[SerializeField]
		public float maxZoom = 30f;        

		/// <summary>
		/// Zoom speed.
		/// </summary>

		[SerializeField]
        [Range(0,1)] 
		public float zoomSpeed = 0.5f;

		/// <summary>
		/// Key to zoom in the minimap.
		/// </summary>

		[SerializeField]
		public KeyCode zoomInKey = KeyCode.KeypadPlus;

		/// <summary>
		/// Key to zoom out the minimap.
		/// </summary>

		[SerializeField]
		public KeyCode zoomOutKey = KeyCode.KeypadMinus;

		/// <summary>
		/// Limit bounds
		/// </summary>

		[SerializeField]
        public bool limitBounds = true;

		/// <summary>
		/// Rotate map with target.
		/// </summary>

		[SerializeField]
        public bool rotateWithPlayer = false;

		/// <summary>
		/// Enable Mouse Wheel Zoom.
		/// </summary>

		[SerializeField]
		public bool mouseWheelEnabled = true;

		/// <summary>
		/// Enable Map panning.
		/// </summary>

		[SerializeField]
		public bool panning = true;

		/// <summary>
		/// Panning ease type
		/// </summary>

		[SerializeField]
        public LeanTweenType panningEasing = LeanTweenType.easeOutCirc;

		/// <summary>
		/// How fast the panning should move
		/// </summary>

		[SerializeField]
		public float panningSpeed = 1;

		/// <summary>
		/// How fast the panning should go when mouse is moving.
		/// </summary>

		[SerializeField]
		public float panningSensitivity = 5;

		/// <summary>
		/// If true returns the panning position to the targets position.
		/// </summary>

		[SerializeField]
		public bool panningMoveBack = true;

		/// <summary>
		/// Current panning position.
		/// </summary>

		[SerializeField]
		public Vector2 panningPosition = Vector2.zero;

		/// <summary>
		/// Current map angle.
		/// </summary>

		public float mapAngle;

		/// <summary>
		/// Current scroll position.
		/// </summary>

		public Vector2 scrollPosition = Vector2.zero;

		/// <summary>
		/// Enable directonal lines drawing.
		/// </summary>

		public bool drawDirectionalLines = false;

		/// <summary>
		/// Lines shader.
		/// </summary>

		public Shader linesShader;

		/// <summary>
		/// Number of line points.
		/// </summary>

		public int linePoints = 20;

		/// <summary>
		/// Lines color.
		/// </summary>

		public Color lineColor = Color.red;

		/// <summary>
		/// Lines width.
		/// </summary>

		public float lineWidth = 0.1f;

		/// <summary>
		/// List of line points that are going to be drawn.
		/// </summary>

		public List<Transform> controlPoints = new List<Transform>();

        /// <summary>
        /// Key to toggle the minimap lock.
        /// </summary>

        public KeyCode lockKey = KeyCode.L;

        /// <summary>
        /// Optional north icon. Will be automatically placed if its assigned.
        /// </summary>

        public GameObject northIcon;

        /// <summary>
        /// North icon offset.
        /// </summary>

        //public int northIconOffset = 10;

        /// <summary>
        /// Key to toggle the NJGMap.instance.
        /// </summary>

        public KeyCode toggleKey = KeyCode.M;

        #endregion

        #region Getters and Setters

        /// <summary>
        /// The target that Minimap is going to follow.
        /// </summary>

        public Transform target
        {
            get
            {
                return mTarget;
            }
            set
            {
                mTarget = value;
                if (onTargetChanged != null && Application.isPlaying) onTargetChanged(value);
            }
        }

        /// <summary>
        /// Map texture color.
        /// </summary>

        public virtual Color mapColor { get { return mColor; } set { mColor = value; material.color = value; } }

        /// <summary>
        /// Zoom easing method.
        /// </summary>

        [SerializeField]
        public LeanTweenType zoomEasing { set { mZoomEasing = value; } get { return mZoomEasing; } }

        /// <summary>
        /// Content root.
        /// </summary>

        [SerializeField]
        public RectTransform contentRoot
        {
            get
            {
                if (mRoot == null) 
                {
                    if (transform.FindChild("MapContent") != null) mRoot = transform.FindChild("MapContent").GetComponent<RectTransform>(); 
                }
                if (mRoot == null)
                {
                    mRoot = NJGTools.AddChild(gameObject).AddComponent<RectTransform>();
                    mRoot.name = "MapContent";
                    mRoot.anchorMin = Vector2.zero;
                    mRoot.anchorMax = Vector2.one;
                    mRoot.sizeDelta = Vector2.zero;
                }
                return mRoot;
            }
        }

		/// <summary>
		/// Root of map icons.
		/// </summary>

		public RectTransform iconRoot
		{
			get
			{
                if (mIconRoot == null)
                {
                    if (transform.FindChild("MapContent/Icons") != null) mIconRoot = transform.FindChild("MapContent/Icons").GetComponent<RectTransform>();
                }
                if (mIconRoot == null)
				{
                    mIconRoot = NJGTools.AddChild(rendererTransform.gameObject).AddComponent<RectTransform>();

                    mIconRoot.gameObject.AddComponent<Image>();
                    Mask mask = mIconRoot.gameObject.AddComponent<Mask>();
                    mask.showMaskGraphic = false;

                    if (worldMap == this)
                    {
                        Canvas c = mIconRoot.gameObject.AddComponent<Canvas>();
                        c.overridePixelPerfect = true;
                        c.pixelPerfect = false;
                        c.overrideSorting = true;
                        c.sortingOrder = 5;

                        mIconRoot.gameObject.AddComponent<GraphicRaycaster>();
                    }

					if (rendererTransform != null)
					{
                        //mIconRoot.SetParent(cachedTransform, false);
						//mIconRoot.localPosition = new Vector3(rendererTransform.localPosition.x, rendererTransform.localPosition.y, 1);
						mIconRoot.localPosition = Vector3.zero;
                        mIconRoot.localEulerAngles = rendererTransform.localEulerAngles;
                        mIconRoot.anchorMin = Vector2.zero;
                        mIconRoot.anchorMax = Vector2.one;
                        mIconRoot.sizeDelta = Vector2.zero;
                        //mIconRoot.localScale = Vector3.one;
                    }

					mIconRoot.name = "Icons";

                    // Uggly workaround for some weird issue with arrows
                    //gameObject.SetActive(false);
                    //gameObject.SetActive(true);
				}
				return mIconRoot;
			}
		}

        /// <summary>
        /// Mask texture for the NJGMap.instance.
        /// </summary>

        [SerializeField]
        public Sprite maskTexture
        {
            get
            {
                return mMask;
            }

            set
            {
                mMask = value;
                material.SetTexture("_Mask", mMask.texture);
                mapRenderer.material.SetTexture("_Mask", mMask.texture);
                if(iconRoot != null) iconRoot.GetComponent<Image>().sprite = mMask;
                mapRenderer.enabled = false;
                mapRenderer.enabled = true;
            }
        }

		/// <summary>
		/// Check if mouse is over.
		/// </summary>

		public virtual bool isMouseOver { get; set; }

        public virtual RectTransform rendererTransform { get { if (mMapTrans == null) mMapTrans = mapRenderer.GetComponent<RectTransform>(); return mMapTrans; } }

#if NJG_NGUI

        /// <summary>
        /// Plane used to display the NJGMap.instance.
        /// </summary>

        public virtual UITexture mapRenderer
        {
            get
            {
                if (mRenderer == null) mRenderer = gameObject.GetComponentInChildren<UITexture>();
                if (mRenderer == null)
                {
                    UIPanel panel = NGUITools.AddChild<UIPanel>(contentRoot.gameObject);
                    panel.gameObject.name = "_MapPanel";

                    mRenderer = NGUITools.AddChild<UITexture>(panel.cachedGameObject);
                    mRenderer.gameObject.name = "_MapTexture";
                    mRenderer.width = (int)mapScale.x;
                    mRenderer.height = (int)mapScale.y;

                    UpdateAlignment();
                }
                return mRenderer;
            }
            set { mRenderer = value; }
        }
#else

        /// <summary>
        /// Plane used to display the NJGMap.instance.
        /// </summary>
		public virtual RawImage mapRenderer 
		{ 
			get 
			{
                if (mRenderer == null) mRenderer = gameObject.GetComponentInChildren<RawImage>();
                if (mRenderer == null)
                {
                    GameObject go = NJGTools.AddChild(contentRoot.gameObject);
                    go.gameObject.name = "MapTexture";
                    mRenderer = go.AddComponent<RawImage>();
                    mMapTrans = mRenderer.GetComponent<RectTransform>();
                    mMapTrans.anchorMin = Vector2.zero;
                    mMapTrans.anchorMax = Vector2.one;
                    mMapTrans.sizeDelta = Vector2.zero;
                    //rendererTransform.sizeDelta = mapScale;
                }
				return mRenderer; 
			} 
			set { mRenderer = value; } 
        }

#endif

        public virtual Vector2 mapScale 
		{ 
			get 
			{
                return contentRoot.rect.size;
			} 
			/*set
			{
                Vector2 size = value;
                rendererTransform.sizeDelta = size;
			} */
		}

		public virtual Vector2 mapHalfScale 
		{ 
			get
			{
                if (mMapHalfScale == Vector2.zero)	
				{
                    //rendererTransform.hasChanged || 
					//rendererTransform.hasChanged = false;					
					mMapHalfScale = mapScale * 0.5f;
				}
				return mMapHalfScale; 
			} 
		}

		[SerializeField]
		public virtual Material material
		{
            get { if (mMaterial == null) mMaterial = mapRenderer.material;  return mMaterial; }
			set { mapRenderer.material = mMaterial = value; }
		}

		public virtual Vector3 arrowScale
		{
			get
			{
				if (mArrowSize != NJGMap.instance.arrowSize)
				{
					mArrowSize = NJGMap.instance.arrowSize;
					mArrScale.x = mArrScale.y = NJGMap.instance.arrowSize;
				}
				return mArrScale;
			}
		}

        public RectTransform arrowRoot
        {
            get
            {
                if (mArrowRoot == null && Application.isPlaying)
                {
                    mArrowRoot = NJGTools.AddChild(iconRoot.gameObject).AddComponent<RectTransform>();
                    mArrowRoot.anchorMin = Vector2.zero;
                    mArrowRoot.anchorMax = Vector2.one;
                    mArrowRoot.sizeDelta = Vector2.zero;
                    mArrowRoot.name = "Arrows";
                }
                return mArrowRoot;
            }
        }

		/// <summary>
		/// True if the map is visible.
		/// </summary>

        public bool isVisible { get { return contentRoot.gameObject.activeInHierarchy; } set { enabled = value; contentRoot.gameObject.SetActive(value); } }

		/// <summary>
		/// Check if mouse is out of the screen.
		/// </summary>

		public bool isMouseOut { get { Vector3 p = Input.mousePosition; return p.x > Screen.width || p.y > Screen.height || p.x < 0 || p.y < 0; } }

#endregion

#region Private Variables

        [SerializeField] Transform mTarget;
        [SerializeField] RectTransform mMapTrans;
		[SerializeField] Vector3 mZoom = Vector3.zero;
		[SerializeField] List<MapIcon> mList = new List<MapIcon>();

#if NJG_NGUI
        [SerializeField] UIWidget mRoot;
        [SerializeField] UITexture mRenderer;
#else
        [SerializeField] RectTransform mRoot;
        [SerializeField] RawImage mRenderer;
#endif
        [SerializeField] RectTransform mIconRoot;
		[SerializeField] Vector2 mMapScale;
		[SerializeField] Vector2 mMapHalfScale;
		[SerializeField] Vector2 mIconScale;
		//[SerializeField] Matrix4x4 mMatrix;
		[SerializeField] Matrix4x4 rMatrix;
        [SerializeField] Sprite mMask;
        [SerializeField] Material mMaterial;
		Vector3 mLastScale = Vector3.zero;
		Vector3 mMapPos = Vector3.zero;
		
		int mLastHeight = 0;
		float mNextUpdate = 0f;
        
		List<MapIcon> mUnused = new List<MapIcon>();

		int mCount = 0;
        
		[SerializeField] Color mColor = Color.white;
		Quaternion mapRotation;
		Vector3 rotationPivot = new Vector3(0.5f, 0.5f);

		[SerializeField]
        LeanTweenType mZoomEasing = LeanTweenType.easeOutExpo;
		
		Vector2 mPanningMousePosLast = Vector2.zero;
		bool mTargetWarning;
		protected Vector3 mIconEulers = Vector3.zero;
		int mArrowSize = 0;
		[SerializeField]
		Vector3 mArrScale = Vector3.one;
		Camera mUICam;
		bool mIsPanning;
		Transform mLinesRoot;
		LineRenderer mLineRenderer;		
        GameObject northRoot;
        RectTransform mArrowRoot;

        //List<MapItem> mPingList = new List<MapItem>();
        //List<MapItem> mPingUnused = new List<MapItem>();
        List<MapArrow> mListArrow = new List<MapArrow>();
        List<MapArrow> mUnusedArrow = new List<MapArrow>();
        Animator mAnim;
        bool mAnimCheck;
        int animIn = Animator.StringToHash("TransitionIn");
        int animOut = Animator.StringToHash("TransitionOut");

        //MapItem pingMarker;
        protected int mArrowCount = 0;
        Pivot mPivot;
        Vector2 mMargin;
        Transform anchorTarget;
        //Material iconMat;
        //Vector3 mClickOffset;

#endregion

        static public System.Action<Transform> onTargetChanged;

#region Monobehaviour Methods

        protected virtual void Start()
        {
            rendererTransform.hasChanged = true;
            //iconMat = new Material(Shader.Find("UI/Default"));
            NJGMap.onTextureChanged += OnTextureChanged;

            Canvas canvas = GetComponentInParent<Canvas>();
            mUICam = canvas.worldCamera;

            if (material != null)
            {
                if (NJGMap.instance.fow.enabled)
                    material.shader = Shader.Find("Ninjutsu Games/Map FOW");
                else 
                    material.shader = Shader.Find("Ninjutsu Games/Map");
            }

            if (drawDirectionalLines && Application.isPlaying)
            {
                if (linesShader == null) linesShader = Shader.Find("Particles/Additive");

                GameObject go = NJGTools.AddChild(gameObject);
                mLinesRoot = go.transform;
                mLinesRoot.parent = iconRoot;
                mLinesRoot.localPosition = Vector3.zero;
                mLinesRoot.localEulerAngles = Vector3.zero;

                mLineRenderer = go.GetComponent<LineRenderer>();

                if (mLineRenderer == null)
                    go.AddComponent<LineRenderer>();

                mLineRenderer = go.GetComponent<LineRenderer>();
                mLineRenderer.useWorldSpace = true;
                mLineRenderer.material = new Material(linesShader);

                mLinesRoot.name = "_Lines";
            }

            if (material == null)
            {
                if (Application.isPlaying) Debug.LogWarning("The MapTexture does not have a material assigned", this);
            }
            else
            {
                /*if (NJGMap.instance.generateMapTexture)
                {
                    material.mainTexture = NJGMap.instance.mapTexture;
                }
                else
                {
                    material.mainTexture = NJGMap.instance.userMapTexture;
                }*/

                material.color = mapColor;
            }

            MapItem.onListChanged += OnItemsChanged;
            if (!contentRoot.gameObject.activeInHierarchy) enabled = false;

            if (Application.isPlaying)
            {
                // Create north root and place icon north properly
                northRoot = NJGTools.AddChild(iconRoot.gameObject);
                northRoot.name = "North";
                northRoot.transform.localPosition = Vector3.zero;

                if (northIcon != null)
                {
                    northIcon.transform.SetParent(northRoot.transform, false);
                    northIcon.transform.localRotation = Quaternion.identity;
                }
            }
            if (worldMap == this) contentRoot.gameObject.SetActive(false);

        }        

        void OnTextureChanged(Texture texture)
        {
            SetTexture(texture);

            enabled = false;
            enabled = true;
        }

        public void SetTexture(Texture texture)
        {
#if NJG_NGUI
            mapRenderer.mainTexture = texture;
#else
            mapRenderer.texture = texture;

#if UNITY_EDITOR
            mapRenderer.gameObject.SetActive(false);
            mapRenderer.gameObject.SetActive(true);
#endif
#endif
            if (Application.isPlaying) material.SetTexture("_Main", texture);
        }

        void OnItemsChanged()
        {
            //Debug.Log("OnItemsChanged " + MapItem.list.Count);
            //shouldUpdateItems = true;
        }

        /// <summary>
        /// Update what's necessary.
        /// </summary>		

        protected virtual void Update()
        {
            //Debug.Log("EventSystem.current.alreadySelecting " + EventSystem.current.alreadySelecting);

            if (Application.isPlaying)
            {
                if (!EventSystem.current.alreadySelecting)
                {
                    if (Input.GetKeyDown(toggleKey)) Toggle();
                }
            }
/*#if UNITY_EDITOR
            else
            {
                if(NJG.MapRenderer.instance.camera.targetTexture != null) mapRenderer.texture = NJG.MapRenderer.instance.camera.targetTexture;
            }
#endif*/

            if (!isVisible) return;

            if (mPivot != pivot || mMargin != margin || mMapScale != mapScale)
            {
                mMapScale = mapScale;
                mPivot = pivot;
                mMargin = margin;
                UpdateAlignment();
            }

            if (target == null)
            {
                if (!string.IsNullOrEmpty(targetTag))
                {
                    if (GameObject.FindGameObjectWithTag(targetTag) != null)
                        target = GameObject.FindGameObjectWithTag(targetTag).transform;
                }

                // If there is no target defined lets use the mainCamera
                /*if (target == null && Camera.main != null)
                {
                    if (target != Camera.main.transform) target = Camera.main.transform;
                }*/
                //else if (target != null && mTarget != target) mTarget = target;
            }

            if (isMouseOut && isMouseOver) isMouseOver = false;            

            if (target != null && controlPoints.Count == 0)
            {
                if (!controlPoints.Contains(target)) controlPoints.Add(target);
            }

            if (target == null && !mTargetWarning)
            {
                mTargetWarning = true;
            }

            if (Application.isPlaying && !EventSystem.current.alreadySelecting)
            {
                if (Input.GetKeyDown(lockKey))
                    rotateWithPlayer = !rotateWithPlayer;

                if (Input.GetKeyDown(zoomInKey))
                    ZoomIn(zoomAmount);

                if (Input.GetKeyDown(zoomOutKey))
                    ZoomOut(zoomAmount);
            }            

            if (mouseWheelEnabled)
            {
                if (isMouseOver)
                {
                    float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
                    if (scrollWheel != 0)
                    {
                        if (scrollWheel > 0.1)
                            ZoomIn(zoomAmount);
                        else if (scrollWheel < 0.1)
                            ZoomOut(zoomAmount);
                    }
                }
            }

            /*if (panning)
            {
                Profiler.BeginSample("Panning");
                UpdatePanning();
                Profiler.EndSample();
            }*/

            int height = Screen.height;
            bool heightChanged = (mLastHeight != height);

            if (mLastScale != rendererTransform.localScale)
            {
				rendererTransform.hasChanged = true;
                mLastScale = rendererTransform.localScale;
            }

            if (mNextUpdate < Time.time || (!Application.isPlaying && mNextUpdate < Time.time)) //heightChanged ||  || !Application.isPlaying
            {
                mLastHeight = height;
                mNextUpdate = Time.time + NJGMap.instance.updateFrequency;

                //bool sizeChanged = false;	


                UnityEngine.Profiling.Profiler.BeginSample("UpdateIcons");
                UpdateIcons();
                UnityEngine.Profiling.Profiler.EndSample();

                UnityEngine.Profiling.Profiler.BeginSample("CleanIcons");
                CleanIcons();
                UnityEngine.Profiling.Profiler.EndSample();

                //Profiler.BeginSample("UpdateFrustm");
                //UpdateFrustum();
                //Profiler.EndSample();

                if (drawDirectionalLines)
                {
                    UnityEngine.Profiling.Profiler.BeginSample("DrawLines");
                    DrawLines();
                    UnityEngine.Profiling.Profiler.EndSample();
                }

                UnityEngine.Profiling.Profiler.BeginSample("UpdateScrollPosition");
                UpdateScrollPosition();
                UnityEngine.Profiling.Profiler.EndSample();

                UnityEngine.Profiling.Profiler.BeginSample("UpdateMatrix");
                UpdateMatrix();
                UnityEngine.Profiling.Profiler.EndSample();

                /*if (mLastPos != NJGMap.instance.mapRenderer.transform.position || mLastSize != NJGMap.instance.mapRenderer.cachedCamera.orthographicSize)
                {
                    posChanged = true;
                    mLastPos = NJGMap.instance.mapRenderer.transform.position;
                    mLastSize = NJGMap.instance.mapRenderer.cachedCamera.orthographicSize;
                }*/
                if (heightChanged)
                {
                    if (NJGMap.instance.renderMode != NJGMap.RenderMode.Once) // && (this is UIMiniMapBase)
                    {
                        UnityEngine.Profiling.Profiler.BeginSample("GenerateMap");
                        NJGMap.GenerateMap();
                        UnityEngine.Profiling.Profiler.EndSample();
                    }
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (NJGMap.instance.fow.enabled == false)
            {
                if (material != null) material.SetColor("_MaskColor", NJGMap.instance.cameraBackgroundColor);
            }
        }

        protected virtual void OnDestroy()
        {
            NJGMap.onTextureChanged -= OnTextureChanged;
        }	

#endregion

#region UIEvents
        
        /// <summary>
        /// Event triggered when the map is double clicked. This returns the map position and world position.
        /// </summary>

        static public System.Action<Map, Vector2, Vector3> onMapDoubleClick;

        /// <summary>
        /// Event triggered when the map is clicked. This returns the map position and world position.
        /// </summary>

        static public System.Action<Map, Vector2, Vector3> onMapClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 worldPos = MapToWorld();

            if (eventData.clickCount > 1)
            {
                if (onMapDoubleClick != null) onMapDoubleClick(this, WorldToMap(worldPos), worldPos);
            }
            else
            {
                if (onMapClick != null) onMapClick(this, WorldToMap(worldPos), worldPos);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) { isMouseOver = true; }
        public void OnPointerExit(PointerEventData eventData) { isMouseOver = false; }

#endregion

		protected virtual void CleanIcons()
		{
			if (Application.isPlaying)
			{
				// Remove invalid entries
				for (int i = mList.Count; i > 0;)
				{
					MapIcon icon = mList[--i];

					if (!icon.isValid) // && icon.item.map == this
                    {
						Delete(icon);
                        //DeleteArrow(icon.item.arrow);
                    }
				}
			}
		}

		/// <summary>
		/// Get the map icon entry associated with the specified unit.
		/// </summary>

		public MapIcon GetEntry(MapItem item) 
        {
            // Try to find an existing entry
            for (int i = 0, imax = mList.Count; i < imax; ++i)
            {
                MapIcon ic = (MapIcon)mList[i];
                if (ic.item == item)
                {
                    Sprite spr = NJGMap.instance.GetSprite(item.type);
                    if (ic.icon.sprite != spr) ic.icon.sprite = spr;
#if NJG_NGUI
                    if (ic.sprite.depth != item.depth) ic.sprite.depth = item.depth;
#endif
                    /*if (!ic.sprite.color.Equals(item.color)) ic.sprite.color = item.color;
                    if (ic.sprite.cachedTransform.localScale != item.iconScale)
                    {
                        ic.collider.size = item.iconScale;
                        ic.sprite.cachedTransform.localScale = item.iconScale;
                    }*/

                    /*Sprite s2 = NJGMap.instance.GetSpriteBorder(item.type);
                    if (s2 != null)
                    {
                        if (ic.border != null)
                        {
                            ic.border.sprite = s2;
                            ic.border.color = item.color;
                            if (ic.border.preferredWidth != item.borderScale.x || ic.border.preferredHeight != item.borderScale.y)
                            {
                                RectTransform ert = ic.border.GetComponent<RectTransform>();
                                ert.sizeDelta = item.borderScale;
                            }
                        } else
                        {
                            GameObject go2 = NJGTools.AddChild(item.gameObject);
                            go2.name = "Selection";

                            Image border = go2.AddComponent<Image>();
                            border.name = "Selection";
                            border.sprite = s2;
                            border.color = item.color;
                            border.material = iconMat;

                            RectTransform brt = border.GetComponent<RectTransform>();
                            brt.sizeDelta = item.borderScale;

                            ic.border = border;
                        }
                    }*/
                    return ic;
                }
            }

            Sprite s = null;

            // See if an unused entry can be reused
            if (mUnused.Count > 0)
            {
                MapIcon ent = (MapIcon)mUnused[mUnused.Count - 1];
                ent.item = item;
                ent.item.map = this;
                ent.icon.sprite = NJGMap.instance.GetSprite(item.type);
                //ent.sprite.depth = 1 + item.depth;
                ent.icon.color = item.color;
                if (ent.icon.preferredWidth != item.iconScale.x || ent.icon.preferredHeight != item.iconScale.y)
                {
                    RectTransform ert = ent.icon.GetComponent<RectTransform>();
                    ert.sizeDelta = item.iconScale;
                }

                s = NJGMap.instance.GetSpriteBorder(item.type);
                if (s != null)
                {
                    if (ent.border != null)
                    {
                        ent.border.sprite = s;
                        ent.border.color = item.color;
                        if (ent.border.preferredWidth != item.borderScale.x || ent.border.preferredHeight != item.borderScale.y)
                        {
                            RectTransform ert = ent.border.GetComponent<RectTransform>();
                            ert.sizeDelta = item.borderScale;
                        }
                    }
                }
                mUnused.RemoveAt(mUnused.Count - 1);
                //ent.Enable();
                ent.gameObject.SetActive(true);
                mList.Add(ent);
                return ent;
            }

            // Create this new icon
            GameObject go = NJGTools.AddChild(iconRoot.gameObject);
            go.name = "Icon" + mCount;

            Image sprite = go.AddComponent<Image>();
            sprite.name = "Icon";
            sprite.sprite = NJGMap.instance.GetSprite(item.type);
            sprite.color = item.color;
            //sprite.material = iconMat;

            RectTransform srt = sprite.GetComponent<RectTransform>();
            srt.sizeDelta = item.iconScale;

            MapIcon mi = go.AddComponent<MapIcon>();
            mi.item = item;
            mi.item.map = this;
            mi.icon = sprite;
            if (item.interaction)
            {
                s = NJGMap.instance.GetSpriteBorder(item.type);
                if (s != null)
                {
                    GameObject go2 = NJGTools.AddChild(go);
                    go2.name = "Selection";

                    Image border = go2.AddComponent<Image>();
                    border.name = "Selection";
                    border.sprite = s;
                    border.color = item.color;
                    //border.material = iconMat;

                    RectTransform brt = border.GetComponent<RectTransform>();
                    brt.sizeDelta = item.borderScale;

                    mi.border = border;

                }
            }

            if (mi == null)
            {
                Debug.LogError("Expected to find a Game Map Icon on the prefab to work with", this);
                Destroy(go);
            }
            else
            {
                mCount++;
                mi.item = item;
                mList.Add(mi);
            }
            return mi;
        }

		/// <summary>
		/// Delete the specified entry, adding it to the unused list.
		/// </summary>

		public virtual void Delete(MapIcon ent)
		{
			mList.Remove(ent);
			mUnused.Add(ent);
            ent.gameObject.SetActive(false);            
        }

        public void UpdateItem(MapItem item)
        {

            //if (item.type < 1) continue;
            /*if (drawDirectionalLines)
            {
                if (item.transform != target)
                {
                    if (item.drawDirection)
                    {
                        if (!controlPoints.Contains(item.transform)) controlPoints.Add(item.transform);
                    }
                    else
                    {
                        if (controlPoints.Contains(item.transform)) controlPoints.Remove(item.transform);
                    }
                }
            }*/

            Vector2 pos = WorldToMap(item.transform.position);



            if (NJGMap.instance.fow.enabled && item.transform != target) // && !item.alwaysVisible
            {
                Vector2 pos2 = WorldToMap(item.transform.position, false);
                bool shouldReveal = NJGFOW.instance.IsExplored(pos2, 150) || NJGFOW.instance.IsVisible(pos2, 150);

                if (item.isRevealed != shouldReveal)
                {
                    // If FOW is enabled, unit is covered by FOW and if doesn't reveal FOW, don't consider it.						
                    item.isRevealed = item.revealFOW ? true : shouldReveal;
                }
            }
            else
            {
                if (!item.isRevealed) item.isRevealed = true;
            }

            if (item.isRevealed) UpdateIcon(item, pos.x, pos.y);
            else if (item.haveArrow && item.arrow != null && item.arrow.gameObject.activeInHierarchy) item.arrow.gameObject.SetActive(false);
        }

        /// <summary>
        /// Update the icon icon for the specified unit, assuming it's visible.
        /// </summary>

        public virtual void UpdateIcon(MapItem item, float x, float y) 
        {
            // If the unit is not visible, don't consider it
            bool isVisible = false;

            /*isVisible = (((x - mapBorderRadius) >= -mapHalfScale.x) &&
                     ((x + mapBorderRadius) <= mapHalfScale.x)) &&
                     (((y - mapBorderRadius) >= -mapHalfScale.y) &&
                     ((y + mapBorderRadius) <= mapHalfScale.y));*/

            isVisible = (x >= -mapHalfScale.x && x <= mapHalfScale.x) && (y >= -mapHalfScale.y && y <= mapHalfScale.y);

            //bool isOutOfBounds = false;

            //isOutOfBounds = (x > NJGMap.instance.bounds.size.x || x < -NJGMap.instance.bounds.size.x) || (y > NJGMap.instance.bounds.size.y || y < -NJGMap.instance.bounds.size.y);

            if (NJGMap.instance.fow.enabled && !item.isRevealed) isVisible = false;

            //if (item.alwaysVisible) isVisible = true;
             
            if (!isPanning && item.haveArrow) // || item.arrowAlwaysVisible  && !isOutOfBounds
            {
                
                Transform tempTarget = target == null ? PlayerServicesAndModulesContainer.Instance.PlayerCamera.transform : target;


                if (!isVisible && item.isRevealed)
                {
                    if (item.arrow != null)
                    {
                        if (!item.arrow.gameObject.activeInHierarchy) item.arrow.gameObject.SetActive(true);
                        item.arrow.UpdateRotation(tempTarget == null ? Vector3.zero : tempTarget.position);
                    }
                }
                else if (isVisible && item.haveArrow && item.arrow != null)
                {
                    if (item.arrow.gameObject.activeInHierarchy)
                    {
                        item.arrow.gameObject.SetActive(false);
                    }
                }
            }

            

            if (!isVisible)
            {
                if (item.icon.gameObject.activeInHierarchy)
                {
                    item.icon.gameObject.SetActive(false);
                }
                return; // && !item.alwaysVisible
            }


            //MapIcon icon = GetEntry(item);

            //Debug.Log("UpdateItem " + item + " / " + icon + " / " + x + "/" + y + " / " + icon.isValid);

            if (item.icon != null) // && !icon.isValid
            {
                if (!item.icon.gameObject.activeInHierarchy)
                {
                    item.icon.gameObject.SetActive(true);
                }

                //item.icon.isMapIcon = (worldMap == this);
                item.icon.isValid = true;
                item.icon.isVisible = true;
                Transform t = item.icon.transform;
                Vector3 newPos = new Vector3(x, y, 0f);
                if (item.updatePosition && t.localPosition != newPos)
                {
                    //t.localPosition = newPos;
                    //t.hasChanged = false;
                    if(item.smoothPosition) t.localPosition = Vector3.Lerp(t.localPosition, newPos, Time.deltaTime * 20f);
                    else t.localPosition = newPos;
                }

                if (item.rotate)
                {
                    float angle = ((Vector3.Dot(item.transform.forward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f) ? 1f : -1f) * Vector3.Angle(item.transform.forward, Vector3.forward);
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y, angle);
                }
                else if (!item.rotate && rotateWithPlayer)
                {
                    Vector3 eu = new Vector3(0, 0, -iconRoot.localEulerAngles.z);
                    if (t.localEulerAngles != eu) t.localEulerAngles = eu;
                }
                else
                {
                    if (t.localEulerAngles != Vector3.zero) t.localEulerAngles = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// Updates MiniMap alignment.
        /// </summary>

        public virtual void UpdateAlignment()
        {
#if NJG_NGUI
            if (anchorTarget == null)
            {
                anchorTarget = NGUITools.FindInParents<UIRoot>(gameObject).transform;
            }

            // Anchor the root if its not anchored.
            if (contentRoot.isAnchored == false)
            {                
                contentRoot.SetAnchor(anchorTarget);
            }

            // Match root size with settings.
            //contentRoot.SetDimensions((int)mapScale.x, (int)mapScale.y);

            // Change root anchors according to the pivot from settings
            switch (pivot)
            {
                case Pivot.Center:
                    contentRoot.leftAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.rightAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 0.5f, 0);
                    break;
                case Pivot.TopRight:
                    contentRoot.leftAnchor.Set(anchorTarget, 1, -(mapScale.x + margin.x));
                    contentRoot.rightAnchor.Set(anchorTarget, 1, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 1, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 1, -(mapScale.y + margin.y));
                    break;
                case Pivot.Right:
                    contentRoot.leftAnchor.Set(anchorTarget, 1, -(mapScale.x + margin.x));
                    contentRoot.rightAnchor.Set(anchorTarget, 1, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 0.5f, 0);
                    break;
                case Pivot.BottomRight:
                    contentRoot.leftAnchor.Set(anchorTarget, 1, -(mapScale.x + margin.x));
                    contentRoot.rightAnchor.Set(anchorTarget, 1, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 0, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 0, (mapScale.y + margin.y));
                    break;
                case Pivot.Bottom:
                    contentRoot.leftAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.rightAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 0, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 0, (mapScale.y + margin.y));
                    break;
                case Pivot.Top:
                    contentRoot.leftAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.rightAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 1, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 1, -(mapScale.y + margin.y));
                    break;
                case Pivot.TopLeft:
                    contentRoot.leftAnchor.Set(anchorTarget, 0, (mapScale.x + margin.x));
                    contentRoot.rightAnchor.Set(anchorTarget, 0, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 1, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 1, -(mapScale.y + margin.y));
                    break;
                case Pivot.Left:
                    contentRoot.leftAnchor.Set(anchorTarget, 0, (mapScale.x + margin.x));
                    contentRoot.rightAnchor.Set(anchorTarget, 0, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 0.5f, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 0.5f, 0);
                    break;
                case Pivot.BottomLeft:
                    contentRoot.leftAnchor.Set(anchorTarget, 0, (mapScale.x + margin.x));
                    contentRoot.rightAnchor.Set(anchorTarget, 0, 0);
                    contentRoot.topAnchor.Set(anchorTarget, 0, 0);
                    contentRoot.bottomAnchor.Set(anchorTarget, 0, (mapScale.y + margin.y));
                    break;
            }
#else
            // TODO
#endif
        }

		/// <summary>
		/// Update Minimap scroll position.
		/// </summary>

		Vector3 mExt;
		public float mMod = 1f;

		void UpdateScrollPosition()
		{
			Bounds bounds = NJGMap.instance.bounds;
			Vector3 extents = bounds.extents;

			/*if (NJGMap.instance.boundsOffset > 0)
			{
				extents.x += NJGMap.instance.boundsOffset;
				extents.y += NJGMap.instance.boundsOffset;
				extents.z += NJGMap.instance.boundsOffset;
			}*/

			float nZoom = 1f / zoom;

            Transform tr = target == null ? PlayerServicesAndModulesContainer.Instance.PlayerCamera.transform : target;

            //if (target == null) return;

			scrollPosition = Vector3.zero;

			Vector3 vector = tr.position - bounds.center;	 		

			mExt.x = (0.5f / extents.x);
			mExt.y = (0.5f / extents.y);
			mExt.z = (0.5f / extents.z);

			if (NJGMap.instance.mapResolution == NJGMap.Resolution.Double)
			{
				mExt.x *= mMod;
				mExt.y *= mMod;
				mExt.z *= mMod;
			}

			scrollPosition.x = vector.x * mExt.x;
			if (NJGMap.instance.orientation == NJGMap.Orientation.XZDefault) scrollPosition.y = vector.z * mExt.z;
			else scrollPosition.y = vector.y * mExt.y;

			if (panning) scrollPosition = scrollPosition + panningPosition;

			// Limit minimap position
			if (limitBounds)
			{
				scrollPosition.x = Mathf.Max(-((1f - nZoom) * 0.5f), scrollPosition.x);
				scrollPosition.x = Mathf.Min((1f - nZoom) * 0.5f, scrollPosition.x);
				scrollPosition.y = Mathf.Max(-((1f - nZoom) * 0.5f), scrollPosition.y);
				scrollPosition.y = Mathf.Min((1f - nZoom) * 0.5f, scrollPosition.y);
			}

			mMapPos.x = ((1f - nZoom) * 0.5f) + scrollPosition.x;
			mMapPos.y = ((1f - nZoom) * 0.5f) + scrollPosition.y;
			mMapPos.z = 0;			

			// Relative zoom.
			mZoom.x = mZoom.y = mZoom.z = nZoom;
		}

		/// <summary>
		/// Updates texture matrix
		/// </summary>

		protected virtual void UpdateMatrix()
		{
			// Move and scale matrix
			Matrix4x4 m = Matrix4x4.TRS(mMapPos, Quaternion.identity, mZoom);

			if (rotateWithPlayer)
			{
                Transform tempTarget = target == null ? Camera.main.transform : target;

                if (tempTarget == null) return;

                // Get target angle.
                Vector3 mForward = tempTarget.forward;
				mForward.Normalize();
				mapAngle = ((Vector3.Dot(mForward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f) ? 1f : -1f) * Vector3.Angle(mForward, Vector3.forward);

				mapRotation = Quaternion.Euler(0, 0, mapAngle);

				// Rotation matrix
				Matrix4x4 mPivotInventory = Matrix4x4.TRS(-rotationPivot, Quaternion.identity, Vector3.one);
				Matrix4x4 mRot = Matrix4x4.TRS(Vector3.zero, mapRotation, Vector3.one);
				Matrix4x4 mPivot = Matrix4x4.TRS(rotationPivot, Quaternion.identity, Vector3.one);

				rMatrix = m * mPivot * mRot * mPivotInventory;

                //Debug.Log("rMatrix " + rMatrix.m00 + " / " + rMatrix.GetRow(0) + " -- " + mMatrix.m00 + " / " + mMatrix.GetRow(0));

				//if (!mMatrix.Equals(rMatrix))
				//{
					//mMatrix = rMatrix;
                    mapRenderer.material.SetMatrix("_Matrix", rMatrix);
					//material.SetMatrix("_Matrix", rMatrix);
				//}

				if (iconRoot != null)
				{
					mIconEulers.z = -mapAngle;
					if (iconRoot.localEulerAngles != mIconEulers) iconRoot.localEulerAngles = mIconEulers;
				}
			}
			else
			{
                //Debug.Log("matrix " + m.m00 + " / " + m.GetRow(0) + " -- " + mMatrix.m00 + " / " + mMatrix.GetRow(0));
                //if (!mMatrix.Equals(m))
                //if (m.GetRow(0) != mMatrix.GetRow(0))
				//{
					//mMatrix = m;
                    mapRenderer.material.SetMatrix("_Matrix", m);
					//material.SetMatrix("_Matrix", m);
                    if (iconRoot != null && iconRoot.localEulerAngles != Vector3.zero) iconRoot.localEulerAngles = Vector3.zero;
				//}
			}
		}		

		/// <summary>
		/// Update the coordinates and colors of map indicators.
		/// </summary>

		protected void UpdateIcons()
		{
			// Mark all entries as invalid
			for (int i = 0, imax = mList.Count; i < imax; i++)
			{
				MapIcon ic = mList[i];
				ic.isValid = false;
				/*if (drawDirectionalLines)
				{
					if (ic.item.transform != target)
					{
						if (ic.item.drawDirection)
						{
							if (!controlPoints.Contains(ic.transform)) controlPoints.Add(ic.transform);
						}
						else
						{
							if (controlPoints.Contains(ic.transform)) controlPoints.Remove(ic.transform);
						}
					}
					else
					{
						if (controlPoints[0] != ic.transform) controlPoints[0] = ic.transform;
					}
				}

                MapItem item = mList[i].item;
                if (item.type < 1) continue;
                UpdateItem(item);*/
			}

            //if (shouldUpdateItems)
            //{
                // Update all entries, marking them as valid in the process
                for (int i = 0, imax = MapItem.list.Count; i < imax; ++i)
                {
                    MapItem item = MapItem.list[i];
                    if (item.type < 1) continue;
                    UpdateItem(item);
                }

                //shouldUpdateItems = false;
            //}
		}

        //bool shouldUpdateItems = true;

        

        void UpdateZoom(float zoom)
        {
            this.zoom = zoom;
        }

        /// <summary>
        /// Get the map icon entry associated with the specified unit.
        /// </summary>

        public MapArrow GetArrow(MapItem item) 
        {
            //MapItem item = (MapItem)o;
            // Try to find an existing entry
            for (int i = 0, imax = mListArrow.Count; i < imax; ++i)
            {
                if (mListArrow[i].item == item)
                {
                    MapArrow ic = (MapArrow)mListArrow[i];
                    /*string spr = NJGMap.instance.GetArrowSprite(item.type).name;
                    if (ic.sprite.spriteName != spr) ic.sprite.spriteName = spr;*/
                    /*if(ic.sprite.depth != item.depth) ic.sprite.depth = item.arrowDepth;
                    if(!ic.sprite.color.Equals(item.color)) ic.sprite.color = item.color;
                    if(ic.sprite.transform.localScale != arrowScale) ic.sprite.transform.localScale = arrowScale;
                    Vector3 offset = new Vector3(0, mapScale.y / 2 - item.arrowOffset, 0);
                    if(ic.sprite.transform.localPosition != offset) ic.sprite.transform.localPosition = offset;*/
                    return ic;
                }
            }

            // See if an unused entry can be reused
            if (mUnusedArrow.Count > 0)
            {
                MapArrow ent = (MapArrow)mUnusedArrow[mUnusedArrow.Count - 1];
                ent.item = item;
                ent.child = ent.sprite.transform;
                ent.sprite.sprite = NJGMap.instance.GetArrowSprite(item.type);
                ent.sprite.color = item.color;
                ent.enabled = true;

                RectTransform ert = ent.sprite.GetComponent<RectTransform>();
                ert.sizeDelta = arrowScale;

                ent.map = this;
                ent.sprite.transform.localPosition = new Vector3(0, mapScale.y / 2 - item.arrowOffset, 0);
                mUnusedArrow.RemoveAt(mUnusedArrow.Count - 1);
                ent.gameObject.SetActive(true);
                mListArrow.Add(ent);
                return ent;
            }

            // Create this new icon
            GameObject go = NJGTools.AddChild(arrowRoot.gameObject);
            go.name = "Arrow" + mArrowCount;

            GameObject arrowGo = NJGTools.AddChild(go);
            Image sprite = arrowGo.AddComponent<Image>();
            sprite.sprite = NJGMap.instance.GetArrowSprite(item.type);
            sprite.color = item.color;
            //sprite.material = iconMat;

            RectTransform srt = sprite.GetComponent<RectTransform>();
            srt.sizeDelta = arrowScale;

            sprite.transform.localPosition = new Vector3(0, mapScale.y / 2 - item.arrowOffset, 0);

            MapArrow mi = go.AddComponent<MapArrow>();
            mi.child = sprite.transform;
            mi.child.localEulerAngles = new Vector3(0, 180f, 0);
            mi.item = item;
            mi.sprite = sprite;
            mi.map = this;

            if (mi == null)
            {
                Debug.LogError("Expected to find a MapArrow on the prefab to work with");
                Destroy(go);
            }
            else
            {
                mArrowCount++;
                mi.item = item;
                mListArrow.Add(mi);
            }
            return mi;
        }

#region Camera Frustum

		GameObject mFrustum;
		Mesh mFrustumMesh = null;
		Material mFrustumMat;

		/// <summary>
		/// Updates and draws the camera frustum on the NJGMap.instance.
		/// </summary>
		
		protected virtual void UpdateFrustum()
		{
			if (NJGMap.instance.cameraFrustum == null) return;
			if (NJGMap.instance.orientation == NJGMap.Orientation.XYSideScroller) return;
			
			if (mFrustumMesh == null)
			{
				mFrustum = new GameObject();// GameObject.CreatePrimitive(PrimitiveType.Quad);
				mFrustumMat = new Material(Shader.Find("Ninjutsu Games/Map TextureMask"));
				mFrustum.AddComponent<MeshRenderer>().material = mFrustumMat;
				//Destroy(mFrustum.collider);
				mFrustum.name = "_Frustum";
				mFrustum.transform.parent = iconRoot;
				mFrustum.transform.localEulerAngles = new Vector3(270, 0, 0);
				mFrustum.transform.localPosition = Vector3.zero;
				mFrustum.transform.localScale = Vector3.one;
				mFrustum.layer = gameObject.layer;
				mFrustumMesh = mFrustum.AddComponent<MeshFilter>().mesh = NJGTools.CreatePlane();
			}

			Vector3[] vertices = mFrustumMesh.vertices;

			vertices[1] = NJGMap.instance.cameraFrustum.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, NJGMap.instance.cameraFrustum.farClipPlane));
			vertices[2] = NJGMap.instance.cameraFrustum.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, NJGMap.instance.cameraFrustum.nearClipPlane));
			vertices[3] = NJGMap.instance.cameraFrustum.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, NJGMap.instance.cameraFrustum.nearClipPlane));
			vertices[0] = NJGMap.instance.cameraFrustum.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, NJGMap.instance.cameraFrustum.farClipPlane));

			float height = NJGMap.instance.orientation == NJGMap.Orientation.XZDefault ? 
				NJGMap.instance.bounds.min.y - 1.0f + 0.1f : NJGMap.instance.bounds.max.z + 1.0f + 0.1f;

			for (int i = 0; i < 4; i++)
			{
				vertices[i].y = height;
			}

			mFrustumMesh.vertices = vertices;
			mFrustumMesh.RecalculateBounds();

			mFrustumMat.SetColor("_Color", NJGMap.instance.cameraFrustumColor);
		}

#endregion

#region Panning

		/// <summary>
		/// Checks if panning.
		/// </summary>

		public bool isPanning { get { if (!panning) return false; return mIsPanning; } }

		/// <summary>
		/// Update panning position.
		/// </summary>
        /// 
        public void OnPointerDown(PointerEventData eventData)
        {
            if(mUICam == null) mUICam = eventData.enterEventCamera;

            MapIcon.UnSelectAll();

            Camera cam = mUICam ? mUICam : eventData.enterEventCamera;
            if(cam) mPanningMousePosLast = cam.ScreenToViewportPoint(Input.mousePosition);
            if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            arrowRoot.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            mIsPanning = true;
            Camera cam = mUICam ? mUICam : eventData.enterEventCamera;

            if (cam != null)
            {
                Vector2 d = (Vector2)cam.ScreenToViewportPoint(Input.mousePosition) - mPanningMousePosLast;
                Vector2 r = GetDirection(d) * panningSensitivity;
                panningPosition -= r / zoom;
                mPanningMousePosLast = cam.ScreenToViewportPoint(Input.mousePosition);
            }
        }

		/// <summary>
		/// Moves the map back to the target position.
		/// </summary>		

        public void OnEndDrag(PointerEventData eventData)
		{
            ResetPanning();

		}

        public void ResetPanning()
        {
            if (panningPosition == Vector2.zero)
            {
                mIsPanning = false;
                arrowRoot.localScale = Vector3.one;
                return;
            }

            //if(mResetPan == null)
            //	mResetPan = new TweenParms().Prop("panningPosition", Vector2.zero).OnComplete(OnPanningComplete);

            //LeanTween.value(.move(gameObject, Vector2.zero, panningSpeed).onComplete = OnPanningComplete;
            //HOTween.To(this, panningSpeed, mResetPan).easeType = panningEasing;		

            LeanTween.value(gameObject, UpdatePanningPos, (Vector3)panningPosition, Vector3.zero, panningSpeed).setEase(panningEasing).setOnComplete(OnPanningComplete);

        }

        void UpdatePanningPos(Vector3 val)
        {
            panningPosition = val;
        }

		/// <summary>
		/// Resets panning position when tween completes.
		/// </summary>

		void OnPanningComplete()
		{
			panningPosition = Vector2.zero;
			mIsPanning = false;
            arrowRoot.localScale = Vector3.one;
		}

#endregion

#region Lines Drawing

		int mVertextCount = 0;
		int mLastVertextCount = 0;

		Color mLastColor;
		float mLastWidth;

		void DrawLines()
		{
			// not enough points specified
			if (null == mLineRenderer || controlPoints == null || controlPoints.Count < 2)
				return;

			// update line renderer
			if (mLastColor != lineColor)
			{
				mLastColor = lineColor;
				mLineRenderer.SetColors(lineColor, lineColor);
			}

			if (mLastWidth != lineWidth)
			{
				mLastWidth = lineWidth;
				mLineRenderer.SetWidth(lineWidth * 0.1f, lineWidth * 0.1f);
			}

			if (linePoints < 2)
				linePoints = 2;

			mVertextCount = linePoints * (controlPoints.Count - 1);

			if (mLastVertextCount != mVertextCount)
			{
				mLastVertextCount = mVertextCount;
				mLineRenderer.SetVertexCount(mVertextCount);
			}

			// loop over segments of spline
			Vector3 p0;
			Vector3 p1;
			Vector3 m0;
			Vector3 m1;
			for (int j = 0, jmax = controlPoints.Count - 1; j < jmax; j++)
			{
				// check control points
				if (controlPoints[j] == null ||
				   controlPoints[j + 1] == null ||
				   (j > 0 && controlPoints[j - 1] == null) ||
				   (j < controlPoints.Count - 2 &&
				   controlPoints[j + 2] == null))
				{
					return;
				}
				// determine control points of segment
				p0 = controlPoints[j].position;
				p1 = controlPoints[j + 1].position;

				if (j > 0)				
					m0 = 0.5f * (controlPoints[j + 1].position - controlPoints[j - 1].position);				
				else				
					m0 = controlPoints[j + 1].position - controlPoints[j].position;				
				if (j < controlPoints.Count - 2)				
					m1 = 0.5f * (controlPoints[j + 2].position - controlPoints[j].position);				
				else				
					m1 = controlPoints[j + 1].position - controlPoints[j].position;
				
				/*p0 = WorldToMap(controlPoints[j].position);
				Debug.Log("Pos " + p0 + " / " + controlPoints[j].position + " - " + j + " / " + controlPoints[j]);
				p1 = WorldToMap(controlPoints[j + 1].position);
				if (j > 0)
				{
					m0 = 0.5f * (WorldToMap(controlPoints[j + 1].position) - WorldToMap(controlPoints[j - 1].position));
				}
				else
				{
					m0 = WorldToMap(controlPoints[j + 1].position) - WorldToMap(controlPoints[j].position);
				}
				if (j < controlPoints.Count - 2)
				{
					m1 = 0.5f * (WorldToMap(controlPoints[j + 2].position) - WorldToMap(controlPoints[j].position));
				}
				else
				{
					m1 = WorldToMap(controlPoints[j + 1].position) - WorldToMap(controlPoints[j].position);
				}*/

				// set points of Hermite curve
				Vector3 position;
				float t;
				float pointStep = 1.0f / linePoints;

				// last point of last segment should reach p1
				if (j == controlPoints.Count - 2) pointStep = 1.0f / (linePoints - 1.0f);

				for (int i = 0; i < linePoints; i++)
				{
					t = i * pointStep;
					position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
					   + (t * t * t - 2.0f * t * t + t) * m0
					   + (-2.0f * t * t * t + 3.0f * t * t) * p1
					   + (t * t * t - t * t) * m1;
					mLineRenderer.SetPosition(i + j * linePoints, position);
				}
			}
		}

#endregion

#region Public Methods

        /// <summary>
        /// Delete the specified entry, adding it to the unused list.
        /// </summary>

        public virtual void DeleteArrow(MapArrow ent)
        {
            if (ent != null)
            {
                mListArrow.Remove(ent);
                mUnusedArrow.Add(ent);
                ent.gameObject.SetActive(false);
            }
        }

		/// <summary>
		/// Zoom out the minimap
		/// </summary>

		public void ZoomIn(float amount)
		{
			if (zoom == maxZoom) return;
            //if (HOTween.IsTweening(this)) HOTween.Complete(this);
            //HOTween.To(this, zoomSpeed, "zoom", Mathf.Clamp(zoom + amount, (int)minZoom, (int)maxZoom)).easeType = zoomEasing;
		    if (Application.isPlaying)
		    {
		        //if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);
		        float newZoom = Mathf.Clamp(zoom + amount, (int) minZoom, (int) maxZoom);
		        LeanTween.value(gameObject, UpdateZoom, zoom, newZoom, zoomSpeed).setEase(zoomEasing);
		    }
		}

		/// <summary>
		/// Zoom out the minimap
		/// </summary>

		public void ZoomOut(float amount)
		{
			if (zoom == minZoom) return;
            //if (HOTween.IsTweening(this)) HOTween.Complete(this);
            //HOTween.To(this, zoomSpeed, "zoom", Mathf.Clamp(zoom - amount, (int)minZoom, (int)maxZoom)).easeType = zoomEasing;

		    if (Application.isPlaying)
		    {
		        //if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);
		        float newZoom = Mathf.Clamp(zoom - amount, (int) minZoom, (int) maxZoom);
		        LeanTween.value(gameObject, UpdateZoom, zoom, newZoom, zoomSpeed).setEase(zoomEasing);
		    }
        }

		/// <summary>
		/// Transform map coords to world coords.
		/// </summary>		

        public Vector3 MapToWorld()
		{
            //find the position of the world map on the screeen
            Vector3 mScreen = mUICam.WorldToScreenPoint(rendererTransform.position);
            //calculate the mouse position on the worldmap
            Vector3 mMouse = new Vector3(Input.mousePosition.x - mScreen.x, 0f, Input.mousePosition.y - mScreen.y);
            //apply the scale of the world map vs screen and the zoom
            mMouse = new Vector3(mMouse.x * (NJGMap.instance.bounds.size.x / rendererTransform.rect.width / zoom), mMouse.y, mMouse.z * (NJGMap.instance.bounds.size.z / rendererTransform.rect.height / zoom));
            //apply paned position
            Vector3 v2 = WorldScrollPosition();            

            mMouse.x = mMouse.x + v2.x + NJGMap.instance.bounds.extents.x;
            mMouse.z = mMouse.z + v2.z + NJGMap.instance.bounds.extents.z;
            mMouse.y = NJGMap.instance.bounds.size.y;

            RaycastHit hit;
            if (Physics.Raycast(mMouse, -Vector3.up, out hit, NJGMap.instance.bounds.size.y))
            {
                mMouse.y = hit.point.y;
            }

            //Debug.Log("mMouse " + mMouse + " / v2 " + v2+ " / mapHalfScale" + mapHalfScale+ " / mapScale " + mapScale+ " / width " + rendererTransform.rect.width+ " / sizeDelta " + rendererTransform.sizeDelta.x);

            return mMouse;
		}

		/// <summary>
		/// Transform world coords to map coords.
		/// </summary>	

		public Vector2 WorldToMap(Vector3 worldPos)
		{
			return WorldToMap(worldPos, true);
		}

		/// <summary>
		/// Transform world coords to map coords.
		/// </summary>			

		public Vector2 WorldToMap(Vector3 worldPos, bool calculateZoom)
		{
			Bounds bounds = NJGMap.instance.bounds;
			Vector3 extents = bounds.extents;
			Vector3 v = worldPos - bounds.center;

			float x = mapHalfScale.x / extents.x;
			float y = mapHalfScale.y / ((NJGMap.instance.orientation == NJGMap.Orientation.XZDefault) ? extents.z : extents.y);
			Vector3 v2 = WorldScrollPosition();

			if (calculateZoom)
			{
				x *= zoom;
				y *= zoom;
			}
			else
			{
				x *= 1f;
				y *= 1f;
				v2 = Vector3.zero;
			}

			Vector2 mWTM = Vector2.zero;
			mWTM.x = (v.x - v2.x) * x;
			mWTM.y = (NJGMap.instance.orientation == NJGMap.Orientation.XZDefault) ? (v.z - v2.z) * y : (v.y - v2.y) * y;

			if (NJGMap.instance.mapResolution == NJGMap.Resolution.Double)
			{
				x *= mMod;
				y *= mMod;
			}

			return mWTM;
		}

		Vector3 mScrollPos;

		public Vector3 WorldScrollPosition()
		{
			Vector3 v = NJGMap.instance.bounds.size;
			mScrollPos.x = scrollPosition.x * v.x; 
			mScrollPos.y = scrollPosition.y * v.y;
			mScrollPos.z = scrollPosition.y * v.z;
			return mScrollPos;
		}

        public void ToggleLockRotation()
        {
            rotateWithPlayer = !rotateWithPlayer;
        }

		

		/// <summary>
		/// Toggle NJGMap.instance.
		/// </summary>

		public void Toggle()
		{
            if (isVisible) Hide();
			else Show();
		}

		/// <summary>
		/// Show NJGMap.instance.
		/// </summary>

		public virtual void Show()
		{
			if (!isVisible)
			{
				if (mAnim == null && !mAnimCheck)
				{
                    mAnim = contentRoot.GetComponent<Animator>();
                    if (mAnim == null) mAnim = contentRoot.GetComponentInChildren<Animator>();
					mAnimCheck = true;
				}

                if (mAnim != null)
                {
                    contentRoot.gameObject.SetActive(true);
                    mAnim.Play(animIn);
                    /*mAnim[mAnim.clip.name].speed = 1;
                    mAnim[mAnim.clip.name].time = 0;
                    if (mAnim.clip != null) mAnim.Play();*/
                }
                else { contentRoot.gameObject.SetActive(true); }
			}
		}

		/// <summary>
		/// Hide NJGMap.instance.
		/// </summary>	

		public virtual void Hide()
		{
            if (isVisible)
			{
				if (mAnim == null && !mAnimCheck)
				{
                    mAnim = gameObject.GetComponentInChildren<Animator>();
					mAnimCheck = true;
				}

                if (mAnim != null)
                {
                    //if (mAnim.clip != null)
                    //{
                    //mAnim[mAnim.clip.name].speed = -1;
                    //mAnim[mAnim.clip.name].time = mAnim[mAnim.clip.name].length;
                    mAnim.Play(animOut);
                    StartCoroutine(DisableOnFinish());
                    //}
                }
                else { contentRoot.gameObject.SetActive(false); }
			}
		}

		IEnumerator DisableOnFinish()
		{
            while(mAnim.IsInTransition(0))
            {
                yield return new WaitForSeconds(0.5f);
            }
			//yield return new WaitForSeconds(mAnim[mAnim.clip.name].length);
            contentRoot.gameObject.SetActive(false);
		}

		/// <summary>
		/// Gets direction of position based on map rotation.
		/// </summary>

		public Vector2 GetDirection(Vector2 position) { return mapRotation * position; }

		/// <summary>
		/// Gets direction of position based on map rotation.
		/// </summary>

		public Vector3 GetDirection(Vector3 position) { return mapRotation * position; }

#endregion
	}
}