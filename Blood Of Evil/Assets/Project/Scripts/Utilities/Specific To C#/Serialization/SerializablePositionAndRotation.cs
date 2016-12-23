using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.Serialization
{
    [System.Serializable]
    public class SerializablePositionAndRotation
    {
        #region Fields
        public Vector3 Position;
        public Vector3 EulerAngles;
        #endregion

        #region Save & Load
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="transform"></param>
        public SerializablePositionAndRotation(Transform transform)
        {
            this.Position = transform.position;
            this.EulerAngles = transform.eulerAngles;
        }

        public void Load(Transform transform)
        {
            transform.position = this.Position;
            transform.eulerAngles = this.EulerAngles;
        }
        #endregion
    }
}