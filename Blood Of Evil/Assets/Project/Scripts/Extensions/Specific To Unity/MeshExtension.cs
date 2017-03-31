using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
    using Helpers;

    public static class MeshExtension
    {
        /// <summary>
        /// Renvoit la position d'un sommet par rapport à la position du monde ou à la locale de l'objet.
        /// </summary>
        public static Vector3 GetARandomVerticePosition(this Mesh mesh, Transform meshTransform, bool asLocalOrWorldPosition = false)
        {
            Debug.Log(mesh);
            Vector3 aRandomVerticePosition = mesh.vertices[RandomHelper.GetARandomValueBetweenTwoInts(0, mesh.vertexCount - 1)];

            return asLocalOrWorldPosition ?
                    aRandomVerticePosition :
                    meshTransform.TransformPoint(aRandomVerticePosition);
        }
    }
}