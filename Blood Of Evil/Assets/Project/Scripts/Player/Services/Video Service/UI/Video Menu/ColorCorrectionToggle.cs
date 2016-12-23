using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using Helpers;
    using ObjectInScene;
    using Utilities.UI;

    public class ColorCorrectionToggle : AToggle
    {
        #region Abstract Behaviour
        public override void InitializeValue()
        {
            base.InitializeValue(PlayerServicesAndModulesContainer.Instance.VideoService.ColorCorrectionCurves);
        }

        public override void OnValueChangedListener(bool state)
        {
            PlayerServicesAndModulesContainer.Instance.VideoService.ColorCorrectionCurves = state;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
        }
        #endregion
    }
}