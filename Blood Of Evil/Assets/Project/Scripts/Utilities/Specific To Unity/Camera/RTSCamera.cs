using UnityEngine;
using System.Collections;
using BloodOfEvil.Extensions;

namespace BloodOfEvil.Utilities.Cameras
{
    // Code récupérer bien dégueulasse
    public class RTSCamera : MonoBehaviour
    {
        [SerializeField]
        private float ScrollSpeed = 15;
        //[SerializeField]
        //private float ScrollEdge = 0.01f;

        [SerializeField]
        private float PanSpeed = 10;
        [SerializeField]
        private Vector2 ZoomRange = new Vector2(-5, 5);
        private float CurrentZoom = 0;
        private float ZoomZpeed = 1;
        private float ZoomRotation = 1;
        private Vector3 InitPos;
        private Vector3 InitRotation;
        private Transform myTransform;


        void Update()
        {
            this.Pan();
            this.ZoomInZoomOut();
        }

        private void ZoomInZoomOut()
        {
            CurrentZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * ZoomZpeed;
            CurrentZoom = Mathf.Clamp(CurrentZoom, ZoomRange.x, ZoomRange.y);

            this.myTransform.SetPositionY(this.myTransform.position.y - (this.myTransform.position.y - (InitPos.y + CurrentZoom)) * 0.1f);
            this.myTransform.SetRotationX(this.myTransform.eulerAngles.x - (this.myTransform.eulerAngles.x - (InitRotation.x + CurrentZoom * ZoomRotation)) * 0.1f);
        }

        private void Pan()
        {
            if (Input.GetKey("mouse 2"))
            {
                this.myTransform.Translate(Vector3.right * Time.deltaTime * PanSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f), Space.World);
                this.myTransform.Translate(Vector3.forward * Time.deltaTime * PanSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f), Space.World);
            }
            else
            {
                if (Input.GetKey("d") || Input.GetKey("right")/* || Input.mousePosition.x >= Screen.width * (1 - ScrollEdge)*/)
                    this.myTransform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
                else if (Input.GetKey("a") || Input.GetKey("left")/*  || Input.mousePosition.x <= Screen.width * ScrollEdge*/)
                    this.myTransform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed, Space.World);

                if (Input.GetKey("w") || Input.GetKey("up")/*  || Input.mousePosition.y >= Screen.height * (1 - ScrollEdge)*/)
                    this.myTransform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
                else if (Input.GetKey("s") || Input.GetKey("down")/*  || Input.mousePosition.y <= Screen.height * ScrollEdge*/)
                    this.myTransform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed, Space.World);
            }
        }

        private void SetCameraPosition(Vector3 position)
        {
            this.Initialize();

            this.myTransform.position = position;
        }

        private void Initialize()
        {
            this.myTransform = transform;

            InitPos = this.myTransform.position;
            InitRotation = this.myTransform.eulerAngles;
        }
    }
}