using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public class PerWindowVars
	{
		/// <summary>
		/// Initializes an EditorWindow by assigning a unique name.
		/// </summary>
		/// <param name="window"></param>
		public static void	InitWindow(EditorWindow window, string prefix)
		{
			if (string.IsNullOrEmpty(window.name) == true)
				window.name = prefix + Guid.NewGuid();
		}
	}

	[Serializable]
	public class PerWindowVars<T> : PerWindowVars where T : class, new()
	{
		[Serializable]
		public sealed class Vars
		{
			public string		name;
			[NonSerialized]
			public EditorWindow	window;
			public T			vars;
		}

		[SerializeField]
		private List<Vars>		vars = new List<Vars>();
		[NonSerialized]
		private EditorWindow	lastWindow;
		[NonSerialized]
		private T				lastVars;

		public T	Get(EditorWindow window)
		{
			if (this.lastWindow == window)
				return this.lastVars;

			for (int i = 0; i < this.vars.Count; i++)
			{
				if (this.vars[i].window == window ||
					this.vars[i].name == window.name)
				{
					this.lastWindow = window;
					this.lastVars = this.vars[i].vars;

					this.vars[i].window = window;
					return this.vars[i].vars;
				}
			}

			Vars	vars = new Vars();

			vars.name = window.name;
			vars.window = window;
			vars.vars = new T();
			this.vars.Add(vars);

			return vars.vars;
		}

		public IEnumerable<T>	Each()
		{
			for (int i = 0; i < this.vars.Count; i++)
				yield return this.vars[i].vars;
		}
	}
}