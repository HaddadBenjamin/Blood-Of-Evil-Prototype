using System.Collections;
using System.Collections.Generic;
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
