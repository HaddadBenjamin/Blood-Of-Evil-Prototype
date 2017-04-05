using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Service permettant de faire défiler plusieurs sprites de splash screens avec un effet fade in fade out.
    /// </summary>
    public class SplashScreenMenu : MonoBehaviour
    {
        #region Fields
        [Header("Les images splashs screens : "), SerializeField, Tooltip("Les sprites qui défileront.")]
        private Sprite[] sprites;

        [SerializeField, Tooltip("Le script permettant de gérer la transparence des sprites qui défileront.")]
        private CanvasGroupFade splashScreenCanvasGroupFade;

        [SerializeField, Tooltip("C'est l'image qui contiendra les sprites qui défileront.")]
        private Image splashScreenImage;

        [SerializeField, Tooltip("C'est le temps en seconde que passera chaque image à défiler en fade in fade out.")]
        private float timePerPlashScreenImage = 4.0f;

        /// <summary>
        /// C'est l'index du sprite que l'on affiche actuellement à l'écran.
        /// </summary>
        private int spriteIndex;
        #endregion

        #region Unity Behaviour
        private void Start()
        {
            /// Configure la vitesse de défilement de chaque image.
            this.splashScreenCanvasGroupFade.SetSpeed(2.0f / this.timePerPlashScreenImage);

            /// Charge la première image et lui donne un effet de fade in.
            this.GoToNextImage();

            /// Si l'effet de fade in de l'image est terminé on la lui lance un éffet de fade out.
            this.splashScreenCanvasGroupFade.OnFadeInFinish += () =>
            {
                this.splashScreenCanvasGroupFade.FadeOut();
            };

            /// Si l'effet de fade out de l'image est terminé on charge la prochaine image.
            this.splashScreenCanvasGroupFade.OnFadeOutFinish += () =>
            {
                this.GoToNextImage();
            };
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Permet de charger la prochaine image.
        /// Lorsqu'il n'y a plus d'images, lance la méthode end qui permet de charger la prochaine scène.
        /// </summary>
        private void GoToNextImage()
        {
            if (this.spriteIndex >= this.sprites.Length)
                this.End();
            else
            {
                /// Change d'image, incrémente l'index de l'image, et lance un effet de fade in.
                this.splashScreenImage.sprite = this.sprites[this.spriteIndex];

                this.splashScreenCanvasGroupFade.SetAlpha(0.0f);

                this.splashScreenCanvasGroupFade.FadeIn();

                ++this.spriteIndex;
            }
        }

        /// <summary>
        /// C'est la méthode qui est appelé lorsque le défilement des images de splashs screen est terminé.
        /// Elle charge la prochaine scène, c'est à dire celle d'index 1.
        /// </summary>
        private void End()
        {
            SceneManager.LoadScene(1);
        }
        #endregion
    }
}
