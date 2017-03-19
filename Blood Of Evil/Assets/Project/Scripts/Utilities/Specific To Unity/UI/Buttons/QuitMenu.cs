using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Utilities.UI
{
    public class QuitMenu : AButton
    {
        #region Fields
        [SerializeField]
        private string menuToClose;
        #endregion

        #region Abstract Behaviour
        public override void ButtonActionOnClick()
        {
            Player.PlayerServicesAndModulesContainer.Instance.CanvasesService.Close(this.menuToClose);
        }
        #endregion
    }
}