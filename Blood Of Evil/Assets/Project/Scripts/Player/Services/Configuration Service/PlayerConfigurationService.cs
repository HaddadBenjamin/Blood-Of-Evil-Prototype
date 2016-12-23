using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Configuration
{
    using Helpers;
    using ObjectInScene;

    using Modules.Attributes.Configuration;

    [System.Serializable]
    public class PlayerConfigurationService : IDataInitializable
    {
        #region Fields
        #region Attributes
        [SerializeField, Header("Characteristics Configuration")]
        private PlayerCharacteristicCategoryAttributeConfiguration characteristicAttributes;

        [SerializeField, Header("Class Configuration")]
        private PlayerClassConfiguration croMagnon;
        [SerializeField]
        private PlayerClassConfiguration rogue;
        [SerializeField]
        private PlayerClassConfiguration assasin;
        [SerializeField]
        private PlayerClassConfiguration mage;
        [SerializeField]
        private PlayerClassConfiguration druid;
        [SerializeField]
        private PlayerClassConfiguration demonist;
        [SerializeField]
        private PlayerClassConfiguration priest;
        [SerializeField]
        private PlayerClassConfiguration tank;
        private PlayerClassConfiguration[] classes;
        #endregion
        #endregion

        #region Properties
        public PlayerCharacteristicCategoryAttributeConfiguration CharacteristicAttributes
        {
            get { return characteristicAttributes; }
            private set { characteristicAttributes = value; }
        }
        #endregion

        #region Interface Behaviour
        void IDataInitializable.Initialize()
        {
            this.classes = new PlayerClassConfiguration[EnumerationHelper.Count<EPlayerClass>()];

            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Assasin)] = this.assasin;
            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Druid)] = this.druid;
            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Cro_Magnon)] = this.croMagnon;
            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Demonist)] = this.demonist;
            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Mage)] = this.mage;
            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Priest)] = this.priest;
            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Rogue)] = this.rogue;
            this.classes[EnumerationHelper.GetIndex(EPlayerClass.Tank)] = this.tank;
        }
        #endregion

        #region Public Behaviour
        public PlayerClassConfiguration GetClass(EPlayerClass classe)
        {
            return this.classes[EnumerationHelper.GetIndex(classe)];
        }
        #endregion
    }
}