using System;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	public class MemoryData : DataConsole
	{
		public override void	ShortGUI()
		{
			this.label.text = (GC.GetTotalMemory(false) / 1024 / 1024).ToString() + " MB";
			this.DrawSimpleShortGUI();
		}

		public override void	FullGUI()
		{
			this.label.text = "Memory: " + GC.GetTotalMemory(false).ToString() + " B (" + (GC.GetTotalMemory(false) / 1024 / 1024).ToString() + " MB)";
			GUILayout.TextArea(this.label.text, this.fullStyle);
		}

		public override string	Copy()
		{
			return this.label.text;
		}
	}
}