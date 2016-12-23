using UnityEngine;

namespace BloodOfEvil.Helpers
{
    public static class GridHelper
    {
        /// <summary>
        /// Renvoie l'index de la colonne à l'index "index".
        /// </summary>
        public static int GetColumn(int index, int numberOfElementsPerLine)
        {
            return index % numberOfElementsPerLine;
        }

        /// <summary>
        /// Renvoie l'index de la ligne à l'index "index".
        /// </summary>
        public static int GetLine(int index, int numberOfElementsPerLine)
        {
            return Mathf.CeilToInt(index / numberOfElementsPerLine);
        }

        /// <summary>
        /// Renvoie la position horizontal de la colonne "column".
        /// </summary>
        public static float GetHorizontalPosition(Vector2 startPosition, Vector2 offset, int column)
        {
            return startPosition.x + offset.x * column;
        }

        /// <summary>
        /// Renvoie la position horizontal de la colonne "column".
        /// </summary>
        public static float GetHorizontalPosition(int startHorizontalPosition, int offsetHorizontal, int column)
        {
            return startHorizontalPosition + offsetHorizontal * column;
        }

        /// <summary>
        /// Renvoie la position verticale de la ligne "ligne".
        /// </summary>
        public static float GetVerticalPosition(Vector2 startPosition, Vector2 offset, int line)
        {
            return startPosition.y + offset.y * line;
        }

        /// <summary>
        /// Renvoie la position verticale de la ligne "ligne".
        /// </summary>
        public static float GetVerticalPosition(int startVerticalPosition, int offsetVertical, int line)
        {
            return startVerticalPosition + offsetVertical * line;
        }
    }
}