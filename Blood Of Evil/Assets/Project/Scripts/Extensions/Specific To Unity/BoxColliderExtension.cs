using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
    using Helpers;

    /// <summary>
    /// Ensemble de fonctionnalité permettant de faciliter l'utilisation de la classe UnityEngine.BoxCollider.
    /// </summary>
    public static class BoxColliderExtension
    {
        /// <summary>
        /// Récupére la position globale des coins de votre box.
        /// </summary>
        public static Vector3 GetCornerWorldPosition(this BoxCollider boxCollider, E3DCorner corner)
        {
            Vector3 colliderBoundsExtents = boxCollider.bounds.extents;

            return boxCollider.bounds.center +
                    (E3DCorner.UpRightForward == corner ? new Vector3(colliderBoundsExtents.x, colliderBoundsExtents.y, colliderBoundsExtents.z) :
                     E3DCorner.UpLeftForward == corner ? new Vector3(colliderBoundsExtents.x, colliderBoundsExtents.y, -colliderBoundsExtents.z) :
                     E3DCorner.UpRightBackWard == corner ? new Vector3(colliderBoundsExtents.x, colliderBoundsExtents.y, -colliderBoundsExtents.z) :
                     E3DCorner.UpLeftBackWard == corner ? new Vector3(-colliderBoundsExtents.x, colliderBoundsExtents.y, -colliderBoundsExtents.z) :

                     E3DCorner.BottomRightForward == corner ? new Vector3(colliderBoundsExtents.x, -colliderBoundsExtents.y, colliderBoundsExtents.z) :
                     E3DCorner.BottomLeftBackWard == corner ? new Vector3(-colliderBoundsExtents.x, -colliderBoundsExtents.y, -colliderBoundsExtents.z) :
                     E3DCorner.BottomRightBackWard == corner ? new Vector3(colliderBoundsExtents.x, -colliderBoundsExtents.y, -colliderBoundsExtents.z) :
                     new Vector3(-colliderBoundsExtents.x, -colliderBoundsExtents.y, colliderBoundsExtents.z));
        }

        /// <summary>
        /// Renvoit une position aléatoire de la boîte de collision.
        /// Si l'option "asWorldPositionOrLocalPosition" alors on renvera une position dans le monde 3D autrement on renverra la position local à l'objet.
        /// Si l'option "useOnlyTheHalfColliderPosition" alors on utilisera une position par rapport à la moitié de la taille du collider.
        /// </summary>
        public static Vector3 GetARandomColliderPosition(
            this BoxCollider boxCollider,
            bool asWorldPositionOrLocalPosition = true,
            bool useOnlyTheHalfColliderPosition = false)
        {
            Vector3 colliderBoundsExtents = boxCollider.bounds.extents;

            return
                (asWorldPositionOrLocalPosition ?
                boxCollider.transform.LocalSpacePositionToWorldPosition(boxCollider.center) :
                boxCollider.center) +
                new Vector3(
                    RandomHelper.GetARandomValueBetweenTwoFloats(useOnlyTheHalfColliderPosition ? 0.0f : -colliderBoundsExtents.x, colliderBoundsExtents.x),
                    RandomHelper.GetARandomValueBetweenTwoFloats(useOnlyTheHalfColliderPosition ? 0.0f : -colliderBoundsExtents.y, colliderBoundsExtents.y),
                    RandomHelper.GetARandomValueBetweenTwoFloats(useOnlyTheHalfColliderPosition ? 0.0f : -colliderBoundsExtents.z, colliderBoundsExtents.z));
        }

        /// <summary>
        /// Retourne la largeur de la boîte de collision.
        /// </summary>
        public static float GetWidth(this BoxCollider boxCollider)
        {
            return boxCollider.bounds.extents.x * 2;
        }

        /// <summary>
        /// Retourne la hauteur de la boîte de collision.
        /// </summary>
        public static float GetHeight(this BoxCollider boxCollider)
        {
            return boxCollider.bounds.extents.y * 2;
        }

        /// <summary>
        /// Retourne lalargeur de la boîte de collision.
        /// </summary>
        public static float GetLength(this BoxCollider boxCollider)
        {
            return boxCollider.bounds.extents.z * 2;
        }
    }

    public enum E3DCorner
    {
        UpRightForward,
        UpLeftForward,

        UpRightBackWard,
        UpLeftBackWard,

        BottomRightForward,
        BottomLeftForward,

        BottomRightBackWard,
        BottomLeftBackWard,
    }
}
