using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Utilities.UI
{
    public class CloseMenuAndOpenANewOneAtClick : AButton
    {
        #region Fields
        [SerializeField]
        private string menuToClose;
        [SerializeField]
        private string menuToOpen;
        #endregion

        #region Public Behaviour
        public override void ButtonActionOnClick()
        {
            Player.PlayerServicesAndModulesContainer.Instance.CanvasesService.Close(this.menuToClose);
            Player.PlayerServicesAndModulesContainer.Instance.CanvasesService.Open(this.menuToOpen);
        }
        #endregion
    }
}