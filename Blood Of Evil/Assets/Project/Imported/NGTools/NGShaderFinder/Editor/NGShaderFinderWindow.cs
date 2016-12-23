using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[InitializeOnLoad]
	public class NGShaderFinderWindow : EditorWindow, IHasCustomMenu
	{
		public const string	Title = "NG Shader Finder";

		private bool	canReplace;
		private Shader	targetShader;
		private Shader	replaceShader;

		private bool			hasResult;
		private List<Material>	results = new List<Material>();

		private bool	isSearching;
		private float	searchTime;
		private float	lastFrameTime;
		private double	lastClick;
		private Vector2	scrollPosition;

		static	NGShaderFinderWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGShaderFinderWindow.Title);
			Utility.AddMenuItemPicker("Assets/Search Material using this Shader");
		}

		#region Menu Items
		[MenuItem(Constants.MenuItemPath + NGShaderFinderWindow.Title, priority = Constants.MenuItemPriority + 335)]
		private static void	Open()
		{
			EditorWindow.GetWindow<NGShaderFinderWindow>(true, Title);
		}

		[MenuItem("CONTEXT/Shader/Search Material using this Shader", priority = 502)]
		private static void	SearchShader(MenuCommand menuCommand)
		{
			NGShaderFinderWindow	window = EditorWindow.GetWindow<NGShaderFinderWindow>(true, NGShaderFinderWindow.Title);

			window.targetShader = menuCommand.context as Shader;
			window.ClearResults();
		}

		[MenuItem("CONTEXT/Material/Search Material using this Shader", priority = 502)]
		private static void	SearchShaderFromMaterial(MenuCommand menuCommand)
		{
			NGShaderFinderWindow	window = EditorWindow.GetWindow<NGShaderFinderWindow>(true, NGShaderFinderWindow.Title);

			window.targetShader = (menuCommand.context as Material).shader;
			window.ClearResults();
		}

		[MenuItem("Assets/Search Material using this Shader")]
		private static void	SearchAsset(MenuCommand menuCommand)
		{
			NGShaderFinderWindow	window = EditorWindow.GetWindow<NGShaderFinderWindow>(true, NGShaderFinderWindow.Title);

			window.targetShader = Selection.activeObject as Shader;

			if (window.targetShader == null)
				window.targetShader = (Selection.activeObject as Material).shader;
			window.ClearResults();
		}

		[MenuItem("Assets/Search Material using this Shader", true)]
		private static bool	ValidateSearchAsset(MenuCommand menuCommand)
		{
			return Selection.activeObject is Shader || Selection.activeObject is Material;
		}
		#endregion

		protected virtual void	OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.BeginDisabledGroup(this.targetShader == null || this.isSearching == true);
				{
					if (GUILayout.Button("Search all Material") == true)
						this.FindReferences();
				}
				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button("Clear", GUILayout.Width(70F)) == true)
					this.ClearResults();
			}
			EditorGUILayout.EndHorizontal();
			
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
							if (GUILayout.Button("⇅", GeneralStyles.BigFontToolbarButton) == true)
							{
								Shader	tmp = this.replaceShader;
								this.replaceShader = this.targetShader;
								this.targetShader = tmp;
							}
						}
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.BeginVertical();
					{
						EditorGUI.BeginChangeCheck();
						Shader	newTarget = EditorGUILayout.ObjectField("Find Asset", this.targetShader, typeof(Shader), false) as Shader;
						if (EditorGUI.EndChangeCheck() == true)
							this.targetShader = newTarget;

						if (this.canReplace == true)
							this.replaceShader = EditorGUILayout.ObjectField("Replace Asset", this.replaceShader, typeof(Shader), false) as Shader;

						GUILayout.Space(4F);
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.EndDisabledGroup();

			if (this.canReplace == true)
			{
				EditorGUI.BeginDisabledGroup(this.isSearching);
				{
					EditorGUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("Replace") == true)
							this.ReplaceReferences();
						if (GUILayout.Button("Set all") == true)
							this.SetAllReferences();
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.EndDisabledGroup();
			}

			Rect	r = GUILayoutUtility.GetLastRect();
			r.yMin = r.yMax - 1F;
			r.x = 0F;
			r.width = this.position.width;
			r.y += 2F;
			EditorGUI.DrawRect(r, new Color(.3F, .3F, .3F));
			r.y += 1F;

			if (this.hasResult == true)
			{
				this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
				{
					EditorGUILayout.LabelField("Results:", this.results.Count.ToString());

					for (int i = 0; i < this.results.Count; i++)
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUI.BeginChangeCheck();
							Shader	o = EditorGUILayout.ObjectField(this.results[i].name, this.results[i].shader, typeof(Shader), false) as Shader;
							if (EditorGUI.EndChangeCheck() == true)
							{
								this.results[i].shader = o;
								EditorUtility.SetDirty(this.results[i]);
							}

							if (GUILayout.Button(LC.G("Ping"), GUILayout.Width(40F)) == true)
							{
								if (Event.current.button != 0 || this.lastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
									Selection.activeObject = this.results[i];
								else
									EditorGUIUtility.PingObject(this.results[i]);

								this.lastClick = EditorApplication.timeSinceStartup;
							}
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndScrollView();
			}
		}

		private void	FindReferences()
		{
			Utility.StartBackgroundTask(this.ProcessFolder(), this.PrepareResults);
		}

		private void	ReplaceReferences()
		{
			AssetDatabase.StartAssetEditing();

			int	count = 0;

			for (int i = 0; i < this.results.Count; i++)
			{
				if (this.results[i].shader == this.targetShader)
				{
					this.results[i].shader = this.replaceShader;
					++count;
				}
			}

			AssetDatabase.StopAssetEditing();

			AssetDatabase.SaveAssets();

			if (count == 0)
				EditorUtility.DisplayDialog(NGShaderFinderWindow.Title, "No reference updated.", "OK");
			else if (count == 1)
				EditorUtility.DisplayDialog(NGShaderFinderWindow.Title, count + " reference updated.", "OK");
			else
				EditorUtility.DisplayDialog(NGShaderFinderWindow.Title, count + " references updated.", "OK");
		}

		private void	SetAllReferences()
		{
			AssetDatabase.StartAssetEditing();

			int	count = 0;

			for (int i = 0; i < this.results.Count; i++)
			{
				if (this.results[i].shader != this.replaceShader)
				{
					this.results[i].shader = this.replaceShader;
					++count;
				}
			}

			AssetDatabase.StopAssetEditing();

			AssetDatabase.SaveAssets();

			if (count == 0)
				EditorUtility.DisplayDialog(NGShaderFinderWindow.Title, "No reference updated.", "OK");
			else if (count == 1)
				EditorUtility.DisplayDialog(NGShaderFinderWindow.Title, count + " reference updated.", "OK");
			else
				EditorUtility.DisplayDialog(NGShaderFinderWindow.Title, count + " references updated.", "OK");
		}

		private void	ClearResults()
		{
			this.hasResult = false;
			this.results.Clear();
		}
		
		private void	PrepareResults()
		{
			this.isSearching = false;
			this.hasResult = true;

			this.Repaint();
			EditorUtility.ClearProgressBar();
		}
		
		private IEnumerator	ProcessFolder()
		{
			this.ClearResults();

			this.isSearching = true;
			this.searchTime = Time.realtimeSinceStartup;

			this.lastFrameTime = Time.realtimeSinceStartup;

			string[]	files = Directory.GetFiles("Assets/", "*.mat", SearchOption.AllDirectories);
			int			max = files.Length;

			for (int i = 0; i < max; i++)
			{
				try
				{
					Material	mat = AssetDatabase.LoadAssetAtPath(files[i], typeof(Material)) as Material;

					if (mat.shader == this.targetShader)
						this.results.Add(mat);
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException("Exception thrown on file \"" + files[i] + "\".", ex);
				}

				if (Time.realtimeSinceStartup - lastFrameTime >= NGAssetsFinderWindow.MaxProcessTimePerFrame)
				{
					lastFrameTime += NGAssetsFinderWindow.MaxProcessTimePerFrame;

					if (i + 1 < max)
						EditorUtility.DisplayProgressBar(NGShaderFinderWindow.Title + " - Project (" + (i + 1) + " / " + max + ")", files[i], (float)(i + 1) / (float)max);
					else
						break;

					this.Repaint();

					yield return null;
				}
			}

			this.searchTime = Time.realtimeSinceStartup - this.searchTime;

			EditorUtility.ClearProgressBar();
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGShaderFinderWindow.Title, Constants.WikiBaseURL + "#markdown-header-112-ng-shader-finder");
		}
	}
}