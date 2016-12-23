using UnityEngine;
using System.Collections;
using NJG;
using UnityEngine.EventSystems;

namespace NJG
{
    public class InstantiateOnClick : MonoBehaviour
    {
        public GameObject prefab;

        void Start()
        {
            Map.onMapClick += OnMapClick;
            Map.onMapDoubleClick += OnMapDoubleClick;
        }

        void OnMapClick(Map map, Vector2 mapPos, Vector3 worldPos)
        {
            Debug.Log("OnMapClick");
            GameObject go = Instantiate(prefab, worldPos, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
        }

        void OnMapDoubleClick(Map map, Vector2 mapPos, Vector3 worldPos)
        {
            Debug.Log("OnMapDoubleClick");
            GameObject go = Instantiate(prefab, worldPos, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
        }
    }
}

