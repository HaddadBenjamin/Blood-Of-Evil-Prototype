using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Audio
{
    [System.Serializable]
    public sealed class AudioCategory
    {
        #region Fields
        private float volume = 1.0f;
        public Action<float> VolumeHaveBeenModified;
        #endregion

        #region Properties
        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;

                if (null != VolumeHaveBeenModified)
                    this.VolumeHaveBeenModified(volume);
            }
        }
        #endregion
    }
}