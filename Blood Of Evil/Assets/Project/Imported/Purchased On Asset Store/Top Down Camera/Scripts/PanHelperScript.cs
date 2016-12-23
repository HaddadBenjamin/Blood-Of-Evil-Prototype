using UnityEngine;
using System.Collections;

public sealed class PanHelperScript : MonoBehaviour {

    private Vector3 dragOrigin;
    private float dis;
    public float dragSpeed = 8;
    public float rotY;
    public Camera cam;
  
    void Start ()
    {
        dragOrigin = Input.mousePosition;
        Vector3 adj = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotY, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Euler(adj);
    }
	
	
	void Update ()
    {

        if (Input.GetMouseButtonDown(2))        
            dragOrigin = Input.mousePosition;
        
        if (Input.GetMouseButton(2))
        {
            Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 mousePos = Input.mousePosition;
            Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
            dis = Vector3.Distance(mousePos, dragOrigin);

            if (dis != 0)            
                transform.Translate(-move, Space.Self);
          
            dragOrigin = Input.mousePosition;

        }

        
    }
}
