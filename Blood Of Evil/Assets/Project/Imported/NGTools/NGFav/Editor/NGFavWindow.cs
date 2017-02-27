using NGTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace NGToolsEditor.NGFav
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGFavWindow : EditorWindow, IHasCustomMenu
	{
		public const string	Title = "ƝƓ Ḟav";
		public const char	FavSeparator = ':';
		public const char	SaveSeparator = ',';
		public const char	SaveSeparatorCharPlaceholder = (char)4;
		public const float	FavSpacing = 5F;
		public const int	ForceRepaintRefreshTick = 100;
		public const string	DefaultSaveName = "default";

		public static int				UpdateHierarchyCounter = 0;
		public static List<GameObject>	RootObjects
		{
			get
			{
				if (NGFavWindow.rootObjects.Count == 0)
				{
					GameObject[]	objects = Resources.FindObjectsOfTypeAll<GameObject>();

					for (int i = 0; i < objects.Length; i++)
					{
						if ((objects[i].hideFlags & HideFlags.HideInHierarchy) != 0)
							continue;

						GameObject	root = objects[i].transform.root.gameObject;

						if (NGFavWindow.rootObjects.Contains(root) == false)
							NGFavWindow.rootObjects.Add(root);
					}
				}

				return NGFavWindow.rootObjects;
			}
		}

		private List<HorizontalScrollbar>	horizontalScrolls;
		private int							delayToDelete;
		private ReorderableList				list;

		[SerializeField]
		private Vector2	scrollPosition;
		private double	lastClick;
		[SerializeField]
		private int		currentSave;
		private Vector2	dragOriginPosition;

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGFavWindow.Title + ", clear the favorite or reset the settings.");

		private static bool				avoidMultiAdd;
		private static SectionDrawer	sectionDrawer;
		private static List<GameObject>	rootObjects = new List<GameObject>();

		static	NGFavWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGFavWindow.Title);
			Utility.AddMenuItemPicker("Assets/Add Selection");
			Utility.AddMenuItemPicker("GameObject/Add Selection");

			Preferences.SettingsChanged += NGFavWindow.Preferences_SettingsChanged;
		}

		[MenuItem(Constants.MenuItemPath + NGFavWindow.Title, priority = Constants.MenuItemPriority + 307)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGFavWindow>(NGFavWindow.Title);
		}

		private static void	Preferences_SettingsChanged()
		{
			if (Preferences.Settings != null)
				NGFavWindow.sectionDrawer = new SectionDrawer(NGFavWindow.Title, typeof(NGSettings.FavSettings));
			else
			{
				if (NGFavWindow.sectionDrawer != null)
				{
					NGFavWindow.sectionDrawer.Uninit();
					NGFavWindow.sectionDrawer = null;
				}
			}
		}

		#region Menu Items
		[MenuItem("Assets/Add Selection")]
		private static void	AssetAddSelection(MenuCommand menuCommand)
		{
			NGFavWindow	fav = EditorWindow.GetWindow<NGFavWindow>(NGFavWindow.Title);

			fav.CreateSelection(UnityEditor.Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered));
		}

		[MenuItem("GameObject/Add Selection", priority = 12)]
		private static void	SceneAddSelection(MenuCommand menuCommand)
		{
			if (NGFavWindow.avoidMultiAdd == true)
				return;

			NGFavWindow.avoidMultiAdd = true;

			NGFavWindow	fav = EditorWindow.GetWindow<NGFavWindow>(NGFavWindow.Title);

			fav.CreateSelection(UnityEditor.Selection.objects);
		}

		// Use validation to prevent multi add.
		[MenuItem("GameObject/Add Selection", true)]
		private static bool	ValidateAddSelection()
		{
			NGFavWindow.avoidMultiAdd = false;
			return UnityEditor.Selection.activeObject is GameObject;
		}

		[MenuItem("Edit/Selection/Load Selection 1 #F1", priority = -999)]
		private static void	GoToFav1()
		{
			NGFavWindow.SelectFav(0);
		}

		[MenuItem("Edit/Selection/Load Selection 2 #F2", priority = -999)]
		private static void	GoToFav2()
		{
			NGFavWindow.SelectFav(1);
		}

		[MenuItem("Edit/Selection/Load Selection 3 #F3", priority = -999)]
		private static void	GoToFav3()
		{
			NGFavWindow.SelectFav(2);
		}

		[MenuItem("Edit/Selection/Load Selection 4 #F4", priority = -999)]
		private static void	GoToFav4()
		{
			NGFavWindow.SelectFav(3);
		}

		[MenuItem("Edit/Selection/Load Selection 5 #F5", priority = -999)]
		private static void	GoToFav5()
		{
			NGFavWindow.SelectFav(4);
		}

		[MenuItem("Edit/Selection/Load Selection 6 #F6", priority = -999)]
		private static void	GoToFav6()
		{
			NGFavWindow.SelectFav(5);
		}

		[MenuItem("Edit/Selection/Load Selection 7 #F7", priority = -999)]
		private static void	GoToFav7()
		{
			NGFavWindow.SelectFav(6);
		}

		[MenuItem("Edit/Selection/Load Selection 8 #F8", priority = -999)]
		private static void	GoToFav8()
		{
			NGFavWindow.SelectFav(7);
		}

		[MenuItem("Edit/Selection/Load Selection 9 #F9", priority = -999)]
		private static void	GoToFav9()
		{
			NGFavWindow.SelectFav(8);
		}

		[MenuItem("Edit/Selection/Load Selection 10 #F10", priority = -999)]
		private static void	GoToFav10()
		{
			NGFavWindow.SelectFav(9);
		}

		private static void	SelectFav(int i)
		{
			if (Preferences.Settings == null)
				return;

			NGFavWindow[]	favs = Resources.FindObjectsOfTypeAll<NGFavWindow>();

			if (favs.Length == 0)
				return;

			NGFavWindow	fav = favs[0];

			if (fav.currentSave >= 0 &&
				fav.currentSave < Preferences.Settings.fav.favorites.Count &&
				Preferences.Settings.fav.favorites[fav.currentSave].favorites.Count > i)
			{
				for (int j = 0; j < Preferences.Settings.fav.favorites[fav.currentSave].favorites[i].refs.Count; j++)
				{
					if (Preferences.Settings.fav.favorites[fav.currentSave].favorites[i].refs[j].@object == null)
						Preferences.Settings.fav.favorites[fav.currentSave].favorites[i].refs[j].TryReconnect();
				}

				Object[]	selection = Preferences.Settings.fav.favorites[fav.currentSave].favorites[i].refs.Where(si => si.@object != null).Select((si) => si.@object).ToArray();

				if (selection.Length > 0)
				{
					UnityEditor.Selection.objects = selection;

					if (UnityEditor.Selection.activeGameObject != null)
					{
						EditorGUIUtility.PingObject(UnityEditor.Selection.activeGameObject);

						if (SceneView.lastActiveSceneView != null)
						{
							PrefabType	type = PrefabUtility.GetPrefabType(UnityEditor.Selection.activeGameObject);
							if (type != PrefabType.ModelPrefab &&
								type != PrefabType.Prefab)
							{
								SceneView.lastActiveSceneView.FrameSelected();
							}
						}
					}
				}

				fav.list.index = i;
				fav.Repaint();
			}
		}
		#endregion

		protected virtual void	OnEnable()
		{
			this.wantsMouseMove = true;

			this.minSize = new Vector2(this.minSize.x, 0F);

			this.horizontalScrolls = new List<HorizontalScrollbar>();
			this.delayToDelete = -1;

			this.list = new ReorderableList(null, typeof(GameObject), true, false, false, false);
#if UNITY_5
			this.list.showDefaultBackground = false;
#endif
			this.list.headerHeight = 0F;
			this.list.footerHeight = 0F;
			this.list.drawElementCallback = this.DrawElement;
			this.list.onReorderCallback = (ReorderableList list) => { Preferences.InvalidateSettings(); };

			Preferences.SettingsChanged += this.Repaint;
			Utility.RegisterIntervalCallback(this.Repaint, NGFavWindow.ForceRepaintRefreshTick);
			NGEditorApplication.ChangeScene += this.ReconnectOnChangeScene;
			EditorApplication.hierarchyWindowChanged += this.ResetRootObjects;
			Undo.undoRedoPerformed += this.Repaint;
		}

		protected virtual void	OnDisable()
		{
			Preferences.SettingsChanged -= this.Repaint;
			Utility.UnregisterIntervalCallback(this.Repaint);
			NGEditorApplication.ChangeScene -= this.ReconnectOnChangeScene;
			EditorApplication.hierarchyWindowChanged -= this.ResetRootObjects;
			Undo.undoRedoPerformed -= this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			if (Preferences.Settings == null)
			{
				GUILayout.Label(string.Format(LC.G("RequiringConfigurationFile"), NGFavWindow.Title));
				if (GUILayout.Button(LC.G("ShoWPreferencesWindow")) == true)
					Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);
				return;
			}

			FreeOverlay.First(this, NGFavWindow.Title + " is restrained to:\n" +
							  "• " + FreeConstants.MaxFavorites + " favorites.\n" +
							  "• " + FreeConstants.MaxSelectionPerFavorite + " selections per favorite.\n" +
							  "• " + FreeConstants.MaxAssetPerSelection+ " assets per selection.");

			// Guarantee there is always one in the list.
			if (Preferences.Settings.fav.favorites.Count == 0)
				Preferences.Settings.fav.favorites.Add(new Favorites() { name = "default" });

			this.currentSave = Mathf.Clamp(this.currentSave, 0, Preferences.Settings.fav.favorites.Count - 1);

			Favorites	fav = Preferences.Settings.fav.favorites[this.currentSave];

			this.list.list = fav.favorites;

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button("", GeneralStyles.ToolbarDropDown, GUILayout.Width(20F)) == true)
				{
					GenericMenu	menu = new GenericMenu();

					for (int i = 0; i < Preferences.Settings.fav.favorites.Count; i++)
						menu.AddItem(new GUIContent((i + 1) + " - " + Preferences.Settings.fav.favorites[i].name), i == this.currentSave, this.SwitchFavorite, i);

					menu.AddSeparator("");
					menu.AddItem(new GUIContent(LC.G("Add")), false, this.AddFavorite);

					Rect	r = GUILayoutUtility.GetLastRect();
					r.y += 16F;
					menu.DropDown(r);
					GUI.FocusControl(null);
				}

				EditorGUI.BeginChangeCheck();
				fav.name = EditorGUILayout.TextField(fav.name, GeneralStyles.ToolbarTextField, GUILayout.ExpandWidth(true));
				if (EditorGUI.EndChangeCheck() == true)
					Preferences.InvalidateSettings();

				if (GUILayout.Button(LC.G("Clear"), GeneralStyles.ToolbarButton) == true && ((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(LC.G("NGFav_ClearSave"), string.Format(LC.G("NGFav_ClearSaveQuestion"), fav.name), LC.G("Yes"), LC.G("No")) == true))
				{
					Undo.RecordObject(Preferences.Settings, "Clear favorite");
					fav.favorites.Clear();
					Preferences.InvalidateSettings();
					this.Focus();
					return;
				}

				EditorGUI.BeginDisabledGroup(Preferences.Settings.fav.favorites.Count <= 1);
				if (GUILayout.Button(LC.G("Erase"), GeneralStyles.ToolbarButton) == true && ((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(LC.G("NGFav_EraseSave"), string.Format(LC.G("NGFav_EraseSaveQuestion"), fav.name), LC.G("Yes"), LC.G("No")) == true))
				{
					Undo.RecordObject(Preferences.Settings, "Erase favorite");
					Preferences.Settings.fav.favorites.RemoveAt(this.currentSave);
					this.currentSave = Mathf.Clamp(this.currentSave, 0, Preferences.Settings.fav.favorites.Count - 1);
					this.list.list = fav.favorites;
					Preferences.InvalidateSettings();
					this.Focus();
					return;
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();

			Rect	overallDropZone = this.position;

			overallDropZone.x = 0F;
			overallDropZone.y = 0F;
			overallDropZone.height = 16F;

			// Drop zone to add a new selection.
			if (Event.current.type == EventType.Repaint &&
				DragAndDrop.objectReferences.Length > 0)
			{
				Utility.DropZone(overallDropZone, "Create new selection");
				this.Repaint();
			}
			else if (Event.current.type == EventType.DragUpdated &&
					 overallDropZone.Contains(Event.current.mousePosition) == true)
			{
				if (DragAndDrop.objectReferences.Length > 0)
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			}
			else if (Event.current.type == EventType.DragPerform &&
					 overallDropZone.Contains(Event.current.mousePosition) == true)
			{
				DragAndDrop.AcceptDrag();

				this.CreateSelection(DragAndDrop.objectReferences);

				DragAndDrop.PrepareStartDrag();
				Event.current.Use();
			}

			this.errorPopup.OnGUILayout();

			if (this.currentSave >= 0)
			{
				this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
				{
					try
					{
						while (this.horizontalScrolls.Count < fav.favorites.Count)
							this.horizontalScrolls.Add(new HorizontalScrollbar(0F, 0F, this.position.width, 4F, 0F));

						this.list.DoLayoutList();
					}
					catch (Exception ex)
					{
						this.errorPopup.exception = ex;
						InternalNGDebug.LogFileException(ex);
					}
					finally
					{
						Utility.content.tooltip = string.Empty;
					}
				}
				EditorGUILayout.EndScrollView();

				if (this.delayToDelete != -1)
				{
					Undo.RecordObject(Preferences.Settings, "Delete favorite");
					fav.favorites.RemoveAt(this.delayToDelete);
					this.delayToDelete = -1;
				}
			}

			if (Event.current.type == EventType.MouseDown)
			{
				DragAndDrop.PrepareStartDrag();
				this.dragOriginPosition = Vector2.zero;
			}

			FreeOverlay.Last();
		}

		private void	SwitchFavorite(object data)
		{
			Undo.RecordObject(this, "Switch favorite");
			this.currentSave = Mathf.Clamp((int)data, 0, Preferences.Settings.fav.favorites.Count - 1);
		}

		private void	AddFavorite()
		{
			if (FreeConstants.CheckMaxFavorites(Preferences.Settings.fav.favorites.Count) == true)
			{
				Undo.RecordObject(this, "Add favorite");
				Preferences.Settings.fav.favorites.Add(new Favorites() { name = "Favorite " + (Preferences.Settings.fav.favorites.Count + 1) });
				this.currentSave = Preferences.Settings.fav.favorites.Count - 1;
				Preferences.InvalidateSettings();
			}
		}

		private void	CreateSelection(Object[] objects)
		{
			AssetsSelection	selection = new AssetsSelection(objects);

			if (selection.refs.Count > 0)
			{
				if (FreeConstants.CheckMaxAssetsPerSelection(selection.refs.Count) == true &&
					FreeConstants.CheckMaxSelections(Preferences.Settings.fav.favorites[this.currentSave].favorites.Count) == true)
				{
					Undo.RecordObject(Preferences.Settings, "Add Selection as favorite");
					Preferences.Settings.fav.favorites[this.currentSave].favorites.Add(selection);
					Preferences.InvalidateSettings();
				}
			}
		}

		private void	ReconnectOnChangeScene()
		{
			if (Preferences.Settings == null)
				return;

			Favorites	fav = Preferences.Settings.fav.favorites[this.currentSave];

			NGFavWindow.rootObjects.Clear();

			for (int i = 0; i < fav.favorites.Count; i++)
			{
				for (int j = 0; j < fav.favorites[i].refs.Count; j++)
				{
					if (fav.favorites[i].refs[j].@object == null)
						fav.favorites[i].refs[j].TryReconnect();
				}
			}

			this.Repaint();
		}

		private void	DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			Favorites	fav = Preferences.Settings.fav.favorites[this.currentSave];
			float		x = rect.x;
			float		width = rect.width;

			if (rect.Contains(Event.current.mousePosition) == true)
			{
				float	totalWidth = 0F;

				for (int i = 0; i < fav.favorites[index].refs.Count; i++)
				{
					if (fav.favorites[index].refs[i].@object == null)
					{
						if (fav.favorites[index].refs[i].hierarchy.Count > 0)
							Utility.content.text = (string.IsNullOrEmpty(fav.favorites[index].refs[i].resolverAssemblyQualifiedName) == false ? "(R)" : "") + fav.favorites[index].refs[i].hierarchy[fav.favorites[index].refs[i].hierarchy.Count - 1];
						else
							Utility.content.text = (string.IsNullOrEmpty(fav.favorites[index].refs[i].resolverAssemblyQualifiedName) == false ? "(R)" : "") + fav.favorites[index].refs[i].assetPath;
					}
					else
					{
						Utility.content.text = fav.favorites[index].refs[i].@object.name;

						if (Utility.GetIcon(fav.favorites[index].refs[i].@object.GetInstanceID()) != null)
							totalWidth += rect.height;
					}

					totalWidth += GUI.skin.label.CalcSize(Utility.content).x + NGFavWindow.FavSpacing;
				}

				if (index < 10)
				{
					if (index < 9)
						totalWidth += 24F;
					else
						totalWidth += 22F; // Number 10 is centralized.
				}

				if (fav.favorites[index].refs.Count > 1)
				{
					Utility.content.text = "(" + fav.favorites[index].refs.Count + ")";
					totalWidth += GUI.skin.label.CalcSize(Utility.content).x;
				}

				this.horizontalScrolls[index].realWidth = totalWidth;
				this.horizontalScrolls[index].SetPosition(rect.x, rect.y);
				this.horizontalScrolls[index].SetSize(rect.width);
				this.horizontalScrolls[index].OnGUI();

				this.Repaint();

				rect.width = 20F;
				rect.x = x + width - rect.width;
				rect.y += 4F;
				rect.height -= 4F;

				if (GUI.Button(rect, "X") == true)
				{
					this.delayToDelete = index;
					return;
				}

				rect.y -= 4F;
				rect.height += 4F;
				rect.x = x;
				rect.width = width;
			}

			rect.x -= this.horizontalScrolls[index].offsetX;

			if (index <= 9)
			{
				rect.width = 24F;
				Utility.content.text = "#" + (index + 1);
				Utility.content.tooltip = string.Format(LC.G("NGFav_ShortcutTooltip"), index + 1);
				if (index <= 8)
					EditorGUI.LabelField(rect, Utility.content, GeneralStyles.HorizontalCenteredText);
				else
				{
					rect.width = 27F;
					rect.x -= 4F;
					EditorGUI.LabelField(rect, Utility.content, GeneralStyles.HorizontalCenteredText);
					rect.x += 4F;
				}
				Utility.content.tooltip = string.Empty;
				rect.x += rect.width;
				rect.width = width - rect.x;
			}

			if (fav.favorites[index].refs.Count >= 2)
			{
				Utility.content.text = "(" + fav.favorites[index].refs.Count + ")";
				rect.width = GeneralStyles.HorizontalCenteredText.CalcSize(Utility.content).x;
				GUI.Label(rect, Utility.content, GeneralStyles.HorizontalCenteredText);
				rect.x += rect.width;
				rect.width = width - rect.x;
			}

			Rect	dropZone = rect;
			dropZone.xMin = dropZone.xMax - dropZone.width / 3F;

			for (int i = 0; i < fav.favorites[index].refs.Count; i++)
			{
				SelectionItem	selectionItem = fav.favorites[index].refs[i];;
				if (selectionItem.@object == null)
					selectionItem.TryReconnect();

				Texture	icon = null;

				Utility.content.tooltip = selectionItem.resolverFailedError;

				if (selectionItem.@object == null)
				{
					GUI.enabled = false;

					if (selectionItem.hierarchy.Count > 0)
						Utility.content.text = (string.IsNullOrEmpty(selectionItem.resolverAssemblyQualifiedName) == false ? "(R)" : "") + selectionItem.hierarchy[selectionItem.hierarchy.Count - 1];
					else
						Utility.content.text = (string.IsNullOrEmpty(selectionItem.resolverAssemblyQualifiedName) == false ? "(R)" : "") + selectionItem.assetPath;
				}
				else
				{
					Utility.content.text = selectionItem.@object.name;
					icon = Utility.GetIcon(selectionItem.@object.GetInstanceID());
				}

				if (icon != null)
				{
					rect.width = rect.height;
					GUI.DrawTexture(rect, icon);
					rect.x += rect.width;
				}

				rect.width = GeneralStyles.HorizontalCenteredText.CalcSize(Utility.content).x;
				using (ColorContentRestorer.Get(string.IsNullOrEmpty(selectionItem.resolverFailedError) == false, Color.red))
				{
					GUI.Label(rect, Utility.content, GeneralStyles.HorizontalCenteredText);
					Utility.content.tooltip = string.Empty;
				}

				GUI.enabled = true;

				if (selectionItem.@object != null)
				{
					if (Event.current.type == EventType.MouseDrag &&
						(this.dragOriginPosition - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance &&
						DragAndDrop.GetGenericData("t") as Object == selectionItem.@object)
					{
						DragAndDrop.StartDrag("Drag favorite");
						Event.current.Use();
					}
					else if (Event.current.type == EventType.MouseDown &&
							 rect.Contains(Event.current.mousePosition) == true)
					{
						this.dragOriginPosition = Event.current.mousePosition;

						DragAndDrop.PrepareStartDrag();
						// Add this data to force drag on this object only because user has click down on it.
						DragAndDrop.SetGenericData("t", selectionItem.@object);
						DragAndDrop.objectReferences = new Object[] { selectionItem.@object };

						Event.current.Use();
					}
					else if (Event.current.type == EventType.MouseUp &&
							 rect.Contains(Event.current.mousePosition) == true)
					{
						DragAndDrop.PrepareStartDrag();

						if (Event.current.button == 0 && (int)Event.current.modifiers == ((int)Preferences.Settings.fav.deleteModifiers >> 1))
						{
							Undo.RecordObject(Preferences.Settings, "Delete element in favorite");
							fav.favorites[index].refs.RemoveAt(i);

							if (fav.favorites[index].refs.Count == 0)
								this.delayToDelete = index;
						}
						else if (Event.current.button == 1 ||
								 Preferences.Settings.fav.changeSelection == NGSettings.FavSettings.ChangeSelection.SimpleClick ||
								 ((Preferences.Settings.fav.changeSelection == NGSettings.FavSettings.ChangeSelection.DoubleClick || Preferences.Settings.fav.changeSelection == NGSettings.FavSettings.ChangeSelection.ModifierOrDoubleClick) &&
								  this.lastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup) ||
								 ((Preferences.Settings.fav.changeSelection == NGSettings.FavSettings.ChangeSelection.Modifier || Preferences.Settings.fav.changeSelection == NGSettings.FavSettings.ChangeSelection.ModifierOrDoubleClick) &&
								 // HACK We need to shift the event modifier's value. Bug ref #720211_8cg6m8s7akdbf1r5
								  (int)Event.current.modifiers == ((int)Preferences.Settings.fav.selectModifiers >> 1)))
						{
							NGFavWindow.SelectFav(index);
						}
						else
							EditorGUIUtility.PingObject(fav.favorites[index].refs[i].@object);

						this.lastClick = EditorApplication.timeSinceStartup;

						this.list.index = index;
						this.Repaint();

						Event.current.Use();
					}
				}
				else
				{
					// Clean drag on null object, to prevent starting a drag when passing over non-null one without click down.
					if (Event.current.type == EventType.MouseDown &&
						rect.Contains(Event.current.mousePosition) == true &&
						Preferences.Settings != null)
					{
						if ((int)Event.current.modifiers == ((int)Preferences.Settings.fav.deleteModifiers >> 1))
						{
							Undo.RecordObject(Preferences.Settings, "Delete element in favorite");
							fav.favorites[index].refs.RemoveAt(i);

							if (fav.favorites[index].refs.Count == 0)
								this.delayToDelete = index;

							Event.current.Use();
						}

						DragAndDrop.PrepareStartDrag();
					}
				}

				rect.x += rect.width + NGFavWindow.FavSpacing;

				if (rect.x >= this.position.width)
					break;
			}

			rect.x = x;
			rect.width = width;

			// Drop zone to append new Object to the current selection.
			if (Event.current.type == EventType.Repaint &&
				DragAndDrop.objectReferences.Length > 0 &&
				rect.Contains(Event.current.mousePosition) == true)
			{
				Utility.DropZone(dropZone, "Add to selection");
			}
			else if (Event.current.type == EventType.DragUpdated &&
					 dropZone.Contains(Event.current.mousePosition) == true)
			{
				if (DragAndDrop.objectReferences.Length > 0)
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			}
			else if (Event.current.type == EventType.DragExited)
				DragAndDrop.PrepareStartDrag();
			else if (Event.current.type == EventType.DragPerform &&
					 dropZone.Contains(Event.current.mousePosition) == true)
			{
				DragAndDrop.AcceptDrag();

				for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
				{
					int	j = 0;

					for (; j < fav.favorites[index].refs.Count; j++)
					{
						if (fav.favorites[index].refs[j].@object == DragAndDrop.objectReferences[i])
							break;
					}

					if (j == fav.favorites[index].refs.Count)
					{
						if (FreeConstants.CheckMaxAssetsPerSelection(fav.favorites[index].refs.Count) == true)
						{
							Undo.RecordObject(Preferences.Settings, "Add to favorite");
							fav.favorites[index].refs.Add(new SelectionItem(DragAndDrop.objectReferences[i]));
							Preferences.InvalidateSettings();
						}
						else
							break;
					}
				}

				DragAndDrop.PrepareStartDrag();
				Event.current.Use();
			}

			// Just draw the button in front.
			if (rect.Contains(Event.current.mousePosition) == true)
			{
				rect.width = 20F;
				rect.x = x + width - rect.width;
				rect.y += 4F;
				rect.height -= 4F;

				GUI.Button(rect, "X");
			}
		}

		private void	ResetRootObjects()
		{
			++NGFavWindow.UpdateHierarchyCounter;
			NGFavWindow.rootObjects.Clear();
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGFavWindow.Title, Constants.WikiBaseURL + "#markdown-header-15-ng-fav");
		}
	}
}