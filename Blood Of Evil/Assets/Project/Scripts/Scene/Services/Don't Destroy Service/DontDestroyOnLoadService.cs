using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Scene.Services.DontDestroy
{
    using ObjectInScene;

    public class DontDestroyOnLoadService : AInitializableComponent
    {
        #region Fields
        [SerializeField]
        private GameObject[] gameobjects;
        #endregion

        #region Abstract Behaviour
        public override void Initialize()
        {
            for (int gameObjectIndex = 0; gameObjectIndex < this.gameobjects.Length; gameObjectIndex++)
                DontDestroyOnLoad(this.gameobjects[gameObjectIndex]);
        }
        #endregion

        #region Helper
        public static void DontDestroyObject(GameObject gameObject)
        {
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}