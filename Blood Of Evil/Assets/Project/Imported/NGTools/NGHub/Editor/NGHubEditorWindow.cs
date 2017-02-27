using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	public class NGHubEditorWindow : EditorWindow
	{
		public const string	Title = "NG Hub Editor";

		public NGHubWindow	hub { get; private set; }

		private ReorderableList		list;
		private HubComponentWindow	componentWindow;
		private int					closeComponentWindowFrameIndex;
		private Vector2				scrollPosition;

		protected virtual void	OnEnable()
		{
			Undo.undoRedoPerformed += this.Repaint;
		}

		protected virtual void	OnDestroy()
		{
			if (this.componentWindow != null)
				this.componentWindow.Close();
			Undo.undoRedoPerformed -= this.Repaint;
		}

		public void	Init(NGHubWindow hub)
		{
			this.hub = hub;
			this.list = new ReorderableList(this.hub.components, typeof(HubComponent), true, false, true, true);
			this.list.headerHeight = 24F;
			this.list.drawHeaderCallback = (r) => GUI.Label(r, "Components", GeneralStyles.Title1);
#if UNITY_5
			this.list.showDefaultBackground = false;
#endif
			this.list.drawElementCallback = this.DrawComponent;
			this.list.onAddCallback = this.OpenAddComponentWizard;
			this.list.onRemoveCallback = (l) => { l.list.RemoveAt(l.index); this.hub.SaveComponents(); };
			this.list.onReorderCallback = (l) => this.hub.SaveComponents();
			this.list.onChangedCallback = (l) => this.hub.Repaint();
		}

		protected virtual void	OnGUI()
		{
			if (this.hub.droppableComponents.Count > 0 &&
				DragAndDrop.objectReferences.Length > 0)
			{
				for (int i = 0; i < this.hub.droppableComponents.Count; i++)
				{
					if ((bool)this.hub.droppableComponents[i].Invoke(null, null) == true)
					{
						Rect	r = GUILayoutUtility.GetRect(GUI.skin.label.CalcSize(new GUIContent(this.hub.droppableComponents[i].DeclaringType.Name)).x, this.hub.height, GUI.skin.label);

						if (Event.current.type == EventType.Repaint)
						{
							Utility.DropZone(r, Utility.NicifyVariableName(this.hub.droppableComponents[i].DeclaringType.Name));
							this.Repaint();
						}
						else if (Event.current.type == EventType.DragUpdated &&
								 r.Contains(Event.current.mousePosition) == true)
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Move;
						}
						else if (Event.current.type == EventType.DragPerform &&
								 r.Contains(Event.current.mousePosition) == true)
						{
							DragAndDrop.AcceptDrag();

							HubComponent	component = Activator.CreateInstance(this.hub.droppableComponents[i].DeclaringType) as HubComponent;

							if (component != null)
							{
								component.InitDrop(this.hub);
								this.hub.components.Add(component);
								this.hub.SaveComponents();
							}

							DragAndDrop.PrepareStartDrag();
							Event.current.Use();
						}
					}
				}
			}

			Rect	r2 = this.position;
			r2.x = 0F;

			if (this.hub.dockedAsMenu == false)
			{
				r2.y = 24F;

				using (LabelWidthRestorer.Get(50F))
				{
					EditorGUI.BeginChangeCheck();
					this.hub.height = EditorGUILayout.FloatField("Height", this.hub.height);
					if (EditorGUI.EndChangeCheck() == true)
						this.hub.Repaint();
				}
			}
			else
				r2.y = 0F;

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				this.list.DoLayoutList();
			}
			EditorGUILayout.EndScrollView();
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true || this.hub == null)
				this.Close();
		}

		private HubComponent	lastEditingComponent;

		protected virtual void	OnFocus()
		{
			if (this.componentWindow != null)
			{
				this.lastEditingComponent = this.componentWindow.component;
				this.componentWindow.Close();
				this.componentWindow = null;
				this.closeComponentWindowFrameIndex = Time.renderedFrameCount;
			}
		}

		private void	DrawComponent(Rect rect, int i, bool isActive, bool isFocused)
		{
			if (this.hub.components[i].hasEditorGUI == true)
			{
				Rect	rect2 = rect;

				rect2.width = 40F;
				rect.xMin += rect2.width;

				if (Event.current.type == EventType.MouseDown &&
					rect2.Contains(Event.current.mousePosition) == true)
				{
					if (object.Equals(this.componentWindow, null) == false)
					{
						if (this.componentWindow != null && this.componentWindow.component == this.hub.components[i])
						{
							this.componentWindow.Close();
							return;
						}
						else
						{
							this.componentWindow = null;
							return;
						}
					}

					// This conditions are required to prevent closing and opening in the same frame when toggling the same component.
					if (this.closeComponentWindowFrameIndex == Time.renderedFrameCount && this.lastEditingComponent == this.hub.components[i])
						return;

					this.componentWindow = EditorWindow.CreateInstance<HubComponentWindow>();
					this.componentWindow.SetTitle(this.hub.components[i].name);
					this.componentWindow.position = new Rect(this.position.x + rect.x, this.position.y + rect.y + rect.height - this.scrollPosition.y, this.componentWindow.position.width, this.componentWindow.position.height);
					this.componentWindow.Init(this, this.hub.components[i]);
					this.componentWindow.ShowPopup();
					this.componentWindow.Focus();
				}

				GUI.Button(rect2, "Edit");
			}

			this.hub.components[i].OnPreviewGUI(rect);
		}

		private void	OpenAddComponentWizard(ReorderableList list)
		{
			if (FreeConstants.CheckMaxHubComponents(this.hub.components.Count) == true)
				this.hub.OpenAddComponentWizard();
		}

		private void	DeleteComponent(object i)
		{
			this.hub.components.RemoveAt((int)i);
			this.hub.SaveComponents();
			this.hub.Repaint();
		}
	}
}