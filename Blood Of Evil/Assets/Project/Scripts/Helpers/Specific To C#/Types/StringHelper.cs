using System;
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
        /// Formate un temps en secondes en chaîne de caractères avec la possibilité de choisir le séparateur et ce que l'on doit afficher.
        /// Exemple de base : TimeToString(312.366f) -> MM:SS:ll -> 05:12:36.
        /// </summary>
        public static string TimeToString(
            float timeInSeconds,
            bool showMilliSeconds = true,
            bool showSeconds = true,
            bool showMinutes = true,
            bool showHours = false,
            bool showDays = false,
            string separator = ":",
            int numberLength = 2)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
            string timeString = "";

            if (showDays)
                timeString += FillIntByZeros(timeSpan.Days, 2) +
                              (BooleanHelper.ContainsOneTrue(showHours, showMinutes, showSeconds, showMilliSeconds) ? separator : "");

            if (showHours)
                timeString += FillIntByZeros(timeSpan.Hours, 2) +
                              (BooleanHelper.ContainsOneTrue(showMinutes, showSeconds, showMilliSeconds) ? separator : "");

            if (showMinutes)
                timeString += FillIntByZeros(timeSpan.Minutes, 2) +
                              (BooleanHelper.ContainsOneTrue(showSeconds, showMilliSeconds) ? separator : "");

            if (showSeconds)
                timeString += FillIntByZeros(timeSpan.Seconds, 2) +
                              (BooleanHelper.ContainsOneTrue(showMilliSeconds) ? separator : "");

            if (showMilliSeconds)
            {
                timeString += FillIntByZeros(timeSpan.Milliseconds - (timeSpan.Milliseconds % 10), 2);
                timeString = timeString.Substring(0, timeString.Length - 1);
            }

            return timeString;
        }

        /// <summary>
        /// Convertit un entier chaîne de caractères en le remplissant de zéro en fonction de sa taille.
        /// Exemple : FillIntByZeros(123, 5) -> 00123.
        /// </summary>
        public static string FillIntByZeros(int integer, int integerLength = 2)
        {
            string text = "";

            if (integer < 0)
                text += "-";

            integer = Mathf.Abs(integer);

            for (; integerLength >= 2; integerLength--)
            {
                if (integer < (int)Mathf.Pow(10, integerLength - 1))
                    text += "0";
            }

            if (integer != 0)
                text += integer.ToString();

            return text;
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
