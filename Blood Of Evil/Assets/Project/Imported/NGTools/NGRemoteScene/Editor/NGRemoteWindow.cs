using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public abstract class NGRemoteWindow : EditorWindow
	{
		[NonSerialized]
		private NGRemoteHierarchyWindow	hierarchy;
		public NGRemoteHierarchyWindow	Hierarchy
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
			FreeOverlay.First(this, "NG Remote Scene is exclusive to NG Tools Pro.\n\nFree version is restrained to read-only.");

			if (this.Hierarchy == null)
			{
				NGRemoteHierarchyWindow[]	hierarchies = Resources.FindObjectsOfTypeAll<NGRemoteHierarchyWindow>();

				if (hierarchies.Length == 0)
				{
					EditorGUILayout.LabelField(LC.G("NGRemote_NoHierarchyAvailable"), GeneralStyles.BigCenterText, GUILayout.ExpandHeight(true));

					FreeOverlay.Last();

					return;
				}
				else if (hierarchies.Length == 1)
					hierarchies[0].AddRemoteWindow(this);
				else
				{
					EditorGUILayout.LabelField(LC.G("NGRemote_RequireHierarchy"));

					string[]	hierarchyNames = hierarchies.Select<NGRemoteHierarchyWindow, string>((h) => h.GetTitle() + " - " + h.address + ":" + h.port).ToArray();

					EditorGUILayout.BeginHorizontal();
					{
						this.hierarchyIndex = EditorGUILayout.Popup(this.hierarchyIndex, hierarchyNames);

						if (GUILayout.Button(LC.G("Set")) == true)
							hierarchies[0].AddRemoteWindow(this);
					}
					EditorGUILayout.EndHorizontal();

					FreeOverlay.Last();

					return;
				}
			}

			this.OnGUIHeader();

			if (this.Hierarchy.IsClientConnected() == false)
			{
				GUILayout.Label(LC.G("NGRemote_NotConnected"), GeneralStyles.BigCenterText, GUILayout.ExpandHeight(true));
				FreeOverlay.Last();
				return;
			}

			this.OnGUIConnected();
		}

		public void	SetHierarchy(NGRemoteHierarchyWindow hierarchy)
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