using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	public class NGHubExtensionWindow : EditorWindow
	{
		public int	minI;
		public int	hiddenI;

		private NGHubWindow	source;

		public void	Init(NGHubWindow source)
		{
			this.source = source;
			this.minI = 0;
			this.hiddenI = 0;
		}

		protected virtual void	OnEnable()
		{
			Undo.undoRedoPerformed += this.Repaint;
		}

		protected virtual void	OnDestroy()
		{
			Undo.undoRedoPerformed -= this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			if (this.source == null)
				return;

			if (Event.current.type == EventType.Repaint)
				EditorGUI.DrawRect(new Rect(0F, 0F, this.position.width, this.position.height), NGHubWindow.DockBackgroundColor);

			EditorGUILayout.BeginHorizontal(GUILayout.Height(this.source.height));
			{
				this.source.HandleDrop();

				EventType	catchedType = EventType.Used;

				for (int i = this.minI; i < this.source.components.Count; i++)
				{
					// Catch event from the cropped component.
					if (Event.current.type != EventType.Repaint &&
						Event.current.type != EventType.Layout)
					{
						if (this.hiddenI == i)
						{
							// Simulate context click, because MouseUp is used, therefore ContextClick is not sent.
							if (Event.current.type == EventType.MouseUp &&
								Event.current.button == 1)
							{
								catchedType = EventType.ContextClick;
							}
							else
								catchedType = Event.current.type;
							Event.current.Use();
						}
					}

					EditorGUILayout.BeginHorizontal();
					{
						this.source.components[i].OnGUI();
					}
					EditorGUILayout.EndHorizontal();

					if (Event.current.type == EventType.Repaint)
					{
						Rect	r = GUILayoutUtility.GetLastRect();

						if (r.xMax >= this.position.width)
						{
							// Hide the miserable trick...
							r.xMin -= 2F;
							r.yMin -= 2F;
							r.yMax += 2F;
							r.xMax += 2F;
							EditorGUI.DrawRect(r, NGHubWindow.DockBackgroundColor);
							this.hiddenI = i;
							break;
						}
					}
				}

				if (Event.current.type == EventType.ContextClick ||
					catchedType == EventType.ContextClick)
				{
					this.source.OpenContextMenu();
				}
			}

			GUILayout.FlexibleSpace();

			EditorGUILayout.EndHorizontal();
		}

		protected virtual void	Update()
		{
			if (this.source == null)
			{
				this.Close();
				return;
			}
		}
	}
}