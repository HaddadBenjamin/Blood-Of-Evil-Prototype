using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad, Exportable(ExportableAttribute.ArrayOptions.Overwrite | ExportableAttribute.ArrayOptions.Immutable)]
	public class NGHubWindow : EditorWindow, ISettingExportable, IHasCustomMenu
	{
		public const string	Title = "NG Hub";
		public const string	ForceRecreateKey = "NGHubForceRecreate";
		public static Color	DockBackgroundColor = NGHubWindow.GetBackgroundColor();

		public float	height = EditorGUIUtility.singleLineHeight;

		private bool				initialized;
		public NGSettings			settings { get; private set; }
		public List<MethodInfo>		droppableComponents { get; private set; }
		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		public List<HubComponent>	components { get; private set; }

		public bool	dockedAsMenu = false;

		public NGHubExtensionWindow	extensionWindow;

		static	NGHubWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGHubWindow.Title);
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGHubWindow.Title + " as Dock %#H");

			Preferences.SettingsChanged += NGHubWindow.OnSettingsChanged;

			// In the case of NG Hub as dock, the layout won't load it at the second restart. Certainly due to the window's state as Popup, but it does not explain why it only occurs at the second restart.
			EditorApplication.delayCall += () => {
				if (EditorPrefs.GetInt(NGHubWindow.ForceRecreateKey) == 1 && Resources.FindObjectsOfTypeAll<NGHubWindow>().Length == 0)
					NGHubWindow.OpenAsDock();
			};
		}

		[MenuItem(Constants.MenuItemPath + NGHubWindow.Title, priority = Constants.MenuItemPriority + 300)]
		private static void	Open()
		{
			EditorWindow.GetWindow<NGHubWindow>(NGHubWindow.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGHubWindow.Title + " as Dock %#H", priority = Constants.MenuItemPriority + 301)]
		private static void	OpenAsDock()
		{
			NGHubWindow[]	editors = Resources.FindObjectsOfTypeAll<NGHubWindow>();

			for (int i = 0; i < editors.Length; i++)
			{
				if (editors[i].dockedAsMenu == true)
					return;
				editors[i].Close();
			}

			NGHubWindow	window = ScriptableObject.CreateInstance<NGHubWindow>();
			window.dockedAsMenu = true;
			window.ShowPopup();
		}

		private static void	OnSettingsChanged()
		{
			NGHubWindow[]	instances = Resources.FindObjectsOfTypeAll<NGHubWindow>();

			for (int i = 0; i < instances.Length; i++)
			{
				instances[i].OnDisable();
				instances[i].OnEnable();
				instances[i].Repaint();
			}
		}

		private static IEnumerator	CheckNGHubIsNotRestarted()
		{
			int	i = 1;

			while (i > 0)
			{
				NGHubWindow[]	instances = Resources.FindObjectsOfTypeAll<NGHubWindow>();

				if (instances.Length > 0)
					yield break;

				yield return null;
				--i;
			}

			EditorPrefs.SetInt(NGHubWindow.ForceRecreateKey, -2);
		}

		private static Color	GetBackgroundColor()
		{
			if (EditorGUIUtility.isProSkin == true)
				return new Color(41F / 255F, 41F / 255F, 41F / 255F, 1F);
			else
				return new Color(162F / 255F, 162F / 255F, 162F / 255F, 1F);
		}

		protected virtual void	OnEnable()
		{
			if (this.initialized == true || Preferences.Settings == null)
				return;

			try
			{
				this.SetTitle(NGHubWindow.Title);
				this.minSize = Vector2.zero;
				this.droppableComponents = new List<MethodInfo>();

				foreach (Type type in Utility.EachSubClassesOf(typeof(HubComponent)))
				{
					MethodInfo	method = type.GetMethod(HubComponent.CanDropMethod, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

					if (method != null)
						this.droppableComponents.Add(method);
				}

				this.components = new List<HubComponent>();
				Preferences.Settings.hubData.Deserialize(this.components);
				for (int i = 0; i < this.components.Count; i++)
				{
					// In case of corrupted data.
					if (this.components[i] != null && this.components[i].GetType().IsSubclassOf(typeof(HubComponent)) == true)
						this.components[i].Init(this);
					else
					{
						this.components.RemoveAt(i);
						--i;
					}
				}

				this.settings = Preferences.Settings;

				this.initialized = true;
				if (this.dockedAsMenu == true)
					EditorPrefs.SetInt(NGHubWindow.ForceRecreateKey, -1);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}

		protected virtual void	OnDisable()
		{
			if (this.initialized == false)
				return;

			if (this.dockedAsMenu == true)
			{
				EditorPrefs.SetInt(NGHubWindow.ForceRecreateKey, 1);
				Utility.StartBackgroundTask(NGHubWindow.CheckNGHubIsNotRestarted());
			}

			this.initialized = false;

			for (int i = 0; i < this.components.Count; i++)
				this.components[i].Uninit();

			if (this.extensionWindow != null)
				this.extensionWindow.Close();

			this.SaveComponents();
		}

		protected virtual void	OnGUI()
		{
			if (this.initialized == false)
			{
				GUILayout.Label(string.Format(LC.G("RequiringConfigurationFile"), NGHubWindow.Title));

				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(LC.G("ShoWPreferencesWindow")) == true)
					{
						Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);
					}

					// Especially for NG Hub, we need to add a way to manually close the window when the dock mode is failing.
					if (GUILayout.Button("X", GUILayout.Width(16F)) == true)
					{
						this.Close();
					}
				}
				GUILayout.EndHorizontal();

				return;
			}

			if (Event.current.type == EventType.Repaint && this.dockedAsMenu == true)
				EditorGUI.DrawRect(new Rect(0F, 0F, this.position.width, this.position.height), NGHubWindow.DockBackgroundColor);

			EditorGUILayout.BeginHorizontal(GUILayout.Height(this.height));
			{
				this.HandleDrop();

				bool		overflow = false;
				EventType	catchedType = EventType.Used;

				if (this.components.Count == 0)
				{
					Rect	r = this.position;
					r.x = 1F;
					r.y = 1F;
					r.width -= 1F;
					r.height -= 1F;

					if (this.dockedAsMenu == true && Event.current.type == EventType.Repaint)
						Utility.DrawRectDotted(r, this.position, Color.grey, .02F, 0F);

					GUI.Label(r, "Right-click to add Component", GeneralStyles.CenterText);
				}
				else
				{
					for (int i = 0; i < this.components.Count; i++)
					{
						// Catch event from the cropped component.
						if (this.dockedAsMenu == true &&
							Event.current.type != EventType.Repaint &&
							Event.current.type != EventType.Layout &&
							this.extensionWindow != null)
						{
							if (this.extensionWindow.minI == i)
							{
								// Simulate context click, because MouseUp is used, therefore ContextClick is not sent.
								if (Event.current.type == EventType.MouseUp &&
									Event.current.button == 1)
								{
									catchedType = EventType.ContextClick;
								}
								else
									catchedType = Event.current.type;
								Event.current.Use();
							}
						}

						EditorGUILayout.BeginHorizontal();
						{
							this.components[i].OnGUI();
						}
						EditorGUILayout.EndHorizontal();

						if (this.dockedAsMenu == true && Event.current.type == EventType.Repaint)
						{
							Rect	r = GUILayoutUtility.GetLastRect();

							if (r.xMax >= this.position.width)
							{
								// Hide the miserable trick...
								r.xMin -= 2F;
								r.yMin -= 2F;
								r.yMax += 2F;
								r.xMax += 2F;
								EditorGUI.DrawRect(r, NGHubWindow.DockBackgroundColor);

								if (this.extensionWindow == null)
								{
									this.extensionWindow = ScriptableObject.CreateInstance<NGHubExtensionWindow>();
									this.extensionWindow.Init(this);
									this.extensionWindow.ShowPopup();
								}

								this.extensionWindow.minI = i;
								overflow = true;
								break;
							}
						}
					}
				}

				if (this.dockedAsMenu == true &&
					Event.current.type == EventType.Repaint &&
					overflow == false &&
					this.extensionWindow != null)
				{
					this.extensionWindow.Close();
					this.extensionWindow = null;
				}

				if (Event.current.type == EventType.ContextClick ||
					catchedType == EventType.ContextClick)
				{
					this.OpenContextMenu();
				}
			}

			GUILayout.FlexibleSpace();

			EditorGUILayout.EndHorizontal();
		}

		protected virtual void	Update()
		{
			if (this.initialized == false)
			{
				this.OnDisable();
				this.OnEnable();
				return;
			}

			if (this.dockedAsMenu == true)
			{
				Rect	r = Utility.GetEditorMainWindowPos();
#if UNITY_4_5
				float	leftInputsWidth = 300F;
#elif UNITY_4_6 || UNITY_4_7
				float	leftInputsWidth = 330F;
#else
				float	leftInputsWidth = 330F;
#endif

				r.width = r.width * .5F - leftInputsWidth - 60F; // Half window width - Left inputs width - Half Play/Pause/Next
				this.minSize = new Vector2(r.width, 25F);
				this.maxSize = this.minSize;
				this.position = new Rect(r.x + leftInputsWidth, r.y + 2F, this.minSize.x, this.minSize.y);
				this.height = this.position.height - 4F;

				if (this.extensionWindow != null)
				{
					r.x += r.width + 105F; // Width + Play/Pause/Next
#if UNITY_4_5
					r.width += 70F;
#elif UNITY_4_6 || UNITY_4_7 || UNITY_5_0
					r.width += 100F;
#elif UNITY_5_1
					r.width += 45F;
#else
					r.width += 5F;
#endif
					this.extensionWindow.minSize = new Vector2(r.width, this.minSize.y);
					this.extensionWindow.maxSize = this.extensionWindow.minSize;
					this.extensionWindow.position = new Rect(r.x + leftInputsWidth + 10F, this.position.y, this.extensionWindow.minSize.x, this.extensionWindow.minSize.y);
				}
			}
		}

		public void	HandleDrop()
		{
			if (this.droppableComponents.Count > 0 &&
				DragAndDrop.objectReferences.Length > 0 &&
				1.Equals(DragAndDrop.GetGenericData(Utility.DragObjectDataName)) == false)
			{
				for (int i = 0; i < this.droppableComponents.Count; i++)
				{
					if ((bool)this.droppableComponents[i].Invoke(null, null) == true)
					{
						string	name = this.droppableComponents[i].DeclaringType.Name;

						if (name.EndsWith("Component") == true)
							name = name.Substring(0, name.Length - "Component".Length);

						Utility.content.text = name;
						Rect	r = GUILayoutUtility.GetRect(GUI.skin.label.CalcSize(Utility.content).x, this.height, GUI.skin.label);

						if (Event.current.type == EventType.Repaint)
						{
							Utility.DropZone(r, Utility.NicifyVariableName(name));
							this.Repaint();
						}
						else if (Event.current.type == EventType.DragUpdated &&
								 r.Contains(Event.current.mousePosition) == true)
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Move;
						}
						else if (Event.current.type == EventType.DragPerform &&
								 r.Contains(Event.current.mousePosition) == true)
						{
							DragAndDrop.AcceptDrag();

							HubComponent	component = Activator.CreateInstance(this.droppableComponents[i].DeclaringType) as HubComponent;

							if (component != null)
							{
								component.InitDrop(this);
								this.components.Insert(0, component);
								EditorApplication.delayCall += this.Repaint;
								this.SaveComponents();
							}

							DragAndDrop.PrepareStartDrag();
							Event.current.Use();
						}
					}
				}
			}
		}

		public new void	Repaint()
		{
			base.Repaint();
			if (this.extensionWindow != null)
				this.extensionWindow.Repaint();
		}

		public void	OpenContextMenu()
		{
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Add Component"), false, this.OpenAddComponentWizard);
			menu.AddItem(new GUIContent("Edit"), false, () => EditorWindow.GetWindow<NGHubEditorWindow>(true, NGHubEditorWindow.Title, true).Init(this));
			menu.AddItem(new GUIContent("Dock as menu"), this.dockedAsMenu, (this.dockedAsMenu == true) ? new GenericMenu.MenuFunction(this.ConvertToWindow) : this.ConvertToDock);
			menu.AddItem(new GUIContent("Close"), false, this.Close);
			menu.ShowAsContext();

			Event.current.Use();
		}

		public void	ConvertToDock()
		{
			this.Close();

			NGHubWindow	window = ScriptableObject.CreateInstance<NGHubWindow>();
			window.dockedAsMenu = true;
			window.ShowPopup();
		}

		public void	ConvertToWindow()
		{
			this.Close();

			NGHubWindow	 window = EditorWindow.GetWindow<NGHubWindow>();
			window.position = this.position;
			window.Show();
		}

		public void	OpenAddComponentWizard()
		{
			GenericTypesSelectorWizard.Start(NGHubWindow.Title + " - Add Component", typeof(HubComponent), this.OnCreateComponent, true, true, true);
		}

		public void	SaveComponents()
		{
			this.settings.hubData.Serialize(this.components);
			Preferences.InvalidateSettings();
		}

		private void	OnCreateComponent(Type type)
		{
			HubComponent	component = Activator.CreateInstance(type) as HubComponent;
			component.Init(this);

			HubComponentWindow[]	editors = Resources.FindObjectsOfTypeAll<HubComponentWindow>();

			for (int i = 0; i < editors.Length; i++)
				editors[i].Close();

			if (component.hasEditorGUI == true)
			{
				HubComponentWindow	editor = EditorWindow.CreateInstance<HubComponentWindow>();

				editor.SetTitle(component.name);
				editor.position = new Rect(this.position.x, this.position.y + this.height, editor.position.width, editor.position.height);
				editor.Init(this, component);
				editor.ShowPopup();
			}

			this.components.Add(component);
			this.SaveComponents();
			this.Repaint();
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Add Component"), false, this.OpenAddComponentWizard);
			menu.AddItem(new GUIContent("Edit"), false, () => EditorWindow.GetWindow<NGHubEditorWindow>(true, NGHubEditorWindow.Title, true).Init(this));
			menu.AddItem(new GUIContent("Dock as menu"), this.dockedAsMenu, (this.dockedAsMenu == true) ? new GenericMenu.MenuFunction(this.ConvertToWindow) : this.ConvertToDock);
			menu.AddSeparator("");
			Utility.AddNGMenuItems(menu, this, NGHubWindow.Title, Constants.WikiBaseURL + "#markdown-header-110-ng-hub");
		}

		public void	PreExport()
		{
		}

		public void	PreImport()
		{
		}

		public void	PostImport()
		{
			for (int i = 0; i < this.components.Count; i++)
				this.components[i].hub = this;
		}

		protected virtual void	ShowButton(Rect r)
		{
			EditorGUI.BeginChangeCheck();
			GUI.Toggle(r, false, "E", GUI.skin.label);
			if (EditorGUI.EndChangeCheck() == true)
				EditorWindow.GetWindow<NGHubEditorWindow>(true, NGHubEditorWindow.Title, true).Init(this);
		}
	}
}