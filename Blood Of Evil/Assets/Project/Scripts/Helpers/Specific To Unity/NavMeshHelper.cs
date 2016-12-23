using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Helpers
{
    public static class NavMeshHelper
    {
        /// <summary>
        /// Détermine si un point est accessible partant de "startPoint" et allant à "endPoint".
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static bool IsReachable(Vector3 startPoint, Vector3 endPoint)
        {
            NavMeshPath path = new NavMeshPath();

            return (NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path));
        }
    }
}