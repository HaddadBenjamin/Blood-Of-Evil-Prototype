using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities
{
    using UnityEngine;
    using System.Collections;
    using System;

    /// <summary>
    /// Gère les singletons de type MonoBehaviour et leurs différentes érreurs potentielles.
    /// </summary>
    public class ASingletonMonoBehaviour<TSingletonType> : MonoBehaviour
        where TSingletonType : ASingletonMonoBehaviour<TSingletonType>
    {
        #region Fields
        private static TSingletonType instance;
        private static bool haveBeenInitialized;
        #endregion

        #region Properties
        public static TSingletonType Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = (TSingletonType)FindObjectOfType(typeof(TSingletonType));

                    if (FindObjectsOfType(typeof(TSingletonType)).Length > 1)
                    {
                        Debug.LogErrorFormat("[ASingletonMonoBehaviour] : le singleton de type {0} éxiste en plusieurs éxemplaires ce qui n'est pas logique.",
                            typeof(TSingletonType).Name);

                        return instance;
                    }

                    else if (instance == null)
                    {
                        Debug.LogWarningFormat("[ASingletonMonoBehaviour] : le singleton de type {0} a une instance de valeur null.",
                            typeof(TSingletonType).Name);

                        return null;
                    }
                    else
                        instance.InitializeSingleton();
                }

                return instance;
            }
        }
        #endregion

        #region Virtual Behaviour
        /// <summary>
        /// La méthode d'initialisation de notre singleton.
        /// </summary>
        public virtual void InitializeSingleton()
        {
            if (haveBeenInitialized)
                Debug.LogErrorFormat("[ASingletonMonoBehaviour] : le singleton de type {0} a une instance déjà initialisée.",
                    typeof(TSingletonType).Name);

            haveBeenInitialized = true;
            
            SingletonManager.OnReinitializeAllSingletons += () => Reinitialize();
        }

        /// <summary>
        /// Désinscrit les évênements du singleton.
        /// </summary>
        public virtual void UnsubcribeEvents() { }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Cette méthode détruit proprement le singleton.
        /// </summary>
        public static void Destroy()
        {
            if (null != instance)
            {
                instance.Reinitialize();
                Destroy(instance.gameObject);
            }
        }

        /// <summary>
        /// Cette méthode est appelé lorsque l'on relance l'application.
        /// Les enfants de cette classe devront l'override et réinitialiser tout leurs évênements (= null).
        /// </summary>
        public void Reinitialize()
        {
            haveBeenInitialized = false;
            this.UnsubcribeEvents();
        }
        #endregion
    }
}
