//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NJG
{
    [AddComponentMenu("NJG MiniMap/Map Zone")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(SphereCollider))]
    public class MapZone : MonoBehaviour 
    {
        /// <summary>
        /// Global list of zones
        /// </summary>
    
	    static public List<MapZone> list = new List<MapZone>();

        /// <summary>
        /// Color of this zone
        /// </summary>

	    public Color color { get { return map == null ? Color.white : map.GetZoneColor(level, zone); } }

        /// <summary>
        /// Tag that will trigger this zone
        /// </summary>

	    public string triggerTag = "Player";

        /// <summary>
        /// Zone name
        /// </summary>

	    public string zone;

        /// <summary>
        /// Level name
        /// </summary>

	    public string level;

        /// <summary>
        /// Collider radius that will be used to collide with the target
        /// </summary>

	    public int colliderRadius = 10;
	
        /// <summary>
        /// If true this zone will re-render the map texture
        /// </summary>

        public bool generateOnTrigger;

        /// <summary>
        /// If true the map will use this zone bounds to render it
        /// </summary>

        public bool useZoneBounds;

        /// <summary>
        /// Which layers is the map going to render.
        /// </summary>

        [SerializeField]
        public LayerMask renderLayers = 1;

        /// <summary>
        /// Current level of zoom of the Minimap.
        /// </summary>

        [SerializeField]
        public float zoom = 1;

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
        /// This zone collider
        /// </summary>

	    public SphereCollider zoneCollider 
	    { 
		    get 
		    {
			    mCollider = gameObject.GetComponent<SphereCollider>();

			    if (mCollider == null) 
				    mCollider = gameObject.AddComponent<SphereCollider>(); 

			    mCollider.isTrigger = true; 
			    return mCollider; 
		    } 
	    }

	    [SerializeField]SphereCollider mCollider;
	    NJGMap map;
        //string prevZone;
        LayerMask prevRenderLayers = 1;
        float prevZoom;
        float prevZoomWorld;
        bool triggered;

	    void Awake()
	    {
		    map = NJGMap.instance;
		    zoneCollider.radius = colliderRadius;
            prevRenderLayers = map.renderLayers;        
	    }

        void Start()
        {
            prevZoom = Map.miniMap == null ? 1 : Map.miniMap.zoom;
            prevZoomWorld = Map.worldMap == null ? 1 : Map.worldMap.zoom;
        }

        /// <summary>
        /// Change world name, zone color on trigger or even Render the map.
        /// </summary>
        /// <param name="col"></param>

	    void OnTriggerEnter(Collider col)
	    {
            if (col.CompareTag(triggerTag) && !triggered)
		    {
			    if (map != null)
			    {
                
				    map.zoneColor = color;
				    map.worldName = zone;
                    //prevZone = map.worldName;
                
				    if (generateOnTrigger)
				    {
                        if (useZoneBounds)
                        {
                            map.SetBounds(zoneCollider.bounds);
                        }
                        map.renderLayers = renderLayers;
					    NJGMap.GenerateMap();
                        //map.renderLayers = prevRenderLayers;

                        if (Map.miniMap != null && Map.miniMap.zoom != zoom) Map.miniMap.zoom = zoom;
                        if (Map.worldMap != null && Map.worldMap.zoom != zoom) Map.worldMap.zoom = zoom;
				    }
                    triggered = true;
			    }            
		    }
	    }

        void OnTriggerExit(Collider col)
        {
            if (col.CompareTag(triggerTag) && triggered)
            {
                //map.worldName = prevZone;            

                if (generateOnTrigger)
                {
                    if (useZoneBounds)
                    {
                        map.UpdateBounds();
                    }
                    map.renderLayers = prevRenderLayers;
                    NJGMap.GenerateMap();
                    if (Map.miniMap != null && Map.miniMap.zoom != prevZoom) Map.miniMap.zoom = prevZoom;
                    if (Map.worldMap != null && Map.worldMap.zoom != prevZoomWorld) Map.worldMap.zoom = prevZoomWorld;
                }
                triggered = false;
            }
        }

	    /// <summary>
	    /// Add this unit to the list of in-game units.
	    /// </summary>

	    void OnEnable()
	    {
		    list.Add(this);
	    }

	    /// <summary>
	    /// Remove this unit from the list.
	    /// </summary>

	    void OnDisable()
	    {
		    list.Remove(this);
	    }

        void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, zoneCollider.radius);
        }
    }
}
