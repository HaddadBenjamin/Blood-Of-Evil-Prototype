using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRenamer
{
	public abstract class TextFilter
	{
		[NonSerialized]
		public readonly NGRenamerWindow	renamer;
		[NonSerialized]
		public readonly string	name;
		[NonSerialized]
		public readonly int		priority;

		public bool	enable;
		public bool	open;

		protected	TextFilter(NGRenamerWindow renamer, string name, int priority)
		{
			this.renamer = renamer;
			this.name = name;
			this.priority = priority;
		}

		public void	OnHeaderGUI()
		{
			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				this.open = GUILayout.Toggle(this.open, "", "foldout");

				EditorGUI.BeginChangeCheck();
				this.enable = GUILayout.Toggle(this.enable, this.name, GeneralStyles.ToolbarButton, GUILayout.ExpandWidth(false));
				if (EditorGUI.EndChangeCheck() == true)
					this.renamer.Invalidate();
			}
			EditorGUILayout.EndHorizontal();
		}

		public abstract void	OnGUI();

		public virtual void		Highlight(string input, List<int> highlightedPositions)
		{
		}

		public abstract string	Filter(string input);
	}
}