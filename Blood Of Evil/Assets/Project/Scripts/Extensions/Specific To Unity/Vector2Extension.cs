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

        /////////////////////////////////////////////
        /// Tous le code qui suit vient de Manzalab.
        public static Vector2 Multiply(this Vector2 v, Vector2 v2)
		{
			return new Vector2(v.x * v2.x, v.y * v2.y);
		}

		public static Vector2 Multiply(this Vector2 v, float x, float y)
		{
			return new Vector2(v.x * x, v.y * y);
		}

		public static Vector2 Divide(this Vector2 v, Vector2 v2)
		{
			return new Vector2(v.x / v2.x, v.y / v2.y);
		}

		public static bool GreaterOrEqual(this Vector2 v, Vector2 other)
		{
			return v.x >= other.x && v.y >= other.y;
		}

		public static bool LowerOrEqual(this Vector2 v, Vector2 other)
		{
			return v.x <= other.x && v.y <= other.y;
		}

		public static Vector3 AsVector3(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0);
		}
	}
}
