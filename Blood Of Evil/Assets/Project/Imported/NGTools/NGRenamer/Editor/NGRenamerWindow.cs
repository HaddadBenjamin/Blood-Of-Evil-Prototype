#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace NGToolsEditor.NGRenamer
{
	using UnityEngine;

	/// <remarks>This tool is highly inspired on Bulk Rename Utility.</remarks>
	[InitializeOnLoad]
	public class NGRenamerWindow : EditorWindow
	{
		internal sealed class TransformedNames
		{
			public string	highlighted;
			public string	renamed;
		}

		[Serializable]
		internal sealed class PathFiles
		{
			public string	path;
			public string[]	files;
			public Vector2	scrollPosition;

			public	PathFiles(string path)
			{
				this.path = path;
				this.Update();
			}

			public void	Update()
			{
				this.files = Directory.GetFiles(this.path);
			}
		}

		public const string	Title = "ƝƓ Ʀenamer";

		private static int	drawingIndex;
		public static int	DrawingIndex { get { return NGRenamerWindow.drawingIndex; } }

		private int		lastHash = 0;
		private Vector2	assetsScrollPosition;
		private Vector2	selectionScrollPosition;

		private List<TextFilter>	filters = new List<TextFilter>();
		private List<Object>		objects = new List<Object>();
		private List<PathFiles>		paths = new List<PathFiles>();
		private Dictionary<string, TransformedNames>	cachedNames = new Dictionary<string, TransformedNames>();
		private List<int>			highlightedPositions = new List<int>();

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGRenamerWindow.Title + ", change settings or disable filters.");

		static	NGRenamerWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGRenamerWindow.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGRenamerWindow.Title, priority = Constants.MenuItemPriority + 340)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGRenamerWindow>(true, NGRenamerWindow.Title, true);
		}

		protected virtual void	OnEnable()
		{
			foreach (Type type in Utility.EachAllSubClassesOf(typeof(TextFilter)))
			{
				this.filters.Add(Activator.CreateInstance(type, new object[] { this  }) as TextFilter);
				Utility.LoadEditorPref(this.filters[this.filters.Count - 1], NGEditorPrefs.GetPerProjectPrefix());
			}

			this.filters.Sort((a, b) => b.priority - a.priority);

#if UNITY_4 || UNITY_5_0 || UNITY_5_1
			EditorApplication.update += this.UpdateSelection;
#else
			Selection.selectionChanged += this.UpdateSelection;
#endif
			Utility.RegisterIntervalCallback(this.Repaint, 100);
			Undo.undoRedoPerformed += this.Repaint;
		}

		protected virtual void	OnDisable()
		{
			for (int i = 0; i < this.filters.Count; i++)
				Utility.SaveEditorPref(this.filters[i], NGEditorPrefs.GetPerProjectPrefix());

#if UNITY_4 || UNITY_5_0 || UNITY_5_1
			EditorApplication.update -= this.UpdateSelection;
#else
			Selection.selectionChanged -= this.UpdateSelection;
#endif
			Utility.UnregisterIntervalCallback(this.Repaint);
			Undo.undoRedoPerformed -= this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			FreeOverlay.First(this, NGRenamerWindow.Title + " is restrained to the first filter in " + Constants.PackageTitle + ".");

			float	halfWidth = this.position.width * .5F - 25F;

			this.errorPopup.OnGUILayout();

			for (int i = 0; i < this.filters.Count; i++)
			{
#if NGTOOLS_FREE
				EditorGUI.BeginDisabledGroup(i > 0);
#endif
				this.filters[i].OnHeaderGUI();
				if (this.filters[i].open == true)
				{
					try
					{
						this.filters[i].OnGUI();
					}
					catch (Exception ex)
					{
						this.errorPopup.exception = ex;
					}
				}
#if NGTOOLS_FREE
				EditorGUI.EndDisabledGroup();
#endif
			}

				if (this.objects.Count > 0 || this.paths.Count > 0 || Selection.activeObject != null)
			{
				using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
				{
					if (GUILayout.Button("Replace All") == true)
						this.ReplaceAll();
				}

				if (this.objects.Count > 0)
				{
					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						GUILayout.Label("Assets (" + this.objects.Count + ")", GeneralStyles.Title1);
					}
					EditorGUILayout.EndHorizontal();

					this.assetsScrollPosition = EditorGUILayout.BeginScrollView(this.assetsScrollPosition, GUILayout.ExpandHeight(true));
					{
						for (int i = 0; i < this.objects.Count; i++)
						{
							NGRenamerWindow.drawingIndex = i;

							EditorGUILayout.BeginHorizontal();
							{
								Texture2D	icon = Utility.GetIcon(this.objects[i].GetInstanceID());

								Rect	r = GUILayoutUtility.GetRect(16F, 16F);
								GUI.DrawTexture(r, icon);
								r.width += halfWidth - 32F;
								if (GUI.Button(r, "", GUI.skin.label) == true)
								{
									NGEditorGUILayout.PingObject(this.objects[i]);
									return;
								}

								this.DrawElement(Path.GetFileName(this.objects[i].name), halfWidth - 32F, (s) => this.RenameAsset(this.objects[i], s));

								if (GUILayout.Button("X", GeneralStyles.ToolbarCloseButton) == true)
								{
									this.objects.RemoveAt(i);
									return;
								}
							}
							EditorGUILayout.EndHorizontal();
						}
					}
					EditorGUILayout.EndScrollView();

					GUILayout.Space(5F);
				}

				for (int i = 0; i < this.paths.Count; i++)
				{
					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						Rect	r = GUILayoutUtility.GetRect(0F, 24F, GUILayout.ExpandWidth(true));
						GUI.Label(r, "Path " + this.paths[i].path + " (" + this.paths[i].files.Length + ")", GeneralStyles.Title1);

						Utility.content.text = "↻";
						Utility.content.tooltip = "Refresh folder's content";
						if (GUILayout.Button(Utility.content, GeneralStyles.ToolbarAltButton, GUILayout.Width(20F)) == true)
						{
							try
							{
								this.paths[i].Update();
							}
							catch (Exception ex)
							{
								this.errorPopup.exception = ex;
							}
							return;
						}
						Utility.content.tooltip = string.Empty;

						if (GUILayout.Button("X", GeneralStyles.ToolbarCloseButton, GUILayout.Width(20F)) == true)
						{
							this.paths.RemoveAt(i);
							return;
						}
					}
					EditorGUILayout.EndHorizontal();

					try
					{
						string[]	files = this.paths[i].files;

						this.paths[i].scrollPosition = EditorGUILayout.BeginScrollView(this.paths[i].scrollPosition, GUILayout.ExpandHeight(true));
						{
							for (int j = 0; j < files.Length; j++)
							{
								NGRenamerWindow.drawingIndex = j;

								this.DrawElement(Path.GetFileName(files[j]), halfWidth, (s) => {
									try
									{
										File.Move(files[j], Path.Combine(this.paths[i].path, s));
										this.paths[i].files[j] = s;
									}
									catch (Exception ex)
									{
										Debug.LogException(ex);
									}
								});
							}
						}
						EditorGUILayout.EndScrollView();
					}
					catch (Exception ex)
					{
						this.paths.RemoveAt(i);
						Debug.LogException(ex);
					}

					GUILayout.Space(5F);
				}

				if (Selection.objects.Length > 0)
				{
					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						GUILayout.Label("Selection", GeneralStyles.Title1);
					}
					EditorGUILayout.EndHorizontal();

					this.selectionScrollPosition = EditorGUILayout.BeginScrollView(this.selectionScrollPosition, GUILayout.ExpandHeight(true));
					{
						for (int i = 0; i < Selection.objects.Length; i++)
						{
							NGRenamerWindow.drawingIndex = i;

							this.DrawElement(Selection.objects[i].name, halfWidth, (s) => this.RenameAsset(Selection.objects[i], s));
						}
					}
					EditorGUILayout.EndScrollView();
				}

				using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
				{
					if (GUILayout.Button("Replace All") == true)
						this.ReplaceAll();
				}

				GUILayout.FlexibleSpace();
			}
			else
			{
				Rect	r = GUILayoutUtility.GetRect(this.position.width, 16F, GUILayout.ExpandHeight(true));

				r.x += 2F;
				r.y += 2F;
				r.width -= 4F;
				r.height -= 4F;

				if (Event.current.type == EventType.Repaint)
					Utility.DrawRectDotted(r, this.position, Color.grey, .02F, 0F);

				GUI.Label(r, "Select assets from Hierarchy or Project\n\nDrop any files or folders from\n" + (Application.platform == RuntimePlatform.WindowsEditor ? "Explorer, " : (Application.platform == RuntimePlatform.OSXEditor ? "Finder, " : "")) + "Hierarchy or Project", GeneralStyles.CenterText);
			}

			if (Event.current.type == EventType.DragUpdated)
			{
				if (DragAndDrop.objectReferences.Length > 0 || DragAndDrop.paths.Length > 0)
					DragAndDrop.visualMode = DragAndDropVisualMode.Move;
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			}
			else if (Event.current.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();

				try
				{
					if (DragAndDrop.objectReferences.Length > 0)
					{
						for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
						{
							Object	obj	= DragAndDrop.objectReferences[i];

							if (typeof(Behaviour).IsAssignableFrom(obj.GetType()) == true)
								obj = (obj as Behaviour).gameObject;

							if (this.objects.Contains(obj) == false)
								this.objects.Add(obj);
						}
					}
					else if (DragAndDrop.paths.Length > 0)
					{
						for (int i = 0; i < DragAndDrop.paths.Length; i++)
						{
							string	path = DragAndDrop.paths[i];
							if (Directory.Exists(path) == false)
								path = new DirectoryInfo(path).Parent.FullName;

							if (string.IsNullOrEmpty(path) == false && this.paths.Exists((p) => p.path == path) == false)
								this.paths.Add(new PathFiles(path));
						}
					}
				}
				catch (Exception ex)
				{
					this.errorPopup.exception = ex;
				}

				Event.current.Use();
			}
			else if (Event.current.type == EventType.Repaint && DragAndDrop.visualMode == DragAndDropVisualMode.Move)
				Utility.DropZone(new Rect(0F, 0F, this.position.width, this.position.height), "Drop folder or asset to rename");

			FreeOverlay.Last();
		}

		private void	DrawElement(string input, float halfWidth, Action<string> renamer)
		{
			EditorGUILayout.BeginHorizontal();
			{
				TransformedNames	names;

				if (this.cachedNames.TryGetValue(input, out names) == false)
				{
					try
					{
						names = new TransformedNames();
						names.highlighted = this.HighlightPattern(input);
						names.renamed = this.Rename(input);
						this.cachedNames.Add(input, names);
					}
					catch (Exception ex)
					{
						this.errorPopup.exception = ex;
						names.highlighted = input;
						names.renamed = "ERROR";
					}
				}

				GUILayout.Label(names.highlighted, GeneralStyles.RichLabel, GUILayout.Width(halfWidth));

				if (GUILayout.Button("->", GUILayout.Width(30F)) == true)
				{
					renamer(names.renamed);
					return;
				}

				GUILayout.Label(names.renamed, GUILayout.Width(halfWidth));
			}
			EditorGUILayout.EndHorizontal();
		}

		private void	RenameAsset(Object asset, string name)
		{
			Undo.RecordObject(asset, "Rename asset");
			if (AssetDatabase.Contains(asset) == true)
			{
				if (AssetDatabase.IsMainAsset(asset) == true)
					AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(asset), name);
				else
				{
					asset.name = name;
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset), ImportAssetOptions.ForceUpdate);
				}
			}
			else
				asset.name = name;
			EditorUtility.SetDirty(asset);
		}

		private void	ReplaceAll()
		{
			try
			{
				for (int i = 0; i < Selection.objects.Length; i++)
					this.RenameAsset(Selection.objects[i], this.GetCachedValue(Selection.objects[i].name));

				for (int i = 0; i < this.objects.Count; i++)
					this.RenameAsset(this.objects[i], this.GetCachedValue(this.objects[i].name));

				for (int i = 0; i < this.paths.Count; i++)
				{
					for (int j = 0; j < this.paths[i].files.Length; j++)
						File.Move(this.paths[i].files[j], this.GetCachedValue(this.paths[i].files[j]));
				}
			}
			catch (Exception ex)
			{
				this.errorPopup.exception = ex;
			}
		}

		private string	GetCachedValue(string input)
		{
			TransformedNames	names;

			if (this.cachedNames.TryGetValue(input, out names) == false)
				throw new Exception("Cached value for " + input + " was not found.");

			return names.renamed;
		}

		private void	UpdateSelection()
		{
			int	hash = this.GetCurrentSelectionHash();

			if (this.lastHash == hash)
				return;

			this.lastHash = hash;

			this.Repaint();
		}

		private int	GetCurrentSelectionHash()
		{
			for (int i = 0; i < Selection.instanceIDs.Length; i++)
			{
				if (EditorUtility.InstanceIDToObject(Selection.instanceIDs[i]) != null)
				// Yeah, what? Is there a problem with my complex anti-colisionning hash function?
					return Selection.instanceIDs.Sum();
			}

			return 0;
		}

		private string	Rename(string input)
		{
			for (int i = 0; i < this.filters.Count; i++)
			{
				if (this.filters[i].enable == false)
					continue;

				input = this.filters[i].Filter(input);
			}

			return input;
		}

		private string	HighlightPattern(string input)
		{
			highlightedPositions.Clear();

			for (int i = 0; i < this.filters.Count; i++)
			{
				if (this.filters[i].enable == false)
					continue;

				this.filters[i].Highlight(input, highlightedPositions);
			}

			return this.HighlightPattern(input, highlightedPositions);
		}

		public void	Invalidate()
		{
			this.cachedNames.Clear();
		}

		private string	HighlightPattern(string input, List<int> highlightedPositions)
		{
			StringBuilder	buffer = Utility.GetBuffer(input);
			bool			closed = false;

			for (int i = input.Length - 1; i >= 0; --i)
			{
				if (closed == false)
				{
					if (highlightedPositions.Contains(i) == true)
					{
						closed = true;
						buffer.Insert(i + 1, "</color>");
					}
				}
				else
				{
					if (highlightedPositions.Contains(i) == false)
					{
						closed = false;
						buffer.Insert(i + 1, "<color=green>");
					}
				}
			}

			if (closed == true)
				buffer.Insert(0, "<color=green>");

			return Utility.ReturnBuffer(buffer);
		}
	}
}