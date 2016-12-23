using UnityEngine;
using System.Collections;

public sealed class ScreenShake : MonoBehaviour
{
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shakeDecay = .1f;
    public float shakeIntensity = 0;    
    public bool screenShake = false;

    TopDownCamera cam;

    void Start()
    {
        cam = GetComponent<CameraParent>().cam;
    }

    void Update()
    {
        

        if (screenShake)
        {
            Shake();
            screenShake = false;
        }


        if (shakeIntensity > 0)
        {
            transform.position = originPosition + Random.insideUnitSphere * shakeIntensity;
            shakeIntensity -= shakeDecay;
        }


    }

    public void Shake()
    {
        originPosition = transform.position;
        shakeIntensity = cam.intensity;      
    }
}