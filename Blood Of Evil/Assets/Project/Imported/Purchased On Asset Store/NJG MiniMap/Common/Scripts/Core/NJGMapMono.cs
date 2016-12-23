//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using NJG;

public class NJGMapMono : MonoBehaviour 
{
    static public NJGMapMono instance
    {
        get
        {
            if (mInst == null) Spawn();
            return mInst;
        }
    }
    static NJGMapMono mInst;

    Bounds mBounds;

    #region Monobehaviour Methods

    /// <summary>
    /// Setup map renderer.
    /// </summary>

    void Awake()
    {
        if (NJGMap.instance.fow.textureSize < 200) NJGMap.instance.fow.textureSize = 200;
        //Holoville.HOTween.HOTween.Init(false, true, true);
        //Holoville.HOTween.HOTween.EnableOverwriteManager(false);			

        if (Application.isPlaying)
        {
            if (NJGMap.instance.mapTexture != null) NJGTools.Destroy(NJGMap.instance.mapTexture);
            if (NJGMap.instance.generateAtStart) NJGMap.GenerateMap();
        }
    }

    void OnDrawGizmos()
    {
        if (NJGMap.instance.showBounds)
        {
            NJGMap.instance.UpdateBounds();

            Gizmos.color = new Color(1, 0, 0, 0.5F);
            //Gizmos.DrawCube(bounds.center, bounds.size);
            Gizmos.DrawWireCube(NJGMap.instance.bounds.center, NJGMap.instance.bounds.size);
        }
    }

    /// <summary>
    /// Update bounds on start.
    /// </summary>

    void Start()
    {
        if (NJGMap.onWorldZoneChange != null) NJGMap.onWorldZoneChange(NJGMap.instance.worldName);

        NJGMap.instance.UpdateBounds();

        if (!Application.isPlaying) return;

        if (NJGMap.instance.fow.enabled)
        {
            NJGFOW.instance.Init();
        }
    }

    void OnDestroy()
    {
        if (Map.miniMap != null) Map.miniMap.material.mainTexture = null;
        if (Map.worldMap != null) Map.worldMap.material.mainTexture = null;

        if (NJGMap.instance.mapTexture != null) NJGTools.Destroy(NJGMap.instance.mapTexture);
        NJGMap.instance.mapTexture = null;
    }

    #endregion

    static public void Spawn()
    {
        if (mInst == null)
        {
            mInst = FindObjectOfType<NJGMapMono>();

            if (mInst == null)
            {
                GameObject go = GameObject.Find("_NJGMap");
                if (go == null)
                {
                    go = new GameObject("_NJGMap");
                    //DontDestroyOnLoad(go);
                    go.AddComponent<NJGMapMono>();
                }

                mInst = go.GetComponent<NJGMapMono>();
            }
        }
    }

    /// <summary>
    /// Create bounding box and scale it to contain all scene game objects, if terrain is found it is used
    /// </summary>

    Terrain[] mTerrains;

