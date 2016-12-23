using System;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class HierarchyEnhancer
		{
			public bool				enable = true;
			public float			margin = 0F;
			public EventModifiers	holdModifiers = EventModifiers.Control;
		}
		public HierarchyEnhancer	hierarchy = new HierarchyEnhancer();
	}
}