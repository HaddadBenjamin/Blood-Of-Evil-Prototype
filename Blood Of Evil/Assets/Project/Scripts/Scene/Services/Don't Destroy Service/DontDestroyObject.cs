using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Scene.Services.DontDestroy
{
    public class DontDestroyObject : MonoBehaviour
    {
        #region Unity Behaviour
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}