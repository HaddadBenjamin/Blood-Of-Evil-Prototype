using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BloodOfEvil.Utilities.UI
{
    using Scene;
    using Player.Services.Audio;

    /// <summary>
    /// Permet juste de jouer un son.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PlaySoundButton : AButton
    {
        #region Override Behaviour
        public override void ButtonActionOnClick() { }
        #endregion
    }
}