using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Helpers
{
    public static class QualitySettingsHelper
    {
        public static int GetQualityNameIndex(string qualityName)
        {
            string[] qualitiesNames = QualitySettings.names;

            for (int qualityIndex = 0; qualityIndex < qualitiesNames.Length; qualityIndex++)
            {
                if (qualitiesNames[qualityIndex].Equals(qualityName))
                    return qualityIndex;
            }

            Debug.LogFormat("La quality {0} n'a pas été trouvé", qualityName);

            return 0;
        }
    }
}