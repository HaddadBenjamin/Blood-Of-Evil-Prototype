using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Helpers
{
    /// <summary>
    /// Facilite l'instantiation des objets dans la scène (avec ou sans parent, avec une position local ou non, etc...).
    /// </summary>
    public static class GameObjectHelper
    {
        /// <summary>
        /// Permet d'instancier un objet avec une position, une rotation et un transform parent.
        /// </summary>
        public static GameObject Instantiate(
            GameObject prefabToInstantiate, 
            Vector3 position, 
            Vector3 scale, 
            Transform parent = null,
            bool instantiate = true)
        {
            GameObject objectInstantiated = instantiate ? GameObject.Instantiate(prefabToInstantiate) : prefabToInstantiate;

            objectInstantiated.transform.SetParent(parent);

            objectInstantiated.transform.localPosition = position;
            objectInstantiated.transform.localScale = scale;

            return objectInstantiated;
        }

        /// <summary>
        /// Permet d'instancier un objet à la locale position Vector3.zero et à locale scale Vector.one;
        /// </summary>
        public static GameObject Instantiate(
            GameObject prefabToInstantiate,
            Transform parent = null,
            bool instantiate = true)
        {
            GameObject objectInstantiated = instantiate ? GameObject.Instantiate(prefabToInstantiate) : prefabToInstantiate;

            objectInstantiated.transform.SetParent(parent);

            objectInstantiated.transform.localPosition = Vector3.zero;
            objectInstantiated.transform.localScale = Vector3.one;

            return objectInstantiated;
        }
    }
}
