using UnityEngine;
using System.Collections;

namespace BloodOfEvil.ObjectInScene
{
    public abstract class AInitializableComponent : MonoBehaviour
    {
        public abstract void Initialize();
    }
}   