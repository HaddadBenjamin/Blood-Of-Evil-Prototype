using System;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class Nav
		{
			public bool	enable = true;
			public int	maxHistoric = 100;
			public int	maxDisplayHierarchy = 0;
		}
		public Nav	nav = new Nav();
	}
}