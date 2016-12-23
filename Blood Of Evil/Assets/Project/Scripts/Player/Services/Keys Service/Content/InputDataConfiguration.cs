using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Player.Services.Keys
{
    [System.Serializable]
    public sealed class InputDataConfiguration
    {
        #region Fields
        public List<KeyCode> KeyCodes = new List<KeyCode>();

        // Permet de dire si un évênement à été appuyé la frame courante et précédente. 
        // Permet aussi d'éviter qu'un évênement soit testé plusieurs fois par frame.
        [HideInInspector]
        public bool DoesIsUpAtPreviousFrame;
        [HideInInspector]
        public bool DoesIsUp;

        [HideInInspector]
        public bool DoesIsDownAtPreviousFrame;
        [HideInInspector]
        public bool DoesIsDown;

        [HideInInspector]
        public bool DoesIsContinouslyDownAtPreviousFrame;
        [HideInInspector]
        public bool DoesIsContinouslyDown;
        #endregion
    }
}