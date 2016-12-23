using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[InitializeOnLoad]
	public class NGSettingsWindow : EditorWindow, IHasCustomMenu
	{
		private class Section
		{
			public string	title;
			public Action	onGUI;
			public int		priority;
			public int		optimalFontSize;

			public	Section(string title, Action OnGUI, int priority)
			{
				this.title = title;
				this.onGUI = OnGUI;
				this.priority = priority;
			}

			public void	OptimizeFontSize(GUIStyle style)
			{
				if (this.optimalFontSize != 0)
					return;

				int	fontSize = style.fontSize;

				style.fontSize = NGSettingsWindow.SectionDefaultFontSize;

				// Shrink title to fit the space.
				Utility.content.text = this.title;
				while (style.CalcSize(Utility.content).x >= NGSettingsWindow.SectionWidth &&
					   style.fontSize > NGSettingsWindow.SectionMinFontSize)
				{
					--style.fontSize;
				}

				this.optimalFontSize = style.fontSize;

				style.fontSize = fontSize;
			}
		}

		public const string	Title = "NG Settings";
		public const float	SectionWidth = 140F;
		public const int	SectionDefaultFontSize = 14;
		public const int	SectionMinFontSize = 6;

		private static List<Section>	sections = new List<Section>();

		private Section	workingSection;
		private Vector2	sectionsScrollPosition;
		private Vector2	sectionScrollPosition;

		[NonSerialized]
		private GUIStyle	sectionScrollView;
		[NonSerialized]
		private GUIStyle	sectionElement;
		[NonSerialized]
		private GUIStyle	selected;
		
		static	NGSettingsWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGSettingsWindow.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGSettingsWindow.Title, priority = Constants.MenuItemPriority + 1)]
		private static void	Open()
		{
			EditorWindow.GetWindow<NGSettingsWindow>(false, NGSettingsWindow.Title, true);
		}

		/// <summary></summary>
		/// <param name="title">Defines the name of the Section.</param>
		/// <param name="callback">Method to draw GUI.</param>
		/// <param name="priority">The lower the nearest to the top.</param>
		public static void	AddSection(string title, Action callback, int priority = -1)
		{
			for (int i = 0; i < NGSettingsWindow.sections.Count; i++)
			{
				// Overwrite when found.
				if (NGSettingsWindow.sections[i].title == title)
				{
					NGSettingsWindow.sections[i].onGUI = callback;
					return;
				}
			}

			if (priority >= 0)
			{
				for (int i = 0; i < NGSettingsWindow.sections.Count; i++)
				{
					if (priority <= NGSettingsWindow.sections[i].priority ||
						NGSettingsWindow.sections[i].priority == -1)
					{
						NGSettingsWindow.sections.Insert(i, new Section(title, callback, priority));
						return;
					}
				}
			}

			NGSettingsWindow.sections.Add(new Section(title, callback, priority));
		}

		/// <summary></summary>
		/// <param name="title">Defines the name of the Section.</param>
		public static void	RemoveSection(string title)
		{
			for (int i = 0; i < NGSettingsWindow.sections.Count; i++)
			{
				if (NGSettingsWindow.sections[i].title == title)
				{
					NGSettingsWindow.sections.RemoveAt(i);
					break;
				}
			}
		}

		private void	InitGUIStyles()
		{
			if (this.sectionScrollView != null)
				return;

			this.sectionScrollView = new GUIStyle("PreferencesSectionBox");
			this.sectionScrollView.overflow.bottom++;
			this.sectionScrollView.onNormal = this.sectionScrollView.onHover;

			this.sectionElement = "PreferencesSection";
			
			this.selected = "ServerUpdateChangesetOn";
		}

		protected virtual void	OnEnable()
		{
			if (NGSettingsWindow.sections.Count > 0)
				this.workingSection = NGSettingsWindow.sections[0];
		}

		public void	OnGUI()
		{
			this.InitGUIStyles();

			EditorGUIUtility.labelWidth = 200f;
			GUILayout.BeginHorizontal();
			{
				this.sectionsScrollPosition = EditorGUILayout.BeginScrollView(this.sectionsScrollPosition, false, false,
																			  GUIStyle.none,
																			  GUI.skin.verticalScrollbar,
																			  sectionScrollView,
																			  new GUILayoutOption[] { GUILayout.Width(NGSettingsWindow.SectionWidth) });
				{
					GUILayout.BeginVertical();
					{
						GUILayout.Space(40f);

						for (int i = 0; i < NGSettingsWindow.sections.Count; i++)
						{
							NGSettingsWindow.sections[i].OptimizeFontSize(this.sectionElement);
							this.sectionElement.fontSize = NGSettingsWindow.sections[i].optimalFontSize;

							Rect	rect = GUILayoutUtility.GetRect(new GUIContent(NGSettingsWindow.sections[i].title), this.sectionElement, new GUILayoutOption[]
							{
								GUILayout.ExpandWidth(true)
							});

							if (NGSettingsWindow.sections[i] == this.workingSection && Event.current.type == EventType.Repaint)
							{
								this.selected.Draw(rect, false, false, false, false);
							}

							EditorGUI.BeginChangeCheck();
							if (GUI.Toggle(rect, this.workingSection == NGSettingsWindow.sections[i], NGSettingsWindow.sections[i].title, this.sectionElement))
							{
								this.workingSection = NGSettingsWindow.sections[i];
							}
							if (EditorGUI.EndChangeCheck())
							{
								GUIUtility.keyboardControl = 0;
							}
						}
						GUILayout.FlexibleSpace();

#if NGT_DEBUG
						if (Preferences.Settings != null)
							GUILayout.Label("Version " + Preferences.Settings.version, this.sectionElement);
#endif
					}
					GUILayout.EndVertical();
				}
				EditorGUILayout.EndScrollView();

				if (NGSettingsWindow.sections.Contains(this.workingSection) == true)
				{
					this.sectionScrollPosition = GUILayout.BeginScrollView(this.sectionScrollPosition);
					{
						GUILayout.BeginHorizontal();
						GUILayout.Space(1F);
						GUILayout.BeginVertical();
						{
							GUILayout.Label(this.workingSection.title, GeneralStyles.MainTitle);
							this.workingSection.onGUI();
						}
						GUILayout.EndVertical();
						GUILayout.EndHorizontal();
					}
					GUILayout.EndScrollView();
				}
				else if (NGSettingsWindow.sections.Count > 0 && Event.current.type == EventType.Repaint)
				{
					this.workingSection = NGSettingsWindow.sections[0];
					this.Repaint();
				}
			}
			GUILayout.EndHorizontal();
		}

		public void	Focus(string title)
		{
			for (int i = 0; i < NGSettingsWindow.sections.Count; i++)
			{
				if (NGSettingsWindow.sections[i].title == title)
					this.workingSection = NGSettingsWindow.sections[i];
			}
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGSettingsWindow.Title, Constants.WikiBaseURL + "#markdown-header-114-ng-settings", true);
		}
	}
}