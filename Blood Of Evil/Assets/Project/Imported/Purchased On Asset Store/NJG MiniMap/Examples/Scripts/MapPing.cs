using UnityEngine;
using System.Collections;
using NJG;
using UnityEngine.EventSystems;

namespace NJG
{
    public class MapPing : MonoBehaviour
    {
        public GameObject mapPing;
        public GameObject worldPing;
        public float duration = 10f;

        void Start()
        {
            //Map.onMapClick += OnMapClick;
            Map.onMapDoubleClick += OnMapClick;
        }

        void OnMapClick(Map map, Vector2 mapPos, Vector3 worldPos)
        {
            //Debug.LogFormat("Ping at map: {0} and world: {1} positions", mapPos, worldPos);
            GameObject go = Instantiate(worldPing, worldPos, Quaternion.identity) as GameObject;
            go.transform.parent = transform;

            GameObject go2 = Instantiate(mapPing, mapPos, Quaternion.identity) as GameObject;
            go2.transform.SetParent(map.iconRoot, false);
            go2.transform.localScale = Vector3.one;
            go2.transform.localPosition = mapPos;

            Destroy(go, duration);
            Destroy(go2, duration);
        }
    }
}

