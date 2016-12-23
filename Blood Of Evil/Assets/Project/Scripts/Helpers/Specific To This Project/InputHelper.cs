using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Helpers
{
    using Player.Services.Keys;

    public static class InputDataConfigurationHelper
    {
        public static InputDataConfiguration[] CopyDataConfigurations(InputDataConfiguration[] dataConfigurations)
        {
            InputDataConfiguration[] copy = new InputDataConfiguration[dataConfigurations.Length];

            for (int i = 0; i < dataConfigurations.Length; i++)
                copy[i] = CopyDataConfiguration(dataConfigurations[i]);

            return copy;
        }

        public static InputDataConfiguration CopyDataConfiguration(InputDataConfiguration dataConfiguration)
        {
            InputDataConfiguration copy = new InputDataConfiguration();

            copy.KeyCodes = new List<KeyCode>();

            for (int i = 0; i < dataConfiguration.KeyCodes.Count; i++)
                copy.KeyCodes.Add(dataConfiguration.KeyCodes[i]);

            return copy;
        }
    }
}