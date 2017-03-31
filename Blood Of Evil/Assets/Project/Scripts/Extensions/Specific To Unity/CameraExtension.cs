using UnityEngine;

namespace BloodOfEvil.Extensions
{
    using UnityEngine;

    public static class CameraExtension
	{
        /// <summary>
        /// Renvoit un rayon allant de la caméra et jusqu'à un objet.
        /// Si l'option "useARandomPositionFromTheCollider" est activé alors, on utilisera un point de la boîte de collision de l'objet seléctionné au hazard.
        /// </summary>
        public static Ray RayFromCameraToGameObject(
            this Camera camera,
            GameObject gameObject,
            bool useARandomPositionFromTheCollider = false)
        {
            return new Ray(
                Camera.main.transform.position,
                Camera.main.transform.position.GetDirectionVector(
                    useARandomPositionFromTheCollider ?
                    gameObject.GetComponent<BoxCollider>().GetARandomColliderPosition(true, true) :
                    gameObject.transform.position));
        }

        /// <summary>
        /// Renvoit un rayon allant de la caméra et jusqu'au transform d'un objet.
        /// Si l'option "useARandomPositionFromTheCollider" est activé alors, on utilisera un point de la boîte de collision de l'objet seléctionné au hazard.
        /// </summary>
        public static Ray RayFromCameraToObjectTransform(
            this Camera camera,
            Transform objectTransform,
            bool useARandomPositionFromTheCollider = false)
        {
            return new Ray(
                Camera.main.transform.position,
                Camera.main.transform.position.GetDirectionVector(
                    useARandomPositionFromTheCollider ?
                    objectTransform.GetComponent<BoxCollider>().GetARandomColliderPosition(true, true) :
                    objectTransform.position));
        }

        /// <summary>
        /// Renvoit un rayon allant de la caméra et jusqu'à une position 3D du monde.
        /// </summary>
        public static Ray RayFromCameraToWorldPoint(
            this Camera camera,
            Vector3 worldPosition)
        {
            return new Ray(
                Camera.main.transform.position,
                Camera.main.transform.position.GetDirectionVector(worldPosition));
        }
    }
}