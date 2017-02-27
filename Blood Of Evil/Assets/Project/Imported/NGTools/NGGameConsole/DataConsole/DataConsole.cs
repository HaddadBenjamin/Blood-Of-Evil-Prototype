using System;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	public abstract class DataConsole : MonoBehaviour
	{
		[Header("Display data sharing the same group into a single block.")]
		public string	group = string.Empty;

		[Header("Label used in short GUI.")]
		public GUIContent	label;

		[NonSerialized]
		public GUIStyle	shortStyle;
		[NonSerialized]
		public GUIStyle	fullStyle;

		private bool	init;

		public void	InitOnGUI()
		{
			if (this.init == true)
				return;

			this.init = true;

			if (this.label == null)
				this.label = new GUIContent();

			this.InitGUI();
		}

		protected virtual void	InitGUI()
		{
		}

		public virtual void	ShortGUI()
		{
		}

		public virtual void	FullGUI()
		{
		}

		public virtual string	Copy()
		{
			return string.Empty;
		}

		protected void	DrawSimpleShortGUI()
		{
			GUILayout.Label(label, this.shortStyle);
		}
	}
}