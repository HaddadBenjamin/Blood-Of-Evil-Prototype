using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class QuaternionExtension
    {
        /// <summary>
        /// Calcul le vecteur directionnel d'un quaternion.
        /// </summary>
        public static Vector3 GetDirection(this Quaternion quaternion)
        {
            return quaternion * Vector3.forward;
        }
    }
}   