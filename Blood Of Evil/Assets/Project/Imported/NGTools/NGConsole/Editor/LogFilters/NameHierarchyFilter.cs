using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal sealed class NameHierarchyFilter : ILogFilter
	{
		[Exportable]
		private bool	enabled;
		public bool		Enabled { get { return this.enabled; } set { if (this.enabled != value) { this.enabled = value; if (this.ToggleEnable != null) this.ToggleEnable(); } } }

		public event Action	ToggleEnable;

		[Exportable]
		private string	name;

		public FilterResult	CanDisplay(Row row)
		{
			if (row.log.instanceID == 0 ||
				string.IsNullOrEmpty(this.name) == true)
			{
				return FilterResult.None;
			}

			Object	@object = EditorUtility.InstanceIDToObject(row.log.instanceID);

			if (@object != null && @object.name.Contains(this.name))
				return FilterResult.Accepted;
			return FilterResult.None;
		}

		public void	OnGUI()
		{
			GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				this.name = EditorGUILayout.TextField(LC.G("GameObjectWithName"), this.name);
			}
			GUILayout.EndHorizontal();
		}

		public void	ContextMenu(GenericMenu menu, Row row, int i)
		{
			if (row.log.instanceID != 0)
				menu.AddItem(new GUIContent("#" + i + " " + LC.G("FilterByThisObjectName")), false, this.ActiveFilter, row);
		}

		private void	ActiveFilter(object data)
		{
			Row	row = data as Row;
			this.name = EditorUtility.InstanceIDToObject(row.log.instanceID).name;
			this.Enabled = true;
		}
	}
}