using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Game.UI
{
    using ObjectInScene;
    using Utilities.UI;
    public class AutoSaveMinutesSlider : ASlider
    {
        #region Fields
        private const float CONVERSION_SECONDS_MINUTES = 60.0f;
        #endregion

        #region Override Behaviour
        public override void InitializeValue()
        {
            base.slider.value = PlayerServicesAndModulesContainer.Instance.GameService.AutoSaveEveryXSeconds / CONVERSION_SECONDS_MINUTES;
        }
        public override void OnSliderValueModified(float value)
        {
            base.OnSliderValueModified(value);

            PlayerServicesAndModulesContainer.Instance.GameService.AutoSaveEveryXSeconds = value * CONVERSION_SECONDS_MINUTES;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.GameService).Save();
        }
        #endregion
    }
}