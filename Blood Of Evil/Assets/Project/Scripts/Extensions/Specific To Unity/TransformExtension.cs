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
        /// Le transform suit la position de la souris.
        /// </summary>
        public static void FolowMousePosition(this Transform transform, float speed = float.MaxValue)
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
        /// Récupère tous les enfants d'un transform.
        /// </summary>
        public static Transform[] GetChildrens(this Transform transform)
        {
            return transform.GetComponentsInChildren<Transform>();
        }







        /// <summary>
        /// TOUS Ce qui suit est du code de Manzalab.
        /// </summary>
        public static float cosPI4 = 0.70710678118f; // cos (PI / 4) = sqrt(2) / 2
        public static float cosPI6 = 0.86602540378f; // cos (PI / 6) = sqrt(3) / 2
        public static float cosPI8 = 0.9238796f; // cos (PI / 8) = sqrt(2 + sqrt(2)) / 2
        public static float cos3PI8 = 0.3826834f; // cos (3PI / 8) = sqrt(2 - sqrt(2)) / 2

        [System.Serializable, System.Flags]
        public enum TransformPosition
        {
            None = 0,
            Front = 1,
            Back = 2,
            Left = 4,
            Right = 8,
            FrontLeft = 5,
            FrontRight = 9,
            BackLeft = 6,
            BackRight = 10,
            LeftFront = 5,
            LeftBack = 6,
            RightFront = 9,
            RightBack = 10
        }

        public static List<T> GetChildList<T>(this Transform t) where T : Component
        {
            return t.GetChildList().Select((c) => c.GetComponent<T>()).Where((c) => c != null).ToList();
        }

        public static List<Transform> GetChildList(this Transform t)
        {
            List<Transform> list = new List<Transform>();

            t.ForEach((child) =>
            {
                list.Add(child);
            });

            return list;
        }

        public static void ForEach(this Transform t, System.Action<Transform> action)
        {
            for (int i = 0; i < t.childCount; i++)
                action(t.GetChild(i));
        }

        public static Transform AddEmptyChild(this Transform t)
        {
            GameObject go = new GameObject("child");

            go.transform.parent = t;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            return go.transform;
        }

        public static Vector2 GetTargetTransformPositionTrigo(this Transform me, Transform other)
        {
            Vector3 meToOther = me.position.FromTo(other.position).normalized;
            Vector3 meUp = me.up;
            float dirRight = me.forward.AngleDirReel(meToOther, meUp);
            float dirBack = me.right.AngleDirReel(meToOther, meUp);

            return new Vector2(dirRight, dirBack);
        }

        public static TransformPosition GetTargetTransformPosition(this Transform me, Transform other)
        {
            Vector2 pos = me.GetTargetTransformPositionTrigo(other);
            float dirRight = pos.x;
            float dirBack = pos.y;
            bool isRight;
            bool isBack;

            isRight = dirRight >= 0;
            isBack = dirBack >= 0;
            //Debug.Log("dirRight: " + dirRight);
            //Debug.Log("dirBack: " + dirBack);

            if (isBack)
            {
                if (isRight)
                {
                    if (dirRight < cos3PI8)
                        return TransformPosition.Back;
                    else if (dirRight > cosPI8)
                        return TransformPosition.Right;
                    else
                        return TransformPosition.BackRight;
                }
                else
                {
                    if (dirRight > -cos3PI8)
                        return TransformPosition.Back;
                    else if (dirRight < -cosPI8)
                        return TransformPosition.Left;
                    else
                        return TransformPosition.BackLeft;
                }
            }
            else
            {
                if (isRight)
                {
                    if (dirRight < cos3PI8)
                        return TransformPosition.Front;
                    else if (dirRight > cosPI8)
                        return TransformPosition.Right;
                    else
                        return TransformPosition.FrontRight;
                }
                else
                {
                    if (dirRight > -cos3PI8)
                        return TransformPosition.Front;
                    else if (dirRight < -cosPI8)
                        return TransformPosition.Left;
                    else
                        return TransformPosition.FrontLeft;
                }
            }
        }

        public static Vector3 GetWorldScale(this Transform t)
        {
            if (t == null)
                return Vector3.one;
            Vector3 wScale = t.localScale;

            while (t.parent != null)
            {
                t = t.parent;
                wScale = wScale.Multiply(t.localScale);
            }
            return wScale;
        }

        public static GameObject AddUIChild(this Transform t, string name)
        {
            GameObject go = new GameObject(name);

            go.AddComponent<RectTransform>();
            go.transform.SetParent(t, false);
            return go;
        }

        /// <summary>
        /// Sets the layer of a transform and its children
        /// </summary>
        /// <param name="t">Transform to set</param>
        /// <param name="layer">Name of the layer to give</param>
        public static void SetLayerRecursively(this Transform t, string layer)
        {
            SetLayerRecursively(t, LayerMask.NameToLayer(layer));
        }

        /// <summary>
        /// Sets the layer of a transform and its children
        /// </summary>
        /// <param name="t">Transform to set</param>
        /// <param name="layer">Layer to give</param>
        public static void SetLayerRecursively(this Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
                SetLayerRecursively(child, layer);
        }

        /// <summary>
        /// Searches for a transform by name recursively
        /// </summary>
        /// <param name="source">Source object (parent)</param>
        /// <param name="objname">Searched object name</param>
        /// <param name="obj">Result (null if not found)</param>
        /// <param name="isSubNameContained">If true, the name can be contained in the result's name</param>
        public static void FindChildRecursively(this Transform source, string objname, out Transform obj, bool isSubNameContained = false)
        {
            obj = null;
            for (int i = 0; i < source.childCount; i++)
            {
                if ((source.GetChild(i).name == objname) ||
                    (isSubNameContained && (source.GetChild(i).name.Contains(objname))))
                {
                    obj = source.GetChild(i);
                    return;
                }
            }

            for (int i = 0; i < source.childCount; i++)
            {
                FindChildRecursively(source.GetChild(i), objname, out obj, isSubNameContained);
                if (obj != null)
                    return;
            }
        }
    }
}