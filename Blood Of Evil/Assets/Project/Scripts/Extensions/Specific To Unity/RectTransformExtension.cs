using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class RectTransformExtension
    {
        /// <summary>
        /// Replace l'objet en position 0 et remet sa taille à 1.
        /// </summary>
        public static void ResetPositionAndScaleForResponsivity(this RectTransform rectTransform)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// Modifie la largeur d'un widget.
        /// </summary>
        public static void SetWidth(
            this RectTransform rectTransform,
            float width,
            bool plusEqual = false)
        {
            rectTransform.sizeDelta = new Vector2(
                (plusEqual ? rectTransform.sizeDelta.x : 0.0f) + width, 
                rectTransform.sizeDelta.y);
        }

        /// <summary>
        /// Modifie la hauteur d'un widget.
        /// </summary>
        public static void SetHeight(
            this RectTransform rectTransform,
            float height,
            bool plusEqual)
        {
            rectTransform.sizeDelta = new Vector2(
                rectTransform.sizeDelta.x, 
                (plusEqual ? rectTransform.sizeDelta.y : 0.0f) + height);
        }

        /// <summary>
        /// Récupère la largeur d'un widget.
        /// </summary>
        public static float GetWidth(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.x;
        }

        /// <summary>
        /// Récupère la hauteur d'un widget.
        /// </summary>
        public static float GetHeight(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.y;
        }

        /// <summary>
        /// Modifie la position en Y du widget.
        /// </summary>
        public static void SetYPosition(
            this RectTransform rectTransform, 
            float yPosition, 
            bool plusEqual = false)
        {
            rectTransform.anchoredPosition = new Vector2(
                    rectTransform.anchoredPosition.x,
                    (plusEqual ? rectTransform.anchoredPosition.y : 0.0f) + yPosition);
        }

        /// <summary>
        /// Modifie la position en X du widget.
        /// </summary>
        public static void SetXPosition(
            this RectTransform rectTransform,
            float xPosition,
            bool plusEqual = false)
        {
            rectTransform.anchoredPosition = new Vector2(
                (plusEqual ? rectTransform.anchoredPosition.x : 0.0f) + xPosition,
                rectTransform.anchoredPosition.y);
        }

        /// <summary>
        /// Modifie la position locale d'un rectTransform.
        /// </summary>
        public static void SetPosition(this RectTransform rectTransform, Vector3 position)
        {
            rectTransform.localPosition = position;
        }

        /// <summary>
        /// Modifie la rotation locale d'un rectTransform.
        /// </summary>
        public static void SetRotation(this RectTransform rectTransform, Vector3 rotation)
        {
            rectTransform.localRotation = Quaternion.Euler(rotation);
        }

        /// <summary>
        /// Modifie l'échelle d'un rectTransform.
        /// </summary>
        public static void SetScale(this RectTransform rectTransform, Vector3 scale)
        {
            rectTransform.localScale = scale;
        }

        /// <summary>
        /// Renvoie le décallage horizontal du pivot.
        /// </summary>
        public static float GetOffsetPivotX(this RectTransform rectTransform)
        {
            return -(rectTransform.pivot.x * rectTransform.GetWidth());
        }

        /// <summary>
        /// Renvoie le décallage vertical du pivot.
        /// </summary>
        public static float GetOffsetPivotY(this RectTransform rectTransform)
        {
            return -(rectTransform.pivot.y * rectTransform.GetHeight());
        }
        
        #region Local Position
        /// <summary>
        /// Modifie la position de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalPosition(this RectTransform RectTransform, float x)
        {
            RectTransform.localPosition = new Vector3(x, RectTransform.localPosition.y, RectTransform.localPosition.z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalPosition(this RectTransform RectTransform, float y)
        {
            RectTransform.localPosition = new Vector3(RectTransform.localPosition.x, y, RectTransform.localPosition.z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalPosition(this RectTransform RectTransform, float z)
        {
            RectTransform.localPosition = new Vector3(RectTransform.localPosition.x, RectTransform.localPosition.y, z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalPositionPlusEqual(this RectTransform RectTransform, float x)
        {
            RectTransform.localPosition += new Vector3(x, 0.0f, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalPositionPlusEqual(this RectTransform RectTransform, float y)
        {
            RectTransform.localPosition += new Vector3(0.0f, y, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalPositionPlusEqual(this RectTransform RectTransform, float z)
        {
            RectTransform.localPosition += new Vector3(0.0f, 0.0f, z);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalPositionLessEqual(this RectTransform RectTransform, float x)
        {
            RectTransform.localPosition -= new Vector3(x, 0.0f, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalPositionLessEqual(this RectTransform RectTransform, float y)
        {
            RectTransform.localPosition -= new Vector3(0.0f, y, 0.0f);
        }

        /// <summary>
        /// Modifie la position de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalPositionLessEqual(this RectTransform RectTransform, float z)
        {
            RectTransform.localPosition -= new Vector3(0.0f, 0.0f, z);
        }
        #endregion

        #region Local Scale
        /// <summary>
        /// Modifie l'échelle de l'objet sur l'axe x.
        /// </summary>
        public static void SetXLocalScale(this RectTransform RectTransform, float x)
        {
            RectTransform.localScale = new Vector3(x, RectTransform.localScale.y, RectTransform.localScale.z);
        }

        /// <summary>
        /// Modifie l'échelle de l'objet sur l'axe y.
        /// </summary>
        public static void SetYLocalScale(this RectTransform RectTransform, float y)
        {
            RectTransform.localScale = new Vector3(RectTransform.localScale.x, y, RectTransform.localScale.z);
        }

        /// <summary>
        /// Modifie l'échelle de l'objet sur l'axe z.
        /// </summary>
        public static void SetZLocalScale(this RectTransform RectTransform, float z)
        {
            RectTransform.localScale = new Vector3(RectTransform.localScale.x, RectTransform.localScale.y, z);
        }
        #endregion
    }
}
