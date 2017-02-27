#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif
using NGTools;
using System.Linq;
using UnityEditorInternal;

namespace NGToolsEditor.NGAssetsFinder
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGAssetsFinderWindow : EditorWindow, IHasCustomMenu
	{
		[Serializable]
		private sealed class SceneMatch
		{
			public readonly Object	scene;
			public readonly string	scenePath;
			public int				count;
			public int				prefabCount;

			public	SceneMatch(Object scene, string scenePath)
			{
				this.scene = scene;
				this.scenePath = scenePath;
			}
		}

		private sealed class Asset
		{
			public readonly NGAssetsFinderWindow	assetsFinder;
			public readonly Object		asset;
			public readonly GUIContent	content;
			public readonly List<Asset>	children;

			private bool	open;

			public	Asset(NGAssetsFinderWindow assetsFinder, Object asset)
			{
				this.assetsFinder = assetsFinder;
				this.asset = asset;
				if (this.asset is Component)
					this.content = new GUIContent(this.asset.GetType().Name + " (Component)", Utility.GetIcon(this.asset.GetInstanceID()));
				else
					this.content = new GUIContent(this.asset.name, Utility.GetIcon(this.asset.GetInstanceID()));
				this.children = new List<Asset>();

				this.open = true;
			}

			public Asset	Find(Object target)
			{
				if (this.asset == target)
					return this;

				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i].Find(target) != null)
						return this.children[i];
				}

				return null;
			}

			public void	Draw()
			{
				Rect	r = GUILayoutUtility.GetRect(0F, 16F);

				r.xMin = EditorGUIUtility.labelWidth + 4F;

				float	w = r.width;

				r.width = 16F;
				if (this.children.Count > 0)
				{
					r.x += EditorGUI.indentLevel * 16F;
					if (Event.current.type == EventType.MouseUp && r.Contains(Event.current.mousePosition) == true)
					{
						this.open = !this.open;
						GUI.FocusControl(null);
						Event.current.Use();
					}

					r.x -= EditorGUI.indentLevel * 16F;
					EditorGUI.Foldout(r, this.open, string.Empty);
				}

				r.x += r.width;
				r.width = w - r.width;
				if (EditorGUI.ToggleLeft(r, this.content, this.asset == this.assetsFinder.targetAsset) == true)
					this.assetsFinder.targetAsset = this.asset;

				if (this.open == true)
				{
					++EditorGUI.indentLevel;
					for (int i = 0; i < this.children.Count; i++)
						this.children[i].Draw();
					--EditorGUI.indentLevel;
				}
			}

			public int	CountSubAssets()
			{
				int	c = 1;

				for (int i = 0; i < this.children.Count; i++)
					c += this.children[i].CountSubAssets();

				return c;
			}

			public int	CountOpenSubAssets()
			{
				int	c = 1;

				if (this.open == true)
				{
					for (int i = 0; i < this.children.Count; i++)
						c += this.children[i].CountOpenSubAssets();
				}

				return c;
			}
		}

		[Serializable]
		private sealed class Folder
		{
			public string	path;
			public bool		active;

			public	Folder()
			{
			}

			public	Folder(string path, bool active)
			{
				this.path = path;
				this.active = active;
			}
		}

		public enum Options
		{
			InCurrentScene = 1 << 0,
			InProject = 1 << 1,
			NonPublic = 1 << 2,
			ByInstance = 1 << 3,
			ByComponentType = 1 << 4,
			Asset = 1 << 5,
			Prefab = 1 << 6,
			Scene = 1 << 7,
		}

		public const string	Title = "ƝƓ Ⱥssets Ḟinder";
		public const float	SearchHeaderWidth = 100F;
		public const float	DropZoneHeight = 32F;

		//private static Type[]	PotentialBigResultTypes = new Type[] { typeof(Component), typeof(Behaviour), typeof(MonoBehaviour), typeof(Renderer), typeof(MeshRenderer), typeof(Collider), typeof(BoxCollider), typeof(SphereCollider), typeof(AudioSource), typeof(Transform) };

		public bool	showScene;
		public bool	showProject;

		public bool		displaySearchInGameObjects = true;
		public bool		displaySearchInProject = true;
		public Options	searchOptions;

		private bool			canReplace;
		private Object			targetAsset;
		private Object			replaceAsset;
		private List<GameObject>	searchInGameObjects = new List<GameObject>();
		[SerializeField]
		private List<Folder>	searchInFolders = new List<Folder>();

		private TypeMembersExclusion[]	typeExclusions;
		private List<Component>			components = new List<Component>();
		private Type					targetType;
		private BindingFlags			searchFlags;

		private List<ContainerType>			analyzedTypes = new List<ContainerType>();
		private List<AssetMatches>			matchedInstancesInScene = new List<AssetMatches>();
		private List<AssetMatches>			matchedInstancesInProject = new List<AssetMatches>();
		private List<SceneMatch>			matchedScenes = new List<SceneMatch>();
		private List<TypeMembersExclusion>	tme = new List<TypeMembersExclusion>();
		private Dictionary<string, AssetMatches>	scenePrefabMatches = new Dictionary<string, AssetMatches>();

		private Vector2	scrollPosition;

		private AssetMatches	workingAssetMatches;
		[NonSerialized]
		private bool			hasResult;
		private bool			isSearching;
		private int				potentialMatchesCount;
		private int				effectiveMatchesCount;
		private bool			displayResultScenes = true;
		private bool			debugAnalyzedTypes = false;
		private float			searchTime;
		private double			lastClick;
		private float			lastFrameTime;

		private Asset	mainAsset;

		private bool	displayAllAssets;
		private Vector2	allAssetsScrollPosition;

		private int		updatedReferencesCount;
		private bool	replaceOnTarget;
		
		private List<int>	processedFiles = new List<int>(1024);
		private GUIContent	instanceContent = new GUIContent("By Instance", "Look for references of the target.");
		private GUIContent	componentTypeContent = new GUIContent("By Component Type", "Look for Component of the same type as target type.");
		private GUIContent	nonPublicContent = new GUIContent("Hidden Fields/Properties", "Include non-public fields and properties. This option considerably increases the search time.");

#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
		private static MethodInfo	GetRootGameObjectsMethod;

		private List<GameObject>	roots = new List<GameObject>();
#endif

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGAssetsFinderWindow.Title + ".");

		static	NGAssetsFinderWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGAssetsFinderWindow.Title);
			Utility.AddMenuItemPicker("GameObject/Search Game Object");
			Utility.AddMenuItemPicker("Assets/Search Asset");

#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
			NGAssetsFinderWindow.GetRootGameObjectsMethod = typeof(Scene).GetMethod("GetRootGameObjects", new Type[] { typeof(List<GameObject>) });
