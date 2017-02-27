using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[InitializeOnLoad, Serializable]
	internal sealed class ColorMarkersModule : Module
	{
		private sealed class ColoredRow
		{
			public Row	row;
			public int	i;
		}

		[NonSerialized]
		private List<ColoredRow>	coloredRows;

		static	ColorMarkersModule()
		{
			new SectionDrawer("Color Markers Module", typeof(NGSettings.ColorMarkersModuleSettings), 30);
		}

		public	ColorMarkersModule()
		{
		}

		public override void	OnEnable(NGConsoleWindow editor, int id)
		{
			base.OnEnable(editor, id);

			this.coloredRows = new List<ColoredRow>();

			RowsDrawer.GlobalBeforeFoldout += this.RowsDrawer_GlobalBeforeFoldout;
			RowsDrawer.GlobalLogContextMenu += this.AppendColorsMenuItem;
			this.console.BeforeGUIHeaderRightMenu += this.HeaderButton;
		}

		public override void	OnDisable()
		{
			RowsDrawer.GlobalBeforeFoldout -= this.RowsDrawer_GlobalBeforeFoldout;
			RowsDrawer.GlobalLogContextMenu -= this.AppendColorsMenuItem;
			this.console.BeforeGUIHeaderRightMenu -= this.HeaderButton;
		}

		private void	AppendColorsMenuItem(GenericMenu menu, Row row)
		{
			if (Preferences.Settings == null)
				return;

			for (int i = 0; i < Preferences.Settings.colorMarkersModule.colorBackgrounds.Count; i++)
			{
				if (Preferences.Settings.colorMarkersModule.colorBackgrounds[i].name != string.Empty)
					menu.AddItem(new GUIContent(Preferences.Settings.colorMarkersModule.nestedMenu == true ? "Colors/" + Preferences.Settings.colorMarkersModule.colorBackgrounds[i].name : Preferences.Settings.colorMarkersModule.colorBackgrounds[i].name), this.coloredRows.Exists((e) => e.i == i && e.row == row), this.ToggleColor, new object[] { row, i });
			}
		}

		private void	ToggleColor(object data)
		{
			object[]	array = data as object[];
			Row			row = array[0] as Row;
			int			i = (int)array[1];

			for (int j = 0; j < this.coloredRows.Count;  j++)
			{
				if (this.coloredRows[j].row == row)
				{
					if (this.coloredRows[j].i == i)
						this.coloredRows.RemoveAt(j);
					else
						this.coloredRows[j].i = i;

					return;
				}
			}

			this.coloredRows.Add(new ColoredRow() { row = row, i = i });
		}

		private Rect	RowsDrawer_GlobalBeforeFoldout(Rect r, int i, Row row)
		{
			if (Preferences.Settings == null)
				return r;

			List<ColorMarker>		markers = Preferences.Settings.colorMarkersModule.colorMarkers;
			List<ColorBackground>	stamps = Preferences.Settings.colorMarkersModule.colorBackgrounds;

			for (int j = 0; j < this.coloredRows.Count;  j++)
			{
				if (this.coloredRows[j].row == row)
				{
					if (this.coloredRows[j].i < stamps.Count)
						EditorGUI.DrawRect(r, stamps[this.coloredRows[j].i].color);
					return r;
				}
			}

			for (int j = 0; j < markers.Count; j++)
			{
				if (markers[j].groupFilters.filters.Count > 0 &&
					markers[j].groupFilters.Filter(row) == true)
				{
					EditorGUI.DrawRect(r, markers[j].backgroundColor);
					break;
				}
			}

			return r;
		}

		private void	HeaderButton()
		{
			if (Preferences.Settings != null && GUILayout.Button(LC.G("ColorMarkers"), Preferences.Settings.general.menuButtonStyle) == true)
				ScriptableWizard.GetWindow<ColorMarkersWizard>(true, ColorMarkersWizard.Title, true);
		}
	}
}