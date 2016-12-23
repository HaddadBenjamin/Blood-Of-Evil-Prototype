using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Game.UI
{
    using Scene;
    using ObjectInScene;
    using Utilities.UI;

    public class LoadGameButton : AButton
    {
        #region Override Behaviour
        public override void ButtonActionOnClick()
        {
            ((ISerializable)SceneServicesContainer.Instance.SceneStateModule).Load();
        }
        #endregion
    }
}