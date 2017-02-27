using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGPrefs
{
	[InitializeOnLoad]
	public class NGPrefsWindow : EditorWindow, IHasCustomMenu
	{
		public const string	Title = "ƝƓ Ᵽrefs";
		public const float	MinLabelWidth = 80F;
		public const float	MinValueWidth = 100F;
		public const float	AutoRestoreLabelWidthThreshold = 100F;
		public const int	MaxStringLength = 16382;

		[SetColor(51F / 255F, 51F / 255F, 51F / 255F, 1F, 166F / 255F, 166F / 255F, 166F / 255F, 1F)]
		private static Color	AlteredPrefBackgroundColor = default(Color);
		private static Color	ColumnHeaderBackgroundColor = Color.grey;

		public enum PrefType
		{
			Int,
			Float,
			String
		}

		public bool			showAdd;
		public string		filter;
		public float		labelWidth = EditorGUIUtility.labelWidth;
		public PrefType		prefType;
		public string		newKey;
		public int			newValueInt;
		public float		newValueFloat;
		public string		newValueString;

		private PrefsManager[]	prefManagers;
		private string[]		prefManagerNames;
		private int				currentManager;

		private List<int>	filteredIndexes;

		private Rect		bodyRect;
		private Vector2		scrollPosition;
		private GUIContent	content = new GUIContent();
		private GUIContent	resetContent = new GUIContent();
		private GUIContent	applyContent = new GUIContent();

		private double	lastClick;
		private bool	draggingSplitterBar = false;
		private float	originPositionX;
		private float	originLabelWidth;

		[NonSerialized]
		private int			editingStringIndex = -1;
		private string		tempFilePath;
		private byte[]		lastHash;
		private DateTime	lastFileChange;

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGPrefsWindow.Title + ".");

		static	NGPrefsWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGPrefsWindow.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGPrefsWindow.Title, priority = Constants.MenuItemPriority + 310)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGPrefsWindow>(NGPrefsWindow.Title);
		}

		protected virtual void	OnEnable()
		{
			this.filteredIndexes = new List<int>();

			this.prefManagers = Utility.CreateInstancesOf<PrefsManager>();
			this.prefManagerNames = this.prefManagers.Select(p => Utility.NicifyVariableName(p.GetType().Name)).ToArray();

			if (this.currentManager >= this.prefManagers.Length)
				this.currentManager = this.prefManagers.Length - 1;

			if (this.currentManager >= 0)
			{
				try
				{
					this.prefManagers[this.currentManager].LoadPreferences();
					this.UpdateFilteredIndexes();
				}
				catch (Exception ex)
				{
					this.errorPopup.exception = ex;
				}
			}

			this.minSize = new Vector2(this.minSize.x, 100F);

			this.resetContent = new GUIContent("↺", LC.G("NGPrefs_Reset"));
			this.applyContent = new GUIContent("↣", LC.G("NGPrefs_Apply"));
		}

		protected virtual void	OnGUI()
		{
			Rect	r = this.position;
			r.x = 0F;
			r.y = 0F;

			if (this.prefManagers.Length > 0)
			{
				r.height = EditorGUIUtility.singleLineHeight;
				GUILayout.BeginArea(r);
				{
					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.currentManager = EditorGUILayout.Popup(this.currentManager, this.prefManagerNames, GeneralStyles.ToolbarDropDown);
						if (EditorGUI.EndChangeCheck() == true)
						{
							try
							{
								this.prefManagers[this.currentManager].LoadPreferences();
								this.UpdateFilteredIndexes();
							}
							catch (Exception ex)
							{
								this.errorPopup.exception = ex;
							}
							return;
						}

						if (GUILayout.Button(this.showAdd == true ? "˄" : "˅", GeneralStyles.ToolbarDropDown, GUILayout.Width(24F)) == true)
						{
							this.showAdd = !this.showAdd;
							return;
						}

						EditorGUI.BeginChangeCheck();
						this.filter = EditorGUILayout.TextField(this.filter, GeneralStyles.ToolbarSearchTextField);
						if (EditorGUI.EndChangeCheck() == true)
						{
							this.UpdateFilteredIndexes();
							return;
						}

						if (GUILayout.Button("", GeneralStyles.ToolbarSearchCancelButton) == true)
						{
							this.filter = string.Empty;
							GUI.FocusControl(null);
							this.UpdateFilteredIndexes();
							return;
						}

						if (GUILayout.Button(LC.G("NGPrefs_Refresh"), GeneralStyles.ToolbarButton, GUILayout.MaxWidth(100F)))
						{
							try
							{
								this.prefManagers[this.currentManager].LoadPreferences();
								this.UpdateFilteredIndexes();
							}
							catch (Exception ex)
							{
								this.errorPopup.exception = ex;
							}
							return;
						}

						if (string.IsNullOrEmpty(this.filter) == true)
						{
							if (GUILayout.Button(LC.G("NGPrefs_ClearAll"), GeneralStyles.ToolbarButton, GUILayout.MaxWidth(100F)) &&
								((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(LC.G("NGPrefs_ClearAll"), LC.G("NGPrefs_ClearAllConfirm"), LC.G("Yes"), LC.G("No")) == true))
							{
								try
								{
									this.prefManagers[this.currentManager].DeleteAll();
								}
								catch (Exception ex)
								{
									this.errorPopup.exception = ex;
								}
								return;
							}
						}
						else
						{
							if (GUILayout.Button(LC.G("NGPrefs_ClearList"), GeneralStyles.ToolbarButton, GUILayout.MaxWidth(100F)) &&
								((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(LC.G("NGPrefs_ClearList"), LC.G("NGPrefs_ClearListConfirm"), LC.G("Yes"), LC.G("No")) == true))
							{
								try
								{
									for (int k = 0; k < this.filteredIndexes.Count; k++)
									{
										int	i = this.filteredIndexes[k];

										this.prefManagers[this.currentManager].DeleteKey(this.prefManagers[this.currentManager].keys[i]);
									}

									for (int k = 0; k < this.filteredIndexes.Count; k++)
									{
										int	i = this.filteredIndexes[k];

										this.prefManagers[this.currentManager].DeleteIndex(i - k);
									}

									this.filteredIndexes.Clear();
								}
								catch (Exception ex)
								{
									this.errorPopup.exception = ex;
								}
								return;
							}
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				GUILayout.EndArea();

				if (this.showAdd == true)
				{
					r.y += r.height;
					GUILayout.BeginArea(r);
					{
						EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
						{
							using (LabelWidthRestorer.Get(60F))
							{
								this.prefType = (PrefType)EditorGUILayout.EnumPopup(LC.G("NGPrefs_Type"), this.prefType, GeneralStyles.ToolbarDropDown, GUILayout.ExpandWidth(false));
								this.newKey = EditorGUILayout.TextField(LC.G("NGPrefs_Key"), this.newKey, GeneralStyles.ToolbarTextField, GUILayout.ExpandWidth(true));

								switch (this.prefType)
								{
									case PrefType.Int:
										this.newValueInt = EditorGUILayout.IntField(LC.G("NGPrefs_Value"), this.newValueInt, GeneralStyles.ToolbarTextField, GUILayout.ExpandWidth(true));
										break;
									case PrefType.Float:
										this.newValueFloat = EditorGUILayout.FloatField(LC.G("NGPrefs_Value"), this.newValueFloat, GeneralStyles.ToolbarTextField, GUILayout.ExpandWidth(true));
										break;
									case PrefType.String:
										this.newValueString = EditorGUILayout.TextField(LC.G("NGPrefs_Value"), this.newValueString, GeneralStyles.ToolbarTextField, GUILayout.ExpandWidth(true));
										break;
								}

								GUI.enabled = string.IsNullOrEmpty(this.newKey) == false;
								if (GUILayout.Button(LC.G("NGPrefs_Add"), GeneralStyles.ToolbarButton, GUILayout.MaxWidth(100F)))
								{
									switch (this.prefType)
									{
										case PrefType.Int:
											this.prefManagers[this.currentManager].SetInt(this.newKey, this.newValueInt);
											break;
										case PrefType.Float:
											this.prefManagers[this.currentManager].SetFloat(this.newKey, this.newValueFloat);
											break;
										case PrefType.String:
											this.prefManagers[this.currentManager].SetString(this.newKey, this.newValueString);
											break;
									}

									try
									{
										this.prefManagers[this.currentManager].LoadPreferences();
										this.UpdateFilteredIndexes();
									}
									catch (Exception ex)
									{
										this.errorPopup.exception = ex;
									}
									return;
								}
								GUI.enabled = true;
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					GUILayout.EndArea();
				}
			}

			r.y += r.height;

			if (this.errorPopup.exception != null)
			{
				r.height = this.errorPopup.boxHeight;
				this.errorPopup.OnGUIRect(r);
				r.y += r.height;

				r.height = EditorGUIUtility.singleLineHeight;
			}

			if (this.currentManager < 0)
				return;

			r.y += 1F;

			this.labelWidth = Mathf.Clamp(this.labelWidth, NGPrefsWindow.MinLabelWidth, this.position.width - NGPrefsWindow.MinValueWidth);
			r.width = this.labelWidth;
			EditorGUI.LabelField(r, LC.G("NGPrefs_Key"), GeneralStyles.CenterText);

			r.x += r.width + 20F;
			r.width = 2F;
			EditorGUI.DrawRect(r, Color.grey);

			r.x -= 1F;
			r.width += 2F;
			EditorGUIUtility.AddCursorRect(r, MouseCursor.ResizeHorizontal);

			if (this.draggingSplitterBar == true &&
				Event.current.type == EventType.MouseDrag)
			{
				if (Mathf.Abs(r.y - Event.current.mousePosition.y) > NGPrefsWindow.AutoRestoreLabelWidthThreshold)
					this.labelWidth = this.originLabelWidth;
				else
					this.labelWidth = Mathf.Clamp(this.originLabelWidth - this.originPositionX + Event.current.mousePosition.x, NGPrefsWindow.MinLabelWidth, this.position.width - NGPrefsWindow.MinValueWidth);
				Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseDown &&
					 r.Contains(Event.current.mousePosition) == true)
			{
				this.originPositionX = Event.current.mousePosition.x;
				this.originLabelWidth = this.labelWidth;
				this.draggingSplitterBar = true;
				Event.current.Use();
			}
			else if (this.draggingSplitterBar == true &&
					 Event.current.type == EventType.MouseUp)
			{
				// Auto adjust height on left click or double click.
				if (r.Contains(Event.current.mousePosition) == true &&
					(Event.current.button == 1 ||
					 (this.lastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup &&
					  Mathf.Abs(this.originPositionX - Event.current.mousePosition.x) < 5F)))
				{
					this.labelWidth = EditorGUIUtility.labelWidth;
				}
				this.lastClick = EditorApplication.timeSinceStartup;
				this.draggingSplitterBar = false;
				Event.current.Use();
			}

			r.width = this.position.width - r.x;
			EditorGUI.LabelField(r, LC.G("NGPrefs_Value"), GeneralStyles.CenterText);

			r.x = 0F;
			r.y += r.height + 1F;
			r.width = this.position.width;

			r.height = 1F;
			EditorGUI.DrawRect(r, NGPrefsWindow.ColumnHeaderBackgroundColor);

			r.y += 3F;
			r.height = this.position.height - r.y;

			this.bodyRect = r;

			Rect	viewRect = new Rect();
			viewRect.height = this.filteredIndexes.Count * EditorGUIUtility.singleLineHeight;
			viewRect.width = r.width - ((viewRect.height >= this.bodyRect.height) ? 15F : 0F);

			using (LabelWidthRestorer.Get(this.labelWidth))
			{
				this.scrollPosition = GUI.BeginScrollView(r, this.scrollPosition, viewRect);
				{
					r.y = 0F;
					r.height = EditorGUIUtility.singleLineHeight;
					r.width -= ((viewRect.height >= this.bodyRect.height) ? 15F : 0F);

					this.OnGUIPreferences(r);
				}
				GUI.EndScrollView();
			}
		}

		private void	UpdateFilteredIndexes()
		{
			this.filteredIndexes.Clear();

			if (string.IsNullOrEmpty(this.filter) == false)
			{
				for (int i = 0; i < this.prefManagers[this.currentManager].keys.Count; i++)
				{
					if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(this.prefManagers[this.currentManager].keys[i], this.filter, CompareOptions.IgnoreCase) >= 0)
						this.filteredIndexes.Add(i);
				}
			}
			else
			{
				for (int i = 0; i < this.prefManagers[this.currentManager].keys.Count; i++)
					this.filteredIndexes.Add(i);
			}
		}

		private void	OnGUIPreferences(Rect r)
		{
			PrefsManager	manager = this.prefManagers[this.currentManager];
			float			x = r.x;
			float			width = r.width;
			bool			nullValue = false;

			try
			{
				for (int k = 0; k < this.filteredIndexes.Count; k++)
				{
					if (r.y + r.height <= this.scrollPosition.y)
					{
						r.y += r.height;
						continue;
					}

					int		i = this.filteredIndexes[k];
					string	key = manager.keys[i];
					object	value = manager.values[i];

					if (value == null)
					{
						if (nullValue == false)
							manager.BeginLoadFromRegistrar();
						manager.LoadValueFromRegistrar(i);
						value = manager.values[i];
					}

					r.x = x;
					r.width = 20F;
					if (GUI.Button(r, "X", GeneralStyles.ToolbarCloseButton) == true && ((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(LC.G("NGPrefs_DeletePref"), string.Format(LC.G("NGPrefs_DeletePrefConfirm"), key), LC.G("Yes"), LC.G("No")) == true))
					{
						manager.DeleteKey(i);

						this.UpdateFilteredIndexes();

						if (this.editingStringIndex == i)
							this.editingStringIndex = -1;

						break;
					}

					r.x += r.width;

					this.content.text = key;
					this.content.tooltip = key;

					bool	isAltered = ((value is int && (int)value != manager.GetInt(key)) ||
										 (value is float && (float)value != manager.GetFloat(key)) ||
										 (value is string && (string)value != manager.GetString(key)));

					r.width = (isAltered == true) ? width - r.x - 60F : width - r.x;

					using (BgColorContentRestorer.Get(isAltered, NGPrefsWindow.AlteredPrefBackgroundColor))
					{
						if (value is int)
							manager.values[i] = EditorGUI.IntField(r, this.content, (int)value);
						else if (value is string)
						{
							string	content = (string)value;

							if (content.Length <= NGPrefsWindow.MaxStringLength)
								manager.values[i] = EditorGUI.TextField(r, this.content, content);
							else
							{
								EditorGUI.LabelField(r, this.content);

								r.x += labelWidth;
								r.width -= labelWidth;

								if (this.editingStringIndex != i)
								{
									if (GUI.Button(r, "String has more than " + NGPrefsWindow.MaxStringLength + " chars. Click to edit.") == true)
									{
										this.editingStringIndex = i;
										this.tempFilePath = Path.Combine(Application.temporaryCachePath, Path.GetRandomFileName() + ".txt");

										File.WriteAllText(this.tempFilePath, content);

										this.lastFileChange = DateTime.Now;

										using (var md5 = MD5.Create())
										using (var stream = File.OpenRead(this.tempFilePath))
										{
											this.lastHash = md5.ComputeHash(stream);
										}

										EditorUtility.OpenWithDefaultApp(this.tempFilePath);

										Utility.UnregisterIntervalCallback(this.CheckTempFile);
										Utility.RegisterIntervalCallback(this.CheckTempFile, 100);
									}
								}
								else
								{
									if (GUI.Button(r, "Editing... Last changed at " + this.lastFileChange.ToString("HH:mm:ss") + ". Click to stop.") == true)
									{
										this.editingStringIndex = -1;
										Utility.UnregisterIntervalCallback(this.CheckTempFile);
									}
								}
							}
						}
						else if (value is float)
							manager.values[i] = EditorGUI.FloatField(r, this.content, (float)value);
					}

					r.x += r.width;

					if (isAltered == true)
					{
						r.width = 30F;
						if (GUI.Button(r, this.resetContent, GeneralStyles.ToolbarAltButton) == true)
						{
							if (manager.HasKey(key) == false)
							{
								try
								{
									manager.LoadPreferences();
									this.UpdateFilteredIndexes();
								}
								catch (Exception ex)
								{
									this.errorPopup.exception = ex;
								}
								break;
							}
							else if (value is int)
								manager.values[i] = manager.GetInt(key);
							else if (value is float)
								manager.values[i] = manager.GetFloat(key);
							else if (value is string)
								manager.values[i] = manager.GetString(key);
						}
						r.x += r.width;

						if (GUI.Button(r, this.applyContent, GeneralStyles.ToolbarValidButton) == true)
						{
							if (value is int)
								manager.SetInt(key, (int)value);
							else if (value is float)
								manager.SetFloat(key, (float)value);
							else if (value is string)
								manager.SetString(key, (string)value);
						}
					}

					r.y += r.height;

					if (r.y - this.scrollPosition.y >= this.bodyRect.height)
						break;
				}
			}
			catch (Exception ex)
			{
				this.errorPopup.exception = ex;
			}
			finally
			{
				if (nullValue == true)
					manager.EndLoadFromRegistrar();
			}
		}

		private void	CheckTempFile()
		{
			using (var md5 = MD5.Create())
			using (var stream = File.OpenRead(this.tempFilePath))
			{
				PrefsManager	manager = this.prefManagers[this.currentManager];
				byte[]			newHash = md5.ComputeHash(stream);

				for (int i = 0; i < newHash.Length; i++)
				{
					if (newHash[i] != this.lastHash[i])
					{
						this.lastHash = newHash;
						this.lastFileChange = DateTime.Now;

						manager.values[this.editingStringIndex] = File.ReadAllText(this.tempFilePath);

						this.Repaint();
						break;
					}
				}
			}
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGPrefsWindow.Title, Constants.WikiBaseURL + "#markdown-header-17-ng-prefs-windows-only-for-the-moment");
		}
	}
}