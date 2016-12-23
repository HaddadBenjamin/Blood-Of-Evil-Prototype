using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Extensions
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Renvoie le bytes de la texture encodé en PNG.
        /// </summary>
        public static byte[] GetPNGBytes(this Texture2D texture)
        {
            return texture.EncodeToPNG();
        }

        /// <summary>
        /// Renvoie le bytes de la texture encodé en JPG.
        /// </summary>
        public static byte[] GetJPGBytes(this Texture2D texture)
        {
            return texture.EncodeToJPG();
        }

        /// <summary>
        /// Créer une sprite à partir d'une texture.
        /// </summary>
        public static Sprite ToSprite(
            this Texture2D texture,
            Sprite sprite)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    sprite == null ?
                        new Vector2(0, 0) :
                        sprite.pivot);
        }

        /// <summary>
        /// Colorie entièrement une texture de blanc.
        /// </summary>
        public static void ClearTexture(
            this Texture2D texture,
            int textureWidth,
            int textureHeight)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                    texture.SetPixel(x, y, Color.white);
            }

            texture.Apply();
        }

        /// <summary>
        /// Permet de peindre dans une texture en utilisant le curseur de la souris.
        /// </summary>
        public static void Paint(
            this Texture2D texture,
            int textureWidth,
            int textureHeight,
            ref Vector2 oldPosition, 
            ref Vector2 newdPosition,
            RaycastHit hit)
        {
            if (Input.GetMouseButton(0))
            {
                oldPosition = newdPosition;
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    newdPosition = GetPointFromTexture(hit.textureCoord, textureWidth, textureHeight);

                    texture.DrawCircle(textureWidth, textureHeight, newdPosition, 1, Color.black);

                    texture.Apply();
                }
            }
        }

        public static void DrawCircle(
            this Texture2D texture,
            int textureWidth,
            int textureHeight,
            Vector3 point,
            int pointRadius,
            Color pointColor)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    if (Vector2.Distance(GetPointFromTexture(point, textureWidth, textureHeight), new Vector3(x, y)) <= pointRadius)
                        texture.SetPixel(x, y, pointColor);
                }
            }
        }

        public static Vector2 GetPointFromTexture(
            Vector3 point,
            int textureWidth,
            int textureHeight)
        {
            return new Vector2(point.x * textureWidth, point.y * textureHeight);
        }

        public static void DrawLine(
            this Texture2D tex, 
            int x0, 
            int y0, 
            int x1, 
            int y1, 
            Color col)
        {
            int dy = (int)(y1 - y0);
            int dx = (int)(x1 - x0);
            int stepx, stepy;

            if (dy < 0) { dy = -dy; stepy = -1; }
            else { stepy = 1; }
            if (dx < 0) { dx = -dx; stepx = -1; }
            else { stepx = 1; }
            dy <<= 1;
            dx <<= 1;

            float fraction = 0;

            tex.SetPixel(x0, y0, col);
            if (dx > dy)
            {
                fraction = dy - (dx >> 1);
                while (Mathf.Abs(x0 - x1) > 1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }
                    x0 += stepx;
                    fraction += dy;
                    tex.SetPixel(x0, y0, col);
                }
            }
            else
            {
                fraction = dx - (dy >> 1);
                while (Mathf.Abs(y0 - y1) > 1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }
                    y0 += stepy;
                    fraction += dx;
                    tex.SetPixel(x0, y0, col);
                }
            }
        }
    }
}