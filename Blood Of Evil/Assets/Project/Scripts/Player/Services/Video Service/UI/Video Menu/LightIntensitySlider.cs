using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using Utilities.UI;
    using ObjectInScene;

    public sealed class LightIntensitySlider : ASlider
    {
        #region Unherited Behaviour
        public override void InitializeValue()
        {
            base.InitializeValue(PlayerServicesAndModulesContainer.Instance.VideoService.LightIntensity);
        }

        public override void OnSliderValueModified(float value)
        {
            base.OnSliderValueModified(value);

            PlayerServicesAndModulesContainer.Instance.VideoService.LightIntensity = value;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
        }
        #endregion
    }
}