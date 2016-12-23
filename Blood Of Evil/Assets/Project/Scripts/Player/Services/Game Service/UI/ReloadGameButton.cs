using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Services.Game.UI
{
    using Helpers;
    using Utilities.UI;

    public class ReloadGameButton : AButton
    {
        #region Override Behaviour
        public override void ButtonActionOnClick()
        {
            SceneManagerHelper.LoadSceneWithoutLoadFiles(SceneManagerHelper.GetCurrentSceneName());
        }
        #endregion
    }
}