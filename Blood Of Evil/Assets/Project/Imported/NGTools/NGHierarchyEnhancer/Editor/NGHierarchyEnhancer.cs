using NGTools;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace NGToolsEditor.NGHierarchyEnhancer
{
	using UnityEngine;

	[InitializeOnLoad]
	internal static class NGHierarchyEnhancer
	{
		private const string	Title = "ƝƓ Ħierarchy ∑nhancer";
		private const string	HierarchyMethodName = "OnHierarchyGUI";
		private const float		width = 120F;

		private static Type[]			HierarchyMethodArguments = { typeof(Rect) };
		private static readonly Type	HierarchyMethodReturnType = typeof(float);

		private static Type			hierarchyType = null;
		private static PropertyInfo	wantsMouseMoveProperty = null;
		private static MethodInfo	repaintMethod = null;
		private static Object		instance = null;

		private static int	lastInstanceId = 0;
		private static bool	menuOpen = false;
		private static bool	holding = false;
		private static bool	selectionHolding = false;

		private static DynamicObjectMenu[]	objectMenus;

		private static Dictionary<Type, MethodInfo>	cachedMethods = new Dictionary<Type, MethodInfo>();
		private static int							lastInstanceID;
		private static Object						lastObject;
		private static Behaviour[]					lastBehaviours;
		private static Rect							iconRect = new Rect(0f, 0f, 16F, 16F);
		private static Dictionary<string, float>	cacheXOffset = new Dictionary<string, float>();
		private static List<Component>				cacheComponents = new List<Component>();

		private static List<NGSettings.HierarchyEnhancer.ComponentColor>	colors = new List<NGSettings.HierarchyEnhancer.ComponentColor>();
		private static ReorderableList										reorder;
		private static int													pickTypeIndex = 0;

		private static string[]	eventModifierNames;

		static	NGHierarchyEnhancer()
		{
			Preferences.SettingsChanged += NGHierarchyEnhancer.Preferences_SettingsChanged;

			List<DynamicObjectMenu>	menus = new List<DynamicObjectMenu>();

			foreach (Type c in Utility.EachSubClassesOf(typeof(DynamicObjectMenu)))
				menus.Add(Activator.CreateInstance(c) as DynamicObjectMenu);

			menus.Sort((a, b) => a.priority - b.priority);
			NGHierarchyEnhancer.objectMenus = menus.ToArray();

			NGHierarchyEnhancer.hierarchyType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
			if (NGHierarchyEnhancer.hierarchyType != null)
			{
				NGHierarchyEnhancer.wantsMouseMoveProperty = NGHierarchyEnhancer.hierarchyType.GetProperty("wantsMouseMove");
				NGHierarchyEnhancer.repaintMethod = NGHierarchyEnhancer.hierarchyType.GetMethod("Repaint");
			}

			string[]	names = Enum.GetNames(typeof(EventModifiers));

			NGHierarchyEnhancer.eventModifierNames = new string[names.Length - 1];
			for (int i = 1; i < names.Length; i++)
				NGHierarchyEnhancer.eventModifierNames[i - 1] = names[i];
		}

		private static void	Preferences_SettingsChanged()
		{
			EditorApplication.hierarchyWindowItemOnGUI -= NGHierarchyEnhancer.DrawOverlay;

			if (Preferences.Settings != null)
			{
				Preferences.Settings.hierarchy.InitializeLayers();
				NGSettingsWindow.AddSection(NGHierarchyEnhancer.Title, NGHierarchyEnhancer.OnGUISettings);
				if (Preferences.Settings.hierarchy.enable == true)
					EditorApplication.hierarchyWindowItemOnGUI += NGHierarchyEnhancer.DrawOverlay;
			}
			else
				NGSettingsWindow.RemoveSection(NGHierarchyEnhancer.Title);
		}

		private static void	DrawOverlay(int instanceID, Rect selectionRect)
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

					if (NGHierarchyEnhancer.wantsMouseMoveProperty != null)
						NGHierarchyEnhancer.wantsMouseMoveProperty.SetValue(NGHierarchyEnhancer.instance, true, null);
				}
			}

			if (EditorWindow.mouseOverWindow == NGHierarchyEnhancer.instance)
			{
				// HACK Need to shift by one.
				// Ref Bug #720211_8cg6m8s7akdbf1r5
				if (Preferences.Settings.hierarchy.holdModifiers > 0)
					NGHierarchyEnhancer.holding = ((int)Event.current.modifiers & ((int)Preferences.Settings.hierarchy.holdModifiers)) == ((int)Preferences.Settings.hierarchy.holdModifiers);
				if (Preferences.Settings.hierarchy.selectionHoldModifiers > 0)
					NGHierarchyEnhancer.selectionHolding = ((int)Event.current.modifiers & ((int)Preferences.Settings.hierarchy.selectionHoldModifiers)) == ((int)Preferences.Settings.hierarchy.selectionHoldModifiers);
			}

			selectionRect.width += selectionRect.x;
			selectionRect.x = 0F;

			Object	obj;

			if (instanceID == NGHierarchyEnhancer.lastInstanceID)
				obj = NGHierarchyEnhancer.lastObject;
			else
			{
				obj = EditorUtility.InstanceIDToObject(instanceID);
				NGHierarchyEnhancer.lastInstanceID = instanceID;
				NGHierarchyEnhancer.lastObject = obj;
				NGHierarchyEnhancer.lastBehaviours = null;
			}

			if (obj != null)
			{
				GameObject	go = obj as GameObject;

				if (Preferences.Settings.hierarchy.layers != null &&
					Preferences.Settings.hierarchy.layers.Length > go.layer &&
					Preferences.Settings.hierarchy.layers[go.layer].a > 0F)
				{
					EditorGUI.DrawRect(selectionRect, Preferences.Settings.hierarchy.layers[go.layer]);
				}
				if (Preferences.Settings.hierarchy.layersIcon != null &&
					Preferences.Settings.hierarchy.layersIcon.Length > go.layer &&
					Preferences.Settings.hierarchy.layersIcon[go.layer] != null)
				{
					iconRect.y = selectionRect.y;

					int			indentLevel = 0;
					Transform	t = go.transform;
					for (int i = 0; t != null; i++)
					{
						t = t.parent;
						++indentLevel;
					}

					float	offset;
					if (NGHierarchyEnhancer.cacheXOffset.TryGetValue(go.name, out offset) == false)
					{
						Utility.content.text = go.name;
						offset = GUI.skin.label.CalcSize(Utility.content).x;
					}

					iconRect.x = offset + indentLevel * 16F;
					GUI.DrawTexture(iconRect, Preferences.Settings.hierarchy.layersIcon[go.layer], ScaleMode.ScaleToFit);
				}

				// Draw Component' color over layer's background color.
				go.GetComponents<Component>(cacheComponents);

				Rect	r = selectionRect;

				if (Preferences.Settings.hierarchy.widthPerComponent > 0F)
				{
					r.width = Preferences.Settings.hierarchy.widthPerComponent;
					for (int i = 0; i < cacheComponents.Count; i++)
					{
						for (int j = 0; j < Preferences.Settings.hierarchy.componentColors.Length; j++)
						{
							if (Preferences.Settings.hierarchy.componentColors[j].type == null)
								continue;

							if (cacheComponents[i].GetType() == Preferences.Settings.hierarchy.componentColors[j].type)
							{
								EditorGUI.DrawRect(r, Preferences.Settings.hierarchy.componentColors[j].color);
								r.x += r.width;
								break;
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < Preferences.Settings.hierarchy.componentColors.Length; j++)
					{
						if (Preferences.Settings.hierarchy.componentColors[j].type == null)
							continue;

						int	i = 0;

						for (; i < cacheComponents.Count; i++)
						{
							if (cacheComponents[i].GetType() == Preferences.Settings.hierarchy.componentColors[j].type)
							{
								EditorGUI.DrawRect(r, Preferences.Settings.hierarchy.componentColors[j].color);
								break;
							}
						}

						if (i < cacheComponents.Count)
							break;
					}
				}
			}

			if (NGHierarchyEnhancer.IsInSelection(obj) ||
				(selectionRect.Contains(Event.current.mousePosition) == true && NGHierarchyEnhancer.holding == false) ||
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

				if (obj != null)
				{
					float	x = selectionRect.x;
					float	width = selectionRect.width;

					selectionRect.x += selectionRect.width - 30F - Preferences.Settings.hierarchy.margin;
					selectionRect.width = 30F;

					if ((NGHierarchyEnhancer.selectionHolding == true && NGHierarchyEnhancer.IsInSelection(obj)) ||
						NGHierarchyEnhancer.holding == true ||
						selectionRect.Contains(Event.current.mousePosition) == true)
					{
						NGHierarchyEnhancer.menuOpen = true;
					}

					if (NGHierarchyEnhancer.menuOpen == false)
						GUI.Button(selectionRect, "NG");
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

									if (method != null && method.ReturnType != NGHierarchyEnhancer.HierarchyMethodReturnType)
										method = null;

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

		private static bool	IsInSelection(Object obj)
		{
			if (NGHierarchyEnhancer.selectionHolding == true && obj != null)
			{
				for (int i = 0; i < Selection.objects.Length; i++)
				{
					if (Selection.objects[i] == obj)
						return true;
				}
			}

			return false;
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
					EditorApplication.hierarchyWindowItemOnGUI -= NGHierarchyEnhancer.DrawOverlay;
				else
					EditorApplication.hierarchyWindowItemOnGUI += NGHierarchyEnhancer.DrawOverlay;
				Preferences.InvalidateSettings();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_MarginDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.hierarchy.margin = EditorGUILayout.FloatField(LC.G("NGHierarchyEnhancer_Margin"), Preferences.Settings.hierarchy.margin);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_HoldModifiersDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.hierarchy.holdModifiers = (EventModifiers)EditorGUILayout.MaskField(new GUIContent(LC.G("NGHierarchyEnhancer_HoldModifiers")), (int)Preferences.Settings.hierarchy.holdModifiers, NGHierarchyEnhancer.eventModifierNames);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_SelectionHoldModifiersDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.hierarchy.selectionHoldModifiers = (EventModifiers)EditorGUILayout.MaskField(new GUIContent(LC.G("NGHierarchyEnhancer_SelectionHoldModifiers")), (int)Preferences.Settings.hierarchy.selectionHoldModifiers, NGHierarchyEnhancer.eventModifierNames);
			if (EditorGUI.EndChangeCheck() == true)
				Preferences.InvalidateSettings();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_LayersDescription"), GeneralStyles.WrapLabel);

			float	maxLabelWidth = NGHierarchyEnhancer.width;

			for (int i = 0; i < NGSettings.HierarchyEnhancer.TotalLayers; i++)
			{
				string	layerName = LayerMask.LayerToName(i);

				if (layerName == string.Empty)
					layerName = "Layer " + i;

				Utility.content.text = layerName;
				float	width = GUI.skin.label.CalcSize(Utility.content).x;
				if (maxLabelWidth < width + 20F) // Add width for the icon.
					maxLabelWidth = width + 20F;
			}

			using (LabelWidthRestorer.Get(maxLabelWidth))
			{
				for (int i = 0; i < NGSettings.HierarchyEnhancer.TotalLayers; i++)
				{
					string	layerName = LayerMask.LayerToName(i);

					if (layerName == string.Empty)
						layerName = "Layer " + i;

					EditorGUILayout.BeginHorizontal();

					// (Label + icon) + color picker
					Rect	r = GUILayoutUtility.GetRect(maxLabelWidth + 40F, 16F, GUI.skin.label);

					Utility.content.text = layerName;
					float	width = GUI.skin.label.CalcSize(Utility.content).x;

					Preferences.Settings.hierarchy.layers[i] = EditorGUI.ColorField(r, layerName, Preferences.Settings.hierarchy.layers[i]);
					r.width = maxLabelWidth;
					EditorGUI.DrawRect(r, Preferences.Settings.hierarchy.layers[i]);

					if (Preferences.Settings.hierarchy.layersIcon[i] != null)
					{
						r.x += width + 2F; // Little space before the icon.
						r.width = 16F;
						GUI.DrawTexture(r, Preferences.Settings.hierarchy.layersIcon[i], ScaleMode.ScaleToFit);
					}

					Preferences.Settings.hierarchy.layersIcon[i] = EditorGUILayout.ObjectField(Preferences.Settings.hierarchy.layersIcon[i], typeof(Texture2D), false) as Texture2D;
					EditorGUILayout.EndHorizontal();
				}
			}

			if (EditorGUI.EndChangeCheck() == true)
			{
				ByteBuffer	buffer = Utility.GetBBuffer();

				for (int i = 0; i < NGSettings.HierarchyEnhancer.TotalLayers; i++)
				{
					buffer.Append(Preferences.Settings.hierarchy.layers[i].r);
					buffer.Append(Preferences.Settings.hierarchy.layers[i].g);
					buffer.Append(Preferences.Settings.hierarchy.layers[i].b);
					buffer.Append(Preferences.Settings.hierarchy.layers[i].a);
				}

				Preferences.Settings.hierarchy.serializedLayers = Utility.ReturnBBuffer(buffer);
				Preferences.InvalidateSettings();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_WidthPerComponentDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.hierarchy.widthPerComponent = EditorGUILayout.FloatField(LC.G("NGHierarchyEnhancer_WidthPerComponent"), Preferences.Settings.hierarchy.widthPerComponent);
			if (EditorGUI.EndChangeCheck() == true)
				Preferences.InvalidateSettings();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGHierarchyEnhancer_ComponentColorsDescription"), GeneralStyles.WrapLabel);

			if (reorder == null)
			{
				NGHierarchyEnhancer.colors = new List<NGSettings.HierarchyEnhancer.ComponentColor>(Preferences.Settings.hierarchy.componentColors);
				NGHierarchyEnhancer.reorder = new ReorderableList(NGHierarchyEnhancer.colors, typeof(NGSettings.HierarchyEnhancer.ComponentColor), true, false, true, true);
				NGHierarchyEnhancer.reorder.headerHeight = 0F;
				NGHierarchyEnhancer.reorder.drawElementCallback += NGHierarchyEnhancer.DrawComponentType;
				NGHierarchyEnhancer.reorder.onReorderCallback += (r) => NGHierarchyEnhancer.SerializeComponentColors();
				NGHierarchyEnhancer.reorder.onRemoveCallback += (r) => NGHierarchyEnhancer.SerializeComponentColors();
				NGHierarchyEnhancer.reorder.onAddCallback += (r) => {
					colors.Add(new NGSettings.HierarchyEnhancer.ComponentColor());
					NGHierarchyEnhancer.SerializeComponentColors();
				};
			}

			reorder.DoLayoutList();
		}

		private static void	DrawComponentType(Rect rect, int index, bool isActive, bool isFocused)
		{
			rect.width -= 120F;

			if (NGHierarchyEnhancer.colors[index].type == null)
				GUI.Label(rect, "None");
			else
				GUI.Label(rect, NGHierarchyEnhancer.colors[index].type.Name);

			rect.x += rect.width;
			rect.width = 60F;
			if (GUI.Button(rect, "Type") == true)
			{
				pickTypeIndex = index;
				GenericTypesSelectorWizard.Start("Pick Type", typeof(Component), OnCreate, true, true);
			}

			rect.x += rect.width;
			EditorGUI.BeginChangeCheck();
			NGHierarchyEnhancer.colors[pickTypeIndex].color = EditorGUI.ColorField(rect, NGHierarchyEnhancer.colors[pickTypeIndex].color);
			if (EditorGUI.EndChangeCheck() == true)
				NGHierarchyEnhancer.SerializeComponentColors();
		}

		private static void	OnCreate(Type type)
		{
			NGHierarchyEnhancer.colors[pickTypeIndex].type = type;
			NGHierarchyEnhancer.SerializeComponentColors();
		}

		private static void	SerializeComponentColors()
		{
			ByteBuffer	buffer = Utility.GetBBuffer();

			Preferences.Settings.hierarchy.componentColors = NGHierarchyEnhancer.colors.ToArray();
			buffer.Append(Preferences.Settings.hierarchy.componentColors.Length);

			for (int i = 0; i < Preferences.Settings.hierarchy.componentColors.Length; i++)
			{
				if (Preferences.Settings.hierarchy.componentColors[i].type != null)
					buffer.AppendUnicodeString(Preferences.Settings.hierarchy.componentColors[i].type.GetShortAssemblyType());
				else
					buffer.AppendUnicodeString(string.Empty);
				buffer.Append(Preferences.Settings.hierarchy.componentColors[i].color.r);
				buffer.Append(Preferences.Settings.hierarchy.componentColors[i].color.g);
				buffer.Append(Preferences.Settings.hierarchy.componentColors[i].color.b);
				buffer.Append(Preferences.Settings.hierarchy.componentColors[i].color.a);
			}

			Preferences.Settings.hierarchy.serializedComponentColors = Utility.ReturnBBuffer(buffer);
			Preferences.InvalidateSettings();
		}
	}
}