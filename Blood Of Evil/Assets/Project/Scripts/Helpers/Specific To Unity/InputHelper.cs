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
    }
}
