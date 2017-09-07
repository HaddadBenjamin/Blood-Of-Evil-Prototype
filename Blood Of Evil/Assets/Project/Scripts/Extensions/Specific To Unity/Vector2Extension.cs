using UnityEngine;

namespace BloodOfEvil.Extensions
{
	public static class Vector2Extension
    {
        /// <summary>
        /// Renvoie l'angle signé entre 2 vecteurs copris en [0;360].
        /// </summary>
        public static float AngleSigned360(Vector2 from, Vector2 to)
        {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }
		
		/// <summary>
		/// Setter la valeur de x du vecteur.
		/// </summary>
		public static void SetX(this Vector2 vector, float x)
		{
			vector.x = x;
		}

		/// <summary>
		/// Setter la valeur de y du vecteur.
		/// </summary>
		public static void SetY(this Vector2 vector, float y)
		{
			vector.y = y;
		}
	}
}
