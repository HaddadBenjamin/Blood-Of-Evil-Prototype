//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using NJG;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.EventSystems;

namespace NJG
{
	[System.Serializable]
	public class NJGMap : ScriptableObject
	{
		#region Internal Classes
		[System.Serializable]
		public class FOWSettings
		{

			public enum FOWSystem
			{
				BuiltInFOW,
				TasharenFOW
			}

            public bool enabled
            {
                get
                {
                    return mEnabled;
                }

                set
                {
                    mEnabled = value;

                    if (mEnabled)
                    {
                        if (Map.miniMap != null)
                        {
                            //Map.miniMap.shaderType = NJGMap.ShaderType.FOW;
                            Map.miniMap.material.shader = Shader.Find("Ninjutsu Games/Map FOW");
                        }
                        if (Map.worldMap != null)
                        {
                            //Map.worldMap.shaderType = NJGMap.ShaderType.FOW;
                            Map.worldMap.material.shader = Shader.Find("Ninjutsu Games/Map FOW");
                        }
                        if (Application.isPlaying) NJGFOW.instance.ResetFOW();
                        if (onFOWEnabled != null) onFOWEnabled();
                    }
                    else
                    {
                        if (Map.miniMap != null)
                        {
                            //Map.miniMap.shaderType = NJGMap.ShaderType.TextureMask;
                            Map.miniMap.material.shader = Shader.Find("Ninjutsu Games/Map");
                        }
                        if (Map.worldMap != null)
                        {
                            //Map.worldMap.shaderType = NJGMap.ShaderType.TextureMask;
                            Map.worldMap.material.shader = Shader.Find("Ninjutsu Games/Map");
                        }

                        if (onFOWDisabled != null) onFOWDisabled();
                    }
                }
            }

			public FOWSystem fowSystem;

			//public bool hideIcons = true;

			/// <summary>
			/// Enables a trail effect when on reveal.
			/// </summary>

			public bool trailEffect;

			/// <summary>
			/// How long it takes to reveal the texture.
			/// </summary>

			public float textureBlendTime = 0.5f;

			/// <summary>
			/// How often FOW textures should be updated.
			/// </summary>

			public float updateFrequency = 0.15f;

			/// <summary>
			/// The FOW color.
			/// </summary>

			public Color fogColor = Color.black;

			/// <summary>
			/// How much should be revealed around each unit.
			/// </summary>

			public int revealDistance = 10;

			/// <summary>
			/// Size of your world in units. For example, if you have a 256x256 terrain, then just leave this at '256'.
			/// </summary>

			//public Vector2 worldSize = new Vector2(256, 256);

			/// <summary>
			/// Size of the fog of war texture. Higher resolution will result in more precise fog of war, at the cost of performance.
			/// </summary>

			public int textureSize = 200;

			/// <summary>
			/// If debugging is enabled, the time it takes to calculate the fog of war will be shown in the log window.
			/// </summary>

			public bool debug = false;

			/// <summary>
			/// How many blur iterations will be performed. More iterations result in smoother edges.
			/// Blurring happens on a separate thread and does not affect performance.
			/// </summary>

			public int blurIterations = 2;

            private bool mEnabled;
		}

		[System.Serializable]
		public class MapItemType
		{
			public bool enableInteraction = true;
			public string type = "New Marker";
            public Sprite icon;
            public Sprite border;
            public Sprite arrow;
			//public string selectedSprite;
			public bool useCustomSize = false;
			public bool useCustomBorderSize = false;
			public int size = 32;
			public int borderSize = 32;
			public Color color = Color.white;
            public bool alwaysVisible = false;
			public bool animateOnVisible = true;
			public bool showOnAction = false;
			public bool loopAnimation = false;
			public float fadeOutAfterDelay = 0;
			public bool rotate = true;
			public bool updatePosition = true;
			public bool smoothPosition = false;
            public bool haveArrow = false;
			public bool folded = true;
			public int depth = 0;
			public bool deleteRequest = false;
			public int arrowOffset = 20;
            public bool arrowRotate = true;
            public bool arrowAlwaysVisible = false;
			//public bool revealFOW = false;

			public void OnSelectSprite(Sprite sprite)
			{
                icon = sprite;
			}

            public void OnSelectBorderSprite(Sprite sprite)
			{
                border = sprite;
			}

            public void OnSelectArrowSprite(Sprite sprite)
			{
                arrow = sprite;
			}
		}

