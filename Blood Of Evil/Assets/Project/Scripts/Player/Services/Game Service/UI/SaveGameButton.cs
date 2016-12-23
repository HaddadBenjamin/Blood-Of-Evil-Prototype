using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Game.UI
{
    using Helpers;
    using Utilities.UI;

    public class SaveGameButton : AButton
    {
        #region Override Behaviour
        public override void ButtonActionOnClick()
        {
            SceneManagerHelper.SaveCurrentScene();
        }
        #endregion
    }
}