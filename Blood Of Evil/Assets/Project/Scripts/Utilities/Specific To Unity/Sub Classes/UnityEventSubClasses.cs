using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BloodOfEvil.Utilities
{
    /// Permet de voir notre êvênement unity dans l'inspecteur car les types teplates ne sont pas visible.
    /// Alors que les types prédéfinis le sont.
    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }

    [System.Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }
}