		[System.Serializable]
		public class MapLevel
		{
			public string level = "Level";
			public List<MapZone> zones = new List<MapZone>();
			public bool folded = true;
			public bool itemsFolded = true;
			public bool deleteRequest = false;
		}

		[System.Serializable]
		public class MapZone
		{
			public string type = "New Zone";
			public Color color = Color.white;
			public float fadeOutAfterDelay = 3;
			public bool folded = true;
			public int depth = 0;
			public bool deleteRequest = false;
		}
		#endregion

        static public System.Action onFOWEnabled;
        static public System.Action onFOWDisabled;

		#region Enums

		public enum SettingsScreen
		{
			General,
			Icons,
			FOW,
			Zones,
			_LastDoNotUse
		}

		public enum Resolution
		{
			Normal,
			Double
		}

		public enum RenderMode
		{
			/// <summary>
			/// Render the map only once the system is initiated.
			/// </summary>
			Once,

			/// <summary>
			/// Render every time the screen changes of size.
			/// </summary>
			ScreenChange,

			/// <summary>
			/// Use this mode if you need to update the map often manually.
			/// </summary>
			Dynamic
		}

		public enum NJGTextureFormat
		{
			ARGB32 = TextureFormat.ARGB32,
			RGB24 = TextureFormat.RGB24,
            //RGBAFloat = TextureFormat.RGBAFloat
		}

		public enum NJGCameraClearFlags
		{
			Skybox = CameraClearFlags.Skybox,
			Depth = CameraClearFlags.Depth,
			Color = CameraClearFlags.Color,
			Nothing = CameraClearFlags.Nothing,
		}

		public enum ShaderType
		{
			TextureMask = 0,
			ColorMask = 1,
			FOW = 2
		}

		[SerializeField]
		public enum Orientation
		{
			XZDefault = 0,
			XYSideScroller = 1
		}

		#endregion

		[SerializeField]
		public Map miniMap;
		[SerializeField]
        public Map worldMap;

		static NJGMap mInst;
		static public NJGMap instance
        {
            get
            {
                if (mInst == null)
                    mInst = (NJGMap)Resources.Load("NJGMap", typeof(NJGMap));
                return mInst;
            }
        }

		static GameObject mZRoot;
		static public GameObject zonesRoot { get { if (mZRoot == null) mZRoot = GameObject.Find("_MapZones"); return mZRoot; } set { mZRoot = value; } }

		//public delegate void OnWorldNameChanged(string name);
		static public System.Action<string> onWorldZoneChange;

		[SerializeField]
        public FOWSettings fow;		

		/// <summary>
		/// Draw bounds on the scene view.
		/// </summary>

		[SerializeField]
		public bool showBounds = true;

		/// <summary>
		/// Current zone color.
		/// </summary>

		[SerializeField]
		public Color zoneColor = Color.white;

		/// <summary>
		/// List of map item types.
		/// </summary>

		public List<MapItemType> mapItems = new List<MapItemType>(new MapItemType[] { new MapItemType(){ type = "None" } });

        /// <summary>
        /// List of zones.
        /// </summary>

        public List<MapLevel> levels = new List<MapLevel>();

        /// <summary>
        /// Array of keys in use.
        /// </summary>

        [SerializeField]
        public KeyCode[] keysInUse = new KeyCode[4];

		/// <summary>
		/// Map render mode.
		/// </summary>

		[SerializeField]
		public RenderMode renderMode = RenderMode.Once;

		/// <summary>
		/// This instance wont be destroyed if true.
		/// </summary>

		public bool persistOnLevelLoad;

		/// <summary>
		/// If true it will call NJGMap.instance.GenerateMap(); everytime a level is loaded.
		/// </summary>

		public bool renderOnLevelLoad;

		/// <summary>
		/// If true the instance of NJGMap will be persistence.
		/// </summary>

		public bool dontDestroy;

		/// <summary>
		/// Map resolution.
		/// </summary>

		[SerializeField]
		public Resolution mapResolution = Resolution.Normal;

		/// <summary>
		/// If Dynamic render mode is choosen use this parameter to make the map render every each (dynamicRenderTime).
		/// </summary>

		[SerializeField]
		public float dynamicRenderTime = 1f;

		/// <summary>
		/// Map orientation.
		/// </summary>

		//[SerializeField]
		public Orientation orientation = Orientation.XZDefault;

