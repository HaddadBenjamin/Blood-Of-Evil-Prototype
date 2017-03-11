using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Player.Services;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    [RequireComponent(typeof(ClickableCollider))]
    public class ColliderMoveUserInput : AMoveUserInput
    {
        #region Unity Behaviour
        private void Awake()
        {
            GetComponent<ClickableCollider>().OnClickedDown.AddListener((isClickedDown) =>
            {
                base.DoesInteractionIsActive = isClickedDown;
            });
        }
        #endregion
    }
}
