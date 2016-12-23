//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using NJG;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Map Coords")]
[RequireComponent(typeof(Text))]
public class MapCoords : MonoBehaviour
{
	public string format = "X:{0},Y:{1}";
    Text label;
    Vector3 mLastPos;
    Transform target;

	void Awake() 
    {
        label = GetComponent<Text>();
        Map.onTargetChanged += OnTargetChanged;
    }

    void OnEnable()
    {
        if (target == null) 
        {
            OnTargetChanged(Map.miniMap != null ? Map.miniMap.target : Map.worldMap.target);
        }
    }

    void OnTargetChanged(Transform target)
    {
        this.target = target;
    }

	void Update()
	{
        //Debug.Log(mLastPos);
        if (target != null/* && mLastPos != target.position*/)
		{
            //mLastPos = target.position;

            int x = (int)target.position.x;
			int y = (int)(NJGMap.instance.orientation == NJGMap.Orientation.XZDefault ? target.position.z : target.position.y);

            label.text = string.Format(format, x, y);
		}
	}
}
