using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public static class NGHierarchyEnhancer
	{
		private const string	Title = "NG Hierarchy Enhancer";
		private const string	HierarchyMethodName = "OnHierarchyGUI";

		private static Type[]			HierarchyMethodArguments = { typeof(Rect) };
		private static readonly Type	HierarchyMethodReturnType = typeof(float);

		private static Type			hierarchyType = null;
		private static PropertyInfo	wantsMouseMoveProperty = null;
		private static MethodInfo	repaintMethod = null;
		private static Object		instance = null;

		private static int	lastInstanceId = 0;
		private static bool	menuOpen = false;
		private static bool	holding = false;

		private static DynamicObjectMenu[]	objectMenus;

		private static Dictionary<Type, MethodInfo>	cachedMethods = new Dictionary<Type, MethodInfo>();
		private static int							lastInstanceID;
		private static Object						lastObject;
		private static Behaviour[]					lastBehaviours;

		static	NGHierarchyEnhancer()
		{
			Preferences.SettingsChanged += NGHierarchyEnhancer.Preferences_SettingsChanged;

			List<DynamicObjectMenu>	menus = new List<DynamicObjectMenu>();

			foreach (Type c in Utility.EachSubClassesOf(typeof(DynamicObjectMenu)))
			{
				menus.Add((DynamicObjectMenu)Activator.CreateInstance(c));
			}

			menus.Sort((a, b) => a.priority - b.priority);
			NGHierarchyEnhancer.objectMenus = menus.ToArray();

			NGHierarchyEnhancer.hierarchyType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
			if (NGHierarchyEnhancer.hierarchyType != null)
			{
				NGHierarchyEnhancer.wantsMouseMoveProperty = NGHierarchyEnhancer.hierarchyType.GetProperty("wantsMouseMove");
				NGHierarchyEnhancer.repaintMethod = NGHierarchyEnhancer.hierarchyType.GetMethod("Repaint");
			}
		}

		private static void	Preferences_SettingsChanged()
		{
			EditorApplication.hierarchyWindowItemOnGUI -= NGHierarchyEnhancer.ToggleGameObject;

			if (Preferences.Settings != null)
			{
				NGSettingsWindow.AddSection(NGHierarchyEnhancer.Title, NGHierarchyEnhancer.OnGUISettings);
				if (Preferences.Settings.hierarchy.enable == true)
					EditorApplication.hierarchyWindowItemOnGUI += NGHierarchyEnhancer.ToggleGameObject;
			}
			else
				NGSettingsWindow.RemoveSection(NGHierarchyEnhancer.Title);
		}

		private static void	ToggleGameObject(int instanceID, Rect selectionRect)
		{
			if ((NGHierarchyEnhancer.instance == null ||
				 // When an Object is destroyed, it returns null but is not null...
				 NGHierarchyEnhancer.instance.Equals(null) == true) &&
				NGHierarchyEnhancer.hierarchyType != null)
			{
				Object[]	consoles = Resources.FindObjectsOfTypeAll(NGHierarchyEnhancer.hierarchyType);

				if (consoles.Length > 0)
				{
					NGHierarchyEnhancer.instance = consoles[0];

					if (wantsMouseMoveProperty != null)
						wantsMouseMoveProperty.SetValue(NGHierarchyEnhancer.instance, true, null);
				}
			}

			if (Preferences.Settings.hierarchy.holdModifiers > 0 &&
				EditorWindow.mouseOverWindow == NGHierarchyEnhancer.instance)
			{
				// HACK Need to shift by one.
				// Ref Bug #720211_8cg6m8s7akdbf1r5
				NGHierarchyEnhancer.holding = ((int)Event.current.modifiers & ((int)Preferences.Settings.hierarchy.holdModifiers >> 1)) != 0;
			}

			selectionRect.width += selectionRect.x;
			selectionRect.x = 0F;

			if ((selectionRect.Contains(Event.current.mousePosition) == true && NGHierarchyEnhancer.holding == false) ||
				(NGHierarchyEnhancer.holding == true && NGHierarchyEnhancer.lastInstanceId == instanceID))
			{
				if (NGHierarchyEnhancer.lastInstanceId != instanceID &&
					NGHierarchyEnhancer.holding == false)
				{
					NGHierarchyEnhancer.lastInstanceId = instanceID;
					NGHierarchyEnhancer.menuOpen = false;
				}

				if (repaintMethod != null && instance != null)
					repaintMethod.Invoke(instance, null);

				Object	obj;

				if (instanceID == NGHierarchyEnhancer.lastInstanceID)
				{
					obj = NGHierarchyEnhancer.lastObject;
				}
				else
				{
					obj = EditorUtility.InstanceIDToObject(instanceID);
					NGHierarchyEnhancer.lastInstanceID = instanceID;
					NGHierarchyEnhancer.lastObject = obj;
					NGHierarchyEnhancer.lastBehaviours = null;
				}

				if (obj != null)
				{
					float	x = selectionRect.x;
					float	width = selectionRect.width;

					selectionRect.x += selectionRect.width - 30F - Preferences.Settings.hierarchy.margin;
					selectionRect.width = 30F;

					if (NGHierarchyEnhancer.holding == true ||
						selectionRect.Contains(Event.current.mousePosition) == true)
					{
						NGHierarchyEnhancer.menuOpen = true;
					}

					if (NGHierarchyEnhancer.menuOpen == false)
					{
						GUI.Button(selectionRect, "NG");
					}
					else
					{
						selectionRect.x = 0F;
						selectionRect.width = width + x - Preferences.Settings.hierarchy.margin;

						// Draws DynamicObjectMenu first.
						for (int i = 0; i < NGHierarchyEnhancer.objectMenus.Length; i++)
						{
							// Shrink available width with new end point on X axis.
							selectionRect.width = NGHierarchyEnhancer.objectMenus[i].DrawHierarchy(selectionRect, obj) - selectionRect.x;
						}

						// Then all sub-implementations.
						GameObject	gameObject = obj as GameObject;

						if (gameObject != null)
						{
							Behaviour[]	behaviours;

							if (NGHierarchyEnhancer.lastBehaviours == null)
							{
								behaviours = gameObject.GetComponents<Behaviour>();
								NGHierarchyEnhancer.lastBehaviours = behaviours;
							}
							else
								behaviours = NGHierarchyEnhancer.lastBehaviours;

							for (int i = 0; i < behaviours.Length; i++)
							{
								if (behaviours[i] == null)
									continue;

								MethodInfo	method;
								Type		type = behaviours[i].GetType();

								if (NGHierarchyEnhancer.cachedMethods.TryGetValue(type, out method) == false)
								{
									method = type.GetMethod(NGHierarchyEnhancer.HierarchyMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, NGHierarchyEnhancer.HierarchyMethodArguments, null);

									if (method != null)
									{
										if (method.ReturnType != NGHierarchyEnhancer.HierarchyMethodReturnType)
											method = null;
									}

									NGHierarchyEnhancer.cachedMethods.Add(type, method);
								}

								if (method != null)
									selectionRect.width = (float)method.Invoke(behaviours[i], new object[] { selectionRect }) - selectionRect.x;
							}
						}
					}
				}
			}
		}

		private static void	OnGUISettings()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_EnableDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.hierarchy.enable = EditorGUILayout.Toggle(LC.G("Enable"), Preferences.Settings.hierarchy.enable);
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (Preferences.Settings.hierarchy.enable == false)
					EditorApplication.hierarchyWindowItemOnGUI -= NGHierarchyEnhancer.ToggleGameObject;
				else
					EditorApplication.hierarchyWindowItemOnGUI += NGHierarchyEnhancer.ToggleGameObject;
				Preferences.InvalidateSettings();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_MarginDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.hierarchy.margin = EditorGUILayout.FloatField(LC.G("NGHierarchyEnhancer_Margin"), Preferences.Settings.hierarchy.margin);
			if (EditorGUI.EndChangeCheck() == true)
				Preferences.InvalidateSettings();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_HoldModifiersDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.hierarchy.holdModifiers = (EventModifiers)EditorGUILayout.EnumMaskField(new GUIContent(LC.G("NGHierarchyEnhancer_HoldModifiers")), Preferences.Settings.hierarchy.holdModifiers);
			if (EditorGUI.EndChangeCheck() == true)
				Preferences.InvalidateSettings();
		}
	}
}