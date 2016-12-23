using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Video
{

    [RequireComponent(typeof(Light))]
    public class LightNode : MonoBehaviour
    {
        #region Fields
        private Light lightComponent;
        private float lightIntensityAtInitialisation;
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.lightComponent = GetComponent<Light>();
            this.lightIntensityAtInitialisation = this.lightComponent.intensity;

            VideoService lightService = PlayerServicesAndModulesContainer.Instance.VideoService;

            lightService.LightIntensityListener += this.UpdateLightIntensity;
            this.UpdateLightIntensity(lightService.LightIntensity);
        }

        void OnDestroy()
        {
            //PlayerModulesContainer.Instance.VideoService.LightIntensityListener -= this.UpdateLightIntensity;
        }
        #endregion

        #region Intern Behaviour
        private void UpdateLightIntensity(float lightIntensity)
        {
            this.lightComponent.intensity = this.lightIntensityAtInitialisation * lightIntensity;
        }
        #endregion
    }
}