		/// <summary>
		/// Current settings screen. Internal use only.
		/// </summary>

		//[SerializeField]
		public SettingsScreen screen = SettingsScreen.General;

		/// <summary>
		/// Which layers is the map going to render.
		/// </summary>

		[SerializeField]
		public LayerMask renderLayers = 1;

		/// <summary>
		/// Which layers are going to be used for bounds calculation.
		/// </summary>

		[SerializeField]
		public LayerMask boundLayers = 1;

		/// <summary>
		/// Global size of the map icons.
		/// </summary>

		public int iconSize = 16;

		/// <summary>
		/// Global size of the border map icons.
		/// </summary>

		public int borderSize = 16;

		/// <summary>
		/// Global size of the map arrows.
		/// </summary>

		public int arrowSize = 16;

		/// <summary>
		/// How often the map will be updated.
		/// </summary>

		public float updateFrequency = 0.01f;

		/// <summary>
		/// True if you want to define the bounds manually.
		/// </summary>

		public bool setBoundsManually = false;

		/// <summary>
		/// You can set the bounds manually if setBoundsManually is true.
		/// </summary>

		[SerializeField]
		public Vector3 customBounds = new Vector3(10, 10, 10);

		public Vector3 customBoundsCenter = Vector3.zero;

		/// <summary>
		/// World bounds.
		/// </summary>

		[SerializeField]
		public Bounds bounds;

		/// <summary>
		/// Internally used by the inspector to save fold state of map item types.
		/// </summary>

		public bool typesFolded;

		/// <summary>
		/// Internally used by the inspector to save fold state of zones.
		/// </summary>

		public bool zonesFolded;

		/// <summary>
		/// Texture of the map.
		/// </summary>

		public Texture2D mapTexture;

		/// <summary>
		/// User defined texture of the map.
		/// </summary>

		public Texture2D userMapTexture;

		/// <summary>
		/// If true the map texture will be generated.
		/// </summary>

		public bool generateMapTexture;

		/// <summary>
		/// If true the map texture will be generated at start, if false the method GenerateMap() must be called manually.
		/// </summary>

		public bool generateAtStart = true;

		/// <summary>
		/// The camera that is going to be used to draw the frustum.
		/// </summary>

		public Camera cameraFrustum;

		/// <summary>
		/// Color for the frustum object.
		/// </summary>

		public Color cameraFrustumColor = new Color(255f, 255f, 255f, 50f);        

		public bool useTextureGenerated;

		[SerializeField]
		public FilterMode mapFilterMode = FilterMode.Bilinear;
		[SerializeField]
		public TextureWrapMode mapWrapMode = TextureWrapMode.Clamp;
		[SerializeField]
		public NJGTextureFormat textureFormat = NJGTextureFormat.ARGB32;
		[SerializeField]
		public NJGCameraClearFlags cameraClearFlags = NJGCameraClearFlags.Skybox;
		public Color cameraBackgroundColor = Color.red;
		public bool transparentTexture = false;
#if UNITY_EDITOR
		[SerializeField]
		public TextureCompressionQuality compressQuality = TextureCompressionQuality.Fast;
#endif
		public bool optimize = false;
		public bool generateMipmaps = false;
		public int boundsOffset = 10;
		public int layer = 0;        

		static public System.Action<Texture2D> onTextureChanged;

		public const string VERSION = "1.6.0";

        public Camera mCam;

        #region Getters and Setters vars

        /// <summary>
        /// The name of the world that is going to be displayed on World Map and Mini Map titles.
        /// </summary>

        [SerializeField]
        public string worldName
        {
            get { return mWorldName; }
            set
            {
                mWorldName = value;

                // Only trigger this event if the name is different than the last one
                if (mLastWorldName != mWorldName)
                {
                    mLastWorldName = mWorldName;
                    if (onWorldZoneChange != null) onWorldZoneChange(mWorldName);
                }
            }
        }

        /// <summary>
        /// Use this to check when mouse is over the minimap UI.
        /// </summary>

        public bool isMouseOver
        {
            get
            {
                return (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null || EventSystem.current.alreadySelecting) || ((Map.miniMap == null) || Map.miniMap.isMouseOver) || ((Map.worldMap == null) || Map.worldMap.isMouseOver);
            }
        }

