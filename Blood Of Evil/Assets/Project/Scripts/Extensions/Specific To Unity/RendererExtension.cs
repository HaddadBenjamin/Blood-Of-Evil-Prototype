using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
    /// <summary>
    /// Ensemble de fonctionnalité permettant de faciliter l'utilisation de la classe UnityEngine.Renderer.
    /// </summary>
    public static class RendererExtension
    {
        /// <summary>
        /// Permet de scroller la texture d'un objet.
        /// </summary>
        public static void ScrollTexture(
            this Renderer renderer, 
            float horizontalScrollSpeed = 0.01f, 
            float verticalScrollSpeed = 0.01f)
        {
            renderer.material.mainTextureOffset = new Vector2(horizontalScrollSpeed, verticalScrollSpeed);
        }
    }
}
