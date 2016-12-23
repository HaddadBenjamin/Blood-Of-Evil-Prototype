using UnityEngine;

public class OrbFill : MonoBehaviour
{
    public float AnimationSpeed = 5.0f;
    [Range(0.0f, 1.0f)]
    public float Fill = 1.0f;

    float startFill;

    Material material;

    void Awake()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;

        startFill = material.GetFloat(OrbVariable.FILL);
    }

    void OnDestroy()
    {
        material.SetFloat(OrbVariable.FILL, startFill);
    }

    void Update()
    {
        float rate = Time.deltaTime * AnimationSpeed;
        material.SetFloat(OrbVariable.FILL, Mathf.Lerp(material.GetFloat(OrbVariable.FILL), Fill, rate));
    }
}
