using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Game.UI
{
    using ObjectInScene;
    using Utilities.UI;

    public sealed class AutoSaveToggle : AToggle
    {
        #region Abstract Behaviour
        public override void InitializeValue()
        {
            base.InitializeValue(PlayerServicesAndModulesContainer.Instance.GameService.AutoSave);
        }

        public override void OnValueChangedListener(bool state)
        {
            PlayerServicesAndModulesContainer.Instance.GameService.AutoSave = state;

            ((ISerializable)PlayerServicesAndModulesContainer.Instance.GameService).Save();
        }
        #endregion
    }
}