using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Configuration
{
    using Modules.Attributes.Configuration;

    [System.Serializable]
    public class PlayerClassConfiguration
    {
        #region Fields
        [SerializeField]
        private PlayerClassCharacteristicCategoryAttributeConfiguration characteristicsAttributes;
        #endregion

        #region Properties
        public PlayerClassCharacteristicCategoryAttributeConfiguration CharacteristicAttributes
        {
            get { return characteristicsAttributes; }
            private set { characteristicsAttributes = value; }
        }
        #endregion
    }
}