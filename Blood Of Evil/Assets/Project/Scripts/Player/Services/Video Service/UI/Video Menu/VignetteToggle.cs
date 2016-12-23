using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using Utilities.UI;
    using ObjectInScene;

    public class VignetteToggle : AToggle
    {
        #region Abstract Behaviour
        public override void InitializeValue()
        {
            base.InitializeValue(PlayerServicesAndModulesContainer.Instance.VideoService.VignetteAndChromaticAberation);
        }

        public override void OnValueChangedListener(bool state)
        {
            PlayerServicesAndModulesContainer.Instance.VideoService.VignetteAndChromaticAberation = state;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
        }
        #endregion
    }
}