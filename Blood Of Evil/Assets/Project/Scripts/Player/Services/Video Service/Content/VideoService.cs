using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Video
{
    using Scene;
    using Helpers;
    using ObjectInScene;
    using Serialization;
    using Extensions;

    public class VideoService : ISerializable, IDataInitializable
    {
        #region Fields 
        private float lightIntensity = 1.0f;
        private bool fullScreen = true;
        private bool antiAliasing = true;
        private bool sunShafts = false;
        private bool bloom = false;
        private bool colorCorrectionCurves = true;
        private bool vignetteAndChromaticAberation = true;
        private bool globalFog = false;
        private bool cameraBlur;
        private int qualityIndex;
        private string resolution;

        private GameObject playerCamera;

        // Devrait-être dnas un helper I guess.
        public static string[] Resolutions = new string[]
        {
        "1280x800",
        "1440x900",
        "1680x800",
        "1920x1200",
        "2560x1600",
        };

        public Action<float> LightIntensityListener;
        public Action<int> QualityIndexListener;
        #endregion

        #region Properties
        public bool CameraBlur
        {
            get { return cameraBlur; }
            set
            {
                cameraBlur = value;

                this.UpdateCameraBlur();
            }
        }

        public string Resolution
        {
            get { return resolution; }
            set
            {
                resolution = value;

                this.UpdateResolution();
            }
        }

        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                fullScreen = value;

                this.UpdateFullScreen();
            }
        }
        public int QualityIndex
        {
            get { return qualityIndex; }
            set
            {
                qualityIndex = value;

                this.UpdateQualitySettings();

                QualityIndexListener.SafeCall(qualityIndex);
            }
        }

        public bool AntiAliasing
        {
            get { return antiAliasing; }
            set
            {
                antiAliasing = value;

                this.UpdateQualitySettings();
            }
        }

        public float LightIntensity
        {
            get { return lightIntensity; }
            set
            {
                lightIntensity = value;

                LightIntensityListener.SafeCall(lightIntensity);
            }
        }

        public bool Bloom
        {
            get
            {
                return bloom;
            }

            set
            {
                bloom = value;

                this.UpdateBloom();
            }
        }

        public bool ColorCorrectionCurves
        {
            get
            {
                return colorCorrectionCurves;
            }

            set
            {
                colorCorrectionCurves = value;

                this.UpdateColorCorrectionCurves();
            }
        }

        public bool VignetteAndChromaticAberation
        {
            get
            {
                return vignetteAndChromaticAberation;
            }

            set
            {
                vignetteAndChromaticAberation = value;

                this.UpdateVignette();
            }
        }

        public bool GlobalFog
        {
            get
            {
                return globalFog;
            }

            set
            {
                globalFog = value;

                this.UpdateGlobalFog();
            }
        }

        public bool SunShafts
        {
            get
            {
                return sunShafts;
            }

            set
            {
                sunShafts = value;

                this.UpdateSunShafts();
            }
        }
        #endregion

        #region Interfaces Behaviour
        void ISerializable.Load()
        {
            SerializerHelper.Load<SerializableVideoService>(
                filename: this.GetFileName(),
                isReplicatedNextTheBuild: false,
                isEncrypted: false,
                onLoadSuccess: (SerializableVideoService serializableVideoService) =>
                {
                    this.Resolution = serializableVideoService.Resolution;
                    this.FullScreen = serializableVideoService.FullScreen;
                    this.AntiAliasing = serializableVideoService.AntiAliasing;
                    this.LightIntensity = serializableVideoService.LightIntensity;
                    this.CameraBlur = serializableVideoService.CameraBlur;
                    this.QualityIndex = serializableVideoService.QualityIndex;

                    this.SunShafts = serializableVideoService.SunShafts;
                    this.Bloom = serializableVideoService.Bloom;
                    this.VignetteAndChromaticAberation = serializableVideoService.VignetteAndChromaticAberation;
                    this.ColorCorrectionCurves = serializableVideoService.ColorCorrectionCurves;
                    this.GlobalFog = serializableVideoService.GlobalFog;
                },
                onLoadError: () =>
                {
                    Debug.Log("pas d'inquiétude à avoir, c'est normal que ce fichier n'éxiste pas lorsque l'on a pas sauvegarder au moins une fois.");
                });
        }

        void ISerializable.Save()
        {
            SerializerHelper.Save<SerializableVideoService>(
                filename: this.GetFileName(),
                dataToSave : new SerializableVideoService(this),
                isReplicatedNextTheBuild: false,
                isEncrypted: false);
        }
        #endregion

        #region Interfaces Behaviour
        void IDataInitializable.Initialize()
        {
            this.playerCamera =
                PlayerServicesAndModulesContainer.Instance.
                    GameObjectInSceneReferencesService.Get("Player Camera");
            ((ISerializable) this).Load();

            this.QualityIndex = QualitySettingsHelper.GetQualityNameIndex("Simple");
        }
        #endregion

        #region Intern Behaviour
        private string GetFileName()
        {
            return SceneServicesContainer.Instance.FileSystemConfiguration.VideoSettingsFilename;
        }

        private void UpdateQualitySettings()
        {
            QualitySettings.SetQualityLevel(this.QualityIndex, this.AntiAliasing);
        }

        private void UpdateResolution()
        {
            int indexOfX = this.Resolution.IndexOf('x');
            int width = int.Parse(this.Resolution.Substring(0, indexOfX));
            int height = int.Parse(this.Resolution.Substring(indexOfX + 1, this.Resolution.Length - indexOfX - 1));

            Screen.SetResolution(width, height, this.FullScreen);
        }

        private void UpdateFullScreen()
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, this.FullScreen);
        }

        // c'est crade il faudrait stoquer la caméra à l'init.
        private void UpdateCameraBlur()
        {
            PlayerServicesAndModulesContainer.Instance.
                GameObjectInSceneReferencesService.Get("Player Camera").
                GetComponent<UnityStandardAssets.ImageEffects.CameraMotionBlur>().enabled = this.CameraBlur;
        }

        private void UpdateBloom()
        {
            this.playerCamera.GetComponent<UnityStandardAssets.ImageEffects.Bloom>().enabled = this.Bloom;
        }

        private void UpdateSunShafts()
        {
            this.playerCamera.GetComponent<UnityStandardAssets.ImageEffects.SunShafts>().enabled = this.SunShafts;
        }

        private void UpdateColorCorrectionCurves()
        {
            this.playerCamera.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().enabled = this.ColorCorrectionCurves;
        }

        private void UpdateVignette()
        {
            this.playerCamera.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().enabled = this.VignetteAndChromaticAberation;
        }

        private void UpdateGlobalFog()
        {
            this.playerCamera.GetComponent<UnityStandardAssets.ImageEffects.GlobalFog>().enabled = this.GlobalFog;
        }
        #endregion
    }
}