using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Helpers
{
    public static class InputHelper
    {
        /// <summary>
        /// Récupère la position du premier doigt posé sur la tablette ou le téléphone.
        /// </summary>
        public static Vector2 GetFirstFingerPosition()
        {
            return Input.touchCount > 0 ?
                    Input.GetTouch(0).position :
                    new Vector2(0.0f, 0.0f);
        }
        
        // <summary>
        /// Récupère la position du premier doigt sur mobile ou bien du curseur de la souris sur PC.
        /// </summary>
        public static Vector2 GetResponsiveFingerPosition()
        {
            #if UNITY_EDITOR
                return Input.mousePosition;
            #else
                return Input.touches[0].position;
            #endif
        }
    }
}
