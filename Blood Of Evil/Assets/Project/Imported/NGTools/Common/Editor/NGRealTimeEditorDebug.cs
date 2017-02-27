using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[InitializeOnLoad]
	public class NGRealTimeEditorDebug : EditorWindow
	{
		[Serializable]
		private class Var
		{
			public string	key;
			public string	value;
			public double	startTime;
			public double	lifetime;
		}

		public const string	Title = "NG Real Time Editor Debug";
		public const int	ForceRepaintRefreshTick = 10;

		private static NGRealTimeEditorDebug	instance;

		public static void	Set(string key, object value, double lifetime)
		{
			if (NGRealTimeEditorDebug.instance == null)
				return;

			int	n = NGRealTimeEditorDebug.instance.vars.FindIndex((v) => v.key == key);

			if (n != -1)
			{
				NGRealTimeEditorDebug.instance.vars[n].value = value != null ? value.ToString() : "NULL";
				NGRealTimeEditorDebug.instance.vars[n].startTime = EditorApplication.timeSinceStartup;
				NGRealTimeEditorDebug.instance.vars[n].lifetime = lifetime;
			}
			else
			{
				NGRealTimeEditorDebug.instance.vars.Add(new Var() { key = key, value = value != null ? value.ToString() : "NULL", startTime = EditorApplication.timeSinceStartup, lifetime = lifetime });
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
			GUILayout.Label("Time " + EditorApplication.timeSinceStartup.ToString());
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				for (int i = 0; i < this.vars.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField(this.vars[i].key, this.vars[i].value);

						Rect	r = GUILayoutUtility.GetLastRect();
						r.height = 1F;
						r.width = r.width - r.width * (float)((EditorApplication.timeSinceStartup - this.vars[i].startTime) / this.vars[i].lifetime);
						EditorGUI.DrawRect(r, Color.black);

						if (GUILayout.Button("X", GeneralStyles.ToolbarCloseButton, GUILayout.ExpandWidth(false)) == true)
						{
							this.vars.RemoveAt(i);
							return;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndScrollView();

			for (int i = 0; i < this.vars.Count; i++)
			{
				if (this.vars[i].startTime + this.vars[i].lifetime < EditorApplication.timeSinceStartup)
				{
					this.vars.RemoveAt(i);
					--i;
				}
			}
		}
	}
}