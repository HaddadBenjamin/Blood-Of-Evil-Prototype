using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public abstract class NGRemoteWindow : EditorWindow
	{
		[NonSerialized]
		private NGHierarchyWindow	hierarchy;
		public NGHierarchyWindow	Hierarchy
		{
			get
			{
				return this.hierarchy;
			}
		}

		private int	hierarchyIndex;

		protected virtual void	OnDisable()
		{
			if (this.hierarchy != null)
				this.hierarchy.RemoveRemoteWindow(this);
		}

		protected void	OnGUI()
		{
			if (this.Hierarchy == null)
			{
				NGHierarchyWindow[]	hierarchies = Resources.FindObjectsOfTypeAll<NGHierarchyWindow>();

				if (hierarchies.Length == 0)
				{
					EditorGUILayout.LabelField(LC.G("NGRemote_NoHierarchyAvailable"), GeneralStyles.BigCenterText, GUILayout.ExpandHeight(true));
					return;
				}
				else if (hierarchies.Length == 1)
				{
					hierarchies[0].AddRemoteWindow(this);
				}
				else
				{
					EditorGUILayout.LabelField(LC.G("NGRemote_RequireHierarchy"));

					var	hierarchyNames = hierarchies.Select<NGHierarchyWindow, string>((h) => h.GetTitle() + " - " + h.address + ":" + h.port).ToArray();

					EditorGUILayout.BeginHorizontal();
					{
						this.hierarchyIndex = EditorGUILayout.Popup(this.hierarchyIndex, hierarchyNames);

						if (GUILayout.Button(LC.G("Set")) == true)
						{
							hierarchies[0].AddRemoteWindow(this);
						}
					}
					EditorGUILayout.EndHorizontal();

					return;
				}
			}

			this.OnGUIHeader();

			if (this.Hierarchy.IsClientConnected() == false)
			{
				GUILayout.Label(LC.G("NGRemote_NotConnected"), GeneralStyles.BigCenterText, GUILayout.ExpandHeight(true));
				return;
			}

			this.OnGUIConnected();
		}

		public void	SetHierarchy(NGHierarchyWindow hierarchy)
		{
			if (this.hierarchy == hierarchy)
				return;

			if (this.hierarchy != null)
				this.OnHierarchyUninit();

			this.hierarchy = hierarchy;

			if (this.hierarchy != null)
				this.OnHierarchyInit();
		}
		
		protected virtual void	OnHierarchyInit()
		{
			this.hierarchy.HierarchyConnected += this.OnHierarchyConnected;
			this.hierarchy.HierarchyDisconnected += this.OnHierarchyDisconnected;
		}

		protected virtual void	OnHierarchyUninit()
		{
			this.hierarchy.HierarchyConnected -= this.OnHierarchyConnected;
			this.hierarchy.HierarchyDisconnected -= this.OnHierarchyDisconnected;
		}

		protected virtual void	OnHierarchyConnected()
		{
			this.Repaint();
		}

		protected virtual void	OnHierarchyDisconnected()
		{
			this.Repaint();
		}

		protected virtual void	OnGUIHeader()
		{
		}

		protected abstract void	OnGUIConnected();
	}
}