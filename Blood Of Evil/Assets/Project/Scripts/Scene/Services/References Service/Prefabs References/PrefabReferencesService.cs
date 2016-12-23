using System;
using UnityEngine;

namespace BloodOfEvil.Scene.Services.References
{
    using Helpers;
    using ObjectInScene;

    [System.Serializable]
    public sealed class PrefabReferencesService : AInitializableComponent
    {
        #region Fields
        [SerializeField]
        private GameObject[] references;
        private int[] hashIds;
        #endregion

        #region Initializer
        public override void Initialize()
        {
            ObjectContainerHelper.InitializeHashIds(
                Array.ConvertAll(this.references, reference => reference.name),
                ref this.hashIds);
        }
        #endregion

        #region Behaviour Methods
        public GameObject Get(string refenceName)
        {
            return this.references[ObjectContainerHelper.GetHashCodeIndex(refenceName, this.hashIds)];
        }

        public GameObject Instantiate(string gameobjectName)
        {
            return GameObject.Instantiate(this.Get(gameobjectName)) as GameObject;
        }

        public GameObject Instantiate(string gameobjectName, Vector3 position, Vector3 eulerAngles)
        {
            return GameObject.Instantiate(this.Get(gameobjectName), position, Quaternion.Euler(eulerAngles)) as GameObject;
        }

        public GameObject Instantiate(string gameobjectName, Vector3 position, Vector3 eulerAngler, Transform parent)
        {
            GameObject newObject = this.Instantiate(gameobjectName, parent);

            newObject.transform.localPosition = position;
            newObject.transform.eulerAngles = eulerAngler;

            return newObject;
        }

        public GameObject Instantiate(string gameobjectName, Vector3 position, Vector3 rotation, Vector3 scale, Transform parent)
        {
            GameObject newObject = this.Instantiate(gameobjectName, position, rotation, parent);

            newObject.transform.localScale = scale;

            return newObject;
        }

        public GameObject Instantiate(string gameobjectName, Transform parent, float timeToDestroy = 0.0f)
        {
            GameObject newObject = this.Instantiate(gameobjectName);

            newObject.transform.SetParent(parent);
            newObject.transform.localEulerAngles = newObject.transform.localPosition = Vector3.zero;

            if (timeToDestroy > 0.0f)
                Destroy(newObject, timeToDestroy);

            return newObject;
        }
        #endregion
    }
}