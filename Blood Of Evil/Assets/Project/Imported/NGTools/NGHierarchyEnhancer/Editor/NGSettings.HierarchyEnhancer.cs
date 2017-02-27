using NGTools;
using System;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class HierarchyEnhancer
		{
			[Serializable]
			public class ComponentColor
			{
				public Type		type;
				public Color	color;
			}

			public const int	TotalLayers = 32;

			public bool				enable = true;
			public float			margin = 0F;
			public EventModifiers	holdModifiers = EventModifiers.Shift;
			public EventModifiers	selectionHoldModifiers = EventModifiers.Alt;
			[NonSerialized]
			public Color[]			layers;
			public Texture2D[]		layersIcon;
			public float			widthPerComponent = 16F;
			[NonSerialized]
			public ComponentColor[]	componentColors;

			public byte[]	serializedLayers;
			public byte[]	serializedComponentColors;

			public void	InitializeLayers()
			{
				if (this.layersIcon == null ||
					this.layersIcon.Length != HierarchyEnhancer.TotalLayers)
				{
					this.layersIcon = new Texture2D[HierarchyEnhancer.TotalLayers];
				}

				if (this.layers == null ||
					this.layers.Length != HierarchyEnhancer.TotalLayers)
				{
					this.layers = new Color[HierarchyEnhancer.TotalLayers];
					if (this.serializedLayers != null && this.serializedLayers.Length > 0)
					{
						ByteBuffer	buffer = Utility.GetBBuffer(this.serializedLayers);

						for (int i = 0; i < HierarchyEnhancer.TotalLayers; i++)
							this.layers[i] = new Color(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());

						Utility.RestoreBBuffer(buffer);
					}
				}

				if (this.componentColors == null)
					this.componentColors = new ComponentColor[0];

				if (this.serializedComponentColors != null && this.serializedComponentColors.Length > 0)
				{
					ByteBuffer	buffer = Utility.GetBBuffer(this.serializedComponentColors);

					Preferences.Settings.hierarchy.componentColors = new ComponentColor[buffer.ReadInt32()];

					for (int i = 0; i < Preferences.Settings.hierarchy.componentColors.Length; i++)
					{
						Preferences.Settings.hierarchy.componentColors[i] = new ComponentColor();
						string	type = buffer.ReadUnicodeString();

						if (string.IsNullOrEmpty(type) == false)
							Preferences.Settings.hierarchy.componentColors[i].type = Type.GetType(type);

						Preferences.Settings.hierarchy.componentColors[i].color = new Color(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
					}

					Utility.RestoreBBuffer(buffer);
				}
			}
		}
		public HierarchyEnhancer	hierarchy = new HierarchyEnhancer();
	}
}