using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	internal sealed class TextStartMode : StartMode
	{
		private enum SearchMode
		{
			Content,
			StackTrace,
			Both,
		}

		[Exportable]
		private string			keyword;
		[Exportable]
		private SearchMode		searchMode;
		[Exportable]
		private CompareOptions	caseSensitive;
		[Exportable]
		private bool			wholeWord;
		[Exportable]
		private bool			useRegex;
		private string			regexSyntaxError;

		[NonSerialized]
		private GUIContent	cs;
		[NonSerialized]
		private GUIContent	whole;
		[NonSerialized]
		private GUIContent	regex;

		public override bool	CheckStart(Row row)
		{
			if (string.IsNullOrEmpty(this.keyword) == true)
				return false;

			ILogContentGetter	logContent = row as ILogContentGetter;

			if (logContent == null)
				return false;

			if (this.useRegex == true || this.wholeWord == true)
			{
				RegexOptions	options = RegexOptions.Multiline;

				if (this.caseSensitive == CompareOptions.IgnoreCase)
					options |= RegexOptions.IgnoreCase;

				string	keyword = this.keyword;
				if (this.wholeWord == true)
					keyword = "\\b" + keyword + "\\b";

				try
				{
					if (this.searchMode == SearchMode.Content ||
						this.searchMode == SearchMode.Both)
					{
						if (Regex.IsMatch(logContent.FullMessage, keyword, options))
							return true;
					}

					if (this.searchMode == SearchMode.StackTrace ||
						this.searchMode == SearchMode.Both)
					{
						if (Regex.IsMatch(logContent.StackTrace, keyword, options))
							return true;
					}
				}
				catch (Exception ex)
				{
					this.regexSyntaxError = ex.Message;
				}
			}
			else
			{
				string[]	patterns = this.keyword.Split(' ');

				if (this.searchMode == SearchMode.Content ||
					this.searchMode == SearchMode.Both)
				{
					int	i = 0;

					for (; i < patterns.Length; i++)
					{
						if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(logContent.FullMessage, patterns[i], this.caseSensitive) < 0)
							break;
					}

					if (i == patterns.Length)
						return true;
				}

				if (this.searchMode == SearchMode.StackTrace ||
					this.searchMode == SearchMode.Both)
				{
					int	i = 0;

					for (; i < patterns.Length; i++)
					{
						if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(logContent.StackTrace, patterns[i], this.caseSensitive) < 0)
							break;
					}

					if (i == patterns.Length)
						return true;
				}
			}

			return false;
		}

		public override void	OnGUI()
		{
			if (this.cs == null)
			{
				this.cs = new GUIContent("Ab", LC.G("CaseSensitive"));
				this.regex = new GUIContent("R*", LC.G("RegularExpressions"));
				this.whole = new GUIContent("|abc|", LC.G("WholeMatch"));
			}

			GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				using (LabelWidthRestorer.Get(120F))
				{
					this.searchMode = (SearchMode)EditorGUILayout.EnumPopup(this.searchMode, GeneralStyles.ToolbarDropDown, GUILayout.Width(100F));
				}

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.caseSensitive == CompareOptions.IgnoreCase, this.cs, GeneralStyles.ToolbarButton, GUILayout.Width(30F));
				if (EditorGUI.EndChangeCheck() == true)
				{
					if (this.caseSensitive == CompareOptions.IgnoreCase)
						this.caseSensitive = CompareOptions.None;
					else
						this.caseSensitive = CompareOptions.IgnoreCase;
				}

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.wholeWord, this.whole, GeneralStyles.ToolbarButton, GUILayout.Width(40F));
				if (EditorGUI.EndChangeCheck() == true)
					this.wholeWord = !this.wholeWord;

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.useRegex, this.regex, GeneralStyles.ToolbarButton, GUILayout.Width(30F));
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.useRegex = !this.useRegex;
					this.CheckRegex();
				}

				using (LabelWidthRestorer.Get(70F))
				{
					using (BgColorContentRestorer.Get(string.IsNullOrEmpty(this.regexSyntaxError) == false, Color.red))
					{
						EditorGUI.BeginChangeCheck();
						this.keyword = EditorGUILayout.TextField(this.keyword, GeneralStyles.ToolbarSearchTextField);
						if (EditorGUI.EndChangeCheck() == true)
							this.CheckRegex();

						if (GUILayout.Button(GUIContent.none, GeneralStyles.ToolbarSearchCancelButton) == true)
						{
							this.keyword = string.Empty;
							this.regexSyntaxError = null;
						}
					}
				}
			}
			GUILayout.EndHorizontal();
		}

		private void	CheckRegex()
		{
			this.regexSyntaxError = null;

			if (this.useRegex == true)
			{
				try
				{
					Regex.IsMatch("", this.keyword);
				}
				catch (Exception ex)
				{
					this.regexSyntaxError = ex.Message;
				}
			}
		}
	}
}