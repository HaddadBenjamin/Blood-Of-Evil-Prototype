using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class ExampleScript : MonoBehaviour {

    public Slider heightSlider;
    public Slider depthSlider;

    private TopDownCamera cam;

	// Use this for initialization
	void Start ()
    {
        cam = GetComponent<TopDownCamera>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        cam.height = heightSlider.value;
        cam.depth = depthSlider.value;
	}
}
