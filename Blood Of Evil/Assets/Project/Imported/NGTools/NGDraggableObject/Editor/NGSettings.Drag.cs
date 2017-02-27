using System;

namespace NGToolsEditor
{
	using UnityEngine;

	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class NGDraggableObjectSettings
		{
			[LocaleHeader("NGDraggableObject_EnableDescription")]
			public bool	enable = true;
		}
		public NGDraggableObjectSettings	drag = new NGDraggableObjectSettings();
	}
}