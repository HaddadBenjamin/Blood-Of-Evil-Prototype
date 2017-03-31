using System.Collections;

namespace BloodOfEvil.Extensions
{
    using UnityEngine;

    public static class Vector3Extension
    {
        /// <summary>
        /// Renvoie l'angle signé entre 2 vecteurs copris en [0;360].
        /// </summary>
        public static float AngleSigned360(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }

        /// <summary>
        /// Calcul l'angle entre 2 vecteurs compris entre [-180 et 180].
        /// Signed signifie la valeur de l'angle de retour (négatif ou positif) alors qu'unsigned signifie que l'angle sera forcèment positif.
        /// </summary>
        /// <returns></returns>
        public static float AngleSigned180(this Vector3 a, Vector3 b)
        {
            return Vector3.Angle(a, b) * GetAngleSign(a, b);
        }

        /// <summary>
        ///  Permet d'obtenir le signe de l'angle entre 2 vecteurs.
        /// </summary>
        /// <returns></returns>
        public static float GetAngleSign(this Vector3 a, Vector3 b)
        {
            return Mathf.Sign(Vector3.Dot(Vector3.back, Vector3.Cross(a, b)));
        }

        /// <summary>
        /// Permet d'obtenir le vecteur directionnel partant d'une position vers une target.
        /// </summary>
        public static Vector3 GetDirectionToTargetPosition(this Vector3 position, Vector3 targetPosition)
        {
            return (targetPosition - position).normalized;
        }

        /// <summary>
        /// Retourne le vecteur directionnel allant du vecteur courant à une cible donné.
        /// </summary>
        public static Vector3 GetDirectionVector(this Vector3 vector, Vector3 target, bool normalizedVector = true)
        {
            return normalizedVector ?
                    (target - vector).normalized :
                    target - vector;
        }

        /// <summary>
        /// Retourne le vecteur directionnel allant du vecteur courant à une cible donné.
        /// </summary>
        public static Vector3 GetDirectionVector(this Vector3 vector, Transform target, bool normalizedVector = true)
        {
            return vector.GetDirectionVector(target.position, normalizedVector);
        }

        /// <summary>
        /// Retourne le vecteur directionnel allant du vecteur courant à une cible donné.
        /// </summary>
        public static Vector3 GetDirectionVector(this Vector3 vector, GameObject target, bool normalizedVector = true)
        {
            return vector.GetDirectionVector(target.transform, normalizedVector);
        }

        /// <summary>
        /// Setter la valeur de x du vecteur.
        /// </summary>
        public static void SetX(this Vector3 vector, float x)
        {
            vector.x = x;
        }

        /// <summary>
        /// Setter la valeur de y du vecteur.
        /// </summary>
        public static void SetY(this Vector3 vector, float y)
        {
            vector.y = y;
        }

        /// <summary>
        /// Setter la valeur de z du vecteur.
        /// </summary>
        public static void SetZ(this Vector3 vector, float z)
        {
            vector.z = z;
        }
    }
}