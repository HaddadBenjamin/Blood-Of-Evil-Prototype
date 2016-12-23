using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Gère les singletons qui ne sont pas des MonoBehaviour.
    /// </summary>
    public class ASingleton<TSingletonType> where TSingletonType : class, new() 
    {
        #region Fields
        private static TSingletonType instance;
        #endregion

        #region Properties
        public static TSingletonType Instance
        {
            get
            {
                if (null == instance)
                    instance = new TSingletonType();

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
    }
}