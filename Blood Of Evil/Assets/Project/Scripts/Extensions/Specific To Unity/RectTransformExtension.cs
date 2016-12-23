using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class RectTransformExtension
    {
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
        /// Modifie la largeur d'un rectTransform.
        /// </summary>
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }

        /// <summary>
        /// Modifie la hauteur d'un rectTransform.
        /// </summary>
        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }

        /// <summary>
        /// Renvoie la largeur d'un rectTransform.
        /// </summary>
        public static float GetWidth(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.x;
        }

        /// <summary>
        /// Renvoie la hauteur d'un rectTransform.
        /// </summary>
        public static float GetHeight(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.y;
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


        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static RectTransform GetCanvasRectTransform(this RectTransform child)
        {
            Transform t = child.transform;

            while (t.parent != null)
                t = t.parent;
            return t.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static Vector3 GetAnchorMinPosition(this RectTransform rT)
        {
            return rT.parent.GetComponent<RectTransform>().NormalizedToPosition(rT.anchorMin);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static Vector3 GetAnchorMaxPosition(this RectTransform rT)
        {
            return rT.parent.GetComponent<RectTransform>().NormalizedToPosition(rT.anchorMax);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static Vector3 NormalizedToPosition(this RectTransform rT, Vector2 normalizedPosition)
        {
            //Canvas canvas = rT.GetCanvasRectTransform().GetComponent<Canvas>();
            Vector3 ret = (rT.position + new Vector3(rT.rect.xMin, rT.rect.yMin, 0));

            ret.x = ret.x + (rT.rect.width * normalizedPosition.x);
            ret.y = ret.y + (rT.rect.height * normalizedPosition.y);
            return ret;
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static Vector3 PositionToNormalized(this RectTransform rT, Vector3 position)
        {
            //Canvas canvas = rT.GetCanvasRectTransform().GetComponent<Canvas>();
            Vector3 ret = position - (rT.position - (rT.sizeDelta * 0.5f).AsVector3());

            ret.x = ret.x / rT.rect.width;
            ret.y = ret.y / rT.rect.height;
            return ret;
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static Vector2 NormalizedPositionToLocalPosition(this RectTransform rT, Vector2 normalizedPointPosition)
        {
            return rT.sizeDelta.Multiply(normalizedPointPosition - rT.pivot);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static RectTransform GetRT(this GameObject g)
        {
            if (g == null)
                return null;
            if (!(g.transform is RectTransform))
                return null;
            return g.transform as RectTransform;
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static RectTransform GetRT(this Component g)
        {
            if (g == null)
                return null;
            if (!(g.transform is RectTransform))
                return null;

            return g.transform as RectTransform;
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static Canvas GetCanvas(this RectTransform trans)
        {
            Canvas c = null;
            Transform t = trans;

            while (t != null && c == null)
            {
                c = t.GetComponent<Canvas>();
                t = t.parent;
            }

            return c;
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static void FitIntoParent(this RectTransform rT)
        {
            rT.anchorMin = Vector2.zero;
            rT.anchorMax = Vector2.one;
            rT.offsetMin = Vector2.zero;
            rT.offsetMax = Vector2.zero;
        }
    }
}