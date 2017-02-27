using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGComponentsInspector
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGComponentsInspectorWindow : EditorWindow, IHasCustomMenu
	{
		[Serializable]
		public sealed class ComponentContainer
		{
			public Component		component;
			[NonSerialized]
			public GUIContent		content;
			[NonSerialized]
			public Texture			icon;
			[NonSerialized]
			public Texture2D		gameObjectIcon;
			[NonSerialized]
			public SerializedObject	obj;
			[NonSerialized]
			public string[]			componentNames;
			[NonSerialized]
			public Texture2D[]		componentIcons;
			[NonSerialized]
			public int				workingComponentIndex;
			[NonSerialized]
			public bool				useDefaultEditor;

			public	ComponentContainer(Component component)
			{
				this.Init(component);
			}

			public void	Init(Component component)
			{
				this.component = component;
				this.content = new GUIContent(this.component.GetType().Name, "Left or right click to assign.");
				this.icon = Utility.GetIcon(this.component.GetInstanceID());
				this.gameObjectIcon = Utility.GetIcon(this.component.gameObject.GetInstanceID());

				this.obj = new SerializedObject(component);

				Component[]	components = this.component.gameObject.GetComponents<Component>();

				this.componentNames = new string[components.Length];
				this.componentIcons = new Texture2D[components.Length];

				for (int i = 0; i < components.Length; i++)
				{
					if (components[i] == null)
						this.componentNames[i] = (i + 1) + " - NULL";
					else
					{
						this.componentNames[i] = (i + 1) + " - " + components[i].GetType().Name;
						this.componentIcons[i] = Utility.GetIcon(components[i].GetInstanceID());

						if (components[i] == this.component)
							this.workingComponentIndex = i;
					}
				}
			}
		}

		public const string	Title = "ƝƓ Ҁomponents Ɨnspector";
		public const string	ShortTitle = "ƝƓ Ҁom' Ɨnsp'";

		public List<ComponentContainer>	history = new List<ComponentContainer>();

		public int	master = -1;
		public int	slave = -1;

		private HorizontalScrollbar	scrollbar;
		private Vector2				scrollPositionMaster;
		private Vector2				scrollPositionSlave;
		[NonSerialized]
		private GUIStyle			button;

		static	NGComponentsInspectorWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGComponentsInspectorWindow.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGComponentsInspectorWindow.Title, priority = Constants.MenuItemPriority + 340)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGComponentsInspectorWindow>(NGComponentsInspectorWindow.ShortTitle);
		}

		[MenuItem("CONTEXT/Component/Add to " + NGComponentsInspectorWindow.Title)]
		private static void	ContextAddComponent(MenuCommand command)
		{
			NGComponentsInspectorWindow.AddComponent(command.context as Component);
		}

		public static void	AddComponent(Component component)
		{
			NGComponentsInspectorWindow	instance = EditorWindow.GetWindow<NGComponentsInspectorWindow>(NGComponentsInspectorWindow.ShortTitle);

			instance.history.Add(new ComponentContainer(component));
			
			if (instance.master == -1)
				instance.master = instance.history.Count - 1;
			else
				instance.slave = instance.history.Count - 1;

			instance.Show();
		}

		protected virtual void	OnEnable()
		{
			for (int i = 0; i < this.history.Count; i++)
			{
				if (this.history[i].component != null)
					this.history[i].Init(this.history[i].component);
				else
					this.history.RemoveAt(i--);
			}

			Undo.undoRedoPerformed += this.Repaint;
		}

		protected virtual void	OnDisable()
		{
			Undo.undoRedoPerformed -= this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			if (this.button == null)
			{
				this.button = new GUIStyle(GUI.skin.button);
				this.button.padding.left = 16;
				this.scrollbar = new HorizontalScrollbar(0F, 18F, this.position.height);
				this.scrollbar.interceiptEvent = false;
			}

			float	totalWidth = 0F;

			for (int i = 0; i < this.history.Count; i++)
				totalWidth += GUI.skin.button.CalcSize(this.history[i].content).x + 20F + 4F; // Remove big texture width, because Button calculates using the whole height + Spacing

			this.scrollbar.realWidth = totalWidth;
			this.scrollbar.SetSize(this.position.width);
			this.scrollbar.OnGUI();

			EditorGUILayout.BeginHorizontal();
			{
				for (int i = 0; i < this.history.Count; i++)
				{
					float	w = GUI.skin.button.CalcSize(this.history[i].content).x + 20F; // Remove big texture width, because Button calculates using the whole height.
					Rect	r = GUILayoutUtility.GetRect(0F, 0F, GUI.skin.button, GUILayout.Width(w), GUILayout.Height(EditorGUIUtility.singleLineHeight));

					r.x -= this.scrollbar.offsetX;

					if (GUI.Button(r, this.history[i].content, this.button) == true)
					{
						if (Event.current.button == 0)
							this.master = i;
						else
							this.slave = i;
					}

					if (Event.current.type == EventType.Repaint)
					{
						r.x += 5F;
						r.width = r.height;
						GUI.DrawTexture(r, this.history[i].icon);
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(this.scrollbar.maxHeight);

			Rect	r2 = GUILayoutUtility.GetLastRect();
			float	half = this.position.width * .5F;

			EditorGUI.DrawRect(GUILayoutUtility.GetRect(this.position.width, 2F), Color.black);

			EditorGUILayout.BeginHorizontal();
			{
				this.scrollPositionMaster = GUILayout.BeginScrollView(this.scrollPositionMaster, GUILayout.Width(half));
				{
					if (0 <= this.master && this.master < this.history.Count)
					{
						if (this.history[this.master].component == null)
						{
							this.history.RemoveAt(this.master);
							this.master = -1;
						}
						else
							this.DrawComponent(this.history[this.master]);
					}
					else
						EditorGUILayout.LabelField("Add a Component here from a Component's context menu or left click on the history above.", GeneralStyles.BigCenterText);
				}
				GUILayout.EndScrollView();

				EditorGUI.DrawRect(GUILayoutUtility.GetRect(1F, 1F, 0F, float.MaxValue), Color.black);

				this.scrollPositionSlave = EditorGUILayout.BeginScrollView(this.scrollPositionSlave, GUILayout.Width(half));
				{
					if (0 <= this.slave && this.slave < this.history.Count)
					{
						if (this.history[this.slave].component == null)
						{
							this.history.RemoveAt(this.slave);
							this.slave = -1;
						}
						else
							this.DrawComponent(this.history[this.slave]);
					}
					else
						EditorGUILayout.LabelField("Add a Component here from a Component's context menu or right click on the history above.", GeneralStyles.BigCenterText);
				}
				EditorGUILayout.EndScrollView();

				r2.height = 48F;
				r2.width = this.position.width;

				if (Event.current.type == EventType.DragUpdated &&
					r2.Contains(Event.current.mousePosition) == true)
				{
					if (typeof(Component).IsAssignableFrom(DragAndDrop.objectReferences[0].GetType()) == true)
						DragAndDrop.visualMode = DragAndDropVisualMode.Move;
					else
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
					Event.current.Use();
				}
				else if (Event.current.type == EventType.DragPerform &&
						 r2.Contains(Event.current.mousePosition) == true)
				{
					DragAndDrop.AcceptDrag();

					ComponentContainer	container = new ComponentContainer(DragAndDrop.objectReferences[0] as Component);

					this.history.Add(container);

					if (Event.current.mousePosition.x < half)
						this.master = this.history.Count - 1;
					else
						this.slave = this.history.Count - 1;

					DragAndDrop.PrepareStartDrag();
					Event.current.Use();
				}

				if (Event.current.type == EventType.Repaint &&
					DragAndDrop.objectReferences.Length > 0)
				{
					r2.width = half;
					Utility.DropZone(r2, "Drop here");

					r2.x = r2.width;
					Utility.DropZone(r2, "Drop here");
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void	DrawComponent(ComponentContainer container)
		{
			Editor	e = Editor.CreateEditor(container.component);

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(2F);

				Rect	r = GUILayoutUtility.GetRect(0F, 16F, GUILayout.Width(20F));

				GUI.DrawTexture(r, container.gameObjectIcon);

				NGEditorGUILayout.PingObject(container.component.name, container.component.gameObject, GUILayout.Height(16F));

				container.useDefaultEditor = GUILayout.Toggle(container.useDefaultEditor, new GUIContent("No Custom Editor", "Use default editor."), GeneralStyles.ToolbarToggle, GUILayout.ExpandWidth(false));
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(4F);

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(2F);

				Rect	r = GUILayoutUtility.GetRect(0F, 16F, GUILayout.Width(20F));

				if (container.componentIcons[container.workingComponentIndex] != null)
					GUI.DrawTexture(r, container.componentIcons[container.workingComponentIndex]);

				GUILayout.Space(5F);

				r = GUILayoutUtility.GetRect(0F, 16F, GUILayout.Width(16F));

				int	enabled = EditorUtility.GetObjectEnabled(container.component);
				if (enabled != -1)
				{
					EditorGUI.BeginChangeCheck();
					EditorGUI.Toggle(r, enabled == 1);
					if (EditorGUI.EndChangeCheck() == true)
						EditorUtility.SetObjectEnabled(container.component, enabled != 1);
				}

				EditorGUI.BeginChangeCheck();
				int	n = EditorGUILayout.Popup(container.workingComponentIndex, container.componentNames);
				if (EditorGUI.EndChangeCheck() == true)
				{
					Component[]	components = container.component.gameObject.GetComponents<Component>();

					if (n < components.Length)
						container.Init(components[n]);
				}
			}
			EditorGUILayout.EndHorizontal();

			// Exception for Transform, its Editor messes the GUI up.
			if (container.useDefaultEditor == true || container.component is Transform)
				e.DrawDefaultInspector();
			else
				e.OnInspectorGUI();
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGComponentsInspectorWindow.Title, Constants.WikiBaseURL + "#markdown-header-113-ng-components-inspector");
		}
	}
}