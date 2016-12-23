using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[InitializeOnLoad]
	public class NGRealTimeEditorDebug : EditorWindow
	{
		private class Var
		{
			public string	key;
			public string	value;
			public float	startTime;
			public float	lifetime;
		}

		public const string	Title = "NG Real Time Editor Debug";
		public const int	ForceRepaintRefreshTick = 10;

		private static NGRealTimeEditorDebug	instance;

#if NGT_DEBUG
		static	NGRealTimeEditorDebug()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGRealTimeEditorDebug.Title);
		}

		[MenuItem(Constants.MenuItemPath + NGRealTimeEditorDebug.Title, priority = Constants.MenuItemPriority + 1011)]
		private static void	Open()
		{
			EditorWindow.GetWindow<NGRealTimeEditorDebug>(NGRealTimeEditorDebug.Title);
		}
#endif

		[Conditional(Constants.DebugSymbol)]
		public static void	Set(string key, object value, float lifetime)
		{
			if (NGRealTimeEditorDebug.instance == null)
				return;

			int	n = NGRealTimeEditorDebug.instance.vars.FindIndex((v) => v.key == key);

			if (n != -1)
			{
				NGRealTimeEditorDebug.instance.vars[n].value = value != null ? value.ToString() : "NULL";
				NGRealTimeEditorDebug.instance.vars[n].startTime = Time.realtimeSinceStartup;
				NGRealTimeEditorDebug.instance.vars[n].lifetime = lifetime;
			}
			else
			{
				NGRealTimeEditorDebug.instance.vars.Add(new Var() { key = key, value = value != null ? value.ToString() : "NULL", startTime = Time.realtimeSinceStartup, lifetime = lifetime });
			}
		}

		private List<Var>	vars = new List<Var>();
		private Vector2		scrollPosition;

		protected virtual void	OnEnable()
		{
			NGRealTimeEditorDebug.instance = this;
			Utility.RegisterIntervalCallback(this.Repaint, NGRealTimeEditorDebug.ForceRepaintRefreshTick);
		}

		protected virtual void	OnDestroy()
		{
			NGRealTimeEditorDebug.instance = null;
			Utility.UnregisterIntervalCallback(this.Repaint);
		}

		protected virtual void	OnGUI()
		{
			GUILayout.Label("T=" + Time.realtimeSinceStartup.ToString());
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				for (int i = 0; i < this.vars.Count; i++)
				{
					EditorGUILayout.LabelField(this.vars[i].key, this.vars[i].value);

					Rect	r = GUILayoutUtility.GetLastRect();
					r.height = 1F;
					r.width = r.width - r.width * (Time.realtimeSinceStartup - this.vars[i].startTime) / this.vars[i].lifetime;
					EditorGUI.DrawRect(r, Color.black);
				}
			}
			EditorGUILayout.EndScrollView();

			for (int i = 0; i < this.vars.Count; i++)
			{
				if (this.vars[i].startTime + this.vars[i].lifetime < Time.realtimeSinceStartup)
				{
					this.vars.RemoveAt(i);
					--i;
				}
			}
		}
	}
}