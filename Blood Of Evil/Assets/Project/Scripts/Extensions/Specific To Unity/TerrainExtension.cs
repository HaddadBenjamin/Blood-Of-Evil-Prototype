using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    public static class TerrainExtension
    {
        /// <summary>
        /// This function have been retrieved from internet, I might have to optimized it later.
        /// </summary>
        /// This method don't work !
        public static Texture GetTextureAtPosition(this Terrain terrain, Vector3 position)
        {
            if (null != terrain)
            {
                // NO TERRAIN ?
                // Set up:
                Texture retval = new Texture();
                Vector3 TS; // terrain size
                Vector2 AS; // control texture size

                TS = terrain.terrainData.size;
                AS.x = terrain.terrainData.alphamapWidth;
                AS.y = terrain.terrainData.alphamapHeight;

                // Lookup texture we are standing on:
                int AX = (int)((position.x / TS.x) * AS.x + 0.5f);
                int AY = (int)((position.z / TS.z) * AS.y + 0.5f);
                float[,,] TerrCntrl = terrain.terrainData.GetAlphamaps(AX, AY, 1, 1);

                TerrainData TD = terrain.terrainData;

                for (int i = 0; i < TD.splatPrototypes.Length; i++)
                {
                    if (TerrCntrl[0, 0, i] > .5f)
                        retval = TD.splatPrototypes[i].texture;
                }

                return retval;
            }

            return null;
        }
    }
}