#endif
		}

		#region Menu Items
		[MenuItem(Constants.MenuItemPath + NGAssetsFinderWindow.Title, priority = Constants.MenuItemPriority + 330)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGAssetsFinderWindow>(false, NGAssetsFinderWindow.Title);
		}

		[MenuItem("CONTEXT/Component/Search Component", priority = 503)]
		private static void	SearchComponent(MenuCommand menuCommand)
		{
			NGAssetsFinderWindow	window = EditorWindow.GetWindow<NGAssetsFinderWindow>(false, NGAssetsFinderWindow.Title);

			window.AssignTargetAndLoadSubAssets(menuCommand.context);
			window.ClearResults();

			if (PrefabUtility.GetPrefabType(menuCommand.context) == PrefabType.Prefab)
				window.searchOptions |= Options.InProject;
			else
				window.searchOptions |= Options.InCurrentScene;
		}

		[MenuItem("GameObject/Search Game Object", priority = 12)]
		private static void	SearchGameObject(MenuCommand menuCommand)
		{
			NGAssetsFinderWindow	window = EditorWindow.GetWindow<NGAssetsFinderWindow>(false, NGAssetsFinderWindow.Title);

			window.AssignTargetAndLoadSubAssets(menuCommand.context);
			window.ClearResults();
			window.searchOptions |= Options.InCurrentScene;
		}

		[MenuItem("Assets/Search Asset")]
		private static void	SearchAsset(MenuCommand menuCommand)
		{
			NGAssetsFinderWindow	window = EditorWindow.GetWindow<NGAssetsFinderWindow>(false, NGAssetsFinderWindow.Title);

			window.AssignTargetAndLoadSubAssets(Selection.activeObject);
			window.ClearResults();
			window.searchOptions |= Options.InProject;
		}
		#endregion

		protected virtual void	OnEnable()
		{
			this.typeExclusions = Utility.CreateInstancesOf<TypeMembersExclusion>();

			if (this.targetAsset != null && this.mainAsset == null)
				this.AssignTargetAndLoadSubAssets(this.targetAsset);

			Utility.LoadEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
			Undo.undoRedoPerformed += this.Repaint;

			// Fake null Unity Object creates problem when replacing, raising an invalid cast due to the wrong nature of "null".
			if (this.replaceAsset == null && object.Equals(this.replaceAsset, null) == false)
				this.replaceAsset = null;
		}

		protected virtual void	OnDisable()
		{
			Utility.SaveEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
			Undo.undoRedoPerformed -= this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			FreeOverlay.First(this, NGAssetsFinderWindow.Title + " is restrained to " + FreeConstants.MaxAssetReplacements + " replacements at once.\n\nYou can replace many times.");

			this.errorPopup.OnGUILayout();

			Undo.RecordObject(this, NGAssetsFinderWindow.Title);

			EditorGUI.BeginDisabledGroup(this.targetAsset == null || this.isSearching == true);
			{
				using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
				{
					if (GUILayout.Button("Search References") == true)
						this.FindReferences();
				}
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(this.isSearching);
			{
				EditorGUILayout.BeginHorizontal(this.canReplace == true ? GUILayout.Height(38F) : GUILayout.Height(19F) );
				{
					EditorGUILayout.BeginVertical(GUILayout.Width(24F));
					{
						if (GUILayout.Button(this.canReplace == true ? "˄" : "˅", GeneralStyles.ToolbarDropDown, GUILayout.Width(24F)) == true)
							this.canReplace = !this.canReplace;

						if (this.canReplace == true)
						{
							EditorGUI.BeginDisabledGroup(this.replaceAsset == null);
							{
								if (GUILayout.Button("⇅", GeneralStyles.BigFontToolbarButton) == true)
								{
									Object	tmp = this.targetAsset;
									this.AssignTargetAndLoadSubAssets(this.replaceAsset);
									this.replaceAsset = tmp;
								}
							}
							EditorGUI.EndDisabledGroup();
						}
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.BeginVertical();
					{
						using (LabelWidthRestorer.Get(90F))
						{
							EditorGUI.BeginChangeCheck();
							Object	newTarget = EditorGUILayout.ObjectField("Find Asset", this.targetAsset, typeof(Object), true);
							if (EditorGUI.EndChangeCheck() == true)
								this.AssignTargetAndLoadSubAssets(newTarget);

							if (this.targetAsset != null && this.mainAsset != null && this.mainAsset.children.Count > 0)
							{
								Rect	r = GUILayoutUtility.GetLastRect();

								r.y += r.height;
								r.x = 24F;
								r.width = 24F;
								if (GUI.Button(r, this.displayAllAssets == true ? "˄" : "˅", GeneralStyles.ToolbarDropDown) == true)
									this.displayAllAssets = !this.displayAllAssets;

								r.x += r.width + 4F;
								r.width = EditorGUIUtility.labelWidth - r.x + r.width + 4F;
								GUI.Label(r, "Sub Assets");

								if (this.displayAllAssets == true)
								{
									this.allAssetsScrollPosition = EditorGUILayout.BeginScrollView(this.allAssetsScrollPosition, GUILayout.Height(Mathf.Clamp(this.mainAsset.CountOpenSubAssets() * 18F, 0F, 18F * 6F)));
									{
										this.mainAsset.Draw();
									}
									EditorGUILayout.EndScrollView();
								}
								else
								{
									r.x += r.width;
									r.width = this.position.width - r.x;
									GUI.Label(r, this.mainAsset.CountSubAssets() + " objects");

									GUILayoutUtility.GetRect(0F, 14F);
								}
							}

							if (this.canReplace == true)
							{
								bool	allowSceneObjects = true;

								if (this.targetAsset != null)
								{
									PrefabType	targetPrefabType = PrefabUtility.GetPrefabType(this.targetAsset);
									allowSceneObjects = targetPrefabType == PrefabType.None || (targetPrefabType != PrefabType.Prefab && targetPrefabType != PrefabType.ModelPrefab);
								}

								this.replaceAsset = EditorGUILayout.ObjectField("Replace Asset", this.replaceAsset, typeof(Object), allowSceneObjects);
							}
						}

						GUILayout.Space(4F);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					GUILayout.Label("Search Options :", GUILayout.Width(NGAssetsFinderWindow.SearchHeaderWidth));

					if (this.targetAsset is MonoScript)
						this.searchOptions &= ~Options.ByInstance;
					else if (this.targetAsset is Component)
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.Toggle((this.searchOptions & Options.ByInstance) != 0, this.instanceContent, GeneralStyles.ToolbarToggle);
						if (EditorGUI.EndChangeCheck() == true)
							this.searchOptions ^= Options.ByInstance;
					}
					else
						this.searchOptions |= Options.ByInstance;

					if (this.targetAsset is MonoScript)
						this.searchOptions |= Options.ByComponentType;
					else if (this.targetAsset is Component)
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.Toggle((this.searchOptions & Options.ByComponentType) != 0, this.componentTypeContent, GeneralStyles.ToolbarToggle);
						if (EditorGUI.EndChangeCheck() == true)
							this.searchOptions ^= Options.ByComponentType;
					}
					else
						this.searchOptions &= ~Options.ByComponentType;

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle((this.searchOptions & Options.NonPublic) != 0, this.nonPublicContent, GeneralStyles.ToolbarToggle);
					if (EditorGUI.EndChangeCheck() == true)
						this.searchOptions ^= Options.NonPublic;

					if (Conf.DebugMode == Conf.DebugModes.Verbose)
						this.debugAnalyzedTypes = GUILayout.Toggle(this.debugAnalyzedTypes, "DBG Types", GeneralStyles.ToolbarToggle);
				}
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(8F);

				EditorGUILayout.BeginVertical("ButtonLeft");
				{
					EditorGUILayout.BeginHorizontal();
					{
						Rect	r2 = GUILayoutUtility.GetRect(0F, 16F, GUILayout.Width(20F));

						r2.y += 2F;

						EditorGUI.BeginChangeCheck();
						EditorGUI.Foldout(r2, this.showScene, "");
						if (EditorGUI.EndChangeCheck() == true)
							this.showScene = !this.showScene;

						r2.y -= 1F;
						r2.x += 20F;
						r2.width = this.position.width - r2.x;

						EditorGUI.BeginChangeCheck();
						GUI.Toggle(r2, (this.searchOptions & Options.InCurrentScene) != 0, "Scene");
						if (EditorGUI.EndChangeCheck() == true)
							this.searchOptions ^= Options.InCurrentScene;
					}
					EditorGUILayout.EndHorizontal();

					if (this.showScene == true)
					{
						GUILayout.Space(5F);

						if (DragAndDrop.objectReferences.Length > 0)
						{
							int	i = 0;
							for (; i < DragAndDrop.objectReferences.Length; i++)
							{
								if (DragAndDrop.objectReferences[i] is GameObject)
									break;
							}

							if (i < DragAndDrop.objectReferences.Length)
							{
								Rect	rect = GUILayoutUtility.GetRect(0F, NGAssetsFinderWindow.DropZoneHeight);

								if (Event.current.type == EventType.Repaint)
								{
									Utility.DropZone(rect, "Drop Game Object");
									this.Repaint();
								}
								else if (Event.current.type == EventType.DragUpdated &&
											rect.Contains(Event.current.mousePosition) == true)
								{
									DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
								}
								else if (Event.current.type == EventType.DragPerform &&
											rect.Contains(Event.current.mousePosition) == true)
								{
									DragAndDrop.AcceptDrag();

									for (i = 0; i < DragAndDrop.objectReferences.Length; i++)
									{
										if (DragAndDrop.objectReferences[i] is GameObject && this.searchInGameObjects.Contains(DragAndDrop.objectReferences[i] as GameObject) == false)
											this.searchInGameObjects.Add(DragAndDrop.objectReferences[i] as GameObject);
									}

									DragAndDrop.PrepareStartDrag();
									Event.current.Use();
									this.Repaint();
								}
							}
						}

						if (this.searchInGameObjects.Count == 0)
							EditorGUILayout.Foldout(false, "Search Game Objects : Searching in all hierarchy. (Drop Game Object here to filter.)");
						else
						{
							string	label = "Search Game Objects :";
							if (this.displaySearchInGameObjects == false)
								label += " " + string.Join(", ", this.searchInGameObjects.Where(e => e != null).Select(e => e.name).ToArray());

							this.displaySearchInGameObjects = EditorGUILayout.Foldout(this.displaySearchInGameObjects, label);

							if (this.displaySearchInGameObjects == true)
							{
								for (int i = 0; i < this.searchInGameObjects.Count; i++)
								{
									EditorGUILayout.BeginHorizontal();
									{
										EditorGUI.BeginChangeCheck();
										GameObject	asset = EditorGUILayout.ObjectField(this.searchInGameObjects[i], typeof(GameObject), true) as GameObject;
										if (EditorGUI.EndChangeCheck() == true)
										{
											if (AssetDatabase.GetAssetPath(asset) == string.Empty)
												this.searchInGameObjects[i] = asset;
										}

										if (GUILayout.Button("X", GeneralStyles.ToolbarCloseButton, GUILayout.Width(20F)) == true)
										{
											this.searchInGameObjects.RemoveAt(i);
											return;
										}
									}
									EditorGUILayout.EndHorizontal();
								}
							}
						}
					}
				}
				EditorGUILayout.EndVertical();

				GUILayout.Space(5F);

				if (DragAndDrop.objectReferences.Length > 0)
				{
					int	i = 0;
					for (; i < DragAndDrop.objectReferences.Length; i++)
					{
						string	path = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[i]);

						if (path != string.Empty)
						{
							if (Directory.Exists(path) == false)
								path = Path.GetDirectoryName(path);

							if (this.searchInFolders.Exists((e) => e.path == path) == false)
								break;
						}
					}

					if (i < DragAndDrop.objectReferences.Length)
					{
						Rect	rect = GUILayoutUtility.GetRect(0F, NGAssetsFinderWindow.DropZoneHeight);

						if (Event.current.type == EventType.Repaint)
						{
							Utility.DropZone(rect, "Drop Folder");
							this.Repaint();
						}
						else if (Event.current.type == EventType.DragUpdated &&
									rect.Contains(Event.current.mousePosition) == true)
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						}
						else if (Event.current.type == EventType.DragPerform &&
									rect.Contains(Event.current.mousePosition) == true)
						{
							DragAndDrop.AcceptDrag();

							for (i = 0; i < DragAndDrop.objectReferences.Length; i++)
							{
								string	path = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[i]);

								if (Directory.Exists(path) == false)
									path = Path.GetDirectoryName(path);

								if (this.searchInFolders.Exists((e) => e.path == path) == false)
									this.searchInFolders.Add(new Folder(path, true));
							}

							DragAndDrop.PrepareStartDrag();
							Event.current.Use();
							this.Repaint();
						}
					}
				}

				EditorGUILayout.BeginVertical("ButtonLeft");
				{
					EditorGUILayout.BeginHorizontal();
					{
						Rect	r2 = GUILayoutUtility.GetRect(0F, 16F, GUILayout.Width(20F));

						r2.y += 2F;

						EditorGUI.BeginChangeCheck();
						EditorGUI.Foldout(r2, this.showProject, "");
						if (EditorGUI.EndChangeCheck() == true)
							this.showProject = !this.showProject;
						EditorGUI.BeginChangeCheck();

						r2.y -= 1F;
						r2.x += 20F;
						r2.width = this.position.width - r2.x;

						EditorGUI.BeginChangeCheck();
						GUI.Toggle(r2, (this.searchOptions & Options.InProject) != 0, "Project");
						if (EditorGUI.EndChangeCheck() == true)
							this.searchOptions ^= Options.InProject;
					}
					EditorGUILayout.EndHorizontal();

					if (this.showProject == true)
					{
						GUILayout.Space(5F);

						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Label("Search What :", GUILayout.Width(NGAssetsFinderWindow.SearchHeaderWidth));

							EditorGUI.BeginChangeCheck();
							GUILayout.Toggle((this.searchOptions & Options.Asset) != 0, "Asset", GeneralStyles.ToolbarToggle);
							if (EditorGUI.EndChangeCheck() == true)
								this.searchOptions ^= Options.Asset;

							EditorGUI.BeginChangeCheck();
							GUILayout.Toggle((this.searchOptions & Options.Prefab) != 0, "Prefab", GeneralStyles.ToolbarToggle);
							if (EditorGUI.EndChangeCheck() == true)
								this.searchOptions ^= Options.Prefab;

							if (GUI.enabled == true)
							{
								EditorGUI.BeginDisabledGroup(EditorSettings.serializationMode != SerializationMode.ForceText || (AssetDatabase.GetAssetPath(this.targetAsset) == string.Empty && (this.targetAsset is MonoBehaviour) == false));
								{
									if (GUI.enabled == false)
									{
										this.searchOptions &= ~Options.Scene;
										Utility.content.text = "Scene";
										Utility.content.tooltip = string.Empty;
										if (EditorSettings.serializationMode != SerializationMode.ForceText)
											Utility.content.tooltip += "SerializationMode must be set on ForceText to enable this option.\n";
										if (AssetDatabase.GetAssetPath(this.targetAsset) == string.Empty)
											Utility.content.tooltip += "Not an asset, but might be a scene asset.";
										GUILayout.Toggle(false, Utility.content, GeneralStyles.ToolbarToggle);
									}
									else
									{
										EditorGUI.BeginChangeCheck();
										GUILayout.Toggle((this.searchOptions & Options.Scene) != 0, "Scene", GeneralStyles.ToolbarToggle);
										if (EditorGUI.EndChangeCheck() == true)
											this.searchOptions ^= Options.Scene;
									}
								}
								EditorGUI.EndDisabledGroup();
							}
							else
							{
								if (EditorSettings.serializationMode != SerializationMode.ForceText)
									GUILayout.Toggle(false, Utility.content, GeneralStyles.ToolbarToggle);
								else
									GUILayout.Toggle((this.searchOptions & Options.Scene) != 0, "Scene", GeneralStyles.ToolbarToggle);
							}
						}
						EditorGUILayout.EndHorizontal();

						if (this.searchInFolders.Count == 0)
							 EditorGUILayout.Foldout(false, "Search Paths : Searching in all project. (Drop folders here to filter.)");
						else
						{
							string	label = "Search Paths :";
							if (this.displaySearchInProject == false)
								label += " " + string.Join(", ", this.searchInFolders.Where(e => e.active == true).Select(e => e.path).ToArray());

							this.displaySearchInProject = EditorGUILayout.Foldout(this.displaySearchInProject, label);

							if (this.displaySearchInProject == true)
							{
								if (this.searchInFolders.Count == 0)
									EditorGUILayout.LabelField("Searching in all project. (Drop folders here to filter.)");
								else
								{
									for (int i = 0; i < this.searchInFolders.Count; i++)
									{
										EditorGUILayout.BeginHorizontal();
										{
											this.searchInFolders[i].active = GUILayout.Toggle(this.searchInFolders[i].active, this.searchInFolders[i].path, GeneralStyles.ToolbarButtonLeft);
											if (GUILayout.Button("X", GeneralStyles.ToolbarCloseButton, GUILayout.Width(20F)) == true)
											{
												this.searchInFolders.RemoveAt(i);
												break;
											}
										}
										EditorGUILayout.EndHorizontal();
									}
								}
							}
						}
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUI.EndDisabledGroup();

			if ((this.searchOptions & (Options.InProject | Options.InCurrentScene)) == 0)
				EditorGUILayout.HelpBox("Select search in either Scene or Project.", MessageType.Warning);

			GUILayout.Space(5F);

			if (this.canReplace == true)
			{
				EditorGUI.BeginDisabledGroup(this.hasResult == false || this.isSearching == true);
				{
					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
						{
							if (GUILayout.Button("Replace References") == true)
								this.ReplaceReferences(true);
							if (GUILayout.Button("Set all References") == true)
								this.ReplaceReferences(false);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.EndDisabledGroup();

				GUILayout.Space(5F);
			}

			if (this.hasResult == true)
			{
				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					GUILayout.Label("Result");

					using (BgColorContentRestorer.Get(GeneralStyles.HighlightResultButton))
					{
						if (GUILayout.Button("Clear", GeneralStyles.ToolbarButton, GUILayout.Width(70F)) == true)
							this.ClearResults();
					}
				}
				EditorGUILayout.EndHorizontal();

				this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
				{
					if (Conf.DebugMode != Conf.DebugModes.None)
						EditorGUILayout.LabelField("Time:", this.searchTime.ToString());

					EditorGUILayout.LabelField("Potential Matches:", this.potentialMatchesCount.ToString());

					if (this.effectiveMatchesCount >= 1)
					{
						EditorGUILayout.LabelField("Effective Matches:", this.effectiveMatchesCount.ToString());

						if (this.matchedInstancesInScene.Count > 0)
						{
							EditorGUILayout.LabelField("Scene matches", GeneralStyles.ToolbarButton);
							for (int i = 0; i < this.matchedInstancesInScene.Count; i++)
								this.DrawMatches(this.matchedInstancesInScene[i]);
						}

						if (this.matchedInstancesInProject.Count > 0)
						{
							EditorGUILayout.LabelField("Project matches", GeneralStyles.ToolbarButton);
							for (int i = 0; i < this.matchedInstancesInProject.Count; i++)
								this.DrawMatches(this.matchedInstancesInProject[i]);
						}

						if (this.matchedScenes.Count >= 1)
						{
							EditorGUILayout.LabelField("Scenes files matches", GeneralStyles.ToolbarButton);
							this.DrawScenesMatches();
						}
					}
					else
						EditorGUILayout.LabelField("Effective Matches:", "No result");
				}
				EditorGUILayout.EndScrollView();

				GUILayout.FlexibleSpace();
			}

			FreeOverlay.Last();
		}

		private void	DrawScenesMatches()
		{
			Rect	r = EditorGUILayout.GetControlRect(false);

			this.displayResultScenes = EditorGUI.Foldout(r, this.displayResultScenes, new GUIContent("Scenes", InternalEditorUtility.GetIconForFile(".unity")), true);

			if (this.displayResultScenes == true)
			{
				++EditorGUI.indentLevel;
				for (int i = 0; i < this.matchedScenes.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					{
						if (this.matchedScenes[i].count == 0 && this.matchedScenes[i].prefabCount == 0)
							EditorGUILayout.LabelField("No reference");
						else if (this.matchedScenes[i].count >= 1 && this.matchedScenes[i].prefabCount == 0)
							EditorGUILayout.LabelField(this.matchedScenes[i].scene.name, this.matchedScenes[i].count + " reference(s)");
						else if (this.matchedScenes[i].count == 0 && this.matchedScenes[i].prefabCount >= 1)
							EditorGUILayout.LabelField(this.matchedScenes[i].scene.name, this.matchedScenes[i].prefabCount + " prefab(s)");
						else if (this.matchedScenes[i].count >= 1 && this.matchedScenes[i].prefabCount >= 1)
							EditorGUILayout.LabelField(this.matchedScenes[i].scene.name, this.matchedScenes[i].count + " reference(s) and " + this.matchedScenes[i].prefabCount + " prefab(s)");

						if (this.canReplace == true)
						{
							EditorGUI.BeginDisabledGroup(this.matchedScenes[i].count == 0);
							{
								Utility.content.text = "Replace";
								if (GUI.enabled == false && this.matchedScenes[i].prefabCount >= 1)
									Utility.content.tooltip = "You can not replace prefabs inside a scene.";

								if (GUILayout.Button(Utility.content, GUILayout.ExpandWidth(false)) == true &&
									((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 ||
									 EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, "Replacing references in scene is not cancellable.\nAre you sure you want to replace " + this.matchedScenes[i].count + " reference(s) from " + this.matchedScenes[i].scene.name + "?", "Replace", "Cancel") == true))
									this.ReplaceReferencesInScene(this.matchedScenes[i]);

								Utility.content.tooltip = string.Empty;
							}
							EditorGUI.EndDisabledGroup();
						}

						NGEditorGUILayout.PingObject("Ping", this.matchedScenes[i].scene, GUILayout.ExpandWidth(false));
					}
					EditorGUILayout.EndHorizontal();
				}
				--EditorGUI.indentLevel;
			}
		}

		private void	AssignTargetAndLoadSubAssets(Object target)
		{
			this.targetAsset = target;
			if (this.targetAsset != null)
			{
				string	assetPath = AssetDatabase.GetAssetPath(this.targetAsset);

				this.mainAsset = null;

				if (string.IsNullOrEmpty(assetPath) == false && assetPath.EndsWith(".unity", StringComparison.CurrentCultureIgnoreCase) == false)
				{
					Object[]	allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
					List<Asset>	pendingList = new List<Asset>(allAssets.Length);

					// Pre-create and get the main Asset.
					for (int i = 0; i < allAssets.Length; i++)
					{
						pendingList.Add(new Asset(this, allAssets[i]));

						// Create Main Asset.
						if (AssetDatabase.IsMainAsset(allAssets[i]) == true)
							this.mainAsset = pendingList[i];
					}
					
					// In the case of a DLL, LoadAllAssetsAtPath fetches all assets except the main one.
					if (this.mainAsset == null)
						this.mainAsset = new Asset(this, AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(this.targetAsset), typeof(Object)));

					for (int i = 0; i < pendingList.Count; i++)
					{
						if (pendingList[i] == this.mainAsset)
							continue;

						if (AssetDatabase.IsSubAsset(allAssets[i]) == true)
						{
							this.mainAsset.children.Add(pendingList[i]);
							continue;
						}

						Asset	current = pendingList[i];
						{
							Component	component = current.asset as Component;

							if (component != null)
							{
								for (int j = 0; j < pendingList.Count; j++)
								{
									if (pendingList[j].asset == component.gameObject)
									{
										pendingList[j].children.Add(pendingList[i]);
										current = null;
										break;
									}
								}

								InternalNGDebug.Assert(current == null, "Component \"" + component + "\" has no Game Object affiliated.", component);
								continue;
							}
						}

						{
							GameObject	gameObject = current.asset as GameObject;

							if (gameObject != null)
							{
								GameObject	parent = gameObject.transform.parent.gameObject;

								for (int j = 0; j < pendingList.Count; j++)
								{
									if (pendingList[j].asset == parent)
									{
										pendingList[j].children.Add(pendingList[i]);
										current = null;
										break;
									}
								}

								InternalNGDebug.Assert(current == null, "Game Object \"" + gameObject + "\" has no parent affiliated.", gameObject);
								continue;
							}
						}
					}
				}

				if (this.replaceAsset != null && this.CheckAssetCompatibility() == false)
					this.replaceAsset = null;
			}
			else
				this.replaceAsset = null;
		}

		private bool	CheckAssetCompatibility()
		{
			if (this.targetAsset == null || this.replaceAsset == null)
				return true;

			Type	targetType = this.targetAsset.GetType();
			Type	replaceType = this.replaceAsset.GetType();

			if ((targetType != replaceType && replaceType.IsSubclassOf(targetType) == false) ||
				PrefabUtility.GetPrefabType(this.targetAsset) != PrefabUtility.GetPrefabType(this.replaceAsset))
			{
				return false;
			}

			return true;
		}

		private void	ReplaceReferences(bool replaceOnTarget)
		{
			this.updatedReferencesCount = 0;
			this.replaceOnTarget = replaceOnTarget;

			AssetDatabase.StartAssetEditing();

			try
			{
				for (int i = 0; i < this.matchedInstancesInScene.Count; i++)
					this.ReplaceAssetMatches(this.matchedInstancesInScene[i]);

				for (int i = 0; i < this.matchedInstancesInProject.Count; i++)
					this.ReplaceAssetMatches(this.matchedInstancesInProject[i]);
			}
			catch (MaximumReplacementsReachedException)
			{
			}
			catch (Exception ex)
			{
				this.errorPopup.exception = ex;
			}

#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
			if ((this.searchOptions & Options.InCurrentScene) != 0)
				EditorSceneManager.MarkAllScenesDirty();
#endif

			AssetDatabase.StopAssetEditing();

			AssetDatabase.SaveAssets();

			if (updatedReferencesCount == 0)
				EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, "No reference updated.", "OK");
			else if (updatedReferencesCount == 1)
				EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, updatedReferencesCount + " reference updated.", "OK");
			else
				EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, updatedReferencesCount + " references updated.", "OK");

			this.Focus();
		}

		private void	ReplaceAssetMatches(AssetMatches assetMatches)
		{
			Undo.RecordObject(assetMatches.origin, "Replace Asset");

			for (int j = 0; j < assetMatches.matches.Count; j++)
				this.ReplaceMatch(assetMatches.matches[j]);

			for (int j = 0; j < assetMatches.children.Count; j++)
				this.ReplaceAssetMatches(assetMatches.children[j]);

			EditorUtility.SetDirty(assetMatches.origin);
		}

		private void	ReplaceMatch(Match match)
		{
			if (match.subMatches.Count > 0)
			{
				for (int i = 0; i < match.subMatches.Count; i++)
					this.ReplaceMatch(match.subMatches[i]);
			}
			else if (match.arrayIndexes.Count > 0)
			{
				object	rawArray = match.fieldModifier.GetValue(match.instance);

				if (rawArray != null)
				{
					ICollectionModifier	collectionModifier = NGTools.Utility.GetCollectionModifier(rawArray);

					try
					{
						for (int i = 0; i < match.arrayIndexes.Count; i++)
						{
							if (this.CheckTypeCompatibility(this.replaceOnTarget, collectionModifier.Type, collectionModifier.Get(match.arrayIndexes[i])) == true)
							{
								collectionModifier.Set(match.arrayIndexes[i], this.replaceAsset);
								++this.updatedReferencesCount;
								if (FreeConstants.CheckMaxAssetReplacements(this.updatedReferencesCount) == false)
									throw new MaximumReplacementsReachedException();
							}
						}
					}
					finally
					{
						NGTools.Utility.ReturnCollectionModifier(collectionModifier);
					}
				}
			}
			else if (this.CheckTypeCompatibility(this.replaceOnTarget, match.fieldModifier.Type, match.fieldModifier.GetValue(match.instance)) == true)
			{
				match.fieldModifier.SetValue(match.instance, this.replaceAsset);
				++this.updatedReferencesCount;
				if (FreeConstants.CheckMaxAssetReplacements(this.updatedReferencesCount) == false)
					throw new MaximumReplacementsReachedException();
			}
		}

		private bool	CheckTypeCompatibility(bool replaceOnTarget, Type type, object instance)
		{
			return ((replaceOnTarget == true && Object.ReferenceEquals(instance, this.targetAsset) == true) ||
					(replaceOnTarget == false && Object.ReferenceEquals(instance, this.replaceAsset) == false)) &&
					(this.replaceAsset == null || type.IsAssignableFrom(this.replaceAsset.GetType()) == true);
		}

		private void	DrawMatches(AssetMatches assetMatches)
		{
			if (assetMatches.type == AssetMatches.Type.Reference &&
				assetMatches.matches.Count == 0 &&
				assetMatches.children.Count == 0)
			{
				return;
			}

			if (assetMatches.origin == null)
			{
				Rect	r = EditorGUILayout.GetControlRect(false);

				r.width -= 40F;

				if (assetMatches.matches.Count > 0 ||
					assetMatches.children.Count > 0)
				{
					EditorGUI.BeginChangeCheck();
					bool	open = EditorGUI.Foldout(r, assetMatches.Open, assetMatches.content.text + " (NULL)", true);
					if (EditorGUI.EndChangeCheck() == true)
						assetMatches.Open = open;
				}
				else
				{
					using (LabelWidthRestorer.Get(r.width))
					{
						EditorGUI.LabelField(r, assetMatches.content.text + " (NULL)");
					}
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				{
					Rect	r = EditorGUILayout.GetControlRect(false);

					r.width -= 40F;

					if (assetMatches.matches.Count > 0 ||
						assetMatches.children.Count > 0)
					{
						EditorGUI.BeginChangeCheck();
						bool	open = EditorGUI.Foldout(r, assetMatches.Open, assetMatches.content, true);
						if (EditorGUI.EndChangeCheck() == true)
							assetMatches.Open = open;
					}
					else
					{
						using (LabelWidthRestorer.Get(r.width))
						{
							EditorGUI.LabelField(r, assetMatches.content);
						}
					}

					if ((assetMatches.origin is Component) == false)
					{
						r.x += r.width;
						r.width = 40F;

						if (GUI.Button(r, LC.G("Ping")) == true)
						{
							if (Event.current.button != 0 || this.lastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
								Selection.activeObject = assetMatches.origin;
							else
								EditorGUIUtility.PingObject(assetMatches.origin.GetInstanceID());

							this.lastClick = EditorApplication.timeSinceStartup;
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			if (assetMatches.Open == true)
			{
				EditorGUI.BeginChangeCheck();

				++EditorGUI.indentLevel;
				this.workingAssetMatches = assetMatches;
				for (int j = 0; j < assetMatches.matches.Count; j++)
					this.DrawPath(assetMatches.matches[j]);
				for (int i = 0; i < assetMatches.children.Count; i++)
					this.DrawMatches(assetMatches.children[i]);
				--EditorGUI.indentLevel;

				if (EditorGUI.EndChangeCheck() == true && assetMatches.origin != null)
					EditorUtility.SetDirty(assetMatches.origin);
			}
		}

		private bool	CanReplace(AssetMatches assetMatches, Match match, Object asset)
		{
			if (asset is GameObject)
			{
				PrefabType	type = PrefabUtility.GetPrefabType(asset);
				return type != PrefabType.Prefab && type != PrefabType.ModelPrefab && type != PrefabType.None;
			}
			else if (asset is Component)
			{
				PrefabType	type = PrefabUtility.GetPrefabType((asset as Component).gameObject);
				return type != PrefabType.Prefab && type != PrefabType.ModelPrefab && type != PrefabType.None;
			}
			else
			{
				return true;
			}
		}

		private void	DrawPath(Match match)
		{
			if (match.subMatches.Count > 0)
			{
				Rect	r = EditorGUILayout.GetControlRect();

				EditorGUI.BeginChangeCheck();
				bool	open = EditorGUI.Foldout(r, match.Open, match.nicifiedPath, true);
				if (EditorGUI.EndChangeCheck() == true)
					match.Open = open;

				if (match.Open == true)
				{
					++EditorGUI.indentLevel;
					for (int j = 0; j < match.subMatches.Count; j++)
						this.DrawPath(match.subMatches[j]);
					--EditorGUI.indentLevel;
				}
				return;
			}

			if (match.arrayIndexes.Count > 0)
			{
				Rect	r = EditorGUILayout.GetControlRect();

				EditorGUI.BeginChangeCheck();
				bool	open = EditorGUI.Foldout(r, match.Open, match.nicifiedPath, true);
				if (EditorGUI.EndChangeCheck() == true)
					match.Open = open;

				if (match.Open == true)
				{
					++EditorGUI.indentLevel;

					ICollectionModifier	collectionModifier = null;

					if (this.canReplace == true)
					{
						object	rawArray = match.fieldModifier.GetValue(match.instance);

						if (rawArray != null)
							collectionModifier = NGTools.Utility.GetCollectionModifier(rawArray);
					}

					for (int j = 0; j < match.arrayIndexes.Count; j++)
					{
						if (this.canReplace == false)
						{
							using (LabelWidthRestorer.Get(r.width))
							{
								EditorGUILayout.LabelField("#" + match.arrayIndexes[j].ToString());
							}
						}
						else
						{
							Object	reference = null;

							try
							{
								if (collectionModifier != null)
									reference = collectionModifier.Get(match.arrayIndexes[j]) as Object;

								Utility.content.text = "#" + match.arrayIndexes[j].ToString();
								r = EditorGUILayout.GetControlRect(false);
								float	w = GUI.skin.label.CalcSize(Utility.content).x;

								using (ColorContentRestorer.Get(this.replaceAsset != null && collectionModifier.Type.IsAssignableFrom(this.replaceAsset.GetType()) == false, Color.red))
								{
									EditorGUI.PrefixLabel(r, Utility.content);

									r.x += w;
									r.width -= w;
									EditorGUI.BeginChangeCheck();
									Object	o = EditorGUI.ObjectField(r, reference, this.targetType, this.workingAssetMatches.allowSceneObject);
									if (EditorGUI.EndChangeCheck() == true)
									{
										Undo.RecordObject(this.workingAssetMatches.origin, "Match assignment");
										collectionModifier.Set(match.arrayIndexes[j], o);
									}
								}
							}
							catch (Exception ex)
							{
								this.errorPopup.exception = ex;
								EditorGUILayout.LabelField("Error " + match.nicifiedPath + "	" + this.workingAssetMatches + " /" + reference + "-" + match.instance + "	" + ex.Message + "	" + ex.StackTrace);
							}
						}
					}

					if (collectionModifier != null)
						NGTools.Utility.ReturnCollectionModifier(collectionModifier);

					--EditorGUI.indentLevel;
				}
				return;
			}

			++EditorGUI.indentLevel;

			if (this.canReplace == false)
			{
				Rect	r = EditorGUILayout.GetControlRect(false);
				using (LabelWidthRestorer.Get(r.width))
				{
					EditorGUI.LabelField(r, match.nicifiedPath);
				}
			}
			else
			{
				Object	reference = null;

				try
				{
					if (typeof(Object).IsAssignableFrom(match.fieldModifier.Type) == true)
						reference = match.fieldModifier.GetValue<Object>(match.instance);
					else
						throw new Exception("Field \"" + match.fieldModifier.Name + "\" not an Object.");

					Utility.content.text = match.nicifiedPath;
					Rect	r = EditorGUILayout.GetControlRect(false);
					float	w = GUI.skin.label.CalcSize(Utility.content).x;

					using (ColorContentRestorer.Get(this.replaceAsset != null && match.fieldModifier.Type.IsAssignableFrom(this.replaceAsset.GetType()) == false, Color.red))
					{
						EditorGUI.PrefixLabel(r, Utility.content);

						r.x += w;
						r.width -= w;
						EditorGUI.BeginChangeCheck();
						Object	o = EditorGUI.ObjectField(r, reference, this.targetType, this.workingAssetMatches.allowSceneObject);
						if (EditorGUI.EndChangeCheck() == true)
						{
							Undo.RecordObject(this.workingAssetMatches.origin, "Match assignment");
							match.fieldModifier.SetValue(match.instance, o);
						}
					}
				}
				catch (Exception ex)
				{
					this.errorPopup.exception = ex;
					EditorGUILayout.LabelField("Error " + match.nicifiedPath + "	" + this.workingAssetMatches + " /" + reference + "-" + match.instance + "	" + ex.Message + "	" + ex.StackTrace);
				}
			}

			--EditorGUI.indentLevel;
		}

		private IEnumerator	TaskLoadAssets()
		{
			this.isSearching = true;
			this.searchTime = Time.realtimeSinceStartup;
			this.lastFrameTime = Time.realtimeSinceStartup;

			if ((this.searchOptions & Options.InCurrentScene) != 0)
			{
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
				int	maxGameObjects = 0;

				for (int j = 0; j < SceneManager.sceneCount; ++j)
					maxGameObjects += SceneManager.GetSceneAt(j).rootCount;

				for (int j = 0, i = 1; j < SceneManager.sceneCount; ++j)
				{
					NGAssetsFinderWindow.GetRootGameObjectsMethod.Invoke(SceneManager.GetSceneAt(j), new object[] { this.roots });

					for (int k = 0; k < roots.Count; ++k, ++i)
					{
						try
						{
							this.BrowseGameObject(null, roots[k].transform);
						}
						catch (Exception ex)
						{
							this.errorPopup.exception = ex;
							InternalNGDebug.LogException(ex);
						}

						if (Time.realtimeSinceStartup - lastFrameTime >= Preferences.MaxProcessTimePerFrame)
						{
							lastFrameTime += Preferences.MaxProcessTimePerFrame;

							EditorUtility.DisplayProgressBar(NGAssetsFinderWindow.Title + " - Scenes (" + i + " / " + maxGameObjects + ")", roots[k].name, (float)(i / maxGameObjects));

							this.Repaint();

							yield return null;
						}
					}
				}
#else
				HierarchyProperty	prop = new HierarchyProperty(HierarchyType.GameObjects);
				int[]				expanded = new int[0];
				int					maxGameObjects = prop.CountRemaining(expanded);
				int					i = 1;

				while (prop.Next(expanded))
				{
					try
					{
						this.BrowseGameObject(null, (prop.pptrValue as GameObject).transform);
					}
					catch (Exception ex)
					{
						this.errorPopup.exception = ex;
						InternalNGDebug.LogException(ex);
					}

					if (Time.realtimeSinceStartup - lastFrameTime >= Preferences.MaxProcessTimePerFrame)
					{
						lastFrameTime += Preferences.MaxProcessTimePerFrame;

						EditorUtility.DisplayProgressBar(NGAssetsFinderWindow.Title + " - Scene (" + i + " / " + maxGameObjects + ")", (prop.pptrValue as GameObject).name, (float)(i / maxGameObjects));

						yield return null;
					}
				}
#endif
			}

			yield return null;

			if ((this.searchOptions & Options.InProject) != 0)
			{
				this.processedFiles.Clear();

				if (this.searchInFolders.Count == 0)
				{
					IEnumerator	it = this.ProcessFolder("Assets").GetEnumerator();
					while (it.MoveNext());
				}
				else
				{
					for (int i = 0; i < this.searchInFolders.Count; i++)
					{
						if (this.searchInFolders[i].active == true)
						{
							IEnumerator	it = this.ProcessFolder(this.searchInFolders[i].path).GetEnumerator();
							while (it.MoveNext());
						}
					}
				}
			}

			this.searchTime = Time.realtimeSinceStartup - this.searchTime;

			EditorUtility.ClearProgressBar();
		}

		private IEnumerable	ProcessFolder(string path)
		{
			int			max = 0;
			string[][]	files = new string[][] { null, null, null };

			if ((this.searchOptions & Options.Asset) != 0)
				files[0] = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
			if ((this.searchOptions & Options.Prefab) != 0)
				files[1] = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
			if ((this.searchOptions & Options.Scene) != 0)
				files[2] = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);

			for (int i = 0; i < files.Length; i++)
			{
				if (files[i] != null)
					max += files[i].Length;
			}

			for (int i = 0, n = 1; i < files.Length; i++)
			{
				if (files[i] == null)
					continue;

				for (int k = 0; k < files[i].Length; k++, ++n)
				{
					int	l = 0;
					int	hash = files[i][k].GetHashCode();

					if (this.processedFiles.Contains(hash) == true)
						continue;

					this.processedFiles.Add(hash);

					files[i][k] = files[i][k].Replace('\\', '/');

					for (; l < this.searchInFolders.Count; l++)
					{
						if (this.searchInFolders[l].active == false && files[i][k].StartsWith(this.searchInFolders[l].path) == true)
							break;
					}

					if (l < this.searchInFolders.Count)
						continue;

					try
					{
						Object			mainAsset = AssetDatabase.LoadMainAssetAtPath(files[i][k]);
						AssetMatches	assetMatches = new AssetMatches(mainAsset);

						if (files[i][k].EndsWith(".prefab") == true)
						{
							GameObject	prefab = mainAsset as GameObject;
							if (prefab != null)
								this.BrowseGameObject(assetMatches, prefab.transform);
						}
						else if (files[i][k].EndsWith(".unity", StringComparison.CurrentCultureIgnoreCase) == true)
							this.BrowseScene(files[i][k], mainAsset);
						else
						{
							Object[]	assets = AssetDatabase.LoadAllAssetsAtPath(files[i][k]);

							for (int j = 0; j < assets.Length; j++)
							{
								if (assets[j] == null)
									continue;

								if (assets[j] is GameObject)
									this.BrowseGameObject(assetMatches, (assets[j] as GameObject).transform, true);
								else
								{
									this.BrowseObject(assetMatches, assets[j]);
								}
							}
						}

						if (assetMatches.matches.Count >= 1 ||
							assetMatches.children.Count >= 1)
						{
							this.matchedInstancesInProject.Add(assetMatches);
						}
					}
					catch (Exception ex)
					{
						this.errorPopup.exception = ex;
						InternalNGDebug.LogException("Exception thrown on file \"" + files[i][k] + "\".", ex);
					}

					if (Time.realtimeSinceStartup - lastFrameTime >= Preferences.MaxProcessTimePerFrame)
					{
						lastFrameTime += Preferences.MaxProcessTimePerFrame;

						if (k + 1 < files[i].Length)
							EditorUtility.DisplayProgressBar(NGAssetsFinderWindow.Title + " - Project (" + n + " / " + max + ")", files[i][k + 1], (float)n / (float)max);
						else
							break;

						this.Repaint();

						yield return null;
					}
				}
			}
		}

		private void	ReplaceReferencesInScene(SceneMatch match)
		{
			string		id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this.targetAsset));
			if (string.IsNullOrEmpty(id) == true)
				return;

			string		newID = null;
			if (this.replaceAsset != null)
				newID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this.replaceAsset));

			string[]	lines = File.ReadAllLines(match.scenePath);

			if (newID != null && (this.searchOptions & Options.ByComponentType) != 0)
			{
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].StartsWith("  m_Script: {fileID: ") == true && lines[i].IndexOf(id, "  m_Script: {fileID: ".Length) != -1)
					{
						lines[i] = lines[i].Replace(id, newID);
						--match.count;
					}
				}
			}

			for (int i = 0; i < lines.Length; i++)
			{
				// References in array.
				int	position = lines[i].IndexOf("  - {fileID: ");

				if (position != -1)
				{
					int	p = position;

					// Check if there is only spaces before, to prevent matching a string.
					for (; p >= 0; --p)
					{
						if (lines[i][p] != ' ')
							break;
					}

					if (p < 0 && lines[i].IndexOf(id, position) != -1)
					{
						if (newID == null)
							lines[i] = lines[i].Substring(0, position) + "  - {fileID: 0}";
						else
							lines[i] = lines[i].Replace(id, newID);
						--match.count;
						continue;
					}
				}

				if (newID != null && (this.searchOptions & Options.ByInstance) != 0)
				{
					// References in script.
					position = lines[i].IndexOf(", guid: ");
					if (position != -1)
					{
						if (lines[i].IndexOf(id, position) != -1)
						{
							if (newID == null)
								lines[i] = lines[i].Substring(0, lines[i].IndexOf("{fileID: ")) + "{fileID: 0}";
							else
								lines[i] = lines[i].Replace(id, newID);
							--match.count;
						}
					}
				}
			}

			File.WriteAllLines(match.scenePath, lines);
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}

		private void	BrowseScene(string file, Object mainAsset)
		{
			if (EditorSettings.serializationMode != SerializationMode.ForceText)
				return;

			string[]	lines = File.ReadAllLines(file);
			SceneMatch	match = new SceneMatch(mainAsset, file);

			string			assetPath;
			MonoBehaviour	monoBehaviour = this.targetAsset as MonoBehaviour;
			MonoScript		script = null;

			if (monoBehaviour != null)
				script = MonoScript.FromMonoBehaviour(monoBehaviour);

			if (script != null)
				assetPath = AssetDatabase.GetAssetPath(script);
			else
				assetPath = AssetDatabase.GetAssetPath(this.targetAsset);

			string	id = AssetDatabase.AssetPathToGUID(assetPath);

			if ((this.searchOptions & Options.ByComponentType) != 0)
			{
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].StartsWith("  m_Script: {fileID: ") == true && lines[i].IndexOf(id, "  m_Script: {fileID: ".Length) != -1)
						++match.count;

					if (lines[i].StartsWith("  m_ParentPrefab: {fileID: ") == true)
					{
						if ((this.searchOptions & Options.Prefab) != 0)
						{
							for (int j = 0; j < this.matchedInstancesInProject.Count; j++)
							{
								assetPath = AssetDatabase.GetAssetPath(this.matchedInstancesInProject[j].origin);

								string	prefabGUID = AssetDatabase.AssetPathToGUID(assetPath);

								if (string.IsNullOrEmpty(prefabGUID) == false && lines[i].IndexOf(prefabGUID, "  m_ParentPrefab: {fileID: ".Length) != -1)
									++match.count;
							}
						}
						else
						{
							AssetMatches	assetMatches;
							string			prefabGUID = lines[i].Substring(lines[i].IndexOf("guid: ", "  m_ParentPrefab: {fileID: ".Length) + 6, 32);

							if (this.scenePrefabMatches.TryGetValue(prefabGUID, out assetMatches) == false)
							{
								assetPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
								if (string.IsNullOrEmpty(assetPath) == false)
								{
									GameObject	prefab = AssetDatabase.LoadMainAssetAtPath(assetPath) as GameObject;
									if (prefab != null)
									{
										assetMatches = new AssetMatches(mainAsset);
										this.BrowseGameObject(assetMatches, prefab.transform);

										if (assetMatches.children.Count > 0 || assetMatches.matches.Count > 0)
											this.scenePrefabMatches.Add(prefabGUID, assetMatches);
										else
											this.scenePrefabMatches.Add(prefabGUID, null);
									}
								}
							}
							else if (assetMatches != null)
								++match.prefabCount;
						}
					}
				}
			}

			for (int i = 0; i < lines.Length; i++)
			{
				// References in array.
				int	position = lines[i].IndexOf("  - {fileID: ");

				if (position != -1)
				{
					int	p = position;

					// Check if there is only spaces before, to prevent matching a string.
					for (; p >= 0; --p)
					{
						if (lines[i][p] != ' ')
							break;
					}

					if (p < 0 && lines[i].IndexOf(id, position) != -1)
					{
						++match.count;
						continue;
					}
				}

				if ((this.searchOptions & Options.ByInstance) != 0)
				{
					// References in script.
					position = lines[i].IndexOf(", guid: ");
					if (position != -1)
					{
						if (lines[i].IndexOf("m_Script: ") == -1) // Must avoid Component.
							if (lines[i].IndexOf(id, position) != -1)
								++match.count;
					}
				}
			}

			if (match.count >= 1 || match.prefabCount >= 1)
			{
				this.potentialMatchesCount += match.count + match.prefabCount;
				this.effectiveMatchesCount += match.count + match.prefabCount;
				this.matchedScenes.Add(match);
			}
		}

		private void	BrowseGameObject(AssetMatches parent, Transform transform, bool skipComponents = false)
		{
			AssetMatches	assetMatches;

			if (parent == null || parent.origin != transform.gameObject)
			{
				assetMatches = new AssetMatches(transform.gameObject);

				if (parent != null)
					parent.children.Add(assetMatches);
				else
					this.matchedInstancesInScene.Add(assetMatches);
			}
			else
				assetMatches = parent;

			//if (this.targetAsset == transform.gameObject)
			//{
			//	Match	match = new Match();

			//	match.path.Add(transform.gameObject.name);
			//	assetMatches.matches.Add(match);
			//}

			if (skipComponents == false)
			{
				transform.gameObject.GetComponents<Component>(this.components);

				for (int i = 0; i < this.components.Count; i++)
				{
					if (this.components[i] != null)
						this.BrowseObject(assetMatches, this.components[i], true);
				}
			}

			for (int i = 0; i < transform.childCount; i++)
				this.BrowseGameObject(assetMatches, transform.GetChild(i), skipComponents);

			if (assetMatches.matches.Count == 0 &&
				assetMatches.children.Count == 0)
			{
				if (parent == null || parent.origin != transform.gameObject)
				{
					if (parent != null)
						parent.children.Remove(assetMatches);
					else
						this.matchedInstancesInScene.Remove(assetMatches);
				}
			}
		}

		private void	BrowseObject(AssetMatches parent, Object instance, bool isComponent = false)
		{
			if (((this.searchOptions & Options.ByInstance) != 0 && this.targetAsset == instance) ||
				((this.searchOptions & Options.ByComponentType) != 0 && this.targetType == instance.GetType()))
			{
				AssetMatches	assetMatches = new AssetMatches(instance);

				assetMatches.type = AssetMatches.Type.Component;

				if (parent != null)
					parent.children.Add(assetMatches);
				else
					this.matchedInstancesInScene.Add(assetMatches);

				++this.potentialMatchesCount;
				++this.effectiveMatchesCount;
			}

			if ((this.searchOptions & Options.ByInstance) != 0)
			{
				// Can't look for an scene asset in the Project.
				if ((this.searchOptions & (Options.InProject | Options.InCurrentScene)) == Options.InProject &&
					string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this.targetAsset)) == true)
				{
					return;
				}

				AssetMatches	componentMatches = new AssetMatches(instance);

				if (isComponent == true)
					componentMatches.type = AssetMatches.Type.Component;

				if (this.ParseType(componentMatches.origin.GetType(), true) == true)
					this.CheckClass(componentMatches, null, componentMatches.origin.GetType(), componentMatches.origin);

				if (componentMatches.children.Count > 0 || componentMatches.matches.Count > 0)
				{
					if (parent != null)
						parent.children.Add(componentMatches);
					else
						this.matchedInstancesInScene.Add(componentMatches);
				}
			}
		}

		private void	FindReferences()
		{
			MonoScript		script = this.targetAsset as MonoScript;
			Type			lastTargetType = this.targetType;
			BindingFlags	lastSearchFlags = this.searchFlags;

			if (script != null)
				this.targetType = script.GetClass();
			else
				this.targetType = this.targetAsset.GetType();

			if (this.targetType == null)
			{
				InternalNGDebug.LogWarning("Search aborted. The given script \"" + script.name + "\" contains no valid type.");
				return;
			}

			if ((this.searchOptions & (Options.InProject | Options.InCurrentScene)) == 0)
			{
				EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, "You must search into the scene or the project.", "OK");
				return;
			}

			if ((this.searchOptions & (Options.ByInstance | Options.ByComponentType)) == 0)
			{
				EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, "You must search by Instance or by Component type.", "OK");
				return;
			}

			if ((this.searchOptions & Options.InProject) != 0 && (this.searchOptions & (Options.Asset | Options.Prefab | Options.Scene)) == 0)
			{
				this.showProject = true;
				EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, "You must search by Asset, Prefab or Scene.", "OK");
				return;
			}

			// Seeking an instance of Object from a scene in Project, which is impossible.
			if ((this.searchOptions & (Options.ByInstance | Options.ByComponentType)) == Options.ByInstance &&
				(this.searchOptions & (Options.InProject | Options.InCurrentScene)) == Options.InProject &&
				string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this.targetAsset)) == true)
			{
				EditorUtility.DisplayDialog(NGAssetsFinderWindow.Title, "You can not search an Asset from the scene in the Project.", "OK");
				return;
			}

			//for (int i = 0; i < NGAssetsFinderEditorWindow.PotentialBigResultTypes.Length; i++)
			//{
			//	if (NGAssetsFinderEditorWindow.PotentialBigResultTypes[i] == this.targetType)
			//	{
			//		if (EditorUtility.DisplayDialog("Confirm", "The Type (" + this.targetType + ") you are looking for might potentially generate a tremendous result depending on the size of the scene/project.\nDo you want to continue?", LC.G("Yes"), LC.G("No")) == false)
			//			return;
			//		break;
			//	}
			//}

			if (EditorApplication.isPlaying == true)
				EditorApplication.isPaused = false;

			this.searchFlags = (this.searchOptions & Options.NonPublic) != 0 ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance : BindingFlags.Public | BindingFlags.Instance;

			if (lastTargetType != this.targetType || lastSearchFlags != this.searchFlags)
			{
				this.analyzedTypes.Clear();
				this.analyzedTypes.Add(new ContainerType(typeof(Object), false, true));
				this.analyzedTypes.Add(new ContainerType(typeof(GameObject), false, typeof(GameObject).IsAssignableFrom(this.targetType)));
				this.analyzedTypes.Add(new ContainerType(typeof(Component), false, typeof(Component).IsAssignableFrom(this.targetType)));
				this.analyzedTypes.Add(new ContainerType(typeof(Behaviour), false, typeof(Behaviour).IsAssignableFrom(this.targetType)));
				this.analyzedTypes.Add(new ContainerType(typeof(MonoBehaviour), false, typeof(MonoBehaviour).IsAssignableFrom(this.targetType)));
				this.analyzedTypes.Add(new ContainerType(typeof(ScriptableObject), false, typeof(ScriptableObject).IsAssignableFrom(this.targetType)));
			}

			this.potentialMatchesCount = 0;
			this.effectiveMatchesCount = 0;

			this.ClearResults();

			Utility.StartBackgroundTask(this.TaskLoadAssets(), this.PrepareResults);
		}

		private void	ClearResults()
		{
			this.hasResult = false;
			this.matchedInstancesInScene.Clear();
			this.matchedInstancesInProject.Clear();
			this.matchedScenes.Clear();
			this.scenePrefabMatches.Clear();
		}

		private void	PrepareResults()
		{
			this.isSearching = false;
			this.hasResult = true;

			if (this.debugAnalyzedTypes == true)
			{
				Debug.Log("Working data:");
				for (int i = 0; i < this.analyzedTypes.Count; i++)
				{
					if (this.analyzedTypes[i].HasType == true)
					{
						Debug.Log("Type " + this.analyzedTypes[i].type);
						for (int j = 0; j < this.analyzedTypes[i].fields.Count; j++)
							Debug.Log("	F " + this.analyzedTypes[i].fields[j].Name);
						for (int j = 0; j < this.analyzedTypes[i].properties.Count; j++)
							Debug.Log("	P " + this.analyzedTypes[i].properties[j].Name);
					}
				}
			}

			foreach (var item in this.matchedInstancesInScene)
				item.PreCacheGUI();
			foreach (var item in this.matchedInstancesInProject)
				item.PreCacheGUI();

			if (this.debugAnalyzedTypes == true)
			{
				foreach (var item in this.matchedInstancesInScene)
					this.OutputAssetMatches(0, item);
				foreach (var item in this.matchedInstancesInProject)
					this.OutputAssetMatches(0, item);
			}

			this.Repaint();
			EditorUtility.ClearProgressBar();
		}

		private void	OutputAssetMatches(int indent, AssetMatches assetMatches)
		{
			if (assetMatches.type == AssetMatches.Type.Reference &&
				assetMatches.matches.Count == 0 &&
				assetMatches.children.Count == 0)
			{
				return;
			}

			Debug.Log(new string(' ', indent << 1) + assetMatches.origin);

			for (int i = 0; i < assetMatches.matches.Count; i++)
				this.OutputMatch(indent + 1, assetMatches.matches[i]);

			for (int i = 0; i < assetMatches.children.Count; i++)
				this.OutputAssetMatches(indent + 1, assetMatches.children[i]);
		}

		private void	OutputMatch(int indent, Match match)
		{
			Debug.Log(new string(' ', indent << 1) + match.path);

			for (int i = 0; i < match.subMatches.Count; i++)
				this.OutputMatch(indent + 1, match.subMatches[i]);
		}

		private bool	ParseType(Type type, bool workingOnComponent = false)
		{
			if (type.IsPrimitive == true || type.IsEnum == true || type == typeof(Decimal))
				return false;

			for (int i = 0; i < this.analyzedTypes.Count; i++)
			{
				if (this.analyzedTypes[i].isInstance == workingOnComponent && this.analyzedTypes[i].type == type)
					return this.analyzedTypes[i].HasType;
			}

			if (workingOnComponent == false && typeof(Object).IsAssignableFrom(type) == true)
			{
				ContainerType	containerType = new ContainerType(type, false, type.IsAssignableFrom(this.targetType));

				this.analyzedTypes.Add(containerType);

				return containerType.containObject;
			}
			else if (typeof(IList).IsAssignableFrom(type) == true)
			{
				ContainerType	containerType = new ContainerType(type, false);
				Type			baseSubType = Utility.GetArraySubType(type);

				this.analyzedTypes.Add(containerType);

				if (baseSubType == null)
					baseSubType = typeof(object);

				if (this.ParseType(baseSubType) == true)
					containerType.containObject = true;

				foreach (Type subType in Utility.EachAllSubClassesOf(baseSubType))
				{
					if (this.ParseType(subType) == true)
						containerType.containObject = true;
				}

				return containerType.containObject;
			}
			else if (type.IsClass == true || type.IsStruct() == true)
			{
				ContainerType	containerType = new ContainerType(type, workingOnComponent);

				this.analyzedTypes.Add(containerType);
				this.tme.Clear();

				for (int i = 0; i < this.typeExclusions.Length; i++)
				{
					if (this.typeExclusions[i].CanHandle(type) == true)
						this.tme.Add(this.typeExclusions[i]);
				}

				TypeMembersExclusion[]	tme = this.tme.ToArray();

				foreach (var f in Utility.EachFieldHierarchyOrdered(type, typeof(object), this.searchFlags))
				{
					int	i = 0;

					for (; i < tme.Length; i++)
					{
						if (tme[i].IsExcluded(f.Name) == true)
							break;
					}

					if (i < tme.Length)
						continue;

					if (this.ParseType(f.FieldType) == true)
						containerType.fields.Add(f);
				}

				foreach (var p in Utility.EachPropertyHierarchyOrdered(type, typeof(object), this.searchFlags))
				{
					// Exclude indexers.
					if (p.GetIndexParameters().Length > 0)
						continue;

					// Exclude property without getter or setter.
					if (p.CanRead == false ||
						p.CanWrite == false)
					{
						continue;
					}

					int	i = 0;

					for (; i < tme.Length; i++)
					{
						if (tme[i].IsExcluded(p.Name) == true)
							break;
					}

					if (i < tme.Length)
						continue;

					if (this.ParseType(p.PropertyType) == true)
						containerType.properties.Add(p);
				}

				return containerType.HasType;
			}

			return false;
		}

		private void	CheckClass(AssetMatches	assetMatches, Match match, Type type, object instance)
		{
			bool	found = false;

			for (int i = 0; i < this.analyzedTypes.Count; i++)
			{
				if (this.analyzedTypes[i].type == type)
				{
					found = true;

					if (this.analyzedTypes[i].HasType == true)
					{
						for (int j = 0; j < this.analyzedTypes[i].fields.Count; j++)
							this.CheckMember(assetMatches, match, instance, new FieldModifier(this.analyzedTypes[i].fields[j]));

						for (int j = 0; j < this.analyzedTypes[i].properties.Count; j++)
							this.CheckMember(assetMatches, match, instance, new PropertyModifier(this.analyzedTypes[i].properties[j]));
					}
				}
			}

			if (found == false)
				throw new Exception("Type \"" + type + "\" is missing in \"" + instance + "\" from Object \"" + assetMatches.origin + "\".");
		}

		private void	CheckMember(AssetMatches assetMatches, Match match, object instance, IFieldModifier f)
		{
			Match	subMatch = null;

			if (typeof(Object).IsAssignableFrom(f.Type) == true)
			{
				if (f.Type.IsAssignableFrom(this.targetType) == true)
				{
					++this.potentialMatchesCount;

					if (this.targetAsset.Equals(f.GetValue(instance)) == true)
					{
						++this.effectiveMatchesCount;
						if (match == null)
						{
							match = new Match(instance, f);
							match.path = f.Name;
							assetMatches.matches.Add(match);
						}
						else
						{
							subMatch = new Match(instance, f);
							subMatch.path = f.Name;
							subMatch.instance = instance;
							subMatch.fieldModifier = f;
							subMatch.valid = true;
							match.subMatches.Add(subMatch);
							match.valid = true;
						}
					}
				}
			}
			else if (f.Type.IsUnityArray() == true)
			{
				object	rawArray = f.GetValue(instance);

				if (rawArray == null)
					return;

				ICollectionModifier	collectionModifier = NGTools.Utility.GetCollectionModifier(rawArray);
				bool				newMatch = false;
				Match				indexMatch = null;

				if (match == null)
				{
					newMatch = true;
					match = new Match(instance, f);

					subMatch = match;
				}
				else
				{
					subMatch = new Match(instance, f);
				}

				for (int i = 0; i < collectionModifier.Size; i++)
				{
					object	element = collectionModifier.Get(i);

					if (element != null)
					{
						if (typeof(Object).IsAssignableFrom(element.GetType()) == true)
						{
							++this.potentialMatchesCount;
							if (this.targetAsset.Equals(element) == true)
							{
								++this.effectiveMatchesCount;
								subMatch.valid = true;
								subMatch.arrayIndexes.Add(i);
							}
						}
						else
						{
							if (indexMatch == null)
								indexMatch = new Match(instance, f);
							indexMatch.path = i.ToString();

							this.CheckClass(assetMatches, indexMatch, element.GetType(), element);

							if (indexMatch.valid == true)
							{
								subMatch.subMatches.Add(indexMatch);
								indexMatch = null;
							}
						}
					}
				}

				if (subMatch.subMatches.Count > 0 ||
					subMatch.arrayIndexes.Count > 0)
				{
					match.valid = true;
					if (newMatch == false)
						match.subMatches.Add(subMatch);
					else
						assetMatches.matches.Add(match);
				}

				NGTools.Utility.ReturnCollectionModifier(collectionModifier);
			}
			else if (f.Type.IsClass == true || f.Type.IsStruct() == true)
			{
				object	classInstance = f.GetValue(instance);

				if (classInstance != null)
				{
					bool	newMatch = false;

					if (match == null)
					{
						newMatch = true;
						match = new Match(instance, f);

						subMatch = match;
					}
					else
					{
						subMatch = new Match(instance, f);
					}

					this.CheckClass(assetMatches, subMatch, f.Type, classInstance);

					if (subMatch.valid == true)
					{
						match.valid = true;
						if (newMatch == false)
							match.subMatches.Add(subMatch);
						else
							assetMatches.matches.Add(match);
					}
				}
			}
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGAssetsFinderWindow.Title, Constants.WikiBaseURL + "#markdown-header-111-ng-assets-finder");
		}
	}
}