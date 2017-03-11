using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Gère les singletons qui ne sont pas des MonoBehaviour.
    /// </summary>
    public class ASingleton<TSingletonType> 
        where TSingletonType : ASingleton<TSingletonType>, new()
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
                    instance = new TSingletonType ();
                    instance.Initialize ();
                }

                return instance;
            }
        }
        #endregion

        #region Constructor
        public ASingleton()
        {
            if (null != instance)
                Debug.LogErrorFormat("[ASingleton] : le singleton de type {0} éxiste en plusieurs éxemplaires ce qui n'est pas logique.",
                    typeof(TSingletonType).Name);
        }
        #endregion

        #region Virtual Behaviour
        /// <summary>
        /// La méthode d'initialisation de notre singleton.
        /// </summary>
        public virtual void Initialize()
        {
            if (haveBeenInitialized)
                Debug.LogErrorFormat("[ASingletonMonoBehaviour] : le singleton de type {0} a une instance déjà initialisée.",
                    typeof(TSingletonType).Name);

            haveBeenInitialized = true;
        }
            
        /// <summary>
        /// Cette méthode est appelé lorsque l'on relance l'application.
        /// Les enfants de cette classe devront l'override et réinitialiser tout leurs évênements (= null).
        /// </summary>
        public virtual void Reinitialize()
        {
            haveBeenInitialized = false;
        }
        #endregion
    }
}
