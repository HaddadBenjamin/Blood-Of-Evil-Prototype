using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	public class NGMissingScriptRecoveryWindow : EditorWindow, IHasCustomMenu
	{
		[Serializable]
		public sealed class MissingGameObject
		{
			public GameObject	gameObject;
			public string		path;

			public	MissingGameObject(GameObject go)
			{
				this.gameObject = go;

				string			assetPath = AssetDatabase.GetAssetPath(this.gameObject);
				StringBuilder	buffer = Utility.GetBuffer();
				Stack<string>	hierarchy = new Stack<string>(4);

				Transform	t = this.gameObject.transform;

				while (t != null)
				{
					hierarchy.Push(t.name);
					t = t.parent;
				}

				while (hierarchy.Count > 0)
				{
					buffer.Append(hierarchy.Pop());
					buffer.Append('/');
				}

				buffer.Length -= 1;

				this.path = assetPath.Substring(0, assetPath.LastIndexOf('/')) + '/' + Utility.ReturnBuffer(buffer);
			}
		}

		[Serializable]
		private sealed class CachedLineFix
		{
			public string	brokenLine;
			public string	fixedLine;
			public string	typeName;
		}

		private sealed class RawComponent
		{
			public string		componentID = string.Empty;
			public string		line = string.Empty;
			public string		guid = string.Empty;
			public string		name = string.Empty;
			public Object		asset;
			public List<string>	fields = new List<string>();
			public List<KeyValuePair<Type, int[]>>	potentialTypes = new List<KeyValuePair<Type, int[]>>();

			private bool	open = true;
			private bool	fieldsOpen = true;
			private Vector2	scrollPositionFields;

			public	RawComponent(string componentID)
			{
				this.componentID = componentID;
			}

			public void	Draw(NGMissingScriptRecoveryWindow window, bool force = false)
			{
				GUI.enabled = this.fields.Count > 0;
				Utility.content.text = this.name;
				Utility.content.image = Utility.GetIcon(this.asset ? this.asset.GetInstanceID() : 0);
				this.open = EditorGUILayout.Foldout(this.open, Utility.content) && this.fields.Count > 0 || force;
				Utility.content.image = null;
				GUI.enabled = true;

				if (this.fields.Count == 0 && this.asset == null)
					EditorGUILayout.HelpBox("Component seems to have absolutely no fields. Recovery can not proceed.", MessageType.Warning);

				if (this.open == false)
					return;

				++EditorGUI.indentLevel;
				this.fieldsOpen = EditorGUILayout.Foldout(this.fieldsOpen, "Fields found (" + this.fields.Count + ")");
				if (this.fieldsOpen == true)
				{
					++EditorGUI.indentLevel;
					float	h = this.fields.Count * 20F;

					if (h >= NGMissingScriptRecoveryWindow.MaxFieldsHeight)
						h = NGMissingScriptRecoveryWindow.MaxFieldsHeight;

					this.scrollPositionFields = EditorGUILayout.BeginScrollView(this.scrollPositionFields, GUILayout.Height(h));
					{
						for (int j = 0; j < this.fields.Count; j++)
							EditorGUILayout.LabelField(this.fields[j]);
					}
					EditorGUILayout.EndScrollView();
					--EditorGUI.indentLevel;
				}

				CachedLineFix	lineFix = window.cachedComponentFixes.Find(c => c.brokenLine == this.line);
				if (lineFix != null)
				{
					if (GUILayout.Button("Recover From Cache (" + lineFix.typeName + ")") == true)
					{
						this.FixLine(window, this.componentID, lineFix.typeName);
						window.Diagnose(window.target);
					}
				}

				EditorGUILayout.LabelField("Potential types:");
				++EditorGUI.indentLevel;
				if (this.potentialTypes.Count == 0)
					EditorGUILayout.LabelField("No type available.");
				else
				{
					for (int l = 0; l < this.potentialTypes.Count; l++)
					{
						EditorGUILayout.BeginHorizontal();
						{
							if (GUILayout.Button(this.potentialTypes[l].Key.FullName) == true)
							{
								this.FixMissingComponent(window, this.componentID, this.potentialTypes[l].Key);
								window.Diagnose(window.target);
								return;
							}

							if (this.potentialTypes[l].Value[0] == this.fields.Count &&
								this.fields.Count == this.potentialTypes[l].Value[1])
							{
								GUILayout.Label("(Perfect match)", GUILayout.ExpandWidth(false));
							}
							else
							{
								int	extraFields = this.potentialTypes[l].Value[1] - this.fields.Count;
								GUILayout.Label("(" + this.potentialTypes[l].Value[0] + " matching fields, " + (extraFields > 0 ? "+" + extraFields + " extra fields" : extraFields + " missings fields") + ")", GUILayout.ExpandWidth(false));
							}
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				--EditorGUI.indentLevel;
				--EditorGUI.indentLevel;
			}

			public void	FixMissingComponent(NGMissingScriptRecoveryWindow window, string componentID, Type type)
			{
				GameObject	tempPrefab = new GameObject("MissingComponentRecovery", type);

				PrefabUtility.CreatePrefab(NGMissingScriptRecoveryWindow.TempPrefabPath, tempPrefab, ReplacePrefabOptions.ConnectToPrefab);

				if (File.Exists(NGMissingScriptRecoveryWindow.TempPrefabPath) == true)
				{
					AssetDatabase.SaveAssets();
					this.FixLine(window, componentID, type.Name);
				}

				Object.DestroyImmediate(tempPrefab);
				AssetDatabase.DeleteAsset(NGMissingScriptRecoveryWindow.TempPrefabPath);
				AssetDatabase.Refresh();

				Selection.activeGameObject = null;

				// Select again few updates after, to avoid Inspector to crash.
				EditorApplication.delayCall += () => EditorApplication.delayCall += () => Selection.activeGameObject = window.target;
			}

			public void	FixLine(NGMissingScriptRecoveryWindow window, string componentID, string typeName)
			{
				string		prefabPath = AssetDatabase.GetAssetPath(window.target);
				string[]	lines = File.ReadAllLines(prefabPath);

				for (int m = 0; m < lines.Length; m++)
				{
					if (lines[m].EndsWith(componentID) == true)
					{
						for (; m < lines.Length; m++)
						{
							if (lines[m].StartsWith("  m_Script") == true)
							{
								string			fixedLine;
								CachedLineFix	lineFix = window.cachedComponentFixes.Find(c => c.brokenLine == lines[m]);

								// Get the cached line only if the Type matches.
								if (lineFix != null && lineFix.typeName == typeName)
									fixedLine = lineFix.fixedLine;
								else
								{
									fixedLine = this.ExtractFixGUIDFromTempPrefab();

									if (string.IsNullOrEmpty(fixedLine) == true)
									{
										InternalNGDebug.LogError("Impossible to extract new GUID from temp prefab. Recovery aborted.");
										return;
									}

									if (lineFix != null)
									{
										lineFix.fixedLine = fixedLine;
										lineFix.typeName = typeName;
									}
									else
										window.cachedComponentFixes.Add(new CachedLineFix() { brokenLine = lines[m], fixedLine = fixedLine, typeName = typeName });
								}

								lines[m] = fixedLine;

								File.WriteAllLines(prefabPath, lines);
								AssetDatabase.Refresh();
								break;
							}
						}

						break;
					}
				}
			}

			private string	ExtractFixGUIDFromTempPrefab()
			{
				string[]	lines = File.ReadAllLines(NGMissingScriptRecoveryWindow.TempPrefabPath);

				for (int k = 0; k < lines.Length; k++)
				{
					if (lines[k].StartsWith("  m_Script") == true)
						return lines[k];
				}

				return null;
			}
		}

		public enum RecoveryMode
		{
			Automatic,
			Manual
		}

		private enum Tab
		{
			Selection,
			Project,
			Recovery
		}

		public const string	Title = "ƝƓ Missing Ȿcript Ʀecovery";
		public const string	TempPrefabPath = "Assets/MissingComponentRecovery.prefab";
		public const float	MaxFieldsHeight = 150F;

		private event Action	PostDiagnostic;

		private Tab	tab = Tab.Selection;

		[NonSerialized]
		private GameObject			target;
		[NonSerialized]
		private List<RawComponent>	components = new List<RawComponent>();
		private Vector2				scrollPosition;
		private Vector2				scrollPositionResult;

		private List<CachedLineFix>	cachedComponentFixes = new List<CachedLineFix>();

		private List<string>	componentIDs = new List<string>();
		private Stack<int>		gameObjects = new Stack<int>();

		public bool			openAutoRecovery;
		public RecoveryMode	recoveryMode = RecoveryMode.Automatic;
		public bool			useCache = true;
		public string		recoveryLogFilePath = string.Empty;
		public bool			supaFast = false;
		public bool			promptOnPause = true;

		[NonSerialized]
		private bool					hasResult = false;
		[NonSerialized]
		private List<MissingGameObject>	missings = new List<MissingGameObject>();
		[NonSerialized]
		private int						selectedMissing = -1;

		[NonSerialized]
		private int		currentGameObject;
		[NonSerialized]
		private bool	isRecovering;
		[NonSerialized]
		private bool	isPausing;
		[NonSerialized]
		private bool	isGUIRendered;
		[NonSerialized]
		private bool	skipFix;
		[NonSerialized]
		private Vector2	scrollPositionRecovery;

		private GUIContent	promptOnPauseContent = new GUIContent("Promp On Pause", "When a case is requiring the user intervention, pop a prompt.");
		private GUIContent	useCacheContent = new GUIContent("Use Cache", "Each fix is cached. The process uses the cache to automatically solve future missing scripts.");
		private GUIContent	supaFastContent = new GUIContent("Supa Fast", "No feedback provided, Unity is unusable during the process.");

		[MenuItem(Constants.MenuItemPath + NGMissingScriptRecoveryWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 345)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGMissingScriptRecoveryWindow>(NGMissingScriptRecoveryWindow.Title);
		}

		[MenuItem("CONTEXT/Component/Recover Missing Script (BETA)")]
		private static void	Diagnose(MenuCommand command)
		{
			// We assume that the context menu is opened from the active GameObject.
			PrefabType	prefabType = PrefabUtility.GetPrefabType(Selection.activeGameObject);
			Object		prefab = null;

			if (prefabType == PrefabType.Prefab)
				prefab = Selection.activeGameObject;
			else if (prefabType == PrefabType.PrefabInstance)
				prefab = PrefabUtility.GetPrefabParent(Selection.activeGameObject);
			else if (prefabType == PrefabType.DisconnectedPrefabInstance)
				prefab = PrefabUtility.GetPrefabParent(Selection.activeGameObject);

			if (prefab == null)
			{
				EditorUtility.DisplayDialog(NGMissingScriptRecoveryWindow.Title, "Recover a missing script is only possible on prefab.\n\nYou may temporary create a prefab, recover and then destroy the prefab.", "OK");
				return;
			}

			NGMissingScriptRecoveryWindow	instance = EditorWindow.GetWindow<NGMissingScriptRecoveryWindow>(true, NGMissingScriptRecoveryWindow.Title);

			if (instance.isRecovering == false)
			{
				instance.Diagnose(Selection.activeGameObject);
				instance.tab = Tab.Selection;
				instance.Show();
			}
			else
			{
				EditorUtility.DisplayDialog(NGMissingScriptRecoveryWindow.Title, "A recovery process is still running.", "OK");
			}
		}

		protected virtual void	OnEnable()
		{
			if (this.tab == Tab.Recovery)
				this.tab = Tab.Selection;

			Utility.LoadEditorPref(this);
		}

		protected virtual void	OnDisable()
		{
			Utility.SaveEditorPref(this);
		}

		protected virtual void	OnGUI()
		{
			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUI.BeginDisabledGroup(this.isRecovering);
				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.tab == Tab.Selection, "Selection", GeneralStyles.ToolbarToggle);
				if (EditorGUI.EndChangeCheck() == true)
					this.tab = Tab.Selection;

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.tab == Tab.Project, "Project", GeneralStyles.ToolbarToggle);
				if (EditorGUI.EndChangeCheck() == true)
					this.tab = Tab.Project;
				EditorGUI.EndDisabledGroup();

				if (this.isRecovering == true)
				{
					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(this.tab == Tab.Recovery, "Recovery", GeneralStyles.ToolbarToggle);
					if (EditorGUI.EndChangeCheck() == true)
						this.tab = Tab.Recovery;
				}
			}
			EditorGUILayout.EndHorizontal();

			if (EditorSettings.serializationMode != SerializationMode.ForceText)
			{
				EditorGUILayout.HelpBox(NGMissingScriptRecoveryWindow.Title + " requires asset serialization mode to be set on ForceText to recover from plain text file.", MessageType.Info);

				try
				{
					EditorGUI.BeginChangeCheck();
					SerializationMode mode = (SerializationMode)EditorGUILayout.EnumPopup("Serialization Mode", EditorSettings.serializationMode);
					if (EditorGUI.EndChangeCheck() == true)
						EditorSettings.serializationMode = mode;
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}

				return;
			}

			if (this.tab == Tab.Selection)
				this.DrawSelection();
			else if (this.tab == Tab.Project)
				this.DrawProject();
			else if (this.tab == Tab.Recovery)
				this.DrawRecovery();

			this.isGUIRendered = true;
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
			{
				if (this.isRecovering == true)
				{
					InternalNGDebug.Log("Recovery aborted due to compilation.");
					this.isRecovering = false;
				}
			}
		}

		private void	DrawSelection()
		{
			if (this.target == null && Selection.activeGameObject != null)
				this.Diagnose(Selection.activeGameObject);

			if (this.target == null)
			{
				GUILayout.Label("No diagnostic available.", GeneralStyles.BigCenterText, GUILayout.ExpandHeight(true));
				return;
			}

			if (this.selectedMissing >= 0)
			{
				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginDisabledGroup(this.selectedMissing <= 0);
					{
						if (GUILayout.Button("<<", GeneralStyles.ToolbarButton, GUILayout.MaxWidth(100F)) == true)
						{	
							--this.selectedMissing;
							this.Diagnose(this.missings[this.selectedMissing].gameObject);
						}
					}
					EditorGUI.EndDisabledGroup();

					NGEditorGUILayout.PingObject(this.missings[this.selectedMissing].path, this.missings[this.selectedMissing].gameObject, GeneralStyles.ToolbarButton);

					EditorGUI.BeginDisabledGroup(this.selectedMissing >= this.missings.Count - 1);
					{
						if (GUILayout.Button(">>", GeneralStyles.ToolbarButton, GUILayout.MaxWidth(100F)) == true)
						{
							++this.selectedMissing;
							this.Diagnose(this.missings[this.selectedMissing].gameObject);
						}
					}
					EditorGUI.EndDisabledGroup();
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				Utility.content.text = this.target.name;
				Utility.content.image = Utility.GetIcon(this.target.GetInstanceID());
				EditorGUILayout.LabelField(Utility.content);
				Utility.content.image = null;
			}

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				for (int i = 0; i < this.components.Count; i++)
					this.components[i].Draw(this);
			}
			EditorGUILayout.EndScrollView();
		}

		private void	DrawProject()
		{
			GUILayout.Space(5F);

			EditorGUILayout.BeginVertical("ButtonLeft");
			{
				this.openAutoRecovery = EditorGUILayout.Foldout(this.openAutoRecovery, "Recovery Settings (" + this.recoveryMode + ")");

				if (this.openAutoRecovery == true)
				{
					this.recoveryMode = (RecoveryMode)EditorGUILayout.EnumPopup("Recovery Mode", this.recoveryMode);

					if (this.recoveryMode == RecoveryMode.Automatic)
					{
						this.promptOnPause = EditorGUILayout.Toggle(this.promptOnPauseContent, this.promptOnPause);
						this.useCache = EditorGUILayout.Toggle(this.useCacheContent, this.useCache);
						this.supaFast = EditorGUILayout.Toggle(supaFastContent, this.supaFast);
						this.recoveryLogFilePath = NGEditorGUILayout.SaveFileField("Recovery Log File", this.recoveryLogFilePath);
					}

					GUILayout.Space(5F);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
				{
					if (GUILayout.Button("Scan", GeneralStyles.BigButton) == true)
					{
						string[]	assets = AssetDatabase.GetAllAssetPaths();

						this.hasResult = true;
						this.selectedMissing = -1;
						this.missings.Clear();

						for (int i = 0; i < assets.Length; i++)
						{
							if (assets[i].EndsWith(".prefab") == false)
								continue;

							Object[]	content = AssetDatabase.LoadAllAssetsAtPath(assets[i]);

							for (int j = 0; j < content.Length; j++)
							{
								GameObject	go = content[j] as GameObject;

								if (go != null)
								{
									Component[]	components = go.GetComponents<Component>();

									for (int k = 0; k < components.Length; k++)
									{
										if (components[k] == null)
										{
											this.missings.Add(new MissingGameObject(go));
											break;
										}
									}
								}
							}
						}
					}
				}

				GUILayout.FlexibleSpace();

				if (this.cachedComponentFixes.Count > 0)
				{
					if (GUILayout.Button("Clear Recovery Cache (" + this.cachedComponentFixes.Count + " elements)", GeneralStyles.BigButton) == true)
						this.cachedComponentFixes.Clear();
				}
			}
			EditorGUILayout.EndHorizontal();

			if (this.hasResult == true)
			{
				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					GUILayout.Label("Result");

					GUILayout.FlexibleSpace();

					if (GUILayout.Button("X", GeneralStyles.ToolbarCloseButton) == true)
						this.hasResult = false;
				}
				EditorGUILayout.EndHorizontal();

				if (this.missings.Count == 0)
				{
					GUILayout.Label("No missing script found.");
				}
				else
				{
					this.scrollPositionResult = EditorGUILayout.BeginScrollView(this.scrollPositionResult);
					{
						for (int i = 0; i < this.missings.Count; i++)
						{
							EditorGUILayout.BeginHorizontal();
							{
								NGEditorGUILayout.PingObject(this.missings[i].path, this.missings[i].gameObject, GeneralStyles.LeftButton);

								if (GUILayout.Button("Fix", GUILayout.Width(75F)) == true)
								{
									this.selectedMissing = i;
									this.tab = 0;
									this.Diagnose(this.missings[i].gameObject);
								}
							}
							EditorGUILayout.EndHorizontal();
						}
					}
					EditorGUILayout.EndScrollView();

					GUILayout.FlexibleSpace();

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						using (BgColorContentRestorer.Get(GeneralStyles.HighlightResultButton))
						{
							if (GUILayout.Button("Start Recovery", GeneralStyles.BigButton) == true)
								Utility.StartBackgroundTask(this.RecoveryTask());
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private void	DrawRecovery()
		{
			if (this.currentGameObject >= this.missings.Count)
			{
				EditorGUILayout.HelpBox("Recovery finished.", MessageType.Info);
				return;
			}

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button((this.currentGameObject + 1) + " / " + this.missings.Count + " ☰", "GV Gizmo DropDown", GUILayout.ExpandWidth(false)) == true)
				{
					GenericMenu	menu = new GenericMenu();

					for (int i = 0; i < this.missings.Count; i++)
						menu.AddItem(new GUIContent(this.missings[i].path), false, (n) => this.currentGameObject = (int)n, i);

					menu.ShowAsContext();
				}

				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Stop", GeneralStyles.ToolbarButton, GUILayout.Width(100F)) == true)
				{
					this.isRecovering = false;
					this.tab = Tab.Project;
					return;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUI.BeginDisabledGroup(this.currentGameObject <= 0);
				{
					if (GUILayout.Button("<<", GeneralStyles.ToolbarButton, GUILayout.Width(50F)) == true)
					{	
						--this.currentGameObject;
						this.isPausing = false;
						this.skipFix = true;
					}
				}
				EditorGUI.EndDisabledGroup();

				NGEditorGUILayout.PingObject(this.missings[this.currentGameObject].path, this.missings[this.currentGameObject].gameObject, GeneralStyles.ToolbarButton);

				EditorGUI.BeginDisabledGroup(this.currentGameObject >= this.missings.Count - 1);
				{
					if (GUILayout.Button(">>", GeneralStyles.ToolbarButton, GUILayout.Width(50F)) == true)
					{
						++this.currentGameObject;
						this.isPausing = false;
						this.skipFix = true;
					}

					if (GUILayout.Button("Skip", GeneralStyles.ToolbarButton, GUILayout.Width(100F)) == true)
					{
						++this.currentGameObject;
						this.isPausing = false;
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.EndHorizontal();

			this.PostDiagnostic += this.OnPostDiagnosticManualRecovery;
			this.scrollPositionRecovery = EditorGUILayout.BeginScrollView(this.scrollPositionRecovery);
			{
				for (int i = 0; i < this.components.Count; i++)
					this.components[i].Draw(this);
			}
			EditorGUILayout.EndScrollView();
			this.PostDiagnostic -= this.OnPostDiagnosticManualRecovery;
		}

		private IEnumerator	RecoveryTask()
		{
			Selection.activeObject = null;

			this.currentGameObject = 0;
			this.isRecovering = true;
			this.tab = Tab.Recovery;

			StringBuilder	buffer = null;

			if (string.IsNullOrEmpty(this.recoveryLogFilePath) == false)
			{
				buffer = Utility.GetBuffer();
				buffer.AppendLine("Started recovery at " + DateTime.Now.ToLongTimeString() + ".");
			}

			this.PostDiagnostic += this.OnPostDiagnosticAutomaticRecovery;

			for (; this.currentGameObject < this.missings.Count && this.isRecovering == true;)
			{
				this.Diagnose(this.missings[this.currentGameObject].gameObject);

				if (this.recoveryMode == RecoveryMode.Automatic && this.skipFix == false)
				{
					if (buffer != null)
						buffer.AppendLine("Recovering GameObject " + this.missings[this.currentGameObject].path + ".");

					for (int i = 0; i < this.components.Count && this.isRecovering == true; ++i)
					{
						if (this.components[i].fields.Count == 0)
							continue;

						if (this.useCache == true)
						{
							// Fix from cache.
							CachedLineFix	lineFix = this.cachedComponentFixes.Find(c => c.brokenLine == this.components[i].line);
							if (lineFix != null)
							{
								this.components[i].FixLine(this, this.components[i].componentID, lineFix.typeName);
								this.Repaint();

								if (buffer != null)
									buffer.AppendLine("Recovered Component \"" + this.components[i].name + "\" (" + i + ") from cache (Type \"" + lineFix.typeName + "\").");

								continue;
							}
						}

						int		perfectMatches = 0;
						Type	matchType = null;

						for (int j = 0; j < this.components[i].potentialTypes.Count; j++)
						{
							if (this.components[i].potentialTypes[j].Value[0] == this.components[i].fields.Count &&
								this.components[i].fields.Count == this.components[i].potentialTypes[j].Value[1])
							{
								++perfectMatches;
								matchType = this.components[i].potentialTypes[j].Key;
							}
						}

						if (perfectMatches == 1)
						{
							this.components[i].FixMissingComponent(this, this.components[i].componentID, matchType);

							if (buffer != null)
								buffer.AppendLine("Recovered Component \"" + this.components[i].name + "\" (" + i + ") with Type \"" + matchType.FullName + "\".");
						}
						else if (this.components[i].potentialTypes.Count == 0)
						{
							if (buffer != null)
								buffer.AppendLine("Component \"" + this.components[i].name + "\" (" + i + ") has no potential type.");
						}
						else // Ask the user to fix it.
						{
							if (this.promptOnPause == true)
							{
								if (EditorUtility.DisplayDialog(NGMissingScriptRecoveryWindow.Title, "Recovery requires your attention.", "OK", "Skip alert for next cases") == false)
									this.promptOnPause = false;
							}

							this.Diagnose(this.missings[this.currentGameObject].gameObject);
							this.isPausing = true;

							if (buffer != null)
								buffer.AppendLine("Paused on Component \"" + this.components[i].name + "\" (" + i + ").");

							break;
						}

						this.Repaint();
					}

					if (this.isPausing == false)
						this.currentGameObject++;
				}
				else
					this.isPausing = true;

				this.skipFix = false;
				this.isGUIRendered = false;

				while (this.isPausing == this.isRecovering)
					yield return null;

				while (this.supaFast == false && this.isGUIRendered == false)
				{
					this.Repaint();
					yield return null;
				}
			}

			this.PostDiagnostic -= this.OnPostDiagnosticAutomaticRecovery;

			if (buffer != null)
			{
				buffer.AppendLine("Ended recovery at " + DateTime.Now.ToShortTimeString() + ".");
				buffer.AppendLine();
				File.AppendAllText(this.recoveryLogFilePath, Utility.ReturnBuffer(buffer));
				InternalNGDebug.Log("Recovery log saved at \"" + this.recoveryLogFilePath + "\".");
			}

			this.target = null;
			this.hasResult = false;

			this.Repaint();

			this.isRecovering = false;
		}

		private void	OnPostDiagnosticManualRecovery()
		{
			if (this.IsRecoverable() == false)
			{
				this.currentGameObject++;
				if (this.currentGameObject < this.missings.Count)
					this.Diagnose(this.missings[this.currentGameObject].gameObject);
				else
					this.isRecovering = false;
			}
		}

		private void	OnPostDiagnosticAutomaticRecovery()
		{
			this.isPausing = false;
		}

		private void	ExtractComponentsIDs(string[] lines, GameObject target, string path)
		{
			string	lineName = "  m_Name: " + target.name;
			int		countSameName = 0;
			int		lastSameName = 0;

			this.componentIDs.Clear();
			this.gameObjects.Clear();

			// Look for GameObject sharing the same name.
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].StartsWith("GameObject:") == true)
					this.gameObjects.Push(i);
				else if (lines[i] == lineName)
				{
					++countSameName;
					lastSameName = i;
				}
			}

			// If no duplicate, then we got it!
			if (countSameName == 1)
			{
				for (; lastSameName >= 0; --lastSameName)
				{
					if (lines[lastSameName].StartsWith("GameObject:") == true)
						break;
				}

				for (int i = lastSameName; i < lines.Length; ++i)
				{
					if (lines[i].StartsWith("  m_Component:") == true)
					{
						// Finally, extract the Component.
						for (i += 1; i < lines.Length; i++)
						{
							if (lines[i].StartsWith("  -") == false)
								return;

							string	id = lines[i].Substring(lines[i].IndexOf("fileID: ") + 8);
							componentIDs.Add(id.Remove(id.Length - 1));
						}

						return;
					}
				}
			}
			else
			{
				string	realName = target.name;
				string	tempName = "__RECOVERY_TOKEN__" + DateTime.Now.Ticks.ToString();
				target.name = tempName;
				EditorUtility.SetDirty(target);
				AssetDatabase.SaveAssets();

#if UNITY_4_5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
				// AssetDatabase.SaveAssets is restoring the name, in the name of what, I don't know.
				target.name = tempName;
#endif

				lines = File.ReadAllLines(path);
				this.ExtractComponentsIDs(lines, target, path);

				target.name = realName;
				EditorUtility.SetDirty(target);
				AssetDatabase.SaveAssets();
				return;
			}

			// Look for the root GameObject.
			for (int i = 0; i < lines.Length; i++)
			{
				// Detect the root line.
				if (lines[i].StartsWith("  m_Father: {fileID: 0}") == true)
				{
					// Seek upward for Transform ID
					for (; i >= 0; --i)
					{
						if (lines[i].StartsWith("Transform:") == true)
						{
							// Restart with ID.
							--i;
							string	id = ' ' + lines[i].Substring("--- !u!4 &".Length) + '}';

							for (i = 0; i < lines.Length; i++)
							{
								// Find the line.
								if (lines[i].EndsWith(id) == true)
								{
									// Move upward to ensure we get all the Component.
									for (; i >= 0; --i)
									{
										if (lines[i].StartsWith("  m_Component:") == true)
										{
											// Finally, extract the Component.
											for (i += 1; i < lines.Length; i++)
											{
												if (lines[i].StartsWith("  -") == false)
													return;

												id = lines[i].Substring(lines[i].IndexOf("fileID: ") + 8);
												componentIDs.Add(id.Remove(id.Length - 1));
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		protected virtual void	Diagnose(GameObject	gameObject)
		{
			this.components.Clear();

			PrefabType	prefabType = PrefabUtility.GetPrefabType(gameObject);

			if (prefabType == PrefabType.Prefab)
				this.target = gameObject;
			else if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
				this.target = PrefabUtility.GetPrefabParent(gameObject) as GameObject;

			// Look into a prefab.
			if (this.target != null)
			{
				Component[]	components = this.target.GetComponents<Component>();
				string		prefabPath = AssetDatabase.GetAssetPath(this.target);
				string[]	lines = File.ReadAllLines(prefabPath);

				this.ExtractComponentsIDs(lines, this.target, prefabPath);

				for (int k = 0; k < componentIDs.Count; k++)
				{
					RawComponent	rawComponent = new RawComponent(componentIDs[k]);

					this.components.Add(rawComponent);

					for (int i = 0; i < lines.Length; i++)
					{
						if (lines[i].StartsWith("---") == true && lines[i].EndsWith(componentIDs[k]) == true)
						{
							++i;
							if (components != null && components[k] != null)
							{
								rawComponent.asset = components[k];
								rawComponent.name = rawComponent.asset.GetType().Name;
							}
							else
								rawComponent.name = lines[i].Remove(lines[i].Length - 1);

							if (lines[i] == "MonoBehaviour:")
							{
								bool	inFields = false;

								++i;
								for (; i < lines.Length; i++)
								{
									if (rawComponent.guid == string.Empty && lines[i].StartsWith("  m_Script") == true)
									{
										rawComponent.line = lines[i];

										int	n = lines[i].IndexOf("guid");

										if (n == -1)
											break;

										rawComponent.guid = lines[i].Substring(n + 6, 32);
										string	path = AssetDatabase.GUIDToAssetPath(rawComponent.guid);

										if (string.IsNullOrEmpty(path) == true)
											rawComponent.guid = string.Empty;
									}
									else if (inFields == true && lines[i].StartsWith("---") == true)
									{
										rawComponent.guid = string.Empty;

										break;
									}
									else if (rawComponent.guid == string.Empty && lines[i].StartsWith("  m_EditorClassIdentifier") == true)
										inFields = true;
									else if (inFields == true)
									{
										if (lines[i][3] != ' ' && lines[i][3] != '-')
											rawComponent.fields.Add(lines[i].Substring(2, lines[i].IndexOf(':') - 2));
									}
								}
							}

							break;
						}
					}

					if (rawComponent.fields.Count == 0)
						continue;

					foreach (Type type in Utility.EachAllSubClassesOf(typeof(MonoBehaviour)))
					{
						if (type.IsAbstract == true || type.IsGenericType == true)
							continue;

						int			matchingFieldsCount = 0;
						int			countInspectedFields = 0;
						FieldInfo[]	fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

						for (int i = 0; i < fields.Length; i++)
						{
							if (NGTools.Utility.CanExposeTypeInInspector(fields[i].FieldType) == false ||
								fields[i].IsDefined(typeof(HideInInspector), false) == true ||
								fields[i].IsDefined(typeof(NonSerializedAttribute), true) == true ||
								fields[i].IsPublic == false && fields[i].IsDefined(typeof(SerializeField), true) == false)
							{
								continue;
							}

							countInspectedFields++;
						}

						for (int l = 0; l < rawComponent.fields.Count; l++)
						{
							if (type.GetField(rawComponent.fields[l]) != null)
								++matchingFieldsCount;
						}

						if (matchingFieldsCount > 0)
						{
							int	l = 0;

							for (; l < rawComponent.potentialTypes.Count; l++)
							{
								if (rawComponent.potentialTypes[l].Value[0] < matchingFieldsCount ||
									(rawComponent.potentialTypes[l].Value[0] == matchingFieldsCount &&
										Mathf.Abs(rawComponent.fields.Count - rawComponent.potentialTypes[l].Value[1]) > Mathf.Abs(rawComponent.fields.Count - countInspectedFields)))
								{
									rawComponent.potentialTypes.Insert(l, new KeyValuePair<Type, int[]>(type, new int[] { matchingFieldsCount, countInspectedFields }));
									break;
								}
							}

							if (l >= rawComponent.potentialTypes.Count)
								rawComponent.potentialTypes.Add(new KeyValuePair<Type, int[]>(type, new int[] { matchingFieldsCount, countInspectedFields }));
						}
					}
				}
			}
			// Look into a scene.
			else
			{
				// TODO Implement looking into scene.
			}

			if (this.PostDiagnostic != null)
				this.PostDiagnostic();
		}

		private bool	IsRecoverable()
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].fields.Count > 0)
					return true;
			}

			return false;
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGMissingScriptRecoveryWindow.Title, Constants.WikiBaseURL + "#markdown-header-114-ng-missing-script-recovery");
		}
	}
}