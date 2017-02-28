using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Enemies.Serialization
{
    [System.Serializable]
    public class SerializableEEnemyCategory
    {
        #region Fields
        public EEnemyCategory Data;
        #endregion

        #region Constructor
        public SerializableEEnemyCategory() { }
        #endregion

        #region Save & Load
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="transform"></param>
        public SerializableEEnemyCategory(EEnemyCategory Data)
        {
            this.Data = Data;
        }

        public EEnemyCategory Load()
        {
            return this.Data;
        }
        #endregion
    }
}