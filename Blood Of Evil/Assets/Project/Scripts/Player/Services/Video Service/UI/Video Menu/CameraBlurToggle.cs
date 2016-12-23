using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using ObjectInScene;
    using Utilities.UI;

    public sealed class CameraBlurToggle : AToggle
    {
        #region Abstract Behaviour
        public override void InitializeValue()
        {
            base.InitializeValue(PlayerServicesAndModulesContainer.Instance.VideoService.CameraBlur);
        }

        public override void OnValueChangedListener(bool state)
        {
            PlayerServicesAndModulesContainer.Instance.VideoService.CameraBlur = state;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
        }
        #endregion
    }
}