using UnityEngine;
using System.Collections;
using System.Globalization;

namespace BloodOfEvil.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Converti puis renvoie une valeur normalizé [0-1] en pourcentage.
        /// </summary>
        public static string NormalizedValuetoPercentageText(float normalizedValue)
        {
            return string.Format("{0}%", Mathf.CeilToInt(normalizedValue * 100.0f));
        }

        /// <summary>
        /// 5000 -> 5,000
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string PriceToText(int price)
        {
            string text = price.ToString("N", new CultureInfo("en-US"));

            return text.Remove(text.IndexOf('.'));
        }

        /// <summary>
        /// Créer puis renvoie une chaîne formatée d'un TimeSpan.
        /// </summary>
        public static string TimeToString(float time)
        {
            System.TimeSpan timespan = System.TimeSpan.FromSeconds(time);

            return
                timespan.Hours > 0 ? string.Format("{0}h {1:D2}m", timespan.Hours, timespan.Minutes) :
                timespan.Minutes > 0 ? string.Format("{0}m {1:D2}s", timespan.Minutes, timespan.Seconds) :
                                        string.Format("{0}s", timespan.Seconds);
        }

        /// <summary>
        /// Fusionne 2 string[] ensemble.
        /// </summary>
        public static string[] FusionStringArray(string[] arrayA, string[] arrayB)
        {
            string[] fusionArray = new string[arrayA.Length + arrayB.Length];

            arrayA.CopyTo(fusionArray, 0);
            arrayB.CopyTo(fusionArray, arrayA.Length);

            return fusionArray;
        }
    }
}