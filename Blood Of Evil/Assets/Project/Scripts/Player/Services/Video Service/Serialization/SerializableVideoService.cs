using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Video.Serialization
{
    [System.Serializable]
    public class SerializableVideoService
    {
        #region Fields
        public float LightIntensity = 1.0f;
        public bool FullScreen;
        public bool AntiAliasing = true;

        public bool SunShafts = true;
        public bool Bloom = true;
        public bool ColorCorrectionCurves = true;
        public bool VignetteAndChromaticAberation = true;
        public bool GlobalFog = true;

        public bool CameraBlur;
        public int QualityIndex;
        public string Resolution; // = default ?
        #endregion

        #region Constructor
        public SerializableVideoService() {  }
        #endregion

        #region Constructor
        public SerializableVideoService(Player.Services.Video.VideoService videoService)
        {
            this.LightIntensity = videoService.LightIntensity;
            this.FullScreen = videoService.FullScreen;
            this.AntiAliasing = videoService.AntiAliasing;
            this.CameraBlur = videoService.CameraBlur;
            this.QualityIndex = videoService.QualityIndex;
            this.Resolution = videoService.Resolution;

            this.SunShafts = videoService.SunShafts;
            this.Bloom = videoService.Bloom;
            this.VignetteAndChromaticAberation = videoService.VignetteAndChromaticAberation;
            this.ColorCorrectionCurves = videoService.ColorCorrectionCurves;
            this.GlobalFog = videoService.GlobalFog;
        }
        #endregion
    }
}