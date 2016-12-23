using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class ColorMarkersWizard : ScriptableWizard
	{
		public const string	Title = "Color Markers Wizard";
		public const string	AssetFolderEditorPref = "CreateAssetWizardAssetFolder";

		private List<bool>	folds = new List<bool>();
		private Vector2		scrollView;

		protected virtual void	OnEnable()
		{
			this.scrollView = new Vector2();
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
			{
				this.Close();
				return;
			}
		}

		protected virtual void	OnGUI()
		{
			if (Preferences.Settings == null)
			{
				GUILayout.Label(string.Format(LC.G("RequiringConfigurationFile"), ColorMarkersWizard.Title));
				if (GUILayout.Button(LC.G("ShoWPreferencesWindow")) == true)
				{
					Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);
				}
				return;
			}

			if (GUILayout.Button(LC.G("AddMarker")) == true)
			{
				ColorMarker	newMarker = new ColorMarker();
				Preferences.Settings.colorMarkersModule.colorMarkers.Add(newMarker);
				Preferences.InvalidateSettings();
				Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
			}

			this.scrollView = GUILayout.BeginScrollView(this.scrollView);
			{
				for (int i = 0; i < Preferences.Settings.colorMarkersModule.colorMarkers.Count; i++)
				{
					while (this.folds.Count < Preferences.Settings.colorMarkersModule.colorMarkers.Count)
						this.folds.Add(false);

					GUILayout.BeginVertical();
					{
						GUILayout.BeginHorizontal();
						{
							this.folds[i] = EditorGUILayout.Foldout(this.folds[i], LC.G("Marker") + " #" + (i + 1));

							GUILayout.Space(155F);

							Utility.content.text = "# # # # #";
							Rect	rect = GUILayoutUtility.GetRect(Utility.content, GUI.skin.label, new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(false)
							});

							EditorGUI.DrawRect(rect, Preferences.Settings.colorMarkersModule.colorMarkers[i].backgroundColor);
							EditorGUI.LabelField(rect, "# # # # #");

							GUI.enabled = i > 0;
							if (GUILayout.Button("↑", GUILayout.Width(20F)) == true)
							{
								Preferences.Settings.colorMarkersModule.colorMarkers.Reverse(i - 1, 2);
								Preferences.InvalidateSettings();
								Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
								break;
							}

							GUI.enabled = i < Preferences.Settings.colorMarkersModule.colorMarkers.Count - 1;
							if (GUILayout.Button("↓", GUILayout.Width(20F)) == true)
							{
								Preferences.Settings.colorMarkersModule.colorMarkers.Reverse(i, 2);
								Preferences.InvalidateSettings();
								Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
								break;
							}

							GUI.enabled = true;

							if (GUILayout.Button("X", GUILayout.Width(20F)) == true)
							{
								Preferences.Settings.colorMarkersModule.colorMarkers.RemoveAt(i);
								Preferences.InvalidateSettings();
								Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
								break;
							}
						}
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(16F);

							GUILayout.BeginVertical();
							{
								if (this.folds[i] == true)
								{
									EditorGUI.BeginChangeCheck();
									Preferences.Settings.colorMarkersModule.colorMarkers[i].backgroundColor = EditorGUILayout.ColorField("Color", Preferences.Settings.colorMarkersModule.colorMarkers[i].backgroundColor);
									if (EditorGUI.EndChangeCheck() == true)
									{
										Preferences.InvalidateSettings();
										Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
									}

									GUILayout.BeginHorizontal();
									{
										EditorGUI.BeginChangeCheck();
										Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.OnGUI();
										if (EditorGUI.EndChangeCheck() == true)
										{
											Preferences.InvalidateSettings();
											Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
										}
									}
									GUILayout.EndHorizontal();

									for (int j = 0; j < Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.filters.Count; j++)
									{
										if (Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.filters[j].Enabled == true)
										{
											GUILayout.BeginHorizontal();
											{
												EditorGUI.BeginChangeCheck();
												Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.filters[j].OnGUI();
												if (EditorGUI.EndChangeCheck() == true)
												{
													Preferences.InvalidateSettings();
													Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
												}
											}
											GUILayout.EndHorizontal();
										}
									}
								}
							}
							GUILayout.EndVertical();
						}
						GUILayout.EndHorizontal();

						EditorGUILayout.Separator();
					}
					GUILayout.EndVertical();
				}
			}
			GUILayout.EndScrollView();
		}
	}
}