using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGCheckGUIDWindow : EditorWindow
	{
		[Serializable]
		private sealed class Entry
		{
			public string		lastGUID;
			public string		assetPath;
			public Object		assetObject;
			public Texture2D	assetPreview;
		}

		public const string	Title = "ƝƓ Ҁheck GUID";

		private List<string>	GUIDs = new List<string>();
		private List<Entry>		entries = new List<Entry>();

		private Vector2	scrollPosition;
		private string	lastCopyBuffer;

		static	NGCheckGUIDWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGCheckGUIDWindow.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGCheckGUIDWindow.Title, priority = Constants.MenuItemPriority + 369)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGCheckGUIDWindow>(true, NGCheckGUIDWindow.Title, true);
		}

		protected virtual void	OnEnable()
		{
			if (this.GUIDs.Count == 0)
			{
				this.GUIDs.Add(string.Empty);
				this.entries.Add(new Entry());
			}
		}

		protected virtual void	OnGUI()
		{
			if (this.GUIDs[this.GUIDs.Count - 1] != string.Empty)
			{
				this.GUIDs.Add(string.Empty);
				this.entries.Add(new Entry());
			}

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				using (LabelWidthRestorer.Get(50F))
				{
					for (int i = 0; i < this.GUIDs.Count; i++)
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.BeginVertical();
							{
								EditorGUI.BeginChangeCheck();
								this.GUIDs[i] = EditorGUILayout.TextField("GUID", this.GUIDs[i]);
								if (EditorGUI.EndChangeCheck() == true)
								{
									if (string.IsNullOrEmpty(this.GUIDs[i]) == false)
									{
										if (this.entries[i].lastGUID != this.GUIDs[i])
										{
											this.entries[i].lastGUID = this.GUIDs[i];

											this.ProcessGUID(i);
										}
									}
									else
									{
										this.entries[i].lastGUID = string.Empty;
										this.entries[i].assetPath = string.Empty;
										this.entries[i].assetObject = null;
										this.entries[i].assetPreview = null;
									}
								}

								if (string.IsNullOrEmpty(this.GUIDs[i]) == true)
								{
									for (int j = i + 1; j < this.GUIDs.Count;)
									{
										if (string.IsNullOrEmpty(this.GUIDs[j]) == true)
										{
											this.GUIDs.RemoveAt(j);
											this.entries.RemoveAt(j);
											this.Repaint();
										}
										else
											break;
									}
								}

								EditorGUI.BeginChangeCheck();
								Object	asset = EditorGUILayout.ObjectField("Asset", this.entries[i].assetObject, typeof(Object), false);
								if (EditorGUI.EndChangeCheck() == true)
								{
									this.entries[i].assetObject = asset;
									if (asset != null)
										this.GUIDs[i] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
									else
										this.GUIDs[i] = string.Empty;
								}
							}
							EditorGUILayout.EndVertical();

							if (this.entries[i].assetPreview != null)
							{
								EditorGUI.DrawTextureTransparent(GUILayoutUtility.GetRect(0F, 0F, GUILayout.Width(32F), GUILayout.Height(32F)),
																 this.entries[i].assetPreview,
																 ScaleMode.ScaleToFit);
							}
						}
						EditorGUILayout.EndHorizontal();

						GUILayout.Space(10F);
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}

		protected virtual void	Update()
		{
			if (EditorGUIUtility.systemCopyBuffer.Length == 32 &&
				this.lastCopyBuffer != EditorGUIUtility.systemCopyBuffer)
			{
				this.lastCopyBuffer = EditorGUIUtility.systemCopyBuffer;

				for (int i = 0; i < this.GUIDs.Count; i++)
				{
					if (this.GUIDs[i] == EditorGUIUtility.systemCopyBuffer)
						return;
				}

				if (this.GUIDs[this.GUIDs.Count - 1] == string.Empty)
					this.GUIDs[this.GUIDs.Count - 1] = EditorGUIUtility.systemCopyBuffer;
				else
				{
					this.GUIDs.Add(EditorGUIUtility.systemCopyBuffer);
					this.entries.Add(new Entry());
				}

				this.ProcessGUID(this.GUIDs.Count - 1);
			}
		}

		private void	ProcessGUID(int i)
		{
			if (this.GUIDs[i].Length != 32)
			{
				this.entries[i].assetPath = "GUID must be 32 length.";
				this.entries[i].assetObject = null;
				this.entries[i].assetPreview = null;
				return;
			}

			this.entries[i].assetPath = AssetDatabase.GUIDToAssetPath(this.GUIDs[i]);

			if (string.IsNullOrEmpty(this.entries[i].assetPath) == true)
			{
				this.entries[i].assetPath = "Not found";
				this.entries[i].assetObject = null;
				this.entries[i].assetPreview = null;
				return;
			}

			this.entries[i].assetObject = AssetDatabase.LoadAssetAtPath(this.entries[i].assetPath, typeof(Object));

			if (this.entries[i].assetObject != null)
				this.entries[i].assetPreview = Utility.GetIcon(this.entries[i].assetObject.GetInstanceID());
		}
	}
}