using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class GUICallbackWindow : EditorWindow
	{
		public event Action	callback;

		private bool	forceClose;

		public static void	Open(Action callback)
		{
			GUICallbackWindow	w = EditorWindow.GetWindow<GUICallbackWindow>();

			w.maxSize = new Vector2(1F, 1F);
			w.minSize = new Vector2(1F, 1F);
			w.callback += callback;
		}

		protected virtual void	OnGUI()
		{
			// Callback can be null in the rare case the window is still alive.
			if (this.forceClose == false && this.callback != null)
				this.callback();

			this.forceClose = true;
		}

		protected virtual void	Update()
		{
			if (this.forceClose == true)
				this.Close();
		}
	}
}