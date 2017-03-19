using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Créer un DateTime à partir d'un nombre de secondes définit.
        /// </summary>
        public static DateTime CreateDateTimeWithSeconds(int seconds)
        {
            DateTime dateTime = new DateTime();

            dateTime.AddSeconds(ConvertHelper.IntToDouble(Math.Abs(seconds)));

            return dateTime;
        }
    }
}