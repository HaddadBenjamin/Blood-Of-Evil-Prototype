using UnityEngine;
using System.Collections;
using NJG;
using UnityEngine.EventSystems;

namespace NJG
{

    public class TeleportOnClick : MonoBehaviour
    {
        public Transform target;
        public bool onDoubleClick;

        Animator anim;
        int flyHash = Animator.StringToHash("Fly");

        void Start()
        {
            Map.onMapClick += OnMapClick;
            Map.onMapDoubleClick += OnMapDoubleClick;
            Map.onTargetChanged += OnTarget;
        }

        void OnTarget(Transform mapTarget)
        {
            target = mapTarget;
            anim = target.GetComponentInChildren<Animator>();
        }

        void OnMapClick(Map map, Vector2 mapPos, Vector3 worldPos)
        {
            if (!onDoubleClick)
            {
                Debug.Log("OnMapClick");
                if (anim != null) StartCoroutine(Teleport(worldPos));
            }
        }

        void OnMapDoubleClick(Map map, Vector2 mapPos, Vector3 worldPos)
        {
            if (onDoubleClick)
            {
                Debug.Log("OnMapDoubleClick");
                if (anim != null) StartCoroutine(Teleport(worldPos));
            }
        }

        IEnumerator Teleport(Vector3 pos)
        {
            anim.SetBool(flyHash, true);
            yield return new WaitForSeconds(0.5f);

            target.position = pos;

            yield return new WaitForSeconds(1f);

            anim.SetBool(flyHash, false);


        }
    }
}
