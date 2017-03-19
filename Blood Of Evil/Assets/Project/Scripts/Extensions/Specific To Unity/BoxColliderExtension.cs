using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
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