    public Bounds UpdateBounds(LayerMask boundLayers, int boundsOffset, bool setBoundsManually, Vector3 customBounds, Vector3 customBoundsCenter)
    {
        if (mInst == null) Spawn();

        if (setBoundsManually)
        {
            //mBounds = new Bounds(customBounds * 0.5f, customBounds);
            mBounds.size = customBounds;
            mBounds.center = customBoundsCenter;
            if(boundsOffset > 0) mBounds.Expand(new Vector3(boundsOffset, 0, boundsOffset));
            return mBounds;
        }

        bool flag = false;
        int i, imax = 0;

        mTerrains = FindObjectsOfType(typeof(Terrain)) as Terrain[];
        bool multiTerrain = mTerrains != null;
        if (multiTerrain) multiTerrain = mTerrains.Length > 1;

        // First lets see if there is more than one terrain. Multi-terrain handling
        if (multiTerrain)
        {
#if UNITY_EDITOR
			//	Debug.Log("NJGMap: Calculating bounds for multiple terrains ("+mTerrains.Length+")");
#endif
            for (i = 0, imax = mTerrains.Length; i < imax; i++)
            {
                Terrain t = mTerrains[i];
                MeshRenderer mMeshRenderer = t.GetComponent<MeshRenderer>();

                if (!flag)
                {
                    //t.transform.position, new Vector3(1f, 1f, 1f)
                    mBounds = new Bounds();
                    flag = true;
                }

                if (mMeshRenderer != null)
                {
                    //Debug.Log("Terrain Mesh Renderer " + i + " : " + mMeshRenderer.bounds.size + " / " + t.name);
                    mBounds.Encapsulate(mMeshRenderer.bounds);
                }
                else
                {
                    TerrainCollider mTerrainCollider = t.GetComponent<TerrainCollider>();
                    if (mTerrainCollider != null)
                    {
                        //Debug.Log("Terrain Collider " + i + " : " + mTerrainCollider.bounds.size+" / "+t.name);
                        mBounds.Encapsulate(mTerrainCollider.bounds);
                    }
                    else
                    {
                        Debug.LogError("Could not get measure bounds of terrain.", this);
                        return mBounds;
                    }
                }
            }
        }
        // If not then check if there is one activeTerrain
        else if (Terrain.activeTerrain != null)
        {
#if UNITY_EDITOR
			//Debug.Log("NJGMap: Calculating bounds for active terrain");
#endif
            Terrain t = Terrain.activeTerrain;
            MeshRenderer mMeshRenderer = t.GetComponent<MeshRenderer>();

            if (!flag)
            {
                //t.transform.position, new Vector3(1f, 1f, 1f)
                mBounds = new Bounds();
                flag = true;
            }

            if (mMeshRenderer != null)
            {
                Debug.Log("Terrain Mesh Renderer: " + mMeshRenderer.bounds.size + " / " + t.name);
                mBounds.Encapsulate(mMeshRenderer.bounds);
            }
            else
            {
                TerrainCollider mTerrainCollider = t.GetComponent<TerrainCollider>();
                if (mTerrainCollider != null)
                {
                    //Debug.Log("Terrain Collider " + i + " : " + mTerrainCollider.bounds.size+" / "+t.name);
                    mBounds.Encapsulate(mTerrainCollider.bounds);
                }
                else
                {
                    Debug.LogError("Could not get measure bounds of terrain.", this);
                    return mBounds;
                }
            }
        }

        Renderer[] mGameObjects = UnityEngine.Object.FindObjectsOfType(typeof(Renderer)) as Renderer[];
        if (mGameObjects != null)
        {
#if UNITY_EDITOR
				//Debug.Log("NJGMap: Calculating bounds for multiple gameObjects (" + mGameObjects.Length + ")");
#endif
            for (i = 0, imax = mGameObjects.Length; i < imax; i++)
            {
                Renderer go = mGameObjects[i];

                // Dont consider this game object
                //if (go.gameObject.layer == gameObject.layer)
                //    continue;

                // Only use objects from the layer mask
                if (!IsInRenderLayers(go.gameObject, boundLayers))
                    continue;
                
                if (!flag)
                {
                    mBounds = new Bounds(go.transform.position, new Vector3(1f, 1f, 1f));
                    flag = true;
                }

                Collider collider = go.GetComponent<Collider>();
                if (collider)
                {
                    mBounds.Encapsulate(collider.bounds);
                }
                else
                {
                    mBounds.Encapsulate(go.bounds);
                }
                //mBounds.Encapsulate(go.transform.position);
                //Renderer renderer = go.GetComponent<Renderer>();
                //if (renderer)
                //{
                
                //}
                //else
                //{
                    /*Collider collider = go.GetComponent<Collider>();
                    if (collider)
                    {
                        mBounds.Encapsulate(collider.bounds);
                    }*/
                //}
            }
        }

        if (mGameObjects.Length == 0)
        {
            Debug.Log("Could not find terrain nor any other bounds in scene", this);
            mBounds = new Bounds(gameObject.transform.position, new Vector3(1f, 1f, 1f));
        }

        if(boundsOffset > 0) mBounds.Expand(new Vector3(boundsOffset, 0, boundsOffset));

        //if (mapResolution == Resolution.Double)
        //{
            //mBounds.Expand(new Vector3(-mBounds.extents.x, 0f, -mBounds.extents.z));
        //}

        // Set bounds
        return mBounds;
    }

    /// <summary>
    /// Checks if GameObject is within render layers range.
    /// </summary>

    public static bool IsInRenderLayers(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) > 0;
    }
}
