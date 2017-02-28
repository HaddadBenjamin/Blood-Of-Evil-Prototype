using System;
using System.Collections.Generic;
using BloodOfEvil.Extensions;
using UnityEngine;

namespace BloodOfEvil.Scene.Services.ObjectPool
{
    using DontDestroy;
    /// <summary>
    /// Contient une pool de gameobject configurable via l'interface d'Unity.
    /// </summary>
    [System.Serializable]
    public sealed class ObjectsPool
    {
        #region Fields
        /// <summary>
        /// Si la pool est étendable on utilisera ici une liste, parcontre c'est moins rapide qu'un tableau.
        /// </summary>
        private List<GameObject> listPool;

        /// <summary>
        /// Si la pool n'est pas étendable on peut alors utiliser un tableau et profiter d'optimisation intéressantes.
        /// </summary>
        private GameObject[] arrayPool;

        /// <summary>
        /// Correspond à la prefab qui sera dans la pool.
        /// </summary>
        [SerializeField]
        private GameObject prefab;

        /// <summary>
        /// Détermine si le conteneur de la pool est un tableau ou bien une liste.
        /// </summary>
        [SerializeField]
        private EObjecstPoolContainerType containerType;

        /// <summary>
        /// Détermine si la taille de la pool est fixe ou non.
        /// </summary>
        [SerializeField]
        private bool isExtandable;

        /// <summary>
        /// Taille de base de cette pool, dans le cas ou la pool n'est pas extendable ce nombre ne peut pas changer.
        /// </summary>
        [SerializeField]
        private int size;

        /// <summary>
        /// Index de la pool qui est mit à jour dans les méthodes de comportement.
        /// </summary>
        private int poolIndex = 0;

        /// <summary>
        /// Permet de définir que les objets contenu par la pool ne se détruisent pas lorsque l'on charge une scène.
        /// </summary>
        private bool dontDestroyObjectsInPools;
        #endregion

        #region Properties
        public GameObject Prefab
        {
            get { return prefab; }
            private set { prefab = value; }
        }

        public int InitializationSize
        {
            get { return size; }
            private set { size = value; }
        }

