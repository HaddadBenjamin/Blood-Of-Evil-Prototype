using NGTools;
using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[InitializeOnLoad]
	public class NGReplayWindow : EditorWindow, IHasCustomMenu
	{
		public const string		Title = "ƝƓ Ʀeplay";
		public static string	ReplayExtension = "ngreplay";
		public static string[]	Filter = new string[] { "NG Replay", NGReplayWindow.ReplayExtension };

		public  bool	keepAspectRatio = true;

		public  bool	showDBG = true;

		private int				currentReplay;
		private List<Replay>	replays;

		static	NGReplayWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGReplayWindow.Title + "	[BETA]");
		}

		[MenuItem(Constants.MenuItemPath + NGReplayWindow.Title + "	[BETA]", priority = Constants.MenuItemPriority + 230)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGReplayWindow>(NGReplayWindow.Title);
		}

		protected virtual void	OnEnable()
		{
			this.replays = new List<Replay>();
		}

		protected virtual void	OnGUI()
		{
			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button("Open", GeneralStyles.ToolbarButton, GUILayout.Width(100F)) == true)
					this.OpenReplay();

				for (int i = 0; i < this.replays.Count; i++)
				{
					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(i == this.currentReplay, this.replays[i].name, GeneralStyles.ToolbarButton);
					if (EditorGUI.EndChangeCheck() == true)
					{
						if (Event.current.button == 2)
						{
							this.replays.RemoveAt(i);
							return;
						}
						else if (Event.current.button == 1)
						{
							GenericMenu	menu = new GenericMenu();

							menu.AddItem(new GUIContent("Delete"), false, (d) => this.replays.RemoveAt((int)d), i);
							menu.ShowAsContext();
							return;
						}
						else
						{
							if (this.currentReplay >= 0 && this.currentReplay < this.replays.Count)
								this.replays[this.currentReplay].Pause();

							this.currentReplay = i;
						}
					}
				}

				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();

			if (this.replays.Count == 0)
			{
				if (GUILayout.Button("No replay yet, please open one.", GeneralStyles.BigCenterText, GUILayout.ExpandHeight(true)) == true)
					this.OpenReplay();
			}

			if (this.currentReplay >= 0 && this.currentReplay < this.replays.Count)
			{
				Replay	replay = this.replays[this.currentReplay];

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					if (replay.canSave == true)
					{
						if (GUILayout.Button("Save", GeneralStyles.ToolbarButton) == true)
						{
							DateTime	now = DateTime.Now;
							string		filepath = EditorUtility.SaveFilePanel("Save Replay", ".", PlayerSettings.productName + "_" + now.ToString("yyyyMMdd_HHmmss"), NGReplayWindow.ReplayExtension);

							if (string.IsNullOrEmpty(filepath) == false)
							{
								if (replay.Save(filepath) == false)
									InternalNGDebug.LogError("An error occured. Replay could not been saved at \"" + filepath + "\".");
								else
									InternalNGDebug.Log("Replay saved at \"" + filepath + "\".");
							}
						}
					}

					if (GUILayout.Button("<", GeneralStyles.ToolbarButton) == true)
					{
						replay.Pause();
						replay.Set(replay.cursorTime - .1F);
					}
					if (GUILayout.Button(">", GeneralStyles.ToolbarButton) == true)
					{
						replay.Pause();
						replay.Set(replay.cursorTime + .1F);
					}

					if (replay.playing == false)
					{
						if (GUILayout.Button("►", GeneralStyles.ToolbarButton) == true)
							replay.Play();
					}
					else
					{
						if (GUILayout.Button("▮▮", GeneralStyles.ToolbarButton) == true)
							replay.Pause();
					}

					if (GUILayout.Button("■", GeneralStyles.ToolbarButton) == true)
						replay.Stop();

					using (LabelWidthRestorer.Get(50F))
					{
						replay.speed = EditorGUILayout.FloatField("Speed", replay.speed, GUILayout.Width(75F));
					}

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(replay.repeat, "Repeat", GeneralStyles.ToolbarButton);
					if (EditorGUI.EndChangeCheck() == true)
						replay.repeat = !replay.repeat;

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(this.keepAspectRatio, "Aspect Ratio", GeneralStyles.ToolbarButton);
					if (EditorGUI.EndChangeCheck() == true)
						this.keepAspectRatio = !this.keepAspectRatio;

					Rect	r2 = GUILayoutUtility.GetRect(new GUIContent("Modules"), GeneralStyles.ToolbarDropDown);
					if (GUI.Button(r2, "Modules", GeneralStyles.ToolbarDropDown) == true)
					{
						GenericMenu	menu = new GenericMenu();

						for (int i = 0; i < replay.modules.Count; i++)
						{
							if (replay.modules[i].moduleID == ScreenshotModule.ModuleID)
								continue;

							menu.AddItem(new GUIContent(replay.modules[i].name), replay.modules[i].active, this.ToggleModule, replay.modules[i]);
						}

						menu.DropDown(r2);
					}

					if (Conf.DebugMode != Conf.DebugModes.None)
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.Toggle(this.showDBG, "DBG", GeneralStyles.ToolbarButton);
						if (EditorGUI.EndChangeCheck() == true)
							this.showDBG = !this.showDBG;
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					float	t = EditorGUILayout.Slider(replay.cursorTime, 0F, replay.maxTime);
					if (EditorGUI.EndChangeCheck() == true)
					{
						replay.Pause();
						replay.Set(t, true);
					}

					GUILayout.Label("/ " + replay.maxTime, GUILayout.ExpandWidth(false));
				}
				EditorGUILayout.EndHorizontal();

				Rect	r = GUILayoutUtility.GetRect(0F, 0F, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

				for (int i = 0; i < replay.modules.Count; i++)
				{
					if (replay.modules[i].active == true)
						replay.modules[i].OnGUIReplay(r);
				}

				if (Conf.DebugMode != Conf.DebugModes.None && this.showDBG == true)
				{
					EditorGUILayout.LabelField("Time Offset", replay.realTimeOffset.ToString());

					for (int i = 0; i < replay.modules.Count; i++)
					{
						if (replay.modules[i].active == true)
							replay.modules[i].OnGUIDBG();
					}
				}
			}
		}

		protected virtual void	Update()
		{
			if (this.currentReplay >= 0 && this.currentReplay < this.replays.Count)
			{
				this.replays[this.currentReplay].Update();
				this.Repaint();
			}
		}

		public void	AddReplay(Replay replay)
		{
			this.replays.Add(replay);

			this.currentReplay = Mathf.Clamp(this.currentReplay, 0, this.replays.Count - 1);
		}

		private void	OpenReplay()
		{
#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1
			string	filepath = EditorUtility.OpenFilePanelWithFilters("Open Replay", EditorPrefs.GetString(Replay.LastOpenReplayKey, "."), NGReplayWindow.Filter);
#else
					string	filepath = EditorUtility.OpenFilePanel("Open Replay", EditorPrefs.GetString(Replay.LastOpenReplayKey, "."), NGReplayWindow.ReplayExtension);
#endif

			if (string.IsNullOrEmpty(filepath) == false)
			{
				EditorPrefs.SetString(Replay.LastOpenReplayKey, filepath);

				Replay	r = new Replay(Path.GetFileNameWithoutExtension(filepath));

				if (r.Load(filepath) == true)
					this.replays.Add(r);
			}
		}

		private void	ToggleModule(object data)
		{
			ReplayDataModule	module = (ReplayDataModule)data;

			module.active = !module.active;
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			NGRemoteHierarchyWindow.AddTabMenus(menu);
			menu.AddSeparator("");
			Utility.AddNGMenuItems(menu, this, NGReplayWindow.Title, Constants.WikiBaseURL + "#markdown-header-135-ng-replay");
		}
	}
}