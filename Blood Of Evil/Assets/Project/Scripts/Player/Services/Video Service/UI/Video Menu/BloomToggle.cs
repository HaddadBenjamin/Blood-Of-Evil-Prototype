using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using ObjectInScene;
    using Utilities.UI;

    public class BloomToggle : AToggle
    {
        #region Abstract Behaviour
        public override void InitializeValue()
        {
            base.InitializeValue(PlayerServicesAndModulesContainer.Instance.VideoService.Bloom);
        }

        public override void OnValueChangedListener(bool state)
        {
            PlayerServicesAndModulesContainer.Instance.VideoService.Bloom = state;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
        }
        #endregion
    }
}