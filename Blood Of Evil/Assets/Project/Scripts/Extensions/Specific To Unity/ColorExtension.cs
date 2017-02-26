using System.Collections;

namespace BloodOfEvil.Extensions
{
    using UnityEngine;

    public static class ColorExtension
    {
        /// <summary>
        /// Convertie une couleur en chaîne héxadécimale, permet principalement de spécifier la couleur d'un rich text dans l'inspecteur.
        /// </summary>
        public static string ColorToHex(Color32 color)
        {
            return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }
        
        /// <summary>
        /// Change la composante rouge d'une couleur.
        /// </summary>
        public static Color R(this Color color, float red)
        {
            color.r = red;

            return color;
        }

        /// <summary>
        /// Change la composante verte d'une couleur.
        /// </summary>
        public static Color G(this Color color, float green)
        {
            color.g = green;

            return color;
        }

        /// <summary>
        /// Change la composante bleu d'une couleur.
        /// </summary>
        public static Color B(this Color color, float blue)
        {
            color.b = blue;

            return color;
        }

        /// <summary>
        /// Change la composante alpha d'une couleur.
        /// </summary>
        public static Color A(this Color color, float alpha)
        {
            color.a = alpha;

            return color;
        }

        /// <summary>
        /// Inverse chaque composantes d'une couleur.
        /// </summary>
        public static Color Inverse(Color color)
        {
            color.r = 1.0f - color.r;
            color.g = 1.0f - color.g;
            color.b = 1.0f - color.b;
            color.a = 1.0f - color.a;

            return color;
        }
    }
}
