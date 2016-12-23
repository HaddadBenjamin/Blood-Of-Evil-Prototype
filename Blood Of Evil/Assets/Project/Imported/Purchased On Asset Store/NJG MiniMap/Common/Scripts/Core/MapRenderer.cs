//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using NJG;
using System.Collections;
using UnityEngine;

/// <summary>
/// Very basic game map -- renderer component. It's able to draw a map into a 2D texture.
/// </summary>
/// 
namespace NJG
{

    //[ExecuteInEditMode]
    public class MapRenderer : MonoBehaviour
    {
        static MapRenderer mInst;

        /// <summary>
        /// Get instance.
        /// </summary>

        static public MapRenderer instance
        {
            get
            {
                if (mInst == null)
                {
                    mInst = GameObject.FindObjectOfType(typeof(MapRenderer)) as MapRenderer;
                    if (mInst == null)
                    {
                        GameObject go = new GameObject("_MapRenderer");
                        go.transform.parent = NJGMapMono.instance.transform;
                        // Isolate this camera to prevent any interference with other cameras
                        go.layer = LayerMask.NameToLayer("TransparentFX");
                        //go.hideFlags = HideFlags.HideInInspector;
                        mInst = go.AddComponent<MapRenderer>();
                    }
                }
                return mInst;
            }
        }

        /// <summary>
        /// Cached transform for speed.
        /// </summary>

        public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

        /// <summary>
        /// Cached camera for speed.
        /// </summary>

        public Camera cam
        {
            get
            {
                if (mCam == null)
                {
                    mCam = gameObject.GetComponent<Camera>();
                }

                if (gameObject.GetComponent<Camera>() == null)
                {
                    mCam = gameObject.AddComponent<Camera>();
                }

                return mCam;
            }
        }

        public int mapImageIndex = 0;

        Vector2 lastSize;
        Vector2 mSize;
        Transform mTrans;
        bool canRender = true;
        bool mGenerated = false;
        bool mWarning = false;
        bool mReaded = false;
        bool mApplied = false;
        float lastRender = 0;
        NJGMap map;
        Camera mCam;

        void Awake()
        {
            cam.useOcclusionCulling = false;

            Render();
        }

        void Start()
        {
            if (map.boundLayers.value == 0)
            {
                Debug.LogWarning("Can't render map photo. You have not choosen any layer for bounds calculation. Go to the NJGMiniMap inspector.", map);
                //NJGTools.DestroyImmediate(gameObject);
                return;
            }

            if (map.renderLayers.value == 0)
            {
                Debug.LogWarning("Can't render map photo. You have not choosen any layer for rendering. Go to the NJGMiniMap inspector.", map);
                //NJGTools.DestroyImmediate(gameObject);
                return;
            }

            map.UpdateBounds(); 

            ConfigCamera();

            //if (map.optimize) StartCoroutine(DelayedDestroy(gameObject, 2));
            //if(!Application.isPlaying) Render();
        }

#if UNITY_EDITOR
        //private RenderTexture lastTexture = null;
        //float lastCheck;

        /*void Update()
        {
            if(cam.targetTexture != null && NJGMap.instance.generateMapTexture)
            {
                //lastCheck = Time.time + 1f;
                Map[] mps = GameObject.FindObjectsOfType<Map>();
                for (int i = 0, imax = mps.Length; i < imax; i++)
                {
                    Map mp = mps[i];
                    mp.SetTexture(cam.targetTexture);
                }
                //lastTexture = cam.targetTexture;
            }
        }*/

#endif

        public void ConfigCamera()
        {
            if (map == null)
            { 
                map = NJGMap.instance;
            }
#if UNITY_EDITOR
            //cam.targetTexture = Application.isPlaying ? null : UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Ninjutsu Games/NJG MiniMap/Common/Materials/MapPreview.renderTexture", typeof(RenderTexture)) as RenderTexture;
            //RenderTexture.active = cam.targetTexture;
#endif
            if (Application.isPlaying && cam.targetTexture != null) cam.targetTexture.Release();

            Bounds bounds = map.bounds;
            //bounds.Expand(new Vector3(-bounds.extents.x, 0f, -bounds.extents.z));
            cam.depth = -100;
            cam.backgroundColor = map.cameraBackgroundColor;
            cam.cullingMask = map.renderLayers;
            cam.clearFlags = (CameraClearFlags)map.cameraClearFlags;
            cam.orthographic = true;



            float z = 0;
            //float n = 0;

            if (map.orientation == NJGMap.Orientation.XYSideScroller)
            {
                cam.farClipPlane = bounds.size.z * 1.1f;

                //n = bounds.extents.x / bounds.extents.y;
                //if (n < cam.aspect)
                z = bounds.extents.y;
                //else
                //z = bounds.extents.x / cam.aspect;

                cam.aspect = bounds.size.x / bounds.size.y;
            }
            else if (map.orientation == NJGMap.Orientation.XZDefault)
            {
                cam.farClipPlane = bounds.size.y * 1.1f;

                //n = bounds.extents.x / bounds.extents.z;
                //if (n < cam.aspect)
                z = bounds.extents.z;
                //else
                //	z = bounds.extents.x / cam.aspect;

                cam.aspect = bounds.size.x / bounds.size.z;
            }
            cam.farClipPlane = cam.farClipPlane * 5f;
            cam.orthographicSize = z;

            if (map.orientation == NJGMap.Orientation.XZDefault)
            {
                cachedTransform.eulerAngles = new Vector3(90f, 0, 0);
                //cachedTransform.position = new Vector3(bounds.max.x - bounds.extents.x, bounds.size.y, bounds.center.z);//-(Mathf.Abs(bounds.max.z) + Mathf.Abs(bounds.extents.z))
                if (map.mapResolution == NJGMap.Resolution.Double)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z - bounds.extents.z);
                                break;

                            case 1:
                                cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z - bounds.extents.z);
                                break;

                            case 2:
                                cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z + bounds.extents.z);
                                break;

                            case 3:
                                cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z + bounds.extents.z);
                                break;
                        }
                        Debug.Log("cachedTransform.position " + cachedTransform.position + " / mapImageIndex " + mapImageIndex);

                        cam.enabled = true;
                        mapImageIndex = i;
                        //cam.Render();
                    }
                }
                else
                {
                    cachedTransform.position = new Vector3(bounds.max.x - bounds.extents.x, bounds.size.y * 2f, bounds.center.z);
                    cam.enabled = true;
                    //cam.Render();
                }
            }
            else
            {
                cachedTransform.eulerAngles = new Vector3(0, 0, 0);
                cachedTransform.position = new Vector3(bounds.max.x - bounds.extents.x, bounds.center.y, -((Mathf.Abs(bounds.min.z) + Mathf.Abs(bounds.max.z)) + 10));
            }
        }


        /*IEnumerator DelayedDestroy(UnityEngine.Object obj, float delay)
        {
            yield return new WaitForSeconds(delay);

            NJGTools.DestroyImmediate(obj);
        }*/

        /*bool ClearColor(object obj)
        {
            Color32[] list = obj as Color32[];
            newColors = new Color32[list.Length];

            int i = 0;
            int imax = list.Length;

            for (; i < imax; i++)
            {
                Color c = list[i];
                if (c == bgColor) c = Color.clear;
                newColors[i] = c;
            }
            done = true;
            return false;
        }*/

        IEnumerator OnPostRender()
        {
            if (!Application.isPlaying || NJGMap.instance.renderMode == NJGMap.RenderMode.Dynamic) ConfigCamera();

            // Can't re-generate the texture map is makeNoLongReadable flag is turned on.
            if (mGenerated && map.optimize && Application.isPlaying && !mWarning)
            {
                mWarning = true;
                Debug.LogWarning("Can't Re-generate the map texture since 'optimize' is activated");
                canRender = false;
            }
            else
            {
                if (canRender)
                {
                    if (map.mapTexture == null)
                    {
                        mSize = map.mapSize;
                        if (map.mapResolution == NJGMap.Resolution.Double) mSize = map.mapSize * 2;
                        map.mapTexture = new Texture2D((int)mSize.x, (int)mSize.y, (TextureFormat)map.textureFormat, map.generateMipmaps);
                        map.mapTexture.name = "_NJGMapTexture";
                        map.mapTexture.filterMode = map.mapFilterMode;
                        map.mapTexture.wrapMode = map.mapWrapMode;
                        map.mapTexture.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                        lastSize = mSize;
                    }

                    //if(NJGMap.instance.generateMapTexture) yield return new WaitForEndOfFrame();

                    if (!mReaded || !Application.isPlaying)
                    {
                        //mReaded = true;

                        // First take a screenshot from game view when camera is rendering.
                        if (map.generateMapTexture && canRender)
                        {
                            if (NJGMap.instance.renderMode != NJGMap.RenderMode.Once)
                            {
                                mSize = map.mapSize;
                                if (map.mapResolution == NJGMap.Resolution.Double) mSize = map.mapSize * 2;
                                if (mSize.x >= lastSize.x || mSize.y >= lastSize.y)
                                {
                                    lastSize = mSize;
                                    map.mapTexture.Resize((int)mSize.x, (int)mSize.y);
                                }
                            }

                            if (map.mapResolution == NJGMap.Resolution.Double)
                            {

                                Bounds bounds = map.bounds;
                                //bounds.Expand(new Vector3(-bounds.extents.x, 0f, -bounds.extents.z));
                                for (int i = 0; i < 4; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:

                                            //cam.Render();
                                            map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, 0, map.generateMipmaps);
                                            cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z - bounds.extents.z);
                                            //yield return new WaitForEndOfFrame();
                                            //map.mapTexture.Apply(map.generateMipmaps, map.optimize);
                                            break;

                                        case 1:
                                            cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z - bounds.extents.z);
                                            //cam.Render();
                                            map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), (int)map.mapSize.x, 0, map.generateMipmaps);
                                            //yield return new WaitForEndOfFrame();
                                            //map.mapTexture.Apply(map.generateMipmaps, map.optimize);
                                            break;

                                        case 2:
                                            cachedTransform.position = new Vector3(bounds.center.x + bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z + bounds.extents.z);
                                            //cam.Render();
                                            map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), (int)map.mapSize.x, (int)map.mapSize.y, map.generateMipmaps);
                                            //yield return new WaitForEndOfFrame();
                                            //map.mapTexture.Apply(map.generateMipmaps, map.optimize);
                                            break;

                                        case 3:
                                            cachedTransform.position = new Vector3(bounds.center.x - bounds.extents.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z + bounds.extents.z);
                                            //cam.Render();
                                            mReaded = true;
                                            map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, (map.mapSize.y)), 0, (int)map.mapSize.y, map.generateMipmaps);
                                            //map.mapTexture.Apply(map.generateMipmaps, map.optimize);
                                            break;
                                    }
                                    Debug.Log("mapImageIndex " + i + " / map.mapSize " + map.mapSize + " / cachedTransform.position " + cachedTransform.position + " / mReaded " + mReaded);
                                }
                                /*if (mapImageIndex == 0)
                                {
                                    map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, 0, map.generateMipmaps);
                                }
                                else if (mapImageIndex == 1)
                                {
                                    map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), (int)map.mapSize.x, 0, map.generateMipmaps);
                                }
                                else if (mapImageIndex == 2)
                                {
                                    map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), (int)map.mapSize.x, (int)map.mapSize.y, map.generateMipmaps);
                                }
                                else if (mapImageIndex == 3)
                                {
                                    mReaded = true;
                                    map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, (map.mapSize.y)), 0, (int)map.mapSize.y, map.generateMipmaps);
                                }*/

                                //Debug.Log("mapImageIndex " + mapImageIndex + " / map.mapSize " + map.mapSize + " / cachedTransform.position " + cachedTransform.position + " / mReaded " + mReaded);
                            }
                            else
                            {                                
                                mReaded = true;
                                map.mapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, 0, map.generateMipmaps);
                            }
                        }
                        else
                        {
                            if (map.userMapTexture != null)
                                map.userMapTexture.ReadPixels(new Rect(0f, 0f, map.mapSize.x, map.mapSize.y), 0, 0, map.generateMipmaps);
                        }
                    }

                    yield return new WaitForEndOfFrame();

                    if (!mApplied)
                    {
                        mApplied = true;
                        if (map.generateMapTexture)
                        {
                            if (map.optimize)
                            {
                                map.mapTexture.Compress(true);
                                canRender = false;
                            }
                            map.mapTexture.Apply(map.generateMipmaps, map.optimize);
                        }
                        else
                        {
                            /*if (map.transparentTexture)
                            {
                                ClearColor(map.userMapTexture.GetPixels32());
                                map.userMapTexture.SetPixels32(newColors);
                            }*/
                            //if (map.userMapTexture != null && canRender)
                            if (!Application.isPlaying) map.userMapTexture.Apply(map.generateMipmaps, false);//map.optimize
                        }
                    }

                    //yield return new WaitForEndOfFrame();

                    if (canRender && !mGenerated)
                    {
                        if (Application.isPlaying)
                        {
                            mGenerated = true;
                        }

                        if (map.generateMapTexture)
                        {
                            if(Application.isPlaying) NJGMap.SetTexture(map.mapTexture);
                        }
                        else
                        {
                            NJGMap.SetTexture(map.userMapTexture);
                        }
                    }
                    if (cam.enabled && Application.isPlaying) cam.enabled = false;
                }
            }
            lastRender = Time.time + 1f;
        }

        /// <summary>
        /// Redraw the map's texture.
        /// </summary>

        public void Render()
        {
            if (map == null)
            {
                map = NJGMap.instance;
            }

            if (Time.time >= lastRender)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    cam.targetTexture = null;
                    lastRender = Time.time + 1f;
                }
#endif
                mReaded = false;
                mApplied = false;
                mGenerated = false;
                mWarning = false;

                if (!map.optimize) canRender = true;

                if (map.mapSize.x == 0 || map.mapSize.y == 0)
                {
                    map.mapSize = new Vector2(Screen.width, Screen.height);
                }

                if (map.generateMapTexture)
                {
                    if (map.userMapTexture != null)
                    {
                        NJGTools.Destroy(map.userMapTexture);
                        map.userMapTexture = null;
                    }

                    /*if (map.mapTexture != null)
                    {
                        NJGTools.DestroyImmediate(map.mapTexture);
                        map.mapTexture = null;
                    }*/

                    if (map.mapTexture == null)
                    {
                        mSize = map.mapSize;
                        if (map.mapResolution == NJGMap.Resolution.Double) mSize = map.mapSize * 2;
                        map.mapTexture = new Texture2D((int)mSize.x, (int)mSize.y, (TextureFormat)map.textureFormat, map.generateMipmaps);
                        map.mapTexture.name = "_NJGMapTexture";
                        map.mapTexture.filterMode = map.mapFilterMode;
                        map.mapTexture.wrapMode = map.mapWrapMode;
                        lastSize = mSize;
                    }
                }
                else if (!Application.isPlaying)
                {
                    if (map.mapTexture != null)
                    {
                        NJGTools.DestroyImmediate(map.mapTexture);
                        map.mapTexture = null;
                    }

                    //if (map.userMapTexture != null)
                    //{
                    map.userMapTexture = new Texture2D((int)map.mapSize.x, (int)map.mapSize.y, (TextureFormat)map.textureFormat, map.generateMipmaps);
                    map.userMapTexture.name = "_NJGTempTexture";
                    map.userMapTexture.filterMode = map.mapFilterMode;
                    map.userMapTexture.wrapMode = map.mapWrapMode;
                    //}
                }


                ConfigCamera();
                cam.enabled = true;
                if (!Application.isPlaying) cam.Render();
            }
        }
    }
}