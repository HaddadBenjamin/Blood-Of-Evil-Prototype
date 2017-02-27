using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor.NGHub
{
	using UnityEngine;

	[InitializeOnLoad, Exportable(ExportableAttribute.ArrayOptions.Overwrite | ExportableAttribute.ArrayOptions.Immutable)]
	public class NGHubWindow : EditorWindow, ISettingExportable, IHasCustomMenu
	{
		public const string	Title = "ƝƓ Ħub";
		public const string	ForceRecreateKey = "NGHub_ForceRecreate";
		[SetColor(41F / 255F, 41F / 255F, 41F / 255F, 1F, 162F / 255F, 162F / 255F, 162F / 255F, 1F)]
		public static Color	DockBackgroundColor = default(Color);

		public float	height = EditorGUIUtility.singleLineHeight;

		private bool				initialized;
		public NGSettings			settings { get; private set; }
		public List<MethodInfo>		droppableComponents { get; private set; }
		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		public List<HubComponent>	components { get; private set; }

		public bool	dockedAsMenu = false;

		public NGHubExtensionWindow	extensionWindow;

		private ErrorPopup	errorPopup = new ErrorPopup("Error occured");

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
		public static void	Open()
		{
			EditorWindow.GetWindow<NGHubWindow>(NGHubWindow.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGHubWindow.Title + " as Dock %#H", priority = Constants.MenuItemPriority + 301)]
		public static void	OpenAsDock()
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
					MethodInfo	method = type.GetMethod(HubComponent.StaticVerifierMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

					if (method != null)
						this.droppableComponents.Add(method);
				}

				this.components = new List<HubComponent>();
				this.RestoreComponents();

				this.settings = Preferences.Settings;

				this.initialized = true;
				if (this.dockedAsMenu == true)
					EditorPrefs.SetInt(NGHubWindow.ForceRecreateKey, -1);
			}
			catch (Exception ex)
			{
				this.errorPopup.exception = ex;
			}

			Undo.undoRedoPerformed += this.RestoreComponents;
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

			Undo.undoRedoPerformed -= this.RestoreComponents;
		}

		protected virtual void	OnGUI()
		{
			if (this.initialized == false)
			{
				GUILayout.Label(string.Format(LC.G("RequiringConfigurationFile"), NGHubWindow.Title));

				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(LC.G("ShoWPreferencesWindow")) == true)
						Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);

					// Especially for NG Hub, we need to add a way to manually close the window when the dock mode is failing.
					if (GUILayout.Button("X", GUILayout.Width(16F)) == true)
						this.Close();
				}
				GUILayout.EndHorizontal();

				return;
			}

			FreeOverlay.First(this, NGHubWindow.Title + " is restrained to " + FreeConstants.MaxHubComponents + " components.");

			if (Event.current.type == EventType.Repaint && this.dockedAsMenu == true)
				EditorGUI.DrawRect(new Rect(0F, 0F, this.position.width, this.position.height), NGHubWindow.DockBackgroundColor);

			EditorGUILayout.BeginHorizontal(GUILayout.Height(this.height));
			{
				if (this.errorPopup.exception != null)
				{
					Rect	r = GUILayoutUtility.GetRect(0F, 0F, GUILayout.Width(115F), GUILayout.Height(this.height + 3F));
					r.x += 1F;
					r.y += 1F;
					this.errorPopup.OnGUIRect(r);
				}

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

					GUI.Label(r, "Right-click to add Component" + (this.dockedAsMenu == true && Application.platform == RuntimePlatform.OSXEditor? " (Dock mode is buggy under OSX)" : ""), GeneralStyles.CenterText);
				}
				else
				{
					Rect	miseryRect = default(Rect);

					if (this.dockedAsMenu == true &&
						this.extensionWindow != null &&
						maxWidth > 0F)
					{
						miseryRect = new Rect(this.maxWidth, 0F, this.position.width - this.maxWidth, this.height + 4F);
						GUI.Button(miseryRect, new GUIContent("", null, "\0"));

						if (miseryRect.Contains(Event.current.mousePosition) == true)
						{
							GUIUtility.hotControl = 0;
						}
					}

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
							try
							{
								this.components[i].OnGUI();
							}
							catch (Exception ex)
							{
								this.errorPopup.exception = ex;
							}
						}
						EditorGUILayout.EndHorizontal();

						if (this.dockedAsMenu == true && Event.current.type == EventType.Repaint)
						{
							Rect	r = GUILayoutUtility.GetLastRect();

							if (r.xMax >= this.position.width)
							{
								this.maxWidth = r.xMin;

								if (this.extensionWindow == null)
								{
									this.extensionWindow = ScriptableObject.CreateInstance<NGHubExtensionWindow>();
									this.extensionWindow.Init(this);
									this.extensionWindow.ShowPopup();
									this.Repaint();
								}

								this.extensionWindow.minI = i;
								overflow = true;
								break;
							}
							else if (overflow == false)
								this.maxWidth = 0F;
						}
					}

					if (this.dockedAsMenu == true &&
						this.extensionWindow != null &&
						maxWidth > 0F)
					{
						// Hide the miserable trick...
						EditorGUI.DrawRect(miseryRect, NGHubWindow.DockBackgroundColor);
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

			FreeOverlay.Last();
		}

		private float	maxWidth = 0F;

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

							if (FreeConstants.CheckMaxHubComponents(this.components.Count) == true)
							{
								HubComponent	component = Activator.CreateInstance(this.droppableComponents[i].DeclaringType) as HubComponent;

								if (component != null)
								{
									component.InitDrop(this);
									this.components.Insert(0, component);
									EditorApplication.delayCall += this.Repaint;
									this.SaveComponents();
								}
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
			GenericMenu	menu = new GenericMenu();

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
			GenericTypesSelectorWizard	wizard = GenericTypesSelectorWizard.Start(NGHubWindow.Title + " - Add Component", typeof(HubComponent), this.OnCreateComponent, true, true);
			wizard.EnableCategories = true;
			wizard.position = new Rect(this.position.x, this.position.y + 60F, wizard.position.width, wizard.position.height);
		}

		public void	SaveComponents()
		{
			Undo.RecordObject(this.settings, "Change HubComponent");
			this.settings.hubData.Serialize(this.components);
			Preferences.InvalidateSettings();
		}

		private void	OnCreateComponent(Type type)
		{
			if (FreeConstants.CheckMaxHubComponents(this.components.Count) == false)
				return;

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

			Undo.RecordObject(this.settings, "Add HubComponent");
			this.components.Add(component);
			this.SaveComponents();
			this.Repaint();
		}

		protected virtual void	ShowButton(Rect r)
		{
			EditorGUI.BeginChangeCheck();
			GUI.Toggle(r, false, "E", GUI.skin.label);
			if (EditorGUI.EndChangeCheck() == true)
				EditorWindow.GetWindow<NGHubEditorWindow>(true, NGHubEditorWindow.Title, true).Init(this);
		}

		private void	RestoreComponents()
		{
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

		void	ISettingExportable.PreExport()
		{
		}

		void	ISettingExportable.PreImport()
		{
		}

		void	ISettingExportable.PostImport()
		{
			for (int i = 0; i < this.components.Count; i++)
				this.components[i].hub = this;
		}
	}
}