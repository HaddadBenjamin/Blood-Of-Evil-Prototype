using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.Serialization
{
    [System.Serializable]
    public class SerializableBoolean
    {
        #region Fields
        public bool Data;
        #endregion

        #region Save & Load
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="data"></param>
        public SerializableBoolean(bool data)
        {
            this.Data = data;
        }

        public bool Load()
        {
            return this.Data;
        }
        #endregion
    }
}