using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.Serialization
{
    [System.Serializable]
    public class SerializableString
    {
        #region Fields
        public string Data;
        #endregion

        #region Constructor
        public SerializableString() { }
        #endregion

        #region Save & Load
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="data"></param>
        public SerializableString(string data)
        {
            this.Data = data;
        }

        public string Load()
        {
            return this.Data;
        }
        #endregion
    }
}