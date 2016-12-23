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

        /////////////////////////////////////////////

        /// Tous le code qui suit vient de Manzalab.
        public static Vector3 AverageWith(this Vector3 v1, Vector3 v2)
        {
            return (v1 + v2) * 0.5f; // retourne la mediane des deux vecteurs
        }

        public static float CustomAngleBetween(this Vector3 vMiddle, Vector3 v1, Vector3 v2)
        {
            return Vector3.Angle(v1 - vMiddle, v2 - vMiddle);
        }

        public static bool EqualWithGap(this Vector3 v1, Vector3 v2, float gap)
        {
            return v1.x.EqualWithGap(v2.x, gap) && v1.y.EqualWithGap(v2.y, gap) && v1.z.EqualWithGap(v2.z, gap);
        }

        public static Vector3 Divide(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }

        public static Vector3 Multiply(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public static Vector3 Multiply(this Vector3 v1, Vector2 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z);
        }

        public static Vector3 Multiply(this Vector3 v1, float x, float y, float z)
        {
            return new Vector3(v1.x * x, v1.y * y, v1.z * z);
        }

        public static Vector3 IgnoreX(this Vector3 v)
        {
            return v.Multiply(new Vector3(0, 1, 1));
        }

        public static Vector3 IgnoreY(this Vector3 v)
        {
            return v.Multiply(new Vector3(1, 0, 1));
        }

        public static Vector3 IgnoreZ(this Vector3 v)
        {
            return v.Multiply(new Vector3(1, 1, 0));
        }

        public static Vector2 AsVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 RoundToInt(this Vector2 v)
        {
            v.x = Mathf.RoundToInt(v.x);
            v.y = Mathf.RoundToInt(v.y);

            return v;
        }

        public static Vector3 RoundToInt(this Vector3 v)
        {
            v.x = Mathf.RoundToInt(v.x);
            v.y = Mathf.RoundToInt(v.y);
            v.z = Mathf.RoundToInt(v.z);

            return v;
        }

        public static float AngleDir(this Vector3 fwd, Vector3 targetDir, Vector3 up) // left < 0 < right
        {
            float dir = fwd.AngleDirReel(targetDir, up);

            if (dir > 0f)
                return 1f;
            else if (dir < 0f)
                return -1f;
            else
                return 0f;
        }

        public static float AngleDirReel(this Vector3 fwd, Vector3 targetDir, Vector3 up) // left < 0 < right
        {
            // cette fonction renvois une valeur négative sur la direction "targetDir" est a gauche de la direction "fwd"
            // et une valeur positive pour la droite
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);

            return dir;
        }

        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2, bool force = false)
        {
            intersection = Vector3.zero;

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //Lines are not coplanar. Take into account rounding errors.
            if ((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f) && !force)
                return false;

            //Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;

            if ((s >= 0.0f) && (s <= 1.0f) || force)
            {
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
                return false;
        }

        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }

            else
            {
                return false;
            }
        }

        public static Vector3 SetX(this Vector3 v, float x)
        {
            v.x = x;

            return v;
        }

        public static Vector3 SetY(this Vector3 v, float y)
        {
            v.y = y;

            return v;
        }

        public static Vector3 SetZ(this Vector3 v, float z)
        {
            v.z = z;

            return v;
        }

        public static Vector3 Snap(this Vector3 v, float snap)
        {
            return new Vector3(v.x.Snap(snap), v.y.Snap(snap), v.z.Snap(snap));
        }

        public static Vector3 Normalize(this Vector3 v)
        {
            v = v.normalized;

            return v;
        }

        public static Vector3 FromTo(this Vector3 from, Vector3 to)
        {
            return to - from;
        }

        public static Vector3 LerpTo(this Vector3 from, Vector3 to, float lerp)
        {
            return Vector3.Lerp(from, to, lerp);
        }

        public static Vector3 LerpComplex(this Vector3 from, Vector3 to, Vector3 lerp)
        {
            return new Vector3(Mathf.Lerp(from.x, to.x, lerp.x), Mathf.Lerp(from.y, to.y, lerp.y), Mathf.Lerp(from.z, to.z, lerp.z));
        }

        public static Vector3 RotateTowardsIgnoreY(this Vector3 from, Vector3 to, float lerp)
        {
            float angle = Vector3.Angle(from.IgnoreY(), to.IgnoreY());

            return Quaternion.Euler(0, lerp * angle * from.AngleDir(to, Vector3.up), 0) * from;
        }

        public static Vector3 WorldToCanvasPosition(this Vector3 v, Camera cam, RectTransform rectCanvas)
        {
            Vector2 viewportPosition = cam.WorldToViewportPoint(v);

            return new Vector3(viewportPosition.x * rectCanvas.sizeDelta.x, viewportPosition.y * rectCanvas.sizeDelta.y, 0);
        }

        public static bool CheckCollisionBetween(this Vector3 p1, Vector3 p2, int layerMask, bool showLog = false)
        {
            Ray ray = new Ray(p1, p2 - p1);

            if (showLog)
            {
                RaycastHit hit;
                bool isCollision = false;

                if (Physics.Raycast(ray, out hit, (p2 - p1).magnitude, layerMask))
                {
                    isCollision = true;
                    //Debug.Log("Vector3.CheckCollisionBetween: " + hit.transform.name + " at: " + hit.point);
                }
                return isCollision;
            }
            return Physics.Raycast(ray, (p2 - p1).magnitude, layerMask);
        }

        public static Ray GetRayForTarget(this Vector3 v, Vector3 target)
        {
            return new Ray(v, target - v);
        }

        public static Ray GetRayFromOrigin(this Vector3 v, Vector3 origin)
        {
            return new Ray(origin, v - origin);
        }
    }
}