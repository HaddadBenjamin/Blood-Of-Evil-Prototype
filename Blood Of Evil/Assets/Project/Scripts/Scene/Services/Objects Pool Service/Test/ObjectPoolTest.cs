using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Scene.Services.ObjectPool
{
    using Player;

    /// <summary>
    /// Nécessite une préfab ayant pour nom et pour tag SpherePrefab.
    /// </summary>
    public sealed class ObjectPoolTest : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
                PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.AddObjectInPool("SpherePrefab");

            if (Input.GetKey(KeyCode.DownArrow))
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag("SpherePrefab");

                if (objects.Length != 0)
                {
                    GameObject randomObjectToDestroy = objects[Random.Range(0, objects.Length)];

                    float timeToDestroy = Random.Range(0, 1.0f);

                    PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.RemoveObjectInPool("SpherePrefab", randomObjectToDestroy, timeToDestroy);
                    // ne doit pas être présente sur la scène pour éviter un crash ici.
                }
            }
        }
    }
}