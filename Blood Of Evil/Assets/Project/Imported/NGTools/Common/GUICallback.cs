using System;

namespace NGTools
{
	using UnityEngine;

	[ExecuteInEditMode]
	public class GUICallback : MonoBehaviour
	{
		public event Action	callback;

		public static void	Open(Action callback)
		{
			new GameObject().AddComponent<GUICallback>().callback = callback;
#if UNITY_EDITOR
			UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.GameView")).Focus();
#endif
		}

		protected virtual void	OnGUI()
		{
			if (this.callback == null)
				return;

			this.callback();
			this.callback = null;
#if UNITY_EDITOR
			UnityEditor.EditorApplication.delayCall += () => Object.DestroyImmediate(this.gameObject);
#endif
		}
	}
}