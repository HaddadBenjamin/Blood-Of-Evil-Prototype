using NGTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGProjectWindow : NGRemoteWindow, IHasCustomMenu
	{
		private class Folder
		{
			public class File
			{
				public static Color	HighlightSelectedFile = new Color(104F / 255F, 84F / 255F, 104F / 255F, 1F);
				public static File	selectedFile;

				public readonly string	name;
				public readonly int[]	IDs;
				public readonly Type[]	types;

				public	File(string name, int[] IDs, string[] types)
				{
					this.name = name;
					this.IDs = IDs;
					this.types = new Type[this.IDs.Length];
					for (int i = 0; i < types.Length; i++)
						this.types[i] = Type.GetType(types[i]);
				}
			}

			public readonly Folder	parent;
			public readonly string	name;

			private List<Folder>	folders;
			private List<File>		files;
			private bool			open;

			public	Folder(Folder parent, string name)
			{
				this.parent = parent;
				this.name = name;

				if (this.parent == null)
					this.open = true;
			}

			public Folder	GenerateFolder(string name)
			{
				if (this.folders == null)
					this.folders = new List<Folder>();

				for (int i = 0; i < this.folders.Count; i++)
				{
					if (this.folders[i].name.Equals(name) == true)
						return this.folders[i];
				}

				Folder	folder = new Folder(this, name);

				this.folders.Add(folder);

				return folder;
			}

			public Folder	GetFolder(string name)
			{
				for (int i = 0; i < this.folders.Count; i++)
				{
					if (this.folders[i].name.Equals(name) == true)
						return this.folders[i];
				}

				return null;
			}

			public void	AddFile(string filename, int[] IDs, string[] types)
			{
				if (this.files == null)
					this.files = new List<File>();

				this.files.Add(new File(filename, IDs, types));
			}

			public float	GetHeight()
			{
				float	height = EditorGUIUtility.singleLineHeight;

				if (this.parent == null)
					height = 0F;

				if (this.open == true)
				{
					if (this.folders != null)
					{
						for (int i = 0; i < this.folders.Count; i++)
						{
							if (this.folders[i].folders == null && this.folders[i].files == null)
								continue;

							height += this.folders[i].GetHeight();
						}
					}

					if (this.files != null)
						height += this.files.Count * EditorGUIUtility.singleLineHeight;
				}

				return height;
			}

			public void	Draw(Rect r, bool forceOpen = false)
			{
				if (forceOpen == false)
				{
					this.open = EditorGUI.Foldout(r, this.open, this.name);
					++EditorGUI.indentLevel;

					r.y += r.height;
				}

				if (forceOpen == true || this.open == true)
				{
					if (this.folders != null)
					{
						for (int i = 0; i < this.folders.Count; i++)
						{
							if (this.folders[i].folders == null && this.folders[i].files == null)
								continue;

							this.folders[i].Draw(r);
							r.y += this.folders[i].GetHeight();
						}
					}

					if (this.files != null)
					{
						++EditorGUI.indentLevel;

						for (int i = 0; i < this.files.Count; i++)
						{
							if (Event.current.type == EventType.Repaint && this.files[i] == File.selectedFile)
								EditorGUI.DrawRect(r, File.HighlightSelectedFile);

							if (r.Contains(Event.current.mousePosition) == true)
							{
								if (Event.current.type == EventType.MouseDown)
								{
									UnityObject	unityObject = new UnityObject(this.files[i].types[0], this.files[i].IDs[0]);

									NGProjectWindow.dragOriginPosition = Event.current.mousePosition;

									// Initialize drag data.
									DragAndDrop.PrepareStartDrag();

									DragAndDrop.objectReferences = new Object[0];
									DragAndDrop.SetGenericData("r", unityObject);

									File.selectedFile = this.files[i];

									Event.current.Use();
								}
								else if (Event.current.type == EventType.MouseDrag && (NGProjectWindow.dragOriginPosition - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance)
								{
									DragAndDrop.StartDrag("Dragging Game Object");

									Event.current.Use();
								}
								else if (Event.current.type == EventType.DragUpdated)
								{
									DragAndDrop.visualMode = DragAndDropVisualMode.Move;

									Event.current.Use();
								}
							}

							EditorGUI.LabelField(r, this.files[i].name);

							r.y += r.height;
						}

						--EditorGUI.indentLevel;
					}
				}

				if (forceOpen == false)
					--EditorGUI.indentLevel;
			}
		}

		public const string	NormalTitle = "NG Remote Project";
		public const string	ShortTitle = "NG R Project";
		public static Color	AssetTypesBackgroundColor = new Color(21F / 255F, 17F / 255F, 21F / 255F, .2F);

		private static Vector2	dragOriginPosition;

		public bool	autoLoad;

		private bool	showOptions;
		private Vector2	scrollPosition;
		private Rect	bodyRect = default(Rect);
		private Folder	root;

		private ListingAssets.AssetReferences[]	projectAssets;

		static	NGProjectWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGProjectWindow.NormalTitle);
		}

		[MenuItem(Constants.MenuItemPath + NGProjectWindow.NormalTitle, priority = Constants.MenuItemPriority + 219)]
		private static void	Open()
		{
			EditorWindow.GetWindow<NGProjectWindow>(NGProjectWindow.ShortTitle);
		}

		protected override void	OnHierarchyInit()
		{
			base.OnHierarchyInit();

			this.Hierarchy.executer.HandlePacket(PacketId.Scene_ServerSendProject, this.Handle_Scene_ServerSendProject);
		}

		protected override void OnHierarchyUninit()
		{
			base.OnHierarchyUninit();

			this.Hierarchy.executer.UnhandlePacket(PacketId.Scene_ServerSendProject);
		}

		protected override void	OnHierarchyConnected()
		{
			base.OnHierarchyConnected();

			this.projectAssets = null;
		}

		protected override void	OnGUIHeader()
		{
			EditorGUILayout.BeginHorizontal("Toolbar");
			{
				if (GUILayout.Button(this.showOptions == true ? "˄" : "˅", GeneralStyles.ToolbarButton, GUILayout.Width(24F)) == true)
					this.showOptions = !this.showOptions;

				GUILayout.FlexibleSpace();

				GUI.enabled = this.Hierarchy.IsClientConnected();
				if (this.projectAssets == null && (this.autoLoad == true || GUILayout.Button(LC.G("NGProject_Load"), GeneralStyles.ToolbarButton) == true))
				{
				    if (this.Hierarchy.Client != null && this.Hierarchy.BlockRequestChannel(this.GetHashCode()) == true)
					{
						this.Hierarchy.Client.AddPacket(new ClientRequestProjectPacket());
					}
				}
				GUI.enabled = true;
			}
			EditorGUILayout.EndHorizontal();

			if (this.showOptions == true)
			{
				EditorGUILayout.BeginHorizontal("Toolbar");
				{
					Utility.content.text = LC.G("NGProject_AutoLoad");
					this.autoLoad = EditorGUILayout.Toggle(Utility.content, this.autoLoad);
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		protected override void	OnGUIConnected()
		{
			if (this.projectAssets == null || this.root == null)
				return;

			Rect	viewRect = new Rect(0F, 0F, 0F, this.root.GetHeight());

			bodyRect.x = 0F;
			bodyRect.y = EditorGUIUtility.singleLineHeight + 2;
			if (this.showOptions == true)
				bodyRect.y += EditorGUIUtility.singleLineHeight + 2F; // Layout takes little more space.

			bodyRect.width = this.position.width;
			bodyRect.height = this.position.height - bodyRect.y;

			if (Folder.File.selectedFile != null)
				bodyRect.height -= EditorGUIUtility.singleLineHeight;

			float	height = bodyRect.y + bodyRect.height;

			this.scrollPosition = GUI.BeginScrollView(bodyRect, this.scrollPosition, viewRect);
			{
				bodyRect.y = 0F;
				bodyRect.height = EditorGUIUtility.singleLineHeight;

				this.root.Draw(bodyRect, true);
			}
			GUI.EndScrollView();

			if (Folder.File.selectedFile != null)
			{
				StringBuilder	buffer = Utility.GetBuffer();

				for (int j = 0; j < Folder.File.selectedFile.types.Length; j++)
				{
					buffer.Append(Folder.File.selectedFile.types[j].Name);
					buffer.Append(", ");
				}

				buffer.Length -= 2;

				bodyRect.y = height;
				EditorGUI.DrawRect(bodyRect, NGProjectWindow.AssetTypesBackgroundColor);
				EditorGUI.LabelField(bodyRect, Utility.ReturnBuffer(buffer));
			}

			if (Event.current.type == EventType.MouseDown)
			{
				Folder.File.selectedFile = null;
				Event.current.Use();
			}
		}

		private void	GeneratePath(ListingAssets.AssetReferences asset)
		{
			try
			{
				string[]	dirs = Path.GetDirectoryName(asset.asset).Split(Path.DirectorySeparatorChar);
				string		filename = Path.GetFileNameWithoutExtension(asset.asset);
				Folder		folder = this.root;

				// Skip Assets folder.
				for (int i = 1; i < dirs.Length; i++)
					folder = folder.GenerateFolder(dirs[i]);

				folder.AddFile(filename, asset.IDs, asset.types);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("Embedded asset \"" + asset.asset + "\" contains an error.", ex);
			}
		}

		private void	Handle_Scene_ServerSendProject(Client sender, Packet _packet)
		{
			ServerSendProjectPacket	packet = _packet as ServerSendProjectPacket;

			this.projectAssets = packet.assets;
			this.root = new Folder(null, "Assets");

			for (int i = 0; i < this.projectAssets.Length; i++)
				this.GeneratePath(this.projectAssets[i]);

			this.Repaint();
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			NGHierarchyWindow.AddTabMenus(menu);
			menu.AddSeparator("");
			Utility.AddNGMenuItems(menu, this, NGProjectWindow.NormalTitle, Constants.WikiBaseURL + "#markdown-header-133-ng-remote-project");
		}
	}
}