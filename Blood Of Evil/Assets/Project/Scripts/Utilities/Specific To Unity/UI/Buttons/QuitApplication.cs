using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Utilities.UI
{
    public class QuitApplication : AButton
    {
        #region Public Behaviour
        public override void ButtonActionOnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
        }
        #endregion
    }
}