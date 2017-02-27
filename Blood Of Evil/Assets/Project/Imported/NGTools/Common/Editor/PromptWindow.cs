using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class PromptWindow : EditorWindow
	{
		private bool					first;
		private string					placeholder;
		private Action<object, string>	callback;
		private object					data;

		public static void	Start(string placeholder, Action<object, string> callback, object data)
		{
			if (placeholder == null)
				placeholder = string.Empty;

			Rect	r = new Rect(Screen.width - 100F, Screen.height * .5F, 200F, 50F);

			r.x = EditorWindow.focusedWindow.position.x;
			r.y = EditorWindow.focusedWindow.position.y;

			if (Event.current != null)
			{
				r.x += Event.current.mousePosition.x;
				r.y += Event.current.mousePosition.y;

				if (r.x + r.width > Screen.width)
					r.x = Screen.width - r.width;
				if (r.y + r.height > Screen.height)
					r.y = Screen.height - r.height;
			}

			PromptWindow	p = EditorWindow.GetWindowWithRect<PromptWindow>(r, true, "Prompt");

			p.position = r;
			p.first = true;
			p.placeholder = placeholder;
			p.callback = callback;
			p.data = data;

			p.Show();
		}

		protected virtual void	OnGUI()
		{
			if (this.placeholder == null)
			{
				this.Close();
				return;
			}

			if (this.first == true)
				GUI.SetNextControlName("text");

			this.placeholder = GUILayout.TextField(this.placeholder);
			if (this.first == true)
			{
				this.first = false;
				GUI.FocusControl("text");
			}

			GUILayout.BeginHorizontal();

			if (Event.current.type == EventType.KeyUp)
			{
				if (Event.current.keyCode == KeyCode.Return)
				{
					this.callback(this.data, this.placeholder);
					this.Close();
					Event.current.Use();
				}
				else if (Event.current.keyCode == KeyCode.Escape)
				{
					this.Close();
					Event.current.Use();
				}
			}

			if (GUILayout.Button("Confirm"))
			{
				this.callback(this.data, this.placeholder);
				this.Close();
			}

			if (GUILayout.Button("Cancel"))
				this.Close();

			GUILayout.EndHorizontal();
		}
	}
}