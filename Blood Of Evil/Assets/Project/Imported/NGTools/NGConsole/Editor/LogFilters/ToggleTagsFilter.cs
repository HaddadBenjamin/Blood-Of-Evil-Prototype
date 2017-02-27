using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal sealed class ToggleTagsFilter : ILogFilter
	{
		[Exportable]
		private bool	enabled;
		public bool		Enabled { get { return this.enabled; } set { if (this.enabled != value) { this.enabled = value; if (this.ToggleEnable != null) this.ToggleEnable(); } } }

		public event Action	ToggleEnable;

		[Exportable]
		private List<string>	acceptedTags = new List<string>();
		private string			newTag = string.Empty;

		[NonSerialized]
		private GUIContent	logContent;
		[NonSerialized]
		private GUIContent	warningContent;
		[NonSerialized]
		private GUIContent	errorContent;
		[NonSerialized]
		private GUIContent	exceptionContent;

		public void	Init()
		{
			this.logContent = new GUIContent();
			this.logContent.image = Utility.GetConsoleIcon("console.infoicon.sml");

			this.warningContent = new GUIContent();
			this.warningContent.image = Utility.GetConsoleIcon("console.warnicon.sml");

			this.errorContent = new GUIContent();
			this.errorContent.image = Utility.GetConsoleIcon("console.erroricon.sml");

			this.exceptionContent = new GUIContent();
			this.exceptionContent.image = Utility.GetConsoleIcon("console.erroricon.sml");
		}

		public FilterResult	CanDisplay(Row row)
		{
			MultiTagsRow	tagsRows = row as MultiTagsRow;
			if (tagsRows == null)
				return FilterResult.Refused;

			if (tagsRows.isParsed == false)
				tagsRows.ParseLog();

			for (int i = 0; i < tagsRows.tags.Length; i++)
			{
				for (int j = 0; j < this.acceptedTags.Count; j++)
				{
					if (tagsRows.tags[i] == this.acceptedTags[j])
						return FilterResult.Accepted;
				}
			}

			return FilterResult.Refused;
		}

		public void	OnGUI()
		{
			if (this.logContent == null)
				this.Init();

			GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUILayout.PrefixLabel(LC.G("AcceptedTags"));

				for (int i = 0; i < this.acceptedTags.Count; i++)
				{
					if (GUILayout.Button(this.acceptedTags[i], GeneralStyles.ToolbarButton) == true)
					{
						this.acceptedTags.RemoveAt(i);
						break;
					}
				}

				this.newTag = GUILayout.TextField(this.newTag, GUILayout.Width(150F));
				if (GUILayout.Button("+", GeneralStyles.ToolbarButton) == true && string.IsNullOrEmpty(this.newTag) == false)
				{
					this.acceptedTags.Add(this.newTag);
					this.newTag = string.Empty;
				}

				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}

		public void	ContextMenu(GenericMenu menu, Row row, int i)
		{
		}
	}
}