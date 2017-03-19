using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities.UI
{
    using Extensions;

    /// <summary>
    /// Permet de faire scroller une image à la fois à la vertical qu'à l'horizontal.
    /// Pour faire fonctionner ce script, il faut que :
    /// 1) Avoir un objet parent avec un mask et avec un point de pivot en 0.5f et 0.5f.
    /// 2) Lui assigner 3 images enfantes avec un l'encrage du millieu en rouge et un point de pivot en 0.5f, 0.5f.
    /// 3) Appeler la méthode scroll avec une vélocité.
    /// </summary>
    public class ScrollingImage : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est le rectTransform de l'image tout en haut ou tout à gauche.")]
        private RectTransform topOrLeftImageRectTransform;
        [SerializeField, Tooltip("C'est l'image au millieu des deux autres.")]
        private RectTransform middleImageRectTransform;
        //[SerializeField, Tooltip("C'est l'image tout en bas ou tout à droite.")]
        //private RectTransform bottomOrRightImageRectTransform;

        [SerializeField, Tooltip("C'est la direction du scrolling.")]
        private EMoveDirection scrollDirection;
        [SerializeField, Tooltip("C'est la vitesse de scroll.")]
        private float scrollSpeed = 1.0f;
        [SerializeField, Tooltip("C'est la distance supplémentaire à la hauteur et la largeur entre chaque image.")]
        private float additionalDistanceToWidthAndHeightBetweenEachImages;

        /// <summary>
        /// C'est la largeur d'une image.
        /// </summary>
        private float rectTransformWidth;
        /// <summary>
        /// C'est la hauteur d'une image.
        /// </summary>
        private float rectTransformHeight;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            this.rectTransformWidth = this.middleImageRectTransform.GetWidth();
            this.rectTransformHeight = this.middleImageRectTransform.GetHeight();

            this.SetRectTransformPositionToItsInitialPosition();
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Fait défiler vos images et adapte leur positions.
        /// </summary>
        public void Scroll(float scrollOffset)
        {
            this.MoveImageRectTransforms(scrollOffset);

            this.ReplaceRectTransformPosition(this.topOrLeftImageRectTransform);
            this.ReplaceRectTransformPosition(this.middleImageRectTransform);
            //this.ReplaceRectTransformPosition(this.bottomOrRightImageRectTransform);
        }


        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Place les images à leur position initial en prenant compte de la distance supplémentaire entre chaque image.
        /// </summary>
        private void SetRectTransformPositionToItsInitialPosition()
        {
            if (EMoveDirection.LeftToRight == this.scrollDirection ||
                EMoveDirection.RightToLeft == this.scrollDirection)
            {
                this.topOrLeftImageRectTransform.localPosition = new Vector3(-this.rectTransformWidth - this.additionalDistanceToWidthAndHeightBetweenEachImages, 0.0f, 0.0f);
                this.middleImageRectTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else
            {
                this.topOrLeftImageRectTransform.localPosition = new Vector3(0.0f, -this.rectTransformHeight - this.additionalDistanceToWidthAndHeightBetweenEachImages, 0.0f);
                this.middleImageRectTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        /// <summary>
        /// Replace la rect transform si il est au delà de ses limitations de scroll.
        /// </summary>
        private void ReplaceRectTransformPosition(RectTransform rectTransformToMove)
        {
            /// Si le rect transform de l'image est trop sur la droite.
            if (rectTransformToMove.localPosition.x > (this.rectTransformWidth + this.additionalDistanceToWidthAndHeightBetweenEachImages))
                rectTransformToMove.SetXLocalPosition(-this.rectTransformWidth - this.additionalDistanceToWidthAndHeightBetweenEachImages);
            /// Si le rect transform de l'image est trop sur la gauche.
            else if (rectTransformToMove.localPosition.x < -this.rectTransformWidth - this.additionalDistanceToWidthAndHeightBetweenEachImages)
                rectTransformToMove.SetXLocalPosition((this.rectTransformWidth + this.additionalDistanceToWidthAndHeightBetweenEachImages));
            /// Si le rect transform de l'image est trop haute.
            else if (rectTransformToMove.localPosition.y > (this.rectTransformHeight + this.additionalDistanceToWidthAndHeightBetweenEachImages))
                rectTransformToMove.SetYLocalPosition(-this.rectTransformHeight - this.additionalDistanceToWidthAndHeightBetweenEachImages);
            /// Si le rect transform de l'image est trop basse.
            else if (rectTransformToMove.localPosition.y < -this.rectTransformHeight - this.additionalDistanceToWidthAndHeightBetweenEachImages)
                rectTransformToMove.SetYLocalPosition((this.rectTransformHeight + this.additionalDistanceToWidthAndHeightBetweenEachImages));
        }

        /// <summary>
        /// Fait défiler les rect transforms des images.
        /// </summary>
        private void MoveImageRectTransforms(float scrollOffset)
        {
            float velocity = this.scrollSpeed * scrollOffset;

            if (EMoveDirection.BottomToTop == this.scrollDirection)
            {
                this.topOrLeftImageRectTransform.SetYLocalPositionPlusEqual(velocity);
                this.middleImageRectTransform.SetYLocalPositionPlusEqual(velocity);
            }
            else if (EMoveDirection.UpToBottom == this.scrollDirection)
            {

                this.topOrLeftImageRectTransform.SetYLocalPositionLessEqual(velocity);
                this.middleImageRectTransform.SetYLocalPositionLessEqual(velocity);
            }
            else if (EMoveDirection.LeftToRight == this.scrollDirection)
            {
                this.topOrLeftImageRectTransform.SetXLocalPositionPlusEqual(velocity);
                this.middleImageRectTransform.SetXLocalPositionPlusEqual(velocity);
            }
            else
            {
                this.topOrLeftImageRectTransform.SetXLocalPositionLessEqual(velocity);
                this.middleImageRectTransform.SetXLocalPositionLessEqual(velocity);
            }
        }
        #endregion
    }
}
