using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BloodOfEvil.Extensions
{
    public static class TransformExtension
    {
        /// <summary>
        /// Permet de déplacet et de tourner un objet à la position et rotation d'un objet point de collision.
        /// </summary>
        public static void GetTranslationAndRotationOfTheCollisionPoint(
            this Transform transform, 
            RaycastHit raycastHit,
            float distanceToCollisionPoint = 0.005f)
        {
            // Je déplace mon objet sur le point de collision.
            transform.position = raycastHit.point + distanceToCollisionPoint * raycastHit.normal;

            // Je tourne mon l'objet en fonction de la direction de la normale de collision.
            Vector3 lookAt = Vector3.Cross(-raycastHit.normal, transform.up);
            if (lookAt.y > 0.0f)
                lookAt = -lookAt;

            transform.rotation = Quaternion.LookRotation(lookAt, raycastHit.normal);
        }
        
        /// <summary>
        /// Appelle une méthode avec en paramètre pour tous les enfants d'un transform.
        /// </summary>
        public static void ForeachOnChildren(this Transform transform, Action<Transform> foreachMethod)
        {
            Transform[] childrens = transform.GetChildrens();

            foreach (Transform child in childrens)
                foreachMethod(child);
        }

        #region Local Position
        /// <summary>
        /// Modifie la position de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalPosition(this Transform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalPosition(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalPosition(this Transform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalPositionPlusEqual(this Transform transform, float x)
        {
            transform.localPosition += new Vector3(x, 0.0f, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalPositionPlusEqual(this Transform transform, float y)
        {
            transform.localPosition += new Vector3(0.0f, y, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalPositionPlusEqual(this Transform transform, float z)
        {
            transform.localPosition += new Vector3(0.0f, 0.0f, z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalPositionLessEqual(this Transform transform, float x)
        {
            transform.localPosition -= new Vector3(x, 0.0f, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalPositionLessEqual(this Transform transform, float y)
        {
            transform.localPosition -= new Vector3(0.0f, y, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalPositionLessEqual(this Transform transform, float z)
        {
            transform.localPosition -= new Vector3(0.0f, 0.0f, z);
        }
        #endregion

        #region Local Scale
        /// <summary>
        /// Modifie l'échelle de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalScale(this Transform transform, float x)
        {
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        }

        /// <summary>
        /// Modifie l'échelle de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalScale(this Transform transform, float y)
        {
            transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        }

        /// <summary>
        /// Modifie l'échelle de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalScale(this Transform transform, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
        }
        #endregion

        /// <summary>
        /// Converti la position local d'un objet (positionAsLocalSpace) en position du monde.
        /// </summary>
        public static Vector3 LocalSpacePositionToWorldPosition(this Transform transform, Vector3 positionAsLocalSpace)
        {
            return transform.TransformPoint(positionAsLocalSpace);
        }

        /// <summary>
        /// Place le transform à la position du sol.
        /// </summary>
        public static void SetPositionAtGroundPosition(
            this Transform transform,
            Vector3 groundPosition,
            Camera cameraUsed,
            float distanceFromGround = 1.0f)
        {
            Vector3 screenPoint = cameraUsed.WorldToScreenPoint(groundPosition);
            Ray ray = cameraUsed.ScreenPointToRay(screenPoint);

            transform.position = ray.GetPoint(Vector3.Distance(cameraUsed.transform.position, groundPosition) - distanceFromGround);
        }

        /// <summary>
        /// Renvoit une position autour d'un transform d'une distance maximal de "DistanceAround".
        /// </summary> 
        public static Vector3 GetRandomLocationAroundPosition(this Transform transform, float distanceAround)
        {
            Vector3 position = transform.position;

            return new Vector3(
                position.x + UnityEngine.Random.Range(-distanceAround, distanceAround),
                position.y + UnityEngine.Random.Range(-distanceAround, distanceAround),
                position.z + UnityEngine.Random.Range(-distanceAround, distanceAround)
                );
        }

        /// <summary>
        /// Permet d'obtenir le vecteur directionnel partant d'une position vers une cible.
        /// </summary>
        public static Vector3 GetDirectionToTargetPosition(this Transform tranform, Transform target)
        {
            return (target.position - tranform.position).normalized;
        }

        /// <summary>
        /// Détermine si un objet se situe en face d'un autre.
        /// </summary>
        public static bool IsFrontToTarget(this Transform tranform, Transform target)
        {
            return Vector3.Dot(tranform.GetDirectionToTargetPosition(target), tranform.forward) > 0.0f;
        }

        /// <summary>
        /// Détruit tous les enfants d'un transform.
        /// </summary>
        public static void DestroyAllChilds(this Transform transform)
        {
            foreach (Transform child in transform)
                GameObject.Destroy(child.gameObject);
        }

        /// <summary>
        /// Renvoit le transform le plus proche de "transform" par rapport aux transforms "lookingTransforms".
        /// </summary>
        public static Transform GetNearestTransform(this Transform transform, Transform[] lookingTransforms)
        {
            if (null == transform)
                return null;

            Transform nearestTransform = null;
            float maxDistance = float.MaxValue;

            for (int gameObjectIndex = 0; gameObjectIndex < lookingTransforms.Length; gameObjectIndex++)
            {
                if (null == lookingTransforms[gameObjectIndex])
                    return null;

                float distanceFromGameObject = Vector3.Distance(lookingTransforms[gameObjectIndex].position, transform.position);

                if (distanceFromGameObject < maxDistance)
                {
                    maxDistance = distanceFromGameObject;
                    nearestTransform = lookingTransforms[gameObjectIndex];
                }
            }

            return null == nearestTransform ? null : nearestTransform.transform;
        }

        /// <summary>
        /// Renvoit le transform le plus proche de "transform" par rapport aux transforms "lookingTransforms" si il se situe à "distance" distance de l'objet.
        /// </summary>
        public static Transform GetNearestTransformInDistance(this Transform transform, Transform[] lookingTransforms, float distance)
        {
            //Select().ToArray().
            List<Transform> tranformsInDistance = new List<Transform>();

            foreach (Transform lookingTransform in lookingTransforms)
            {
                if (Vector3.Distance(lookingTransform.position, transform.position) < distance)
                    tranformsInDistance.Add(lookingTransform);
            }

            return transform.GetNearestTransform(tranformsInDistance.ToArray());
        }

        /// <summary>
        /// Renvoit l'objet le plus proche de l'objet de tag "tag" et étant à moindre distance de "distance".
        /// </summary>
        public static Transform GetNearestTransformWithTagAndDistance(this Transform transform, string tag, float distance)
        {
            return transform.GetNearestTransformInDistance(
                Array.ConvertAll(GameObject.FindGameObjectsWithTag(tag), gameObject => gameObject.transform),
                distance);
        }


        /// <summary>
        /// Renvoit l'objet le plus proche de l'objet de tag "tag".
        /// </summary>
        public static Transform GetNearestTransformWithTag(this Transform transform, string tag)
        {
            return transform.GetNearestTransform(
                Array.ConvertAll(GameObject.FindGameObjectsWithTag(tag), gameobject => gameobject.transform));
        }


        /// <summary>
        /// Renvoit lous les transforms de "lookingTransform" et ayant une distance à "transform" de moins de "distance".
        /// </summary>
        public static Transform[] GetAllTransformInDistance(this Transform transform, Transform[] lookingTransforms, float distance)
        {
            List<Transform> tranformsInDistance = new List<Transform>();

            float maxDistance = float.MaxValue;

            for (int gameObjectIndex = 0; gameObjectIndex < lookingTransforms.Length; gameObjectIndex++)
            {
                float distanceFromGameObject = Vector3.Distance(lookingTransforms[gameObjectIndex].position, transform.position);

                if (distanceFromGameObject < maxDistance)
                {
                    maxDistance = distanceFromGameObject;
                    tranformsInDistance.Add(lookingTransforms[gameObjectIndex]);
                }
            }

            return 0 == tranformsInDistance.Count ?
                null :
                tranformsInDistance.ToArray();
        }

        /// <summary>
        /// Renvoit lous les transforms de "lookingTransform" et ayant une distance à "transform" de moins de "distance" et de tag "tag".
        /// </summary>
        public static Transform[] GetAllTransformWithTagAndDistance(this Transform transform, string tag, float distance)
        {
            return transform.GetAllTransformInDistance(
                Array.ConvertAll(GameObject.FindGameObjectsWithTag(tag),
                                    gameobject => gameobject.transform),
                                    distance);
        }

        /// <summary>
        /// Remet la rotation et la position en 0 et la taille à 1.
        /// </summary>
        public static void ResetTransform(this Transform transform)
        {
            transform.position = UnityEngine.Camera.main.transform.position;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Positionne un objet enfant de "parent" et définit sa position et sa rotation de "position" et "rotation".
        /// </summary>
        public static void SetPositionAndRotationAndParent(this Transform transform, Vector3 position, Vector3 rotation, Transform parent)
        {
            transform.SetParent(parent);

            transform.localPosition = position;
            transform.localEulerAngles = rotation;
        }

        /// <summary>
        /// Positionne un objet enfant de "parent" et définit sa position de "position".
        /// </summary>
        public static void SetPositionAndParent(this Transform transform, Vector3 position, Transform parent)
        {
            transform.SetParent(parent);

            transform.localPosition = position;
        }

        #region Position
        /// <summary>
        /// Modifie la composante X de la position de l'objet.
        /// </summary>
        public static void SetPositionX(this Transform transform, float positionX)
        {
            transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Modifie la composante Y de la position de l'objet.
        /// </summary>
        public static void SetPositionY(this Transform transform, float positionY)
        {
            transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
        }

        /// <summary>
        /// Modifie la composante Z de la position de l'objet.
        /// </summary>
        public static void SetPositionZ(this Transform transform, float positionZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, positionZ);
        }

        /// <summary>
        /// Modifie la composante X de la position de l'objet par rapport à son parent.
        /// </summary>
        public static void SetLocalPositionX(this Transform transform, float positionX)
        {
            transform.localPosition = new Vector3(positionX, transform.localPosition.y, transform.localPosition.z);
        }

        /// <summary>
        /// Modifie la composante Y de la position de l'objet par rapport à son parent.
        /// </summary>
        public static void SetLocalPositionY(this Transform transform, float positionY)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, positionY, transform.localPosition.z);
        }

        /// <summary>
        /// Modifie la composante Z de la position de l'objet par rapport à son parent.
        /// </summary>
        public static void SetLocalPositionZ(this Transform transform, float positionZ)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, positionZ);
        }
        #endregion

        #region Rotation
        /// <summary>
        /// Modifie la composante X de la rotation de l'objet.
        /// </summary>
        public static void SetRotationX(this Transform transform, float rotationX)
        {
            transform.eulerAngles = new Vector3(rotationX, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Modifie la composante Y de la rotation de l'objet.
        /// </summary>
        public static void SetRotationY(this Transform transform, float rotationY)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotationY, transform.eulerAngles.z);
        }

        /// <summary>
        /// Modifie la composante Z de la rotation de l'objet.
        /// </summary>
        public static void SetRotationZ(this Transform transform, float rotationZ)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotationZ);
        }

        /// <summary>
        /// Modifie la composante X de la rotation de l'objet par rapport à son parent.
        /// </summary>
        public static void SetLocalRotationX(this Transform transform, float rotationX)
        {
            transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Modifie la composante Y de la rotation de l'objet par rapport à son parent.
        /// </summary>
        public static void SetLocalRotationY(this Transform transform, float rotationY)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Modifie la composante Z de la rotation de l'objet par rapport à son parent.
        /// </summary>
        public static void SetLocalRotationZ(this Transform transform, float rotationZ)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, rotationZ);
        }
        #endregion

        #region Scale
        /// <summary>
        /// Modifie la composante X de l'échelle de l'objet.
        /// </summary>
        public static void SetScaleX(this Transform transform, float scaleX)
        {
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }

        /// <summary>
        /// Modifie la composante Y de l'échelle de l'objet.
        /// </summary>
        public static void SetScaleY(this Transform transform, float scaleY)
        {
            transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
        }

        /// <summary>
        /// Modifie la composante Z de l'échelle de l'objet.
        /// </summary>
        public static void SetScaleZ(this Transform transform, float scaleZ)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, scaleZ);
        }
        #endregion

        #region Follow Mouse Position And Rotation
        /// <summary>
        /// Le transform suit la position de la souris et positionne l'objet à "distanceFromMainCamera" de distance par rapport à la caméra.
        /// </summary>
        public static void FolowMousePositionAtDistance(this Transform transform, float distanceFromMainCamera)
        {
            Vector3 position = Input.mousePosition;
            position.z = distanceFromMainCamera;
            transform.position = UnityEngine.Camera.main.ScreenToWorldPoint(position);
        }

        /// <summary>
        /// Le transform suit la rotation de la souris.
        /// </summary>
        public static void FolowMouseRotation(this Transform transform, float speed = float.MaxValue)
        {
            Plane planelane = new Plane(Vector3.up, transform.position);
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitDistance = 0.0f;

            if (planelane.Raycast(ray, out hitDistance))
            {
                Vector3 targetPoint = ray.GetPoint(hitDistance);
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Le transform suit la position et la rotation de la souris.
        /// Super méthode pour refaire le drag & drop de Matthieu en beaucoup plus simple.
        /// </summary>
        public static void FolowMousePosition(
            this Transform transform,
            LayerMask mask,
            Vector3 rotatioEulerAngleOffset,
            bool followRotation = false)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, mask))
            {
                transform.position = hit.point;

                if (followRotation)
                    transform.eulerAngles = hit.collider.transform.eulerAngles + rotatioEulerAngleOffset;
            }
        }
        #endregion

        /// <summary>
        /// Récupère tous les enfants d'un transform.
        /// </summary>
        public static Transform[] GetChildrens(this Transform transform)
        {
            return transform.GetComponentsInChildren<Transform>();
        }
    }   
}