        [SerializeField]
        public Vector3 mapOrigin { get { if (MapRenderer.instance != null) mMapOrigin = MapRenderer.instance.cachedTransform.position; return mMapOrigin; } }
        [SerializeField]
        public Vector3 mapEulers { get { if (MapRenderer.instance != null) mMapEulers = MapRenderer.instance.cachedTransform.eulerAngles; return mMapEulers; } }
        [SerializeField]
        public float ortoSize { get { if (MapRenderer.instance != null) mOrtoSize = MapRenderer.instance.GetComponent<Camera>().orthographicSize; return mOrtoSize; } }
        [SerializeField]
        public float aspect { get { if (MapRenderer.instance != null) mAspect = MapRenderer.instance.GetComponent<Camera>().aspect; return mAspect; } }

        /// <summary>
        /// Map size.
        /// </summary>
        [SerializeField]
        public Vector2 mapSize
        {
            set { mSize = value; }
            get
            {
                if (Application.isPlaying)
                {
                    mSize.x = Screen.width;
                    mSize.y = Screen.height;
                }
                return mSize;
            }
        }

        public float elapsed { get { return mElapsed; } }

        /// <summary>
        /// Get a string list of map item types.
        /// </summary>

        [SerializeField]
        public string[] mapItemTypes
        {
            get
            {
                List<string> types = new List<string>();
                //types.Add("None");
                //if (mapItems != null)
                //{
                for (int i = 0, imax = mapItems.Count; i < imax; i++)
                    types.Add(mapItems[i].type);
                //}

                return types.Count == 0 ? new string[] { "No types defined" } : types.ToArray();
            }
        }

        #endregion 

        #region Private Variables

        Vector2 mSize = new Vector2(1024, 1024);

		Bounds mBounds;
		[SerializeField]
		string mWorldName = "My Epic World";
		string mLastWorldName;
		Vector3 mMapOrigin = Vector2.zero;
		Vector3 mMapEulers = Vector2.zero;
		float mOrtoSize = 0;
		float mAspect = 0;
		Thread mThread;
		float mElapsed = 0f;

        #endregion

		/// <summary>
		/// Need to re-generate the texture map? Use this method at will.
		/// </summary>

		static public void GenerateMap()
		{
            NJGMapMono.Spawn();
			MapRenderer.instance.Render();
		}		

		/// <summary>
		/// Sets the texture for the UITexture of the Minimap and World Map.
		/// </summary>

		static public void SetTexture(Texture2D tex)
		{
            if (!Application.isPlaying)
            {
                if (Map.miniMap != null) Map.miniMap.SetTexture(tex);
                if (Map.worldMap != null) Map.worldMap.SetTexture(tex);
            }
			if (onTextureChanged != null) onTextureChanged(tex);
		}

		/// <summary>
		/// Create bounding box and scale it to contain all scene game objects, if terrain is found it is used
		/// </summary>

		public void UpdateBounds()
		{
            bounds = NJGMapMono.instance.UpdateBounds(boundLayers, boundsOffset, setBoundsManually, customBounds, customBoundsCenter);			
		}

        /// <summary>
        /// Set bounds by given size and center
        /// </summary>
        /// <param name="size"></param>
        /// <param name="center"></param>

        public void SetBounds(Vector3 size, Vector3 center)
        {
            bounds = NJGMapMono.instance.UpdateBounds(boundLayers, boundsOffset, true, size, center);
        }

        /// <summary>
        /// Set bounds.
        /// </summary>
        /// <param name="newBounds"></param>

        public void SetBounds(Bounds newBounds)
        {
            bounds = newBounds;
        }

		#region Getters

        /// <summary>
        /// Get sprite from type.
        /// </summary>

        public Sprite GetSprite(int type)
        {
            return Get(type) == null ? null : Get(type).icon;
        }

        /// <summary>
        /// Get sprite border from type.
        /// </summary>

        public Sprite GetSpriteBorder(int type)
        {
            return Get(type) == null ? null : Get(type).border;
        }

        /// <summary>
        /// Get arrow sprite from type.
        /// </summary>

        public Sprite GetArrowSprite(int type)
        {
            return Get(type) == null ? null : Get(type).arrow;
        }

		public string[] GetZones(string level)
		{
			List<string> list = new List<string>();

			if (levels != null)
			{
				for (int i = 0, imax = levels.Count; i < imax; i++)
				{
					if (levels[i].level == level)
					{
						for (int e = 0, emax = levels[i].zones.Count; e < emax; e++)
						{
							list.Add(levels[i].zones[e].type);
						}
					}
				}
			}

			return list.Count == 0 ? new string[] { "No Zones defined" } : list.ToArray();
		}

		/// <summary>
		/// Get zone by scene.
		/// </summary>

		public string[] GetLevels()
		{
			List<string> list = new List<string>();

			if (levels != null)
			{
				for (int i = 0, imax = levels.Count; i < imax; i++)
					list.Add(levels[i].level);
			}

			return list.Count == 0 ? new string[] { "No Levels defined" } : list.ToArray();
		}

		/// <summary>
		/// Get color from zone.
		/// </summary>

		public Color GetZoneColor(string level, string zone)
		{
			Color c = Color.white;
			for (int i = 0, imax = levels.Count; i < imax; i++)
			{
				if (levels[i].level == level)
				{
					for (int e = 0, emax = levels[i].zones.Count; e < emax; e++)
					{
						if (levels[i].zones[e].type.Equals(zone)) return levels[i].zones[e].color;
					}
				}
			}

			return c;
		}

		/// <summary>
		/// Get interaction.
		/// </summary>

        public bool GetInteraction(int type) { return Get(type) == null ? false : Get(type).enableInteraction; }
        public bool GetAlwaysVisible(int type) { return Get(type) == null ? false : Get(type).arrowAlwaysVisible; }

		/// <summary>
		/// Get color from type.
		/// </summary>

		public Color GetColor(int type) { return Get(type) == null ? Color.white : Get(type).color; }

		/// <summary>
		/// Get animate from type.
		/// </summary>

		public bool GetAnimateOnVisible(int type) { return Get(type) == null ? false : Get(type).animateOnVisible; }

		/// <summary>
		/// Get animate on action from type.
		/// </summary>

		public bool GetAnimateOnAction(int type) { return Get(type) == null ? false : Get(type).showOnAction; }

		/// <summary>
		/// Get animate from type.
		/// </summary>

		public bool GetLoopAnimation(int type) { return Get(type) == null ? false : Get(type).loopAnimation; }

		/// <summary>
		/// Get have arrow from type.
		/// </summary>

        public bool GetHaveArrow(int type) { return Get(type) == null ? false : Get(type).haveArrow; }
        public bool GetArrowAlwaysVisible(int type) { return Get(type) == null ? false : Get(type).arrowAlwaysVisible; }

		/// <summary>
		/// Get animate from type.
		/// </summary>

		public float GetFadeOutAfter(int type) { return Get(type) == null ? 0 : Get(type).fadeOutAfterDelay; }

		/// <summary>
		/// Get rotate flag.
		/// </summary>

		public bool GetRotate(int type) { return Get(type) == null ? false : Get(type).rotate; }

		/// <summary>
		/// Get arrow rotate flag.
		/// </summary>

		public bool GetArrowRotate(int type) { return Get(type) == null ? false : Get(type).arrowRotate; }

		/// <summary>
		/// Get update position flag.
		/// </summary>

		public bool GetUpdatePosition(int type) { return Get(type) == null ? false : Get(type).updatePosition; }

		public bool GetSmoothPosition(int type) { return Get(type) == null ? false : Get(type).smoothPosition; }

        /// <summary>
        /// Get custom icon size.
        /// </summary>

        public int GetSize(int type) { return Get(type) == null ? 0 : Get(type).size; }

		/// <summary>
		/// Get custom icon size.
		/// </summary>

		public int GetBorderSize(int type) { return Get(type) == null ? 0 : Get(type).borderSize; }

		/// <summary>
		/// Get custom icon size flag.
		/// </summary>

		public bool GetCustom(int type) { return Get(type) == null ? false : Get(type).useCustomSize; }

		/// <summary>
		/// Get custom icon size flag.
		/// </summary>

		public bool GetCustomBorder(int type) { return Get(type) == null ? false : Get(type).useCustomBorderSize; }

        /// <summary>
        /// Get arrow offset.
        /// </summary>

        public int GetArrowOffset(int type) { return Get(type) == null ? 0 : Get(type).arrowOffset; }

		/// <summary>
		/// Get map item type.
		/// </summary>

		public MapItemType Get(int type)
		{
			if (type == -1 || type > mapItems.Count) return null;
			MapItemType mRes = mapItems[type];
			return mRes == null ? null : mRes;
		}

		#endregion
    }
}

