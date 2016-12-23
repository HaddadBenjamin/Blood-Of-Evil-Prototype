using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.UI
{
    using Cameras;

    public class CameraFacingBillboard : MonoBehaviour
    {
        private UnityEngine.Camera m_Camera;
        private Transform myTransform;

        void Start()
        {
            this.m_Camera = UnityEngine.Camera.main;
            this.myTransform = transform;
        }

        void Update()
        {
            myTransform.LookAt(myTransform.position + m_Camera.transform.rotation * Vector3.forward,
                m_Camera.transform.rotation * Vector3.up);
        }
    }
}