using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BloodOfEvil.Helpers
{
	/// <summary>
	/// Class de parsing des arguments de "la commande like" "ExcelConvertor/".
	/// </summary>
	static class LinuxCommandParserHelper
	{
		/// <summary>
		/// Test si l'occurrence "NThOccurrence" de l'option "researchedString" éxiste dans "arguments".
		/// </summary>
		public static bool DoesOptionExists(
			string[] arguments,
			string researchedOption,
			/*[Optional, DefaultParameterValue(1)]*/ int NThOccurrence = 1)
		{
			foreach (string argument in arguments)
			{
				if (argument.Equals(researchedOption))
					--NThOccurrence;

				if (0 == NThOccurrence)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Récupère la valeur d'une option. (-ignoreExtension 46, retourne "46")
		/// </summary>
		public static string RetrieveOptionValue(
			string[] arguments,
			string researchedOption,
            /*[Optional, DefaultParameterValue(1)]*/ int NThOccurrence = 1)
        {
			for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
			{
				if (arguments[argumentIndex].Equals(researchedOption))
					--NThOccurrence;

				if (0 == NThOccurrence)
					return argumentIndex + 1 == arguments.Length - 1 ?
							null :
								arguments[1 + argumentIndex];
			}

			return null;
		}

		/// <summary>
		/// Récupère la valeur d'une option. (-ignoreExtension 46, retourne "46")
		/// </summary>
		public static string RetrieveOptionValue(
			string[] arguments,
			string[] researchedOptions,
            /*[Optional, DefaultParameterValue(1)]*/ int NThOccurrence = 1)
        {
			for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
			{
				for (int researchedOptionIndex = 0; researchedOptionIndex < researchedOptions.Length; researchedOptionIndex++)
				{
					if (arguments[argumentIndex].Equals(researchedOptions[researchedOptionIndex]))
						--NThOccurrence;

					if (0 == NThOccurrence)
					{
						return argumentIndex + 1 > arguments.Length -1 ?
								null :
									arguments[1 + argumentIndex];
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Récupère entière d'une option de manière sécurisée. (-ignoreExtension 46, retourne 46)
		/// </summary>
		public static void RetrieveIntegerOptionValue(
			string[] arguments,
			string optionResearched,
			ref int optionValue,
            /*[Optional, DefaultParameterValue(1)]*/ int NThOccurrence = 1)
        {
			string ignoredOptionValue = RetrieveOptionValue(arguments, optionResearched, NThOccurrence);

			if (!string.IsNullOrEmpty(ignoredOptionValue))
				int.TryParse(ignoredOptionValue, out optionValue);
		}

		/// <summary>
		/// Renvoie le nombre d'occurence d'une chaîne de caractère "researchedOption" par rapport à un tableau d'arguments "arguments".
		/// </summary>
		public static int GetTheNumberOfOccurence(
			string[] arguments,
			params string[] researchedOptions)
		{
			int numberOfOccurence = 0;

			foreach (string argument in arguments)
			{
				foreach (string researchedOption in researchedOptions)
				{
					if (argument.Equals(researchedOption))
						++numberOfOccurence;
				}
			}

			return numberOfOccurence;
		}

		/// <summary>
		/// Récupère l'index de l'occurence "NThOccurence" d'une des options "researchedOption" dans le tableau d'arguments "arguments".
		/// </summary>
		public static int GetIndexOfNThOccurenceOption(
			string[] arguments,	
			string[] researchedOptions,
            /*[Optional, DefaultParameterValue(1)]*/ int NThOccurrence = 1)
        {
			for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
			{
				foreach (string researchedOption in researchedOptions)
				{
					if (arguments[argumentIndex].Equals(researchedOption))
					{
						--NThOccurrence;

						if (NThOccurrence == 0)
							return argumentIndex;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Récupère la prochaine option en partant de l'index "optionIndex". REVOIR CE SUMMARY, il n'est pas clair.
		/// </summary>
		public static int GetNextOptionValueIndex(
			string[] arguments,
			int optionIndex)
		{
			//Console.WriteLine((string.Format("optionIndex : {0}", optionIndex)));
			int nextOptionIndex = optionIndex + 2;

			// On regarde si notre tableau à la place d'avoir une prochaine option.
			if (nextOptionIndex < arguments.Length)
			{
				// On retourne l'index de la première option rencontré, une option commence toujours pas "-".
				for (; nextOptionIndex < arguments.Length; nextOptionIndex++)
				{
					if (arguments[nextOptionIndex].StartsWith("-"))
					{
						if (nextOptionIndex + 1 < arguments.Length)
							return nextOptionIndex + 1;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Déterminme si les options des arguments sont toutes attendues par votre parseur.
		/// </summary>
		public static bool ContainsOnlyExcpectedOptions(
			string[] arguments,
			string[] expectedOptions)
		{
			foreach (string argument in arguments)
			{
				// Si l'argument commence par un '-', il est considéré comme une option.
				if (argument.StartsWith("-") &&
					!expectedOptions.Contains(argument))
					throw new Exception(string.Format("L'argument {0} n'est pas attendu par la commande.", argument));
			}
			
			return true;
		}
	}
}
