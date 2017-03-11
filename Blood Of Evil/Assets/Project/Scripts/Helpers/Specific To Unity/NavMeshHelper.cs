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
            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();

            return (UnityEngine.AI.NavMesh.CalculatePath(startPoint, endPoint, UnityEngine.AI.NavMesh.AllAreas, path));
        }
    }
}