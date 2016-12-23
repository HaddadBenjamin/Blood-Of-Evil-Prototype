using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using Utilities.UI;
    using ObjectInScene;

    public class GlobalFogToggle : AToggle
    {
        #region Abstract Behaviour
        public override void InitializeValue()
        {
            base.InitializeValue(PlayerServicesAndModulesContainer.Instance.VideoService.GlobalFog);
        }

        public override void OnValueChangedListener(bool state)
        {
            PlayerServicesAndModulesContainer.Instance.VideoService.GlobalFog = state;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
        }
        #endregion
    }
}