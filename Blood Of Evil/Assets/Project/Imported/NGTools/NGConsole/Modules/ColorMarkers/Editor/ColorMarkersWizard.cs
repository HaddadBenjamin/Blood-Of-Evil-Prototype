using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
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
			Undo.undoRedoPerformed += this.Repaint;
		}

		protected virtual void	OnDisable()
		{
			Undo.undoRedoPerformed -= this.Repaint;
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
					Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);
				return;
			}

			if (GUILayout.Button(LC.G("AddMarker")) == true)
			{
				if (FreeConstants.CheckMaxColorMarkers(Preferences.Settings.colorMarkersModule.colorMarkers.Count) == true)
				{
					Undo.RecordObject(Preferences.Settings, "Add color marker");
					Preferences.Settings.colorMarkersModule.colorMarkers.Add(new ColorMarker());
					Preferences.InvalidateSettings();
					Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
				}
			}

			this.scrollView = GUILayout.BeginScrollView(this.scrollView);
			{
				for (int i = 0; i < Preferences.Settings.colorMarkersModule.colorMarkers.Count; i++)
				{
					while (this.folds.Count < Preferences.Settings.colorMarkersModule.colorMarkers.Count)
						this.folds.Add(true);

					GUILayout.BeginVertical();
					{
						GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
						{
							this.folds[i] = EditorGUILayout.Foldout(this.folds[i], LC.G("Marker") + " #" + (i + 1));

							GUILayout.Space(155F);

							EditorGUI.BeginChangeCheck();
							Color	color = EditorGUILayout.ColorField(Preferences.Settings.colorMarkersModule.colorMarkers[i].backgroundColor);
							if (EditorGUI.EndChangeCheck() == true)
							{
								Undo.RecordObject(Preferences.Settings, "Change color marker color");
								Preferences.Settings.colorMarkersModule.colorMarkers[i].backgroundColor = color;
								Preferences.InvalidateSettings();
								Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
							}

							Utility.content.text = "# # # # #";
							Rect	rect = GUILayoutUtility.GetRect(Utility.content, GUI.skin.label, new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(false)
							});

							EditorGUI.DrawRect(rect, Preferences.Settings.colorMarkersModule.colorMarkers[i].backgroundColor);
							EditorGUI.LabelField(rect, "# # # # #");

							GUI.enabled = i > 0;
							if (GUILayout.Button("↑", GeneralStyles.ToolbarButton, GUILayout.Width(20F)) == true)
							{
								Undo.RecordObject(Preferences.Settings, "Reorder color marker");
								Preferences.Settings.colorMarkersModule.colorMarkers.Reverse(i - 1, 2);
								Preferences.InvalidateSettings();
								Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
								break;
							}

							GUI.enabled = i < Preferences.Settings.colorMarkersModule.colorMarkers.Count - 1;
							if (GUILayout.Button("↓", GeneralStyles.ToolbarButton, GUILayout.Width(20F)) == true)
							{
								Undo.RecordObject(Preferences.Settings, "Reorder color marker");
								Preferences.Settings.colorMarkersModule.colorMarkers.Reverse(i, 2);
								Preferences.InvalidateSettings();
								Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
								break;
							}

							GUI.enabled = true;

							if (GUILayout.Button("X", GeneralStyles.ToolbarCloseButton, GUILayout.Width(20F)) == true)
							{
								Undo.RecordObject(Preferences.Settings, "Delete color marker");
								Preferences.Settings.colorMarkersModule.colorMarkers.RemoveAt(i);
								Preferences.InvalidateSettings();
								Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
								break;
							}
						}
						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
						{
							if (this.folds[i] == true)
							{
								EditorGUI.BeginChangeCheck();
								GUILayout.BeginVertical();
								{
									GUILayout.BeginHorizontal();
									{
										Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.OnGUI();
									}
									GUILayout.EndHorizontal();

									for (int j = 0; j < Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.filters.Count; j++)
									{
										if (Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.filters[j].Enabled == true)
										{
											GUILayout.BeginHorizontal();
											{
												Preferences.Settings.colorMarkersModule.colorMarkers[i].groupFilters.filters[j].OnGUI();
											}
											GUILayout.EndHorizontal();
										}
									}
								}
								GUILayout.EndVertical();
								if (EditorGUI.EndChangeCheck() == true)
								{
									Preferences.InvalidateSettings();
									Utility.RepaintEditorWindow(typeof(NGConsoleWindow));
								}
							}
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