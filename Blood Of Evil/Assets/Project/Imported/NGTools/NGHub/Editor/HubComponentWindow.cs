using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	public class HubComponentWindow : EditorWindow
	{
		public HubComponent	component { get; private set; }

		private EditorWindow	editor;

		public void	Init(EditorWindow editor, HubComponent component)
		{
			this.editor = editor;
			this.component = component;
			this.SetTitle(component.name);
		}

		protected virtual void	OnEnable()
		{
			this.minSize = Vector2.zero;
		}

		protected virtual void	Update()
		{
			if (this.component == null || EditorApplication.isCompiling == true)
			{
				this.Close();
				return;
			}
		}

		protected virtual void	OnGUI()
		{
			Rect	r = this.position;
			r.x = 0F;
			r.y = 0F;
			r.width -= 1F;
			r.height -= 1F;
			GUILayout.Space(5F);
			Utility.DrawUnfillRect(r, Color.grey);

			EditorGUI.BeginChangeCheck();
			this.component.OnEditionGUI();
			if (EditorGUI.EndChangeCheck() == true)
			{
				NGHubWindow	hub = this.editor as NGHubWindow;

				if (hub != null)
				{
					hub.SaveComponents();
					hub.Repaint();
				}
				else
				{
					NGHubEditorWindow	editor = this.editor as NGHubEditorWindow;
					if (editor != null)
					{
						editor.hub.SaveComponents();
						editor.hub.Repaint();
					}
					else
						this.editor.Repaint();
				}
			}

			if (GUILayout.Button("Close") == true)
				this.Close();

			if (Event.current.type == EventType.Repaint)
			{
				r = GUILayoutUtility.GetLastRect();
				r.x = this.position.x;
				r.width = this.position.width;
				r.yMax = this.position.y + r.yMax + 5F;
				r.yMin = this.position.y;
				this.position = r;
			}
		}
	}
}