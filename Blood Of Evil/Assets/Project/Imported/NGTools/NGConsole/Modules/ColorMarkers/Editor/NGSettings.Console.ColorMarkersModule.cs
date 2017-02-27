using NGToolsEditor.NGConsole;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class ColorMarkersModuleSettings
		{
			[HideInInspector]
			public List<ColorMarker>		colorMarkers = new List<ColorMarker>();
			[LocaleHeader("NGSettings_ColorMarkersModule_NestedMenu")]
			public bool						nestedMenu = true;
			[LocaleHeader("NGSettings_ColorMarkersModule_ColorBackgrounds")]
			public List<ColorBackground>	colorBackgrounds = new List<ColorBackground>() {
				new ColorBackground() { name = "Black", color = new Color(Color.black.r, Color.black.g, Color.black.b, .6F) },
				new ColorBackground() { name = "White", color = new Color(Color.white.r, Color.white.g, Color.white.b, .6F) },
				new ColorBackground() { name = "Blue", color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, .6F) },
				new ColorBackground() { name = "Red", color = new Color(Color.red.r, Color.red.g, Color.red.b, .6F) },
				new ColorBackground() { name = "Green", color = new Color(Color.green.r, Color.green.g, Color.green.b, .6F) },
				new ColorBackground() { name = "Yellow", color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, .6F) },
				new ColorBackground() { name = "Cyan", color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, .6F) },
			};
		}
		public ColorMarkersModuleSettings	colorMarkersModule = new ColorMarkersModuleSettings();
	}
}