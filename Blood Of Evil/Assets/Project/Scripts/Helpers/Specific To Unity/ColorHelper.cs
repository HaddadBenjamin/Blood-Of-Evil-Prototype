using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Helpers
{
    public static class ColorHelper
    {
        #region Fields
        //public static Color Red = CreateColor(255.0f, 0.0f, 0.0f);
        //public static Color Green = CreateColor(0.0f, 255.0f, 0.0f);
        public static Color Black = CreateColor(0.0f, 0.0f, 0.0f);
        public static Color LightGreen = CreateColor(118.0f, 240.0f, 157.0f);
        public static Color Green = CreateColor(59.0f, 224.0f, 56.0f);
        public static Color Blue = CreateColor(59.0f, 87.0f, 221.0f);
        public static Color Yellow = CreateColor(246.0f, 230.0f, 13.0f);
        public static Color Purple = CreateColor(233.0f, 13.0f, 246.0f);
        public static Color Red = CreateColor(246.0f, 13.0f, 46.0f);
        public static Color LightRed = CreateColor(215.0f, 27.0f, 58.0f);
        #endregion

        #region Public Behaviour
        public static Color CreateColor(float r, float g, float b, float a = 255.0f)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a);
        }
        #endregion
    }
}