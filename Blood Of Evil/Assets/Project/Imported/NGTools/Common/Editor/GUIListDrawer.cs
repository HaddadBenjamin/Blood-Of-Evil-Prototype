using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	public sealed class GUIListDrawer<T>
	{
		public const string	GenericDataKey = "key";

		public float		elementHeight = EditorGUIUtility.singleLineHeight;
		public T[]			array;
		public List<T>		list;
		public Vector2		scrollPosition = new Vector2();
		public List<int>	selection = new List<int>();

		public bool		reverseList = false;
		public bool		handleDrag = false;
		public bool		handleSelection = false;
		public bool		drawBackgroundColor = false;
		public Color	highlightBackgroundColor;
		public Color	backgroundColor;

		public Action<Rect, T, int>		ElementGUI;
		public Action<GUIListDrawer<T>>	ExecuteSelection;
		public Action<GUIListDrawer<T>>	DeleteSelection;
		public Action<GUIListDrawer<T>>	CopySelection;
		public Action<GUIListDrawer<T>>	CutSelection;
		public Action<GUIListDrawer<T>>	PostGUI;
		public Action<GUIListDrawer<T>>	ArrayReordered;

		public int	Count { get { return this.array != null ? this.array.Length : this.list.Count; } }
		public T	this[int i] { get { return this.array != null ? this.array[i] : this.list[i]; } }

		private Rect	viewRect = new Rect();
		private float	scrollHeight;

		public	GUIListDrawer()
		{
			this.highlightBackgroundColor = EditorGUIUtility.isProSkin == true ? new Color(.2421875F, .37109375F, .5859375F) : new Color(.3421875F, .47109375F, .8859375F);
			this.backgroundColor = EditorGUIUtility.isProSkin == true ? new Color(61F / 255F, 61F / 255F, 61F / 255F) : new Color(220F / 255F, 220F / 255F, 220F / 255F);
		}

		public void	OnGUI(Rect r)
		{
			int	max = this.Count;

			this.scrollHeight = r.height;
			this.viewRect.height = max * this.elementHeight;

			if (this.drawBackgroundColor == true && Event.current.type == EventType.Repaint)
			{
				EditorGUI.DrawRect(r, this.backgroundColor);
				Utility.DrawUnfillRect(r, Color.black);
			}
			
			this.scrollPosition = GUI.BeginScrollView(r, this.scrollPosition, this.viewRect);
			{
				int	i = Mathf.Clamp(Mathf.FloorToInt(this.scrollPosition.y / this.elementHeight), 0, this.Count);

				if (this.viewRect.height > this.scrollHeight)
					r.width -= 15F;

				r.x = 0F;
				r.height = this.elementHeight;
				r.y = i * r.height;

				for (; i < max; i++)
				{
					int	j = i;
					if (this.reverseList == true)
						j = max - i - 1;

					if (this.handleSelection == true && Event.current.type == EventType.Repaint &&
						this.selection.Contains(j) == true)
					{
						EditorGUI.DrawRect(r, this.highlightBackgroundColor);
					}

					this.ElementGUI(r, this[j], j);

					if (this.handleDrag == true)
						this.HandleDrag(r, j);
					if (this.handleSelection == true)
						this.HandleSelection(r, j);

					r.y += r.height;

					// Check if out of view rect.
					if (r.y - this.scrollPosition.y > this.scrollHeight)
						break;
				}

				if (this.handleDrag == true)
					this.PostHandleDrag(r);

				if (this.PostGUI != null)
					this.PostGUI(this);
			}
			GUI.EndScrollView();
		}

		private void	PostHandleDrag(Rect r)
		{
			if (DragAndDrop.GetGenericData(GUIListDrawer<T>.GenericDataKey) == null || r.y >= Event.current.mousePosition.y)
				return;

			if (Event.current.type == EventType.Repaint)
			{
				if (DragAndDrop.visualMode == DragAndDropVisualMode.Move)
				{
					float	h = r.height;
					r.height = 1F;
					EditorGUI.DrawRect(r, Color.blue);
					r.height = h;
				}
			}
			else if (Event.current.type == EventType.DragUpdated)
			{
				int	max;

				if (this.array != null)
					max = this.array.Length - 1;
				else
					max = this.list.Count - 1;

				if (max.Equals(DragAndDrop.GetGenericData(GUIListDrawer<T>.GenericDataKey)) == false)
					DragAndDrop.visualMode = DragAndDropVisualMode.Move;
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

				Event.current.Use();
			}
			else if (Event.current.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();

				int	origin = (int)DragAndDrop.GetGenericData(GUIListDrawer<T>.GenericDataKey);

				if (this.array != null)
				{
					T	t = this.array[origin];

					for (int j = origin; j < this.array.Length - 1; j += 1)
						this.array[j] = this.array[j + 1];

					this.array[this.array.Length - 1] = t;
				}
				else
				{
					T	e = this.list[origin];

					this.list.RemoveAt(origin);
					this.list.Add(e);
				}

				if (this.ArrayReordered != null)
					this.ArrayReordered(this);

				Event.current.Use();
			}
		}

		private void	HandleDrag(Rect r, int i)
		{
			if (Event.current.type == EventType.Used)
				return;

			if (Event.current.type == EventType.MouseDrag)
			{
				if ((Utility.position2D - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance &&
					DragAndDrop.GetGenericData(GUIListDrawer<T>.GenericDataKey) != null)
				{
					DragAndDrop.StartDrag("Drag Element");
					Event.current.Use();
				}
			}
			else if (Event.current.type == EventType.MouseDown)
			{
				if (r.Contains(Event.current.mousePosition) == true)
				{
					Utility.position2D = Event.current.mousePosition;
					DragAndDrop.PrepareStartDrag();
					DragAndDrop.paths = new string[0];
					DragAndDrop.objectReferences = new Object[0];
					DragAndDrop.SetGenericData(GUIListDrawer<T>.GenericDataKey, i);
				}
				else
					DragAndDrop.SetGenericData(GUIListDrawer<T>.GenericDataKey, null);
			}

			if (DragAndDrop.GetGenericData(GUIListDrawer<T>.GenericDataKey) == null)
				return;

			int	origin = (int)DragAndDrop.GetGenericData(GUIListDrawer<T>.GenericDataKey);

			if (Event.current.type == EventType.Repaint && r.Contains(Event.current.mousePosition) == true)
			{
				if (DragAndDrop.visualMode == DragAndDropVisualMode.Move)
				{
					float	h = r.height;
					r.height = 1F;
					EditorGUI.DrawRect(r, Color.blue);
					r.height = h;
				}
			}
			else if (Event.current.type == EventType.DragUpdated && r.Contains(Event.current.mousePosition) == true)
			{
				if (i != origin &&
					i - 1 != origin)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Move;
				}
				else
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

				Event.current.Use();
			}
			else if (Event.current.type == EventType.DragPerform && r.Contains(Event.current.mousePosition) == true)
			{
				DragAndDrop.SetGenericData(GUIListDrawer<T>.GenericDataKey, null);
				DragAndDrop.AcceptDrag();

				if (this.array != null)
				{
					T	t = this.array[origin];

					if (i > origin)
					{
						for (int j = origin; j < i; ++j)
							this.array[j] = this.array[j + 1];

						this.array[i - 1] = t;
					}
					else
					{
						for (int j = origin; j > i; --j)
							this.array[j] = this.array[j - 1];

						this.array[i] = t;
					}
				}
				else
				{
					T	e = this.list[origin];

					this.list.RemoveAt(origin);

					if (origin < i)
						this.list.Insert(i, e);
					else
						this.list.Insert(i - 1, e);
				}

				if (this.ArrayReordered != null)
					this.ArrayReordered(this);

				Event.current.Use();
			}
		}

		private void	HandleSelection(Rect r, int i)
		{
			if (Event.current.type == EventType.Used)
				return;

			if (Event.current.type == EventType.ValidateCommand &&
				((this.CopySelection != null && Event.current.commandName == "Copy") ||
				 (this.CutSelection != null && Event.current.commandName == "Cut") ||
				 Event.current.commandName == "SelectAll"))
			{
				Event.current.Use();
			}
			else if (Event.current.type == EventType.ExecuteCommand &&
					 Event.current.commandName == "Copy")
			{
				this.CopySelection(this);
				Event.current.Use();
			}
			else if (Event.current.type == EventType.ExecuteCommand &&
					 Event.current.commandName == "Cut")
			{
				this.CutSelection(this);
				Event.current.Use();
			}
			else if (Event.current.type == EventType.ExecuteCommand &&
					 Event.current.commandName == "SelectAll")
			{
				this.selection.Clear();
				for (int j = 0, max = this.Count; j < max; j++)
					this.selection.Add(j);
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete))
			{
				if (this.DeleteSelection != null)
					this.DeleteSelection(this);
				this.selection.Clear();
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
			{
				if (this.ExecuteSelection != null)
					this.ExecuteSelection(this);
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 Event.current.keyCode == KeyCode.UpArrow)
			{
				if (this.Count > 0)
				{
					int	min = int.MaxValue;

					for (int j = 0; j < this.selection.Count; j++)
					{
						if (this.selection[j] < min)
							min = this.selection[j];
					}

					this.selection.Clear();
					if (--min < 0)
						min = 0;
					this.selection.Add(min);
					this.ScrollToSelection(this.selection[0]);
				}
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 Event.current.keyCode == KeyCode.DownArrow)
			{
				if (this.Count > 0)
				{
					int	max = int.MinValue;

					for (int j = 0; j < this.selection.Count; j++)
					{
						if (this.selection[j] > max)
							max = this.selection[j];
					}

					this.selection.Clear();
					if (++max > this.Count - 1)
						max = this.Count - 1;
					this.selection.Add(max);
					this.ScrollToSelection(this.selection[0]);
				}
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 Event.current.keyCode == KeyCode.PageUp)
			{
				if (this.Count > 0)
				{
					int	min = int.MaxValue;

					for (int j = 0; j < this.selection.Count; j++)
					{
						if (this.selection[j] < min)
							min = this.selection[j];
					}
				
					this.selection.Clear();
					min -= Mathf.FloorToInt(this.scrollHeight / this.elementHeight);
					if (min < 0)
						min = 0;
					this.selection.Add(min);
					this.ScrollToSelection(this.selection[0]);
				}
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 Event.current.keyCode == KeyCode.PageDown)
			{
				if (this.Count > 0)
				{
					int	max = int.MinValue;

					for (int j = 0; j < this.selection.Count; j++)
					{
						if (this.selection[j] > max)
							max = this.selection[j];
					}

					this.selection.Clear();
					max += Mathf.FloorToInt(this.scrollHeight / this.elementHeight);
					if (max > this.Count - 1)
						max = this.Count - 1;
					this.selection.Add(max);
					this.ScrollToSelection(this.selection[0]);
				}
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 Event.current.keyCode == KeyCode.Home)
			{
				if (this.Count > 0)
				{
					this.selection.Clear();
					this.selection.Add(0);
					this.ScrollToSelection(this.selection[0]);
				}
				Event.current.Use();
			}
			else if (Event.current.type == EventType.KeyDown &&
					 Event.current.keyCode == KeyCode.End)
			{
				if (this.Count > 0)
				{
					this.selection.Clear();
					this.selection.Add(this.Count - 1);
					this.ScrollToSelection(this.selection[0]);
				}
				Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseDown &&
					 Event.current.button == 0 &&
					 r.Contains(Event.current.mousePosition) == true)
			{
				// Handle multi-selection.
				if (Event.current.shift == true)
				{
					int	min = int.MaxValue;
					int	max = int.MinValue;

					for (int j = 0; j < this.selection.Count; j++)
					{
						if (this.selection[j] < min)
							min = this.selection[j];
						if (this.selection[j] > max)
							max = this.selection[j];
					}

					if (min > i)
						min = i;
					if (max < i)
						max = i;

					this.selection.Clear();
					for (; min <= max; min++)
						this.selection.Add(min);
					this.ScrollToSelection(i);
				}
				else if (Event.current.control == true &&
					this.selection.Contains(i) == true)
				{
					this.selection.Remove(i);
					this.ScrollToSelection(i);
				}
				else
				{
					if (Event.current.control == false)
						this.selection.Clear();

					this.selection.Add(i);
					this.ScrollToSelection(i);
				}

				Event.current.Use();
			}
		}

		private void	ScrollToSelection(int i)
		{
			float	y = i * this.elementHeight;

			if (y < this.scrollPosition.y)
				this.scrollPosition.y = y;
			else if (y + this.elementHeight > this.scrollPosition.y + this.scrollHeight)
				this.scrollPosition.y = y + this.elementHeight - this.scrollHeight;
		}
	}
}