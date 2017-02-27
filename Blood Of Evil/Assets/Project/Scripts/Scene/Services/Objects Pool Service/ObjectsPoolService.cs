using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Scene.Services.ObjectPool
{
    using Helpers;
    using ObjectInScene;
    using  Extensions;

    /// <summary>
    /// Permet d'éviter de faire des news et des deletes sur des gameobjects en run-time.
    /// Tous est configurable et cela permet de rendre le programme plus performant.
    /// </summary>
    public sealed class ObjectsPoolService : AInitializableComponent
    {
        #region Fields
        /// <summary>
        /// Toute les pool de gameObject, ces dernières sont configurable à travers l'inspector.
        /// </summary>
        [SerializeField]
        private ObjectsPool[] objectsPools;
        /// <summary>
        /// Permet de récupérer les pool par une comparaison d'entier au lieu d'une comparaison de string : optimisation.
        /// </summary>
        private int[] objectsPoolsHashID;

        [SerializeField]
        private bool dontDestroyObjectsInPools = true;
        #endregion

        #region Initialize
        public override void Initialize()
        {
            ObjectContainerHelper.InitializeHashIds(
                Array.ConvertAll(this.objectsPools, reference => reference.Prefab.name),
                ref this.objectsPoolsHashID);

            Array.ForEach(this.objectsPools, objectPool => objectPool.Initialize(this.dontDestroyObjectsInPools));
        }
        #endregion

        #region Behaviour Methods
        /// <summary>
        /// Permet de récupérer une pool de gameobject en fonction du nom du gameobject que l'on a besoin.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public ObjectsPool GetPool(string poolName)
        {
            int hashID = ObjectContainerHelper.GetHashCodeIndex(poolName, this.objectsPoolsHashID);

            return this.objectsPools[hashID];
        }

        /// <summary>
        /// Desactive un objet de la pool correspondant à poolName.
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="gameObject"></param>
        public void RemoveObjectInPool(string poolName, GameObject gameObject)
        {
            this.GetPool(poolName).RemoveObjectInPool(gameObject);
        }

        /// <summary>
        /// Desactive un objet de la pool correspondant à poolName après timeToWait secondes.
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="gameObject"></param>
        /// <param name="timeToWait"></param>
        public void RemoveObjectInPool(string poolName, GameObject gameObject, float timeToWait)
        {
            object[] parms = new object[3] { poolName, gameObject, timeToWait };

            StartCoroutine("RemoveObjectInPoolAfterNTime", parms);
        }

        /// <summary>
        /// Permet de désactiver un gameobject d'une pool après n temps.
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        private IEnumerator RemoveObjectInPoolAfterNTime(object[] parms)
        {
            yield return new WaitForSeconds((float)parms[2]);

            if (null != (GameObject)parms[1])
                this.GetPool((string)parms[0]).RemoveObjectInPool((GameObject)parms[1]);
        }

        /// <summary>
        /// Permet de désactiver tous les gameobjects d'une pool.
        /// </summary>
        /// <param name="poolName"></param>
        public void RemoveAllObjectInPool(string poolName)
        {
            this.GetPool(poolName).RemoveAllObjectInPool();
        }

        /// <summary>
        /// Rajoute un object dans une pool.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public GameObject AddObjectInPool(string poolName)
        {
            return this.GetPool(poolName).AddObjectInPool();
        }

        /// <summary>
        /// Rajoute un object dans une pool et le positionne à la position objectPosition, avec la rotation objectRotation.
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="objectPosition"></param>
        /// <param name="objectRotation"></param>
        /// <returns></returns>
        public GameObject AddObjectInPool(string poolName, Vector3 objectWorldPosition, Vector3 objectWorldRotation)
        {
            GameObject gameObjectAdded = this.GetPool(poolName).AddObjectInPool();
            Transform gameObjectTransform = gameObjectAdded.transform;

            gameObjectTransform.position = objectWorldPosition;
            gameObjectTransform.rotation = Quaternion.Euler(objectWorldRotation);

            return gameObjectAdded;
        }

        /// <summary>
        /// Rajoute un object dans une pool et lui donne comme parent parentTransform.
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="parentTransform"></param>
        /// <returns></returns>
        public GameObject AddObjectInPool(string poolName, Transform parentTransform)
        {
            GameObject gameObjectAdded = this.AddObjectInPool(poolName);

            gameObjectAdded.transform.SetParent(parentTransform);

            return gameObjectAdded;
        }

        /// <summary>
        /// Rajoute un object dans une pool et le positionne à la position objectPosition, avec la rotation objectRotation et lui donne comme parent parentTransform.
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="objectPosition"></param>
        /// <param name="objectRotation"></param>
        /// <param name="parentTransform"></param>
        /// <returns></returns>
        public GameObject AddObjectInPool(string poolName, Vector3 localPositionToHisParent, Vector3 localRotationToHisParent, Transform parentTransform)
        {
            GameObject gameObjectAdded = this.AddObjectInPool(poolName, localPositionToHisParent, localRotationToHisParent);

            gameObjectAdded.transform.SetParent(parentTransform);

            return gameObjectAdded;
        }

        /// <summary>
        /// Permet de récupérer le nombre d'objet dans la pool.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public int GetTheNumberOfObjectsInPool(string poolName)
        {
            return this.GetPool(poolName).GetTheNumberOfObjectsInPool();
        }
        #endregion
    }
}