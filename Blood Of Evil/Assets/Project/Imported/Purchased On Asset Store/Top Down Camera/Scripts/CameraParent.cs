using UnityEngine;
using System.Collections;

public sealed class CameraParent : MonoBehaviour {

    public Transform target;
    public TopDownCamera cam;
    public bool trackPlayer = true;
    private GameObject panHelper;
    public Vector3 lastRot;
    private bool returning = false;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    private Quaternion rotation;
    public float panSpeed;
    public float yTargetOffset;

    public Vector3 speed;
    private Vector3 avgSpeed;
    private bool isDragging = false;
    private Vector3 targetSpeedX;


    void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y + cam.yTargetOffset, target.position.z);
        gameObject.AddComponent<ScreenShake>();
    }

    void LateUpdate ()
    {
        Vector3 pos = new Vector3(target.position.x, target.position.y + cam.yTargetOffset, target.position.z);


        //Update position
        if (trackPlayer)       
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime * Time.deltaTime);


        //User camera rotation
        if (cam.enableUserRotation && trackPlayer)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isDragging = true;

                if (cam.hideCursorOnRotate)
                    Cursor.visible = false;
            }
                

            if (Input.GetMouseButton(1))
            {
                if (isDragging)
                {
                    speed = new Vector3(-Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0F);
                    avgSpeed = Vector3.Lerp(avgSpeed, speed, Time.deltaTime * 5);
                }
               
            }
            else
            {
                if (isDragging)
                {
                    Cursor.visible = true;
                    speed = avgSpeed;
                    isDragging = false;
                }

                float i = Time.deltaTime * 5;
                speed = Vector3.Lerp(speed, Vector3.zero, i);
            }


            transform.Rotate(transform.up * -speed.x * cam.rotationSpeed, Space.World);
        }

        //Camera panning
        if (cam.panningEnabled)
        {
            if (!trackPlayer)
                transform.position = Vector3.Lerp(transform.position, panHelper.transform.position, Time.deltaTime * 8);

            if (Input.GetMouseButtonDown(2))
            {
                if (trackPlayer)
                    trackPlayer = false;

                lastRot = transform.position;

                if(cam.hideCursorOnPan)
                    Cursor.visible = false;

                if (panHelper == null)
                {
                    panHelper = new GameObject();
                    panHelper.transform.position = transform.position;
                    panHelper.AddComponent<PanHelperScript>();

                    PanHelperScript helper = panHelper.GetComponent<PanHelperScript>();
                    helper.cam = cam.GetComponent<Camera>();
                    helper.rotY = transform.rotation.eulerAngles.y;
                    helper.dragSpeed = panSpeed;
                }

            }

            if (Input.GetMouseButtonDown(1))
            {
                trackPlayer = true;
                returning = true;
                Destroy(panHelper);
            }

            if (Input.GetMouseButtonUp(2) && cam.hideCursorOnPan)
                Cursor.visible = true;

            if (returning)
            {
                smoothTime = 30;

                if (Vector3.Distance(transform.position, new Vector3(target.position.x, target.position.y + yTargetOffset, target.position.z)) < .01f)
                    returning = false;
            }

            else
                smoothTime = .3f;
        }
        
    }

   
}
