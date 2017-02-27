using NGTools;
using NGTools.Network;
using NGTools.NGRemoteScene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public class ResourcesPickerWindow : EditorWindow
	{
		private static Color	FocusBackgroundColor = new Color(50F / 255F, 76F / 255F, 120F / 255F);
		private static Color	InitialBackgroundColor = new Color(30F / 255F, 46F / 255F, 20F / 255F);

		private NGRemoteHierarchyWindow	hierarchy;
		private Type				type;
		private string				valuePath;
		private Func<string, byte[], Packet> packetGenerator;
		private string				searchString;
		private int					selected;
		private Vector2				scrollPosition;
		private List<string>		filteredResources;
		private List<int>			filteredResourceIDs;
		private int					initialInstanceID;

		private string[]	resources;
		private int[]		ids;
		private TypeHandler	typeHandler;

		public static void	Init(NGRemoteHierarchyWindow hierarchy, Type type, string valuePath, Func<string, byte[], Packet> packetGenerator, int initialInstanceID)
		{
			ResourcesPickerWindow	picker = EditorWindow.GetWindow<ResourcesPickerWindow>(true, "Select " + type.Name);

			picker.hierarchy = hierarchy;
			picker.type = type;
			picker.valuePath = valuePath;
			picker.packetGenerator = packetGenerator;
			picker.searchString = string.Empty;
			picker.selected = -1;
			picker.filteredResources = new List<string>(64);
			picker.filteredResourceIDs = new List<int>(64);
			picker.initialInstanceID = initialInstanceID;
			picker.typeHandler = TypeHandlersManager.GetTypeHandler(type);
		}

		protected virtual void	OnGUI()
		{
			InternalNGDebug.AssertFile(this.hierarchy != null, "ResourcesPicker requires to be created through ResourcesPicker.Init.");

			if (this.resources == null)
			{
				this.hierarchy.GetResources(this.type, out this.resources, out this.ids);

				if (this.resources == null)
				{
					EditorGUILayout.LabelField(LC.G("NGHierarchy_ResourcesNotAvailable"), GeneralStyles.WrapLabel);
					return;
				}
				else
				{
					for (int i = 0; i < this.ids.Length; i++)
					{
						if (this.ids[i] == this.initialInstanceID)
						{
							this.selected = i + 1;
							this.FitFocusedRowInScreen(this.selected);
							break;
						}
					}
				}
			}

			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.UpArrow)
				{
					if (this.selected > 0)
					{
						--this.selected;
						this.SendSelection();
						this.Repaint();
					}
				}
				else if (Event.current.keyCode == KeyCode.DownArrow)
				{
					if (this.selected < this.GetCountResources() - 1)
					{
						++this.selected;
						this.SendSelection();
						this.Repaint();
					}
				}
				else if (Event.current.keyCode == KeyCode.PageUp)
				{
					this.selected -= (int)this.position.height / 16;
					if (selected < 0)
						this.selected = 0;
					this.SendSelection();
					this.Repaint();
				}
				else if (Event.current.keyCode == KeyCode.PageDown)
				{
					this.selected += (int)this.position.height / 16;
					if (this.selected > this.GetCountResources() - 1)
						this.selected = this.GetCountResources() - 1;
					this.SendSelection();
					this.Repaint();
				}
				else if (Event.current.keyCode == KeyCode.Home)
				{
					if (this.selected != 0)
					{
						this.selected = 0;
						this.SendSelection();
						this.Repaint();
					}
				}
				else if (Event.current.keyCode == KeyCode.End)
				{
					if (this.selected != this.GetCountResources() - 1)
					{
						this.selected = this.GetCountResources() - 1;
						this.SendSelection();
						this.Repaint();
					}
				}
				else if (Event.current.keyCode == KeyCode.Escape)
					this.SendSelection(this.initialInstanceID);
				else if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{
					this.SendSelection();
					this.Close();
				}
			}

			GUILayout.BeginHorizontal();
			{
				EditorGUI.BeginChangeCheck();
				this.searchString = GUILayout.TextField(this.searchString, "ToolbarSeachTextField");
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.filteredResources.Clear();
					this.filteredResourceIDs.Clear();

					this.selected = -1;
					if (string.IsNullOrEmpty(this.searchString) == false)
					{
						for (int i = 0; i < this.resources.Length; i++)
						{
							if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(this.resources[i], this.searchString, CompareOptions.IgnoreCase) >= 0)
							{
								this.filteredResources.Add(this.resources[i]);
								this.filteredResourceIDs.Add(this.ids[i]);
							}
						}
					}
				}

				if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
				{
					this.searchString = string.Empty;
					this.selected = -1;
					GUI.FocusControl(null);
				}
			}
			GUILayout.EndHorizontal();

			Rect	viewRect = new Rect();
			int		resourcesCount = 0;
			Rect	r = this.position;

			r.x = 0F;
			r.y = GUILayoutUtility.GetLastRect().height;
			r.height -= r.y;

			Rect	bodyRect = r;

			if (string.IsNullOrEmpty(this.searchString) == true)
				resourcesCount = this.resources.Length;
			else
				resourcesCount = this.filteredResources.Count;

			viewRect.height = resourcesCount * EditorGUIUtility.singleLineHeight;

			this.scrollPosition = GUI.BeginScrollView(r, this.scrollPosition, viewRect);
			{
				r.x = 0F;
				r.y = 0F;
				r.height = EditorGUIUtility.singleLineHeight;

				int	i = 0;

				foreach (string resource in this.ForResources(this.resources))
				{
					if (r.y + r.height <= this.scrollPosition.y)
					{
						r.y += r.height;
						++i;
						continue;
					}

					int	id = this.GetIDForResource(i - 1, this.ids);

					if (Event.current.type == EventType.Repaint && i == this.selected)
						EditorGUI.DrawRect(r, ResourcesPickerWindow.FocusBackgroundColor);
					else if (Event.current.type == EventType.Repaint && id == this.initialInstanceID)
						EditorGUI.DrawRect(r, ResourcesPickerWindow.InitialBackgroundColor);

					if (Event.current.type == EventType.MouseDown &&
						r.Contains(Event.current.mousePosition) == true)
					{
						if (this.selected == i)
							this.Close();
						else
						{
							this.selected = i;
							this.SendSelection(id);
						}

						this.Repaint();

						Event.current.Use();
					}

					GUI.Label(r, resource + " " + id);

					r.y += r.height;
					++i;

					if (r.y - this.scrollPosition.y > bodyRect.height)
						break;
				}
			}
			GUI.EndScrollView();
		}

		private void	SendSelection(int id = -1)
		{
			if (id == -1)
				id = this.GetIDForResource(this.selected - 1, this.ids);

			ByteBuffer	buffer = Utility.GetBBuffer();

			this.typeHandler.Serialize(buffer, this.type, new UnityObject(this.type, id));

			this.hierarchy.AddPacket(this.packetGenerator(this.valuePath, Utility.ReturnBBuffer(buffer)));

			this.FitFocusedRowInScreen(this.selected);
		}

		public void	FitFocusedRowInScreen(int target)
		{
			float	y = target * EditorGUIUtility.singleLineHeight;

			if (this.scrollPosition.y > y)
				this.scrollPosition.y = y;
			else if (this.scrollPosition.y + this.position.height - EditorGUIUtility.singleLineHeight < y + EditorGUIUtility.singleLineHeight)
				this.scrollPosition.y = y - this.position.height + EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight;
		}

		protected virtual void	OnLostFocus()
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			// HACK: Workaround to avoid crash in Unity 4.
			EditorApplication.delayCall += this.Close;
#else
			this.Close();
#endif
		}

		private IEnumerable	ForResources(string[] resources)
		{
			yield return "None";

			if (string.IsNullOrEmpty(this.searchString) == false)
			{
				for (int i = 0; i < this.filteredResources.Count; i++)
					yield return this.filteredResources[i];
			}
			else
			{
				for (int i = 0; i < resources.Length; i++)
					yield return resources[i];
			}
		}

		private int	GetIDForResource(int index, int[] IDs)
		{
			if (index <= -1)
				return 0;

			if (string.IsNullOrEmpty(this.searchString) == false)
				return this.filteredResourceIDs[index];
			else
				return IDs[index];
		}

		private int	GetCountResources()
		{
			if (string.IsNullOrEmpty(this.searchString) == false)
				return this.filteredResourceIDs.Count;
			else
				return resources.Length;
		}
	}
}