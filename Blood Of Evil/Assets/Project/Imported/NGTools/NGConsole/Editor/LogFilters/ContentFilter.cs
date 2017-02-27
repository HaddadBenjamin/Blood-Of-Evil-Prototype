using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal sealed class ContentFilter : ILogFilter
	{
		private enum SearchMode
		{
			Content,
			StackTrace,
			Both,
		}

		[Exportable]
		private bool	enabled;
		public bool		Enabled { get { return this.enabled; } set { if (this.enabled != value) { this.enabled = value; if (this.ToggleEnable != null) this.ToggleEnable(); } } }

		public event Action	ToggleEnable;

		[Exportable]
		private string			keyword;
		[Exportable]
		private SearchMode		searchMode;
		[Exportable]
		private CompareOptions	caseSensitive = CompareOptions.IgnoreCase;
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

		public FilterResult	CanDisplay(Row row)
		{
			if (string.IsNullOrEmpty(this.keyword) == true)
				return FilterResult.None;

			ILogContentGetter	logContent = row as ILogContentGetter;

			if (logContent == null)
				return FilterResult.None;

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
							return FilterResult.Accepted;
					}

					if (this.searchMode == SearchMode.StackTrace ||
						this.searchMode == SearchMode.Both)
					{
						if (Regex.IsMatch(logContent.StackTrace, keyword, options))
							return FilterResult.Accepted;
					}
				}
				catch (Exception ex)
				{
					this.regexSyntaxError = ex.Message;
				}
			}
			else
			{
				List<string>	patterns = new List<string>();
				int				j = 0;

				// Handle spaces at start and end.
				for (int i = 0; i < this.keyword.Length; i++)
				{
					if (this.keyword[i] == ' ' && i > 0 && i < this.keyword.Length - 1)
					{
						string	v = this.keyword.Substring(j, i - j);

						if (v != string.Empty)
							patterns.Add(v);

						j = i + 1;
					}
				}

				if (j < this.keyword.Length)
				{
					string	v = this.keyword.Substring(j);

					if (v != string.Empty)
						patterns.Add(v);
				}

				if (this.searchMode == SearchMode.Content ||
					this.searchMode == SearchMode.Both)
				{
					int	i = 0;

					for (; i < patterns.Count; i++)
					{
						if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(logContent.FullMessage, patterns[i], this.caseSensitive) < 0)
							break;
					}

					if (i == patterns.Count)
						return FilterResult.Accepted;
				}

				if (this.searchMode == SearchMode.StackTrace ||
					this.searchMode == SearchMode.Both)
				{
					int	i = 0;

					for (; i < patterns.Count; i++)
					{
						if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(logContent.StackTrace, patterns[i], this.caseSensitive) < 0)
							break;
					}

					if (i == patterns.Count)
						return FilterResult.Accepted;
				}
			}

			return FilterResult.None;
		}

		public void	OnGUI()
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
					EditorGUI.BeginChangeCheck();
					SearchMode	mode = (SearchMode)EditorGUILayout.EnumPopup(LC.G("SearchMode"), this.searchMode, GeneralStyles.ToolbarDropDown, GUILayout.Width(200F));
					if (EditorGUI.EndChangeCheck() == true)
					{
						Undo.RecordObject(Preferences.Settings, "Alter content filter");
						this.searchMode = mode;
					}
				}

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.caseSensitive == CompareOptions.None, this.cs, GeneralStyles.ToolbarToggle, GUILayout.Width(30F));
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Alter content filter");
					if (this.caseSensitive == CompareOptions.IgnoreCase)
						this.caseSensitive = CompareOptions.None;
					else
						this.caseSensitive = CompareOptions.IgnoreCase;
				}

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.wholeWord, this.whole, GeneralStyles.ToolbarToggle, GUILayout.Width(40F));
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Alter content filter");
					this.wholeWord = !this.wholeWord;
				}

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.useRegex, this.regex, GeneralStyles.ToolbarToggle, GUILayout.Width(30F));
				if (EditorGUI.EndChangeCheck() == true)
				{
					Undo.RecordObject(Preferences.Settings, "Alter content filter");
					this.useRegex = !this.useRegex;
					this.CheckRegex();
				}

				using (LabelWidthRestorer.Get(70F))
				{
					using (BgColorContentRestorer.Get(string.IsNullOrEmpty(this.regexSyntaxError) == false, Color.red))
					{
						EditorGUI.BeginChangeCheck();
						string	keyword = EditorGUILayout.TextField(this.keyword, GeneralStyles.ToolbarSearchTextField);
						if (EditorGUI.EndChangeCheck() == true)
						{
							//Undo.RecordObject(Preferences.Settings, "Alter content filter");
							this.keyword = keyword;
							this.CheckRegex();
						}

						if (GUILayout.Button(GUIContent.none, GeneralStyles.ToolbarSearchCancelButton) == true)
						{
							//Undo.RecordObject(Preferences.Settings, "Alter content filter");
							this.keyword = string.Empty;
							this.regexSyntaxError = null;
							GUI.FocusControl(null);
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

		public void	ContextMenu(GenericMenu menu, Row row, int i)
		{
			if (row is ILogContentGetter)
				menu.AddItem(new GUIContent("#" + i + " " + LC.G("FilterByThisContent")), false, this.ActiveFilter, row);
		}

		private void	ActiveFilter(object data)
		{
			ILogContentGetter	logContent = data as ILogContentGetter;

			this.keyword = logContent.HeadMessage;
			this.Enabled = true;
		}
	}
}