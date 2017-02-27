using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal sealed class TagFilter : ILogFilter
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
		private string		tag;

		public FilterResult	CanDisplay(Row row)
		{
			if (string.IsNullOrEmpty(this.tag) == true)
				return FilterResult.None;

			ILogContentGetter	logContent = row as ILogContentGetter;

			if (logContent == null)
				return FilterResult.None;

			GameObject	gameObject = EditorUtility.InstanceIDToObject(row.log.instanceID) as GameObject;

			if (gameObject != null && gameObject.CompareTag(this.tag) == true)
				return FilterResult.Accepted;
			return FilterResult.None;
		}

		public void	OnGUI()
		{
			GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				this.tag = EditorGUILayout.TextField(LC.G("Tag"), this.tag);
			}
			GUILayout.EndHorizontal();
		}

		public void	ContextMenu(GenericMenu menu, Row row, int i)
		{
			if (row is ILogContentGetter)
			{
				GameObject	gameObject = EditorUtility.InstanceIDToObject(row.log.instanceID) as GameObject;

				if (gameObject != null)
					menu.AddItem(new GUIContent("#" + i + " " + LC.G("FilterByTag")), false, this.ActiveFilter, gameObject);
			}
		}

		private void	ActiveFilter(object data)
		{
			GameObject	gameObject = data as GameObject;

			this.tag = gameObject.tag;
			this.Enabled = true;
		}
	}
}