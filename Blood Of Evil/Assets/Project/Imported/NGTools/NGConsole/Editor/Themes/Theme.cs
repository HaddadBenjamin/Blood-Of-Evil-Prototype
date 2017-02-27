using System;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	public abstract class Theme
	{
		public static Color[]	colors = new Color[] {
			Constants.NormalFoldoutColor,
			Constants.WarningFoldoutColor,
			Constants.ErrorFoldoutColor,
			Constants.ExceptionFoldoutColor,
			Constants.ActiveFoldoutColor
		};

		/// <summary>
		/// Overwrites style in the given <paramref name="instance"/>.
		/// </summary>
		/// <param name="instance"></param>
		public abstract void	SetTheme(NGSettings instance);

		/// <summary>
		/// Generates 5 colored textures based on a mask represented by a base-64 string array (Use alpha equals to 1). Respectively for Normal, Warning, Error, Exception types and Active state.
		/// </summary>
		/// <param name="raw"></param>
		/// <returns></returns>
		public static Texture2D[]	GenerateFoldoutTextures(string raw)
		{
			byte[]		c = Convert.FromBase64String(raw);
			Texture2D[]	textures = new Texture2D[colors.Length];

			for (int i = 0; i < textures.Length; ++i)
				textures[i] = Theme.GenerateTexture(c, Theme.colors[i]);

			return textures;
		}

		public static Texture2D	GenerateTexture(byte[] raw, Color color)
		{
			Texture2D	textures = new Texture2D(16, 16);
			textures.alphaIsTransparency = true;
			textures.filterMode = FilterMode.Point;
			textures.LoadImage(raw);

			Color[]	pixels = textures.GetPixels();

			for (int j = 0; j < pixels.Length; j++)
			{
				if (pixels[j].a == 1F)
					pixels[j] = color;
			}

			textures.SetPixels(pixels);
			textures.Apply();

			return textures;
		}
	}
}