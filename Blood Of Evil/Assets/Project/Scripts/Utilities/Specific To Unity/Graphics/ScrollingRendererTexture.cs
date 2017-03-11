using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    using Extensions;
    /// <summary>
    /// Permet de scroller la texture d'un gameobject, ne fonctionne que sur les gameObjects et pas sur les images.
    /// </summary>
    public class ScrollingRendererTexture : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("Vitesse de déplacement horizontal de la texture par seconde de la texture du renderer de l'objet.")]
        private float horizontalScrollSpeed = 0.25f;
        [SerializeField, Tooltip("Vitesse de déplacement vertical de la texture par seconde de la texture du renderer de l'objet.")]
        private float verticalScrollSpeed = 0.25f;
        /// <summary>
        /// Détermine si le scrolling est actif ou non.
        /// </summary>
        private bool isScrolling = true;
        /// <summary>
        /// C'est le renderer de l'objet.
        /// </summary>
        private Renderer myRenderer;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            this.myRenderer = GetComponent<Renderer>();
        }

        private void FixedUpdate()
        {
            if (this.isScrolling)
                this.ScrollTexture();
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Scroll la texture.
        /// </summary>
        private void ScrollTexture()
        {
            this.myRenderer.material.mainTextureOffset = new Vector2(
                    this.horizontalScrollSpeed * Time.deltaTime,
                    this.verticalScrollSpeed * Time.deltaTime);
        }
        #endregion
    }
}
