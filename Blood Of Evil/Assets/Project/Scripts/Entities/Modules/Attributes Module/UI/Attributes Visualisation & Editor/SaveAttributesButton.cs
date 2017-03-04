using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Entities.Modules.Attributes.UI
{
    using Player;
    using Utilities.UI;
    using ObjectInScene;

    public class SaveAttributesButton : AButton
    {
        #region Abstract Behaviour.
        public override void ButtonActionOnClick()
        {
            ((ISerializable)PlayerServicesAndModulesContainer.Instance.AttributesModule).Save();

            PlayerServicesAndModulesContainer.Instance.AttributesModule.SaveCharacteristicsPointsAddedButNotApplied();
        }
        #endregion
    }
}