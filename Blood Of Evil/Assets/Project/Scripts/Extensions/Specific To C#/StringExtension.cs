using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
    public static class StringExtension
	{
		/// <summary>
		/// Renvoie si une châine est égale à une châine des châines d'un tableau de châine.
		/// Il y a beaucoup de fois le mot châine ̿' ̿'\̵͇̿̿\з=(◕_◕)=ε/̵͇̿̿/'̿'̿ ̿
		/// </summary>
		public static bool IsEqualsToAnArrayElement(
			this string text,
			string[] researchedTexts)
		{
			if (string.IsNullOrEmpty(text) ||
				null == researchedTexts ||
				0 == researchedTexts.Length)
				return false;

			return Array.Exists(researchedTexts, researchedText => !string.IsNullOrEmpty(researchedText) && text.Equals(researchedText));
		}

		/// <summary>
		/// Détermine et Renvoie si une chaîne contiend une des autres chaînes contenues par (string[] researchedTexts).
		/// </summary>
		public static bool ContainsAnArrayElement(
			this string text,
			string[] researchedTexts)
		{
			return Array.Exists(researchedTexts, researchedText => text.Contains(researchedText));
		}
		
		/// <summary>
		/// Détermine si une chaîne contiend un des caractères d'un tableau de caractères.
		/// </summary>
		public static bool ContainsAnyCharacter(
			this string text,
			params char[] characters)
		{
			return Array.Exists(characters, character => text.Contains(character));
		}

        /// <summary>
        /// "SalutCavaBienDeviendra" -> "Salut Cava Bien Deviendra"
        /// Remplace les majuscules par un espace suivi de la majuscule en question.
        /// </summary>
        public static string ReplaceUppercaseBySpaceAndUppercase(this string text)
        {
            return Regex.Replace(text, @"\B[A-Z]", m => " " + m.ToString());
        }

        /// <summary>
        /// Met en majuscule le début de chaque mot.
        /// </summary>
        public static string FirstLetterUppercase(this string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static string RemoveAccents(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            str = str.Normalize(NormalizationForm.FormD);
            var chars = str.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Cette méthode vient de Manzalab.
        /// </summary>
        public static string RemovePunctuation(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            str = str.Replace('!', ' ');
            str = str.Replace('?', ' ');

            string toReplace = str.Replace("  ", " ");
            while (str != toReplace)
            {
                str = toReplace;
                toReplace = str.Replace("  ", " ");
            }

            str = str.Trim();

            return str;
        }
    }
}
