using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BloodOfEvil.Utilities
{
    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }

    [System.Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }
}
