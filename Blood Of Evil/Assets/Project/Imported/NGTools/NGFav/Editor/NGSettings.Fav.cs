using NGTools;
using NGToolsEditor.NGFav;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class FavSettings
		{
			public enum ChangeSelection
			{
				SimpleClick,
				DoubleClick,
				Modifier,
				ModifierOrDoubleClick,
			}

			[LocaleHeader("NGFav_ChangeSelectionDescription")]
			public ChangeSelection	changeSelection = ChangeSelection.ModifierOrDoubleClick;
			[EnumMask, LocaleHeader("NGFav_SelectModifiersDescription")]
			public EventModifiers	selectModifiers = (EventModifiers)((int)EventModifiers.Shift << 1);
			[EnumMask, LocaleHeader("NGFav_DeleteModifiersDescription")]
			public EventModifiers	deleteModifiers = (EventModifiers)((int)(EventModifiers.Control | EventModifiers.Shift) << 1);

			[HideInInspector]
			public List<Favorites>	favorites = new List<Favorites>();
		}
		public FavSettings	fav = new FavSettings();
	}
}