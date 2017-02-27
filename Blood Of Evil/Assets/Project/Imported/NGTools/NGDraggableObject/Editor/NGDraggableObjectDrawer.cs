using NGTools;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGDraggableObject
{
	using UnityEngine;

	[InitializeOnLoad]
	[CustomPropertyDrawer(typeof(Object), true)]
	internal sealed class NGDraggableObjectDrawer : PropertyDrawer
	{
		private class DataMenu
		{
			public SerializedProperty	property;
			public Object				component;
		}

		private static Object	copiedObject;

		static	NGDraggableObjectDrawer()
		{
			new SectionDrawer("ƝƓ Ðraggable Øbject", typeof(NGSettings.NGDraggableObjectSettings));
		}

		private bool	CanDrop(SerializedProperty property, Object asset)
		{
			PrefabType	type = PrefabUtility.GetPrefabType(property.serializedObject.targetObject);

			if (type == PrefabType.Prefab ||
				type == PrefabType.ModelPrefab ||
				(type == PrefabType.None && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(property.serializedObject.targetObject)) == false))
			{
				PrefabType	dragType = PrefabUtility.GetPrefabType(asset);

				if (dragType != PrefabType.Prefab &&
					dragType != PrefabType.ModelPrefab &&
					(dragType != PrefabType.None || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(asset)) == true))
				{
					return false;
				}
			}

			return this.CanDrop(this.fieldInfo.FieldType, asset);
		}

		private bool	CanDrop(Type type, Object asset)
		{
			if (asset == null)
				return false;

			if (type.IsUnityArray() == true)
				type = Utility.GetArraySubType(type);

			if (type.IsAssignableFrom(asset.GetType()) == true)
				return true;

			Object[]	assets;
			string		assetPath = AssetDatabase.GetAssetPath(asset);

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7
			// Avoid Unity scenes. They throw error "Do not use ReadObjectThreaded on scene objects!".
			if (assetPath.EndsWith(".unity") && asset.GetType() == typeof(DefaultAsset))
				return false;
#endif

			if (string.IsNullOrEmpty(assetPath) == true)
				assets = new Object[] { asset };
			else
			{
				// In the case of a prefab, we need to enforce fetching assets of the focused GameObject.
				if (assetPath.EndsWith(".prefab", StringComparison.InvariantCultureIgnoreCase) == true)
				{
					assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

					List<Object>	relatedAssets = new List<Object>();
					GameObject		targetGameObject = asset as GameObject;

					if (targetGameObject == null)
						targetGameObject = (asset as Component).gameObject;

					for (int i = 0; i < assets.Length; i++)
					{
						Component	component = assets[i] as Component;

						if (component != null)
						{
							if (component.gameObject == targetGameObject)
								relatedAssets.Add(assets[i]);
						}
						else
						{
							GameObject	go = assets[i] as GameObject;

							if (go != null && go == targetGameObject)
								relatedAssets.Add(assets[i]);
						}
					}

					assets = relatedAssets.ToArray();
				}
				else
					assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
			}

			for (int i = 0; i < assets.Length; i++)
			{
				if (type.IsAssignableFrom(assets[i].GetType()) == true)
					return true;

				GameObject	gameObject = assets[i] as GameObject;

				if (gameObject != null &&
					(typeof(Component).IsAssignableFrom(type) == true ||
					 type.IsInterface == true))
				{
					Component	subComponent = gameObject.GetComponent(type);

					if (subComponent != null)
						return true;
				}

				Component	component = assets[i] as Component;

				if (component != null)
				{
					if (type == typeof(GameObject))
						return true;

					gameObject = component.gameObject;

					if (gameObject != null &&
						(typeof(Component).IsAssignableFrom(type) == true ||
						 type.IsInterface == true))
					{
						Component	subComponent = gameObject.GetComponent(type);

						if (subComponent != null)
							return true;
					}
				}
			}

			return false;
		}

		private void	ExtractAsset(Type type, Object asset, SerializedProperty property)
		{
			if (type.IsUnityArray() == true)
				type = Utility.GetArraySubType(type);

			if (type.IsAssignableFrom(asset.GetType()) == true)
			{
				property.objectReferenceValue = asset;
				return;
			}

			Object[]	assets;
			string		assetPath = AssetDatabase.GetAssetPath(asset);
			int			assignables = 0;

			if (string.IsNullOrEmpty(assetPath) == true)
				assets = new Object[] { asset };
			else
			{
				// In the case of a prefab, we need to enforce fetching assets of the focused GameObject.
				if (assetPath.EndsWith(".prefab", StringComparison.InvariantCultureIgnoreCase) == true)
				{
					assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

					List<Object>	relatedAssets = new List<Object>();
					GameObject		targetGameObject = asset as GameObject;

					if (targetGameObject == null)
						targetGameObject = (asset as Component).gameObject;

					for (int i = 0; i < assets.Length; i++)
					{
						Component	component = assets[i] as Component;

						if (component != null)
						{
							if (component.gameObject == targetGameObject)
								relatedAssets.Add(assets[i]);
						}
						else
						{
							GameObject	go = assets[i] as GameObject;

							if (go != null && go == targetGameObject)
								relatedAssets.Add(assets[i]);
						}
					}

					assets = relatedAssets.ToArray();
				}
				else
					assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
			}

			for (int i = 0; i < assets.Length; i++)
			{
				if (type.IsAssignableFrom(assets[i].GetType()) == true)
					++assignables;
				else
				{
					GameObject	gameObject = assets[i] as GameObject;

					if (gameObject != null &&
						(typeof(Component).IsAssignableFrom(type) == true ||
						 type.IsInterface == true))
					{
						Component	subComponent = gameObject.GetComponent(type);

						if (subComponent != null)
						{
							this.TryDropDownComponents(type, property, assets[i] as GameObject);
							return;
						}
					}

					Component	component = assets[i] as Component;
					// When dropping component, always drop its GameObject, it gathers all cases (Both on GameObject and Component).
					if (component != null)
					{
						if (type == typeof(GameObject))
						{
							property.objectReferenceValue = component.gameObject;
							return;
						}
						else if (typeof(Component).IsAssignableFrom(type) == true ||
								 type.IsInterface == true)
						{
							this.TryDropDownComponents(type, property, component.gameObject);
							return;
						}
					}
				}
			}

			if (assignables == 1)
			{
				for (int i = 0; i < assets.Length; i++)
				{
					if (type.IsAssignableFrom(assets[i].GetType()) == true)
					{
						property.objectReferenceValue = assets[i];
						break;
					}
				}
			}
			else if (assignables >= 2)
				this.DropDownMultiComponents(assets, type, property);
		}

		private void	TryDropDownComponents(Type type, SerializedProperty property, GameObject gameObject)
		{
			if (type == typeof(Object))
				type = typeof(Component);

			Component[]	components = gameObject.GetComponents(type);

			InternalNGDebug.AssertFile(components.Length > 0, "There is no component of type \"" + type.Name + "\" from GameObject \"" + gameObject.name + "\".");

			if (components.Length >= 2)
				this.DropDownMultiComponents(gameObject, type, property);
			else
				property.objectReferenceValue = components[0];
		}

		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (Preferences.Settings != null && Preferences.Settings.drag.enable == false)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			Type	realType = this.fieldInfo.FieldType;
			Color	restore = GUI.backgroundColor;

			if (this.fieldInfo.FieldType.IsUnityArray() == true)
				realType = Utility.GetArraySubType(this.fieldInfo.FieldType);

			if (Event.current.type == EventType.Repaint &&
				DragAndDrop.visualMode == DragAndDropVisualMode.Copy &&
				position.Contains(Event.current.mousePosition) == true)
			{
				GUI.backgroundColor = Color.yellow;
			}
			else if ((Event.current.type == EventType.DragUpdated ||
					  Event.current.type == EventType.DragPerform) &&
					 position.Contains(Event.current.mousePosition) == true)
			{
				if (DragAndDrop.objectReferences.Length > 0 && this.CanDrop(property, DragAndDrop.objectReferences[0]) == true)
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

				if (Event.current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();

					this.ExtractAsset(this.fieldInfo.FieldType, DragAndDrop.objectReferences[0], property);
				}

				DragAndDrop.PrepareStartDrag();
				Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseDrag &&
					 (Utility.position2D - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance &&
					 property.objectReferenceInstanceIDValue.Equals(DragAndDrop.GetGenericData(Utility.DragObjectDataName)) == true)
			{
				DragAndDrop.StartDrag("Drag Object");
				Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseDown)
			{
				Utility.position2D = Event.current.mousePosition;

				if (Event.current.button == 0)
				{
					DragAndDrop.PrepareStartDrag();

					if (position.Contains(Event.current.mousePosition) == true)
					{
						if (property.objectReferenceInstanceIDValue != 0)
						{
							DragAndDrop.objectReferences = new Object[] { property.objectReferenceValue };
							DragAndDrop.SetGenericData(Utility.DragObjectDataName, property.objectReferenceInstanceIDValue);
						}
					}
				}
			}
			else if (Event.current.type == EventType.MouseUp)
			{
				if (Event.current.button == 1 &&
					position.Contains(Event.current.mousePosition) == true)
				{
					Component	cc = property.objectReferenceValue as Component;

					if (cc != null && cc.gameObject != null)
						this.TryDropDownComponents(realType, property, cc.gameObject);
					else
						this.DefaultMenu(property);

					Event.current.Use();
				}

				DragAndDrop.PrepareStartDrag();
			}

			GUI.SetNextControlName(property.serializedObject.GetHashCode() + property.propertyPath);
#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7
			EditorGUI.ObjectField(position, property, this.fieldInfo.FieldType, label);
#else
			EditorGUI.PropertyField(position, property, label);
#endif

			if (Event.current.type == EventType.ValidateCommand &&
				(Event.current.commandName == "Copy" ||
				Event.current.commandName == "Cut" ||
				Event.current.commandName == "Paste") &&
				GUI.GetNameOfFocusedControl() == property.serializedObject.GetHashCode() + property.propertyPath)
			{
				Event.current.Use();
			}
			else if (Event.current.type == EventType.ExecuteCommand &&
					 Event.current.commandName == "Copy" &&
					 GUI.GetNameOfFocusedControl() == property.serializedObject.GetHashCode() + property.propertyPath)
			{
				NGDraggableObjectDrawer.copiedObject = property.objectReferenceValue;
				Event.current.Use();
			}
			else if (Event.current.type == EventType.ExecuteCommand &&
					 Event.current.commandName == "Cut" &&
					 GUI.GetNameOfFocusedControl() == property.serializedObject.GetHashCode() + property.propertyPath)
			{
				NGDraggableObjectDrawer.copiedObject = property.objectReferenceValue;
				property.objectReferenceValue = null;
				Event.current.Use();
			}
			else if (Event.current.type == EventType.ExecuteCommand &&
					 Event.current.commandName == "Paste" &&
					 GUI.GetNameOfFocusedControl() == property.serializedObject.GetHashCode() + property.propertyPath)
			{
				if (NGDraggableObjectDrawer.copiedObject != null)
				{
					property.objectReferenceValue = NGDraggableObjectDrawer.copiedObject;
					Event.current.Use();
				}
			}

			// Display the position of the Component in its GameObject if there is many.
			Component	c = property.objectReferenceValue as Component;

			if (c != null && c.gameObject != null)
			{
				Component[]	cs = c.gameObject.GetComponents(typeof(Component));

				for (int i = 0, j = 0; i < cs.Length; i++)
				{
					if (cs[i] == null)
						continue;

					if (realType.IsAssignableFrom(cs[i].GetType()) == true)
						++j;

					if (j >= 2)
					{
						EditorGUI.indentLevel = 0;
						for (int k = 0; k < cs.Length; k++)
						{
							if (cs[k] == null)
								continue;

							if (cs[k].GetInstanceID() == property.objectReferenceInstanceIDValue)
							{
								if (k < 9)
									position.width = 20F;
								else
									position.width = 28F;

								if (string.IsNullOrEmpty(label.text) == false)
									position.x += EditorGUIUtility.labelWidth - position.width;
								else
									position.x -= position.width;

								EditorGUI.LabelField(position, "#" + (k + 1).ToString());
								break;
							}
						}
						break;
					}
				}
			}

			GUI.backgroundColor = restore;
		}

		private void	DefaultMenu(SerializedProperty property)
		{
			if (PrefabUtility.GetPrefabType(property.serializedObject.targetObject) == PrefabType.None)
				return;

			GenericMenu	menu = new GenericMenu();

			menu.AddItem(new GUIContent("Revert Value to Prefab"), false, this.RevertValueToPrefab, property);

			menu.ShowAsContext();
		}

		private void	DropDownMultiComponents(GameObject gameObject, Type targetType, SerializedProperty property)
		{
			this.DropDownMultiComponents(gameObject.GetComponents<Component>(), targetType, property);
		}

		private void	DropDownMultiComponents(Object[] values, Type targetType, SerializedProperty property)
		{
			GenericMenu	menu = new GenericMenu();

			if (PrefabUtility.GetPrefabType(property.serializedObject.targetObject) != PrefabType.None)
			{
				menu.AddItem(new GUIContent("Revert Value to Prefab"), false, this.RevertValueToPrefab, property);
				menu.AddSeparator("");
			}

			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == null)
					continue;

				Type	type = values[i].GetType();

				if (targetType.IsAssignableFrom(type) == true)
					menu.AddItem(new GUIContent("#" + (i + 1).ToString() + " " + type.Name), property.objectReferenceValue == values[i], this.Set, new DataMenu() { property = property, component = values[i] });
				else
					menu.AddDisabledItem(new GUIContent("#" + (i + 1).ToString() + " " + type.Name));
			}

			menu.ShowAsContext();
		}

		private void	RevertValueToPrefab(object o)
		{
			SerializedProperty	p = o as SerializedProperty;

			p.prefabOverride = false;
			p.serializedObject.ApplyModifiedProperties();
		}

		private void	Set(object _data)
		{
			DataMenu	data = _data as DataMenu;

#if UNITY_5_6 || UNITY_5_6_OR_NEWER
			data.property.serializedObject.UpdateIfRequiredOrScript();
#else
			data.property.serializedObject.UpdateIfDirtyOrScript();
#endif
			data.property.objectReferenceValue = data.component;
			data.property.serializedObject.ApplyModifiedProperties();
		}
	}
}