        public EObjecstPoolContainerType ContainerType
        {
            get { return containerType; }
            private set { containerType = value; }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Crée les gameobject de la pool dans la bonne pool puis les désactive de sorte à pouvoir les reactiver lorsque l'on aura besoin et donc eviter de faire des new / Destroy par GameObject.Instantiate / Destroy.
        /// </summary>
        public void Initialize(bool dontDestroyObjectsInPools)
        {
            this.dontDestroyObjectsInPools = dontDestroyObjectsInPools;

            if (EObjecstPoolContainerType.Array == this.ContainerType)
            {
                this.arrayPool = new GameObject[this.InitializationSize];

                for (int objectToInstantiatedIndex = 0; objectToInstantiatedIndex < this.InitializationSize; objectToInstantiatedIndex++)
                {
                    this.arrayPool[objectToInstantiatedIndex] = GameObject.Instantiate(this.Prefab);

                    if (this.dontDestroyObjectsInPools)
                        DontDestroyOnLoadService.DontDestroyObject(this.arrayPool[objectToInstantiatedIndex]);

                    this.arrayPool[objectToInstantiatedIndex].SetActive(false);
                }
            }
            else
            {
                this.listPool = new List<GameObject>();

                for (int objectToInstantiatedIndex = 0; objectToInstantiatedIndex < this.InitializationSize; objectToInstantiatedIndex++)
                {
                    GameObject newGameObject = GameObject.Instantiate(this.Prefab);

                    if (this.dontDestroyObjectsInPools)
                        DontDestroyOnLoadService.DontDestroyObject(this.arrayPool[objectToInstantiatedIndex]);

                    newGameObject.SetActive(false);

                    this.listPool.Add(newGameObject);
                }
            }
        }
        #endregion

        #region Behaviour Methods
        /// <summary>
        /// Récupère l'objet à index donné puis le désactive.
        /// </summary>
        /// <param name="gameObject"></param>
        public void RemoveObjectInPool(GameObject gameObject)
        {
            this.SetAndGetPoolIndexAtGameObjectIndex(gameObject);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Rajoute un objet dans la pool en fonction de plusieurs critères détaillé plus bas.
        /// </summary>
        /// <returns></returns>
        public GameObject AddObjectInPool()
        {
            if (this.DoesAllObjectsInPoolAreActive())
            {
                if (this.isExtandable)
                {
                    #region Add Object In Pool When All Objects Are Actives And The Pool Is Extandable
                    GameObject newGameObject = GameObject.Instantiate(this.prefab);

                    if (EObjecstPoolContainerType.Array == this.ContainerType)
                    {
                        Array.Resize(ref this.arrayPool, ++this.size);
                        this.arrayPool[arrayPool.Length - 1] = newGameObject; // Ajoute à la fin.

                        this.UpdatePoolIndex();
                        newGameObject.SetActive(true);

                        if (this.dontDestroyObjectsInPools)
                            DontDestroyOnLoadService.DontDestroyObject(newGameObject);

                        return newGameObject;
                    }
                    else
                    {
                        this.listPool.Add(newGameObject);

                        ++this.size;
                        this.UpdatePoolIndex();
                        newGameObject.SetActive(true);

                        if (this.dontDestroyObjectsInPools)
                            DontDestroyOnLoadService.DontDestroyObject(newGameObject);

                        return newGameObject;
                    }
                    #endregion
                }
                else
                {
                    #region Add Object In Pool When All Objects Are Actives And The Pool Is Not Extandable
                    GameObject objectAtPoolIndex = this.GetGameObjectAtPoolIndex();

                    if (null == objectAtPoolIndex) // l'index récupéré correspond au dernière élément de la pool. : objectpool[initialsize]
                    {
                        this.ResetPoolIndex();
                        objectAtPoolIndex = this.GetGameObjectAtPoolIndex();
                    }

                    this.UpdatePoolIndex();
                    objectAtPoolIndex.SetActive(true);

                    return objectAtPoolIndex;
                    #endregion
                }
            }

            GameObject objectAtThePoolIndex = this.GetGameObjectAtPoolIndex();

            #region Add Object In Pool When There Unactive Objects In Pool.
            if (objectAtThePoolIndex != null &&
                !objectAtThePoolIndex.activeSelf)
            {
                objectAtThePoolIndex.SetActive(true);

                this.UpdatePoolIndex();

                return objectAtThePoolIndex;
            }
            else
            {
                GameObject firstDisableGameObject = this.GetTheFirstDisableGameObject();

                this.UpdatePoolIndex();
                firstDisableGameObject.SetActive(true);

                return firstDisableGameObject;
            }
            #endregion


            // If all objects are active
            //  if isExtandable
            //      addInRightContainer : array = reallocate + add + augment size, list =add & addsize
            //  else
            //      return poolIndex, update poolIndex
            // else if (objectpool[objectIndex].isNotActive)
            //  objectpool[objectIndex].setactive(true), updatepoolindex(), return objectpool[objectIndex];
            // else
            //  gameobject firstDisalbeObject = FindFirstDisableObject(); updatepoolindex(); //return firstdisableobect
        }

        /// <summary>
        /// Ajoute un objet dans la pool et lui défini le parent "parent".
        /// </summary>
        public GameObject AddObjectInPool(Transform parent)
        {
            GameObject objectToAdd = this.AddObjectInPool(Vector3.zero, Vector3.zero, parent);

            return objectToAdd;
        }

        /// <summary>
        /// Gère la responsivité d'un objet d'une pool.
        /// </summary>
        public GameObject AddResponsiveObjectInPool(Transform parent)
        {
            GameObject objectToAdd = this.AddObjectInPool(Vector3.zero, Vector3.zero, parent);

            objectToAdd.GetComponent<RectTransform>().ResetPositionAndScaleForResponsivity();

            return objectToAdd;
        }


        public GameObject AddObjectInPool(Vector3 position)
        {
            return AddObjectInPool(position, Vector3.zero, null);
        }

        public GameObject AddObjectInPool(Vector3 position, Vector3 eulerAngles)
        {
            return AddObjectInPool(position, eulerAngles, null);
        }

        public GameObject AddObjectInPool(Vector3 localPosition, Vector3 localEulerAngles, Transform parent)
        {
            GameObject objectToAdd = this.AddObjectInPool();

            objectToAdd.transform.SetParent(parent);
            objectToAdd.transform.localPosition = localPosition;
            objectToAdd.transform.localEulerAngles = localEulerAngles;

            return objectToAdd;
        }

        /// <summary>
        /// Détermine si il est possible de mettre l'index de la pool à 0.
        /// </summary>
        /// <returns></returns>
        private bool CanResetThePoolIndex()
        {
            return this.poolIndex >= this.size &&
                    !this.isExtandable;
        }

        /// <summary>
        ///  Incrémente l'index de la pool.
        /// </summary>
        private void IncrementPoolIndex()
        {
            ++this.poolIndex;
        }

        /// <summary>
        /// Récupère l'index de pool d'un gameobject et modifie la valeur de poolIndex.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private int SetAndGetPoolIndexAtGameObjectIndex(GameObject gameObject)
        {
            if (EObjecstPoolContainerType.Array == this.ContainerType)
            {
                for (int gameObjectIndex = 0; gameObjectIndex < this.arrayPool.Length; gameObjectIndex++)
                {
                    if (this.arrayPool[gameObjectIndex] == gameObject)
                    {
                        this.poolIndex = gameObjectIndex;
                        break;
                    }
                }
            }
            else
            {
                for (int gameObjectIndex = 0; gameObjectIndex < this.listPool.Count; gameObjectIndex++)
                {
                    if (this.listPool[gameObjectIndex] == gameObject)
                    {
                        this.poolIndex = gameObjectIndex;
                        break;
                    }
                }
            }

            this.ResetPoolIndexIfPossible();

            return this.poolIndex;
        }

        /// <summary>
        ///  Incrèmente l'index de la pool puis la remet à 0 si elle est égal ou supérieur à la taille de la pool et que la pool n'est pas étendable.
        /// </summary>
        private void UpdatePoolIndex()
        {
            this.IncrementPoolIndex();

            this.ResetPoolIndexIfPossible();
        }

        /// <summary>
        /// Remet la taille de la pool à 0 si cela est pertinent et possible.
        /// </summary>
        private void ResetPoolIndexIfPossible()
        {
            if (this.CanResetThePoolIndex())
                this.poolIndex = 0;
        }

        /// <summary>
        /// Remet la taille de la pool à 0.
        /// </summary>
        private void ResetPoolIndex()
        {
            this.poolIndex = 0;
        }

        /// <summary>
        /// Récupère l'objet à l'index pool index.
        /// </summary>
        /// <returns></returns>
        private GameObject GetGameObjectAtPoolIndex()
        {
            return this.poolIndex >= this.size ?
                    null :
                        EObjecstPoolContainerType.Array == this.ContainerType ?
                        this.arrayPool[this.poolIndex] :
                        this.listPool[this.poolIndex];
        }

        /// <summary>
        /// Fait une recherche sur la pool et renvoi le première objet étant désactivé.
        /// </summary>
        /// <returns></returns>
        private GameObject GetTheFirstDisableGameObject()
        {
            return (EObjecstPoolContainerType.Array == this.ContainerType) ?
                    Array.Find(this.arrayPool, gameObject =>
                    {
                        if (null == gameObject)
                        {
                            Debug.LogFormat("Object null name : {0} : IT IS POSSIBLE PARENT IS DESTROYED", prefab);
                            return false;
                        }
                        else
                            return !gameObject.activeSelf;

                    }) :
                    this.listPool.Find(gameObject => !gameObject.activeSelf);
        }

        /// <summary>
        /// Détermine si tous les objets dans la pool sont activés.
        /// </summary>
        private bool DoesAllObjectsInPoolAreActive()
        {
            return null == this.GetTheFirstDisableGameObject();
        }

        /// <summary>
        /// Désactiver tous les objects de la pool.
        /// </summary>
        public void RemoveAllObjectInPool()
        {
            if (EObjecstPoolContainerType.Array == this.ContainerType)
                Array.ForEach(this.arrayPool, gameObject => gameObject.SetActive(false));
            else
                this.listPool.ForEach(gameObject => gameObject.SetActive(false));
        }

        /// <summary>
        /// Renvoi le nombre d'objets de la pool.
        /// </summary>
        /// <returns></returns>
        public int GetTheNumberOfObjectsInPool()
        {
            return (EObjecstPoolContainerType.Array == this.ContainerType) ?
                   this.arrayPool.Length :
                   this.listPool.Count;
        }

        /// <summary>
        /// Récupère les game objects de la pool sous forme de tableau.
        /// </summary>
        /// <returns></returns>
        public GameObject[] GetGameobjects()
        {
            return (EObjecstPoolContainerType.Array == this.ContainerType) ?
                this.arrayPool :
                this.listPool.ToArray();
        }
        #endregion
    }
}