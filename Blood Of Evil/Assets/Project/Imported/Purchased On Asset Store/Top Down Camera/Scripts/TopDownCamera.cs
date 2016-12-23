using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class TopDownCamera : MonoBehaviour
{
    [Header("General")]
    public Transform target;   
    public float height;
    public float depth;
    public float yTargetOffset;
   
    [Header("Rotation Controls")]
    public float yRotOffset;
    public bool enableUserRotation = true;
    public float rotationSpeed = 5;
    public bool hideCursorOnRotate = true;

    [Header("Zoom Controls")]
    public bool zoomEnabled = true;
    public float zoomMin = 3;
    public float zoomMax = 10;
    public float zoomSpeed = 1f;

    [Header("Panning Controls")]
    public bool panningEnabled = true;
    public float panSpeed = 15;
    public bool hideCursorOnPan = true;

    [Header("Visual Obstruction Controls")]
    public bool fadeOutObstructions = true;
    public float fadeSpeed = 3f;
    public float targetFadeAlpha = .1f;

    [Header("Screen Shake Controls")]
    public float intensity = 5;

    //Private variables
    private float zoomIncrement;
    private float targetZoom;  
    private CameraParent controller;

    private List<ObstructionObj> obstructiveObjects;
    private List<GameObject> hitObjects;   
   
    void Start()
    {
        //Init variables
        targetZoom = (zoomMax + zoomMin) / 2;
        controller = new GameObject().AddComponent<CameraParent>();
        controller.target = target;
        controller.cam = this;
        controller.panSpeed = panSpeed;
        controller.yTargetOffset = yTargetOffset;
        transform.parent = controller.transform;
        controller.transform.rotation = Quaternion.Euler(new Vector3(0, yRotOffset, 0));      
        obstructiveObjects = new List<ObstructionObj>();

        DontDestroyOnLoad(controller);
    }

    void LateUpdate()
    {
        //Fade Obstructions
        if(fadeOutObstructions)
            FadeObstructions();

        //Screen Shake Test
        if (Input.GetKeyDown(KeyCode.P))
            ShakeScreen();

        //Update camera position      
        transform.localPosition = new Vector3(0, height, - depth);

        //Update camera rotation
        transform.LookAt(controller.transform);
        
        //Camera Zoom
        if (zoomIncrement != (zoomMax - zoomMin) / 10)
            zoomIncrement = (zoomMax - zoomMin) / 10;

        float d = Input.GetAxis("Mouse ScrollWheel");

        if (d > 0f)
        {
            if (targetZoom > zoomMin)
                targetZoom -= zoomIncrement;
        }
        else if (d < 0f)
        {
            if (targetZoom < zoomMax)
                targetZoom += zoomIncrement;
        }

        if(zoomEnabled)
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetZoom, zoomSpeed * Time.deltaTime);

        //Fade Obstructions
        if (fadeOutObstructions && controller.trackPlayer)
        {
            Ray ray = new Ray(transform.position, controller.transform.position - transform.position);
            float dis = Vector3.Distance(transform.position, controller.transform.position);

            hitObjects = new List<GameObject>();

            foreach (RaycastHit rh in Physics.RaycastAll(ray, dis))
            {
                if (rh.collider.gameObject.transform == target)
                    continue;

                hitObjects.Add(rh.collider.gameObject);
            }

            if (Physics.RaycastAll(ray, dis).Length > 0)
            {

                foreach (RaycastHit r in Physics.RaycastAll(ray, dis))
                {

                    if (r.collider.gameObject.transform == target)
                        continue;

                    if (hitObjects.Contains(r.collider.gameObject))
                        hitObjects.Add(r.collider.gameObject);

                    if (!SearchObstructions(r.collider.gameObject))
                        obstructiveObjects.Add(CreateObstructionObj(r.collider.gameObject));


                }

                if(obstructiveObjects != null)
                foreach (ObstructionObj o in obstructiveObjects)
                {
                    if (hitObjects.Contains(o.obj))
                        o.activateFade = true;

                    else
                        o.activateFade = false;
                }
            }
        }
     
    }

    public void ShakeScreen()
    {
        controller.GetComponent<ScreenShake>().screenShake = true;
    }

    bool SearchObstructions(GameObject go)
    {
        bool present = false;

        foreach (ObstructionObj o in obstructiveObjects)
        {
            if (o.obj == go)
                present = true;
        }

        return present;
    }

    ObstructionObj CreateObstructionObj(GameObject go)
    {
        ObstructionObj obstruction = new ObstructionObj();

        obstruction.obj = go;

        List<Material> mats = new List<Material>();

        foreach (Renderer r in go.transform.GetComponentsInChildren<Renderer>())
        {
            foreach(Material m in r.materials)            
                mats.Add(m);          
        }

        obstruction.materials = CreateObstructionMats(mats);
        obstruction.activateFade = true;
        obstruction.tValue = 1;
      
        return obstruction;
    }

    List<ObstructionMat> CreateObstructionMats(List<Material> mats)
    {
        List<ObstructionMat> obsMats = new List<ObstructionMat>();

        foreach (Material m in mats)
        {
            if (m.HasProperty("_Color"))
            {
                ObstructionMat om = new ObstructionMat();
                om.mat = m;

                om.initialAlpha = m.color.a;

                obsMats.Add(om);
            }
    }

        return obsMats;
    }

    public void FadeObstructions()
    {
        List<ObstructionObj> objectsToRemove = new List<ObstructionObj>();

        if (obstructiveObjects == null)
            return;

        foreach(ObstructionObj o in obstructiveObjects)
        {
            foreach (ObstructionMat om in o.materials)
            {
                if (om.mat.HasProperty("_Color"))
                {
                    om.mat.color = Color.Lerp(new Color(om.mat.color.r, om.mat.color.g, om.mat.color.b, targetFadeAlpha),
                        new Color(om.mat.color.r, om.mat.color.g, om.mat.color.b, om.initialAlpha), o.tValue);
                }
            }

            if (o.activateFade)
            {                
                if(o.tValue > 0)
                    o.tValue -= Time.deltaTime * fadeSpeed;               
            }
            else
            {
                if(o.tValue < 1)
                    o.tValue += Time.deltaTime * fadeSpeed;

                if (o.tValue >= 1)
                    objectsToRemove.Add(o);
            }
        }

        foreach(ObstructionObj o in objectsToRemove)
        {
            obstructiveObjects.Remove(o);
        }
    }
}