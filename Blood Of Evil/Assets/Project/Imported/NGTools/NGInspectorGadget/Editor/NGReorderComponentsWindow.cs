using NGTools;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NGToolsEditor.NGInspectorGadget
{
	public class NGReorderComponentsWindow : EditorWindow
	{
		public const string	Title = "ƝƓ Ʀeorder Ҁomponents";

		private GameObject[]			gameObjects;
		private List<List<Component>>	components;
		private ReorderableList			list;
		private Vector2					scrollPosition;
		private int						toDelete;
		private bool					askToBreakPrefab = false;
		private bool					mustBreakAllPrefabs = false;

		[MenuItem("CONTEXT/Component/Reorder Components", priority = 502)]
		private static void	StartReorderComponents(MenuCommand menuCommand)
		{
			Rect					r = EditorWindow.focusedWindow.position;
			NGReorderComponentsWindow	wizard = EditorWindow.GetWindow<NGReorderComponentsWindow>(true, NGReorderComponentsWindow.Title, true);

			wizard.Init(Selection.gameObjects);

			r.height = wizard.components.Count * wizard.list.elementHeight + 38F;

			wizard.position = r;
		}

		public void	Init(GameObject[] gameObjects)
		{
			this.gameObjects = gameObjects;

			this.components = new List<List<Component>>(2);
			this.toDelete = -1;

			this.UpdateComponents();

			this.list = new ReorderableList(this.components, typeof(Component), true, true, false, false);
#if UNITY_5
			this.list.showDefaultBackground = false;
#endif
			this.list.drawElementCallback = this.DrawElement;
			this.list.drawHeaderCallback = this.DrawHeader;
			this.list.onReorderCallback = (ReorderableList list) => { this.UpdateGameObject(); };

			this.wantsMouseMove = true;
		}

		protected virtual void	OnEnable()
		{
			Undo.undoRedoPerformed += this.UpdateComponents;
		}

		protected virtual void	OnDisable()
		{
			Undo.undoRedoPerformed -= this.UpdateComponents;
		}

		protected virtual void	OnGUI()
		{
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				try
				{
					this.list.DoLayoutList();

					// Always keep the Transform at the top.
					if ((this.components[0][0] is Transform) == false)
					{
						List<Component>	tmp = this.components[0];
						this.components[0] = this.components[1];
						this.components[1] = tmp;
					}

					if (this.toDelete != -1)
					{
						for (int i = 0; i < this.components[this.toDelete].Count; i++)
							Undo.DestroyObjectImmediate(this.components[this.toDelete][i]);

						this.components.RemoveAt(this.toDelete);

						this.toDelete = -1;
					}
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException(ex);
				}
			}
			EditorGUILayout.EndScrollView();
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
			{
				this.Close();
				return;
			}
		}

		protected virtual void	OnLostFocus()
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			// HACK: Workaround to avoid crash in Unity 4.
			EditorApplication.delayCall += this.Close;
#else
			this.Close();
#endif
		}

		private void	DrawHeader(Rect rect)
		{
			if (this.gameObjects.Length == 1)
			{
				StringBuilder	buffer = Utility.GetBuffer();
				Transform		current = this.gameObjects[0].transform;

				while (current != null)
				{
					buffer.Insert(0, current.name);
					buffer.Insert(0, "/");

					current = current.parent;
				}

				buffer.Remove(0, 1);

				EditorGUI.LabelField(rect, Utility.ReturnBuffer(buffer));
			}
			else
				EditorGUI.LabelField(rect, this.gameObjects.Length + " x Game Object");
		}

		private void	DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (index == 0)
			{
				float	x = rect.x;

				rect.xMin = 0F;

				if (Event.current.type == EventType.MouseDown &&
					rect.Contains(Event.current.mousePosition) == true)
				{
					Event.current.Use();
				}

				rect.x = x;
			}

			GUI.enabled = !(this.components[index][0] is Transform);

			if (NGTools.Utility.IsComponentEnableable(this.components[index][0]) == true)
			{
				Rect	r = rect;
				bool	enabled = (this.components[index][0] as Behaviour).enabled;

				r.width = 16F;

				for (int i = 1; i < this.components[index].Count; i++)
				{
					if ((this.components[index][i] as Behaviour).enabled != enabled)
					{
						enabled = true;
						EditorGUI.showMixedValue = true;
						break;
					}
				}

				EditorGUI.BeginChangeCheck();
				enabled = EditorGUI.Toggle(r, enabled);
				if (EditorGUI.EndChangeCheck() == true)
				{
					for (int i = 0; i < this.components[index].Count; i++)
						(this.components[index][i] as Behaviour).enabled = enabled;
				}

				EditorGUI.showMixedValue = false;
			}

			GUI.Label(rect, "    #" + index + " " + this.components[index][0].GetType().Name);

			if (Event.current.type == EventType.MouseDown &&
				Event.current.button == 1 &&
				rect.Contains(Event.current.mousePosition) == true)
			{
				this.toDelete = index;

				Event.current.Use();
				return;
			}

			if (index > 0)
			{
				if (rect.Contains(Event.current.mousePosition) == true)
				{
					if (Event.current.type == EventType.MouseMove)
						this.Repaint();

					rect.xMin = rect.xMax - 20F;

					if (GUI.Button(rect, "X") == true)
					{
						this.toDelete = index;
						return;
					}
				}
			}

			GUI.enabled = true;
		}

		private void	UpdateGameObject()
		{
			int	i = this.list.index;

			for (int j = 0; j < this.components[i].Count; j++)
			{
				PrefabType	type = PrefabUtility.GetPrefabType(this.components[i][j]);

				if (type == PrefabType.PrefabInstance)
				{
					if (this.askToBreakPrefab == false)
					{
						this.askToBreakPrefab = true;
						this.mustBreakAllPrefabs = (Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(NGReorderComponentsWindow.Title, "Reordering requires to break prefabs before proceeding, do you want to break prefabs?", "Break all", "Let me choose");
					}

					if (this.mustBreakAllPrefabs == true)
					{
						PrefabUtility.DisconnectPrefabInstance(this.components[i][j]);
						type = PrefabType.DisconnectedPrefabInstance;
					}
				}

				// Move to top.
				while (ComponentUtility.MoveComponentUp(this.components[i][j]) == true)
				{
				}

				if (type == PrefabType.PrefabInstance)
				{
					type = PrefabUtility.GetPrefabType(this.components[i][j]);

					// If the user has not break the prefab, therefore following MoveComponentDown is going to ask to break again and again.
					if (type == PrefabType.PrefabInstance)
						continue;
				}

				// Move to right position (Handling Transform).
				for (int k = 1; k < i; k++)
				{
					if (ComponentUtility.MoveComponentDown(this.components[i][j]) == false)
						break;
				}
			}

			this.UpdateComponents();

			// Due to call to DisplayDialog, the window loses its focus.
			this.Focus();
		}

		private void	UpdateComponents()
		{
			this.components.Clear();

			List<Component>	currentComponents = new List<Component>();

			for (int i = 0; i < gameObjects.Length; i++)
			{
				this.gameObjects[i].GetComponents<Component>(currentComponents);

				for (int j = 0; j < currentComponents.Count; j++)
				{
					if (currentComponents[j] == null)
						continue;

					Type	t = currentComponents[j].GetType();
					int		k = 0;

					// Find in the existing components.
					for (; k < this.components.Count; k++)
					{
						if (this.components[k][0].GetType() == t)
						{
							// Check if a previous Component of the same type is already added.
							int	l = 0;

							for (; l < this.components[k].Count; l++)
							{
								if (this.components[k][l].gameObject == this.gameObjects[i])
									break;
							}

							if (l == this.components[k].Count)
							{
								this.components[k].Add(currentComponents[j]);
								break;
							}
						}
					}

					// Or add a new array of this component's type.
					if (k == this.components.Count)
						this.components.Add(new List<Component>() { currentComponents[j] });
				}
			}

			// Repaint on undo/redo event.
			this.Repaint();
		}
	}
}