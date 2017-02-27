using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal sealed class HierarchyFilter : ILogFilter
	{
		private static Color	ParentBackgroundColor = Color.grey;

		[Exportable]
		private bool	enabled;
		public bool		Enabled { get { return this.enabled; } set { if (this.enabled != value) { this.enabled = value; if (this.ToggleEnable != null) this.ToggleEnable(); } } }

		public event Action	ToggleEnable;

		[NonSerialized]
		private Transform	parent;
		[Exportable]
		private bool		includeChildren;
		[Exportable]
		private string		lastParent;
		[NonSerialized]
		private bool		init = false;

		public FilterResult	CanDisplay(Row row)
		{
			if (row.log.instanceID == 0 ||
				this.parent == null)
			{
				return FilterResult.None;
			}

			Transform	transform = this.GetTransformFromObject(EditorUtility.InstanceIDToObject(row.log.instanceID));

			if (transform != null &&
				(transform == this.parent ||
				 (this.includeChildren == true && transform.IsChildOf(this.parent))))
			{
				return FilterResult.Accepted;
			}

			return FilterResult.None;
		}

		public void	OnGUI()
		{
			// Try to reconnect to last Object.
			if (this.init == false &&
				EditorApplication.isPlaying == true &&
				string.IsNullOrEmpty(this.lastParent) == false)
			{
				GameObject	sceneObject = GameObject.Find(this.lastParent);

				if (sceneObject != null)
				{
					this.parent = sceneObject.transform;
					this.init = true;
				}
			}

			GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUI.BeginChangeCheck();
				using (LabelWidthRestorer.Get(90F))
				{
					using (ColorContentRestorer.Get(string.IsNullOrEmpty(this.lastParent) == false, HierarchyFilter.ParentBackgroundColor))
					{
						Utility.content.text = LC.G("ParentObject");
						Utility.content.tooltip = this.lastParent;
						this.parent = EditorGUILayout.ObjectField(Utility.content, this.parent, typeof(Transform), true) as Transform;
						Utility.content.tooltip = string.Empty;
					}
				}
				if (EditorGUI.EndChangeCheck() == true)
				{
					if (this.parent != null)
						this.lastParent = this.parent.name;
					else
						this.lastParent = string.Empty;
					this.init = true;
				}

				// Handle reference destroyed at runtime.
				if (this.init == true &&
					this.parent == null)
				{
					this.init = false;
				}

				this.includeChildren = GUILayout.Toggle(this.includeChildren, LC.G("IncludeChildren"), Preferences.Settings.general.menuButtonStyle, GUILayout.Width(100F));

				using (LabelWidthRestorer.Get(70F))
				{
					if (//EditorApplication.isPlaying == true &&
						string.IsNullOrEmpty(this.lastParent) == false)
					{
						GUILayout.Label(LC.G("LastParent") + this.lastParent);
					}
				}
			}
			GUILayout.EndHorizontal();
		}

		public void	ContextMenu(GenericMenu menu, Row row, int i)
		{
			if (row.log.instanceID != 0 &&
				this.GetTransformFromObject(EditorUtility.InstanceIDToObject(row.log.instanceID)) != null)
			{
				menu.AddItem(new GUIContent("#" + i + " " + LC.G("FilterByThisObject")), false, this.ActiveFilter, row);
			}
		}

		private void	ActiveFilter(object data)
		{
			Row	row = data as Row;

			this.parent = this.GetTransformFromObject(EditorUtility.InstanceIDToObject(row.log.instanceID));
			this.lastParent = this.parent.name;
			this.init = true;
			this.Enabled = true;
		}

		private Transform	GetTransformFromObject(Object @object)
		{
			if (@object is Transform)
				return (Transform)@object;
			else if (@object is Component)
				return (@object as Component).transform;
			else if (@object is GameObject)
				return (@object as GameObject).transform;
			return null;
		}
	}
}