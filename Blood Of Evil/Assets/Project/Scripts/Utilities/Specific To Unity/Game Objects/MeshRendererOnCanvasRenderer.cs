using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Extensions;

namespace BloodOfEvil.Utilities.UI
{
    [RequireComponent(typeof(RectTransform)),
     RequireComponent(typeof(CanvasRenderer)),
     RequireComponent(typeof(MeshFilter))]
    public class MeshRendererOnCanvasRenderer : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private  Material material;
        #endregion

        #region Unity Behaviour
        // Use this for initialization
        void Start()
        {
            this.AdaptTheScaleToTheWidthAndTheHeightOfRectTransform();

            this.RenderMeshOnCanvasRenderer();
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Modifie l'échelle de l'objet pour qu'il prenne en compte la largeur et la hauteur du rect transform de l'objet.
        /// </summary>
        private void AdaptTheScaleToTheWidthAndTheHeightOfRectTransform()
        {
            UnityEngine.RectTransform rectTransform = GetComponent<RectTransform>();

            rectTransform.localScale = new Vector3(
                rectTransform.GetWidth(),
                rectTransform.GetHeight(),
                rectTransform.localScale.z);
        }

        /// <summary>
        /// Affiche le mesh dans le canvas renderer de l'objet.
        /// </summary>
        private void RenderMeshOnCanvasRenderer()
        {
            Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
            CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] UVs = mesh.uv;
            List<UIVertex> uiVertices = new List<UIVertex>();
            int[] triangles = mesh.triangles;

            for (int triangleIndex = 0; triangleIndex < triangles.Length; ++triangleIndex)
            {
                UIVertex uiVertex = new UIVertex();

                uiVertex.position = vertices[triangles[triangleIndex]];
                uiVertex.uv0 = UVs[triangles[triangleIndex]];
                uiVertex.normal = normals[triangles[triangleIndex]];

                uiVertices.Add(uiVertex);

                if (triangleIndex % 3 == 0)
                    uiVertices.Add(uiVertex);
            }

            canvasRenderer.Clear();

            canvasRenderer.SetMaterial(material, null);
            canvasRenderer.SetVertices(uiVertices);
        }
        #endregion
    }
}