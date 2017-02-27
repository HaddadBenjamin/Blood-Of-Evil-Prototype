using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace NGToolsEditor.NGRenamer
{
	internal sealed class ReplaceFilter : TextFilter
	{
		public string	pattern;
		public string	replace;
		public bool		regex;
		public bool		caseSensitive;

		private string	regexError;

		public	ReplaceFilter(NGRenamerWindow renamer) : base(renamer, "Replace", 1000)
		{
		}

		public override void	OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			using (LabelWidthRestorer.Get(90F))
			{
				this.pattern = EditorGUILayout.TextField("Search For", this.pattern);
				if (EditorGUI.EndChangeCheck() == true)
				{
					try
					{
						this.regexError = null;
						new Regex(this.pattern);
					}
					catch (Exception ex)
					{
						this.regexError = ex.Message;
					}
					this.enable = true;
					this.renamer.Invalidate();
				}
				if (string.IsNullOrEmpty(this.regexError) == false)
					EditorGUILayout.HelpBox(this.regexError, MessageType.Warning);

				EditorGUI.BeginChangeCheck();
				this.replace = EditorGUILayout.TextField("Replace With", this.replace);
				this.regex = EditorGUILayout.Toggle("Regex", this.regex);
				this.caseSensitive = EditorGUILayout.Toggle("Case Sensitive", this.caseSensitive);
			}
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.enable = true;
				this.renamer.Invalidate();
			}
		}

		public override void	Highlight(string input, List<int> highlightedPositions)
		{
			if (string.IsNullOrEmpty(this.pattern) == true)
				return;

			if (this.regex == false)
			{
				if (this.caseSensitive == true)
					this.HighlightPattern(input, this.pattern, StringComparison.InvariantCulture, highlightedPositions);
				else
					this.HighlightPattern(input, this.pattern, StringComparison.InvariantCultureIgnoreCase, highlightedPositions);
			}
			else
			{
				RegexOptions	options = RegexOptions.Multiline;

				if (this.caseSensitive == false)
					options |= RegexOptions.IgnoreCase;

				MatchCollection	matches = Regex.Matches(input, this.pattern, options);

				for (int i = matches.Count - 1; i >= 0; --i)
				{
					for (int j = matches[i].Index; j < matches[i].Index + matches[i].Length; j++)
					{
						if (highlightedPositions.Contains(j) == false)
							highlightedPositions.Add(j);
					}
				}
			}
		}

		public override string	Filter(string input)
		{
			if (string.IsNullOrEmpty(this.pattern) == true)
				return input;

			if (this.regex == false)
			{
				if (this.caseSensitive == true)
					return input.Replace(this.pattern, this.replace);
				return this.Replace(input, this.pattern, this.replace, StringComparison.InvariantCultureIgnoreCase);
			}
			else
			{
				RegexOptions	options = RegexOptions.Multiline;

				if (this.caseSensitive == false)
					options |= RegexOptions.IgnoreCase;

				return Regex.Replace(input, this.pattern, this.replace, options);
			}
		}

		/// <summary>
		/// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
		/// </summary>
		/// <param name="input">The string performing the replace method.</param>
		/// <param name="pattern">The string to be replaced.</param>
		/// <param name="replace">The string replaces all occurrences of pattern.</param>
		/// <param name="comparisonType">Type of the comparison.</param>
		/// <returns></returns>
		/// Thanks Darky711 @ http://stackoverflow.com/questions/6275980/string-replace-ignoring-case
		private string	Replace(string input, string pattern, string replace, StringComparison comparisonType)
		{
			replace = replace ?? string.Empty;
			if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern) || pattern.Equals(replace, comparisonType))
				return input;

			int	foundAt = 0;

			while ((foundAt = input.IndexOf(pattern, foundAt, comparisonType)) != -1)
			{
				input = input.Remove(foundAt, pattern.Length).Insert(foundAt, replace);
				foundAt += replace.Length;
			}

			return input;
		}

		private void	HighlightPattern(string input, string pattern, StringComparison comparisonType, List<int> highlightedPositions)
		{
			int	foundAt = 0;

			while ((foundAt = input.IndexOf(pattern, foundAt, comparisonType)) != -1)
			{
				for (int i = foundAt; i < foundAt + pattern.Length; i++)
				{
					if (highlightedPositions.Contains(i) == false)
						highlightedPositions.Add(i);
				}

				foundAt += pattern.Length;
			}
		}
	}
}