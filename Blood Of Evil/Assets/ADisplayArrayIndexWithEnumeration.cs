using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// C'est la classe de base d'un élément d'un talbeau qu'hérite la classe qui est affiché dans l'inspecteur.
    /// </summary>
    [System.Serializable]
    public class ADisplayArrayIndexWithEnumeration
    {
        #region Fields
        [HideInInspector]
        public string Name = "y";
        #endregion
    }
}