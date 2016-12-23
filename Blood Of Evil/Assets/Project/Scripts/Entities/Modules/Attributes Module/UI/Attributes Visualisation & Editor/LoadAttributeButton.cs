using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes.UI
{
    using Utilities.UI;
    using Player;

    public class LoadAttributeButton : AButton
    {
        #region Abstract Behaviour.
        public override void ButtonActionOnClick()
        {
            PlayerServicesAndModulesContainer.Instance.AttributesModule.SpecificLoadForPlayer();
        }
        #endregion
    }
}