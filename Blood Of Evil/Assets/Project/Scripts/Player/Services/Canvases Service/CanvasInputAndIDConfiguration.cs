using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Canvases
{
    using Keys;
    [RequireComponent(typeof(FadeInFadeOutCanvas))]
    public class CanvasInputAndIDConfiguration : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private EPlayerInput input;

        [SerializeField]
        private InputDataConfiguration inputsToOpenOrCloseCanvas;

        [SerializeField]
        private bool showOnlyOnDebugMode = false;
        #endregion

        #region Properties
        public EPlayerInput Input
        {
            get { return input; }
            private set { input = value; }
        }

        public InputDataConfiguration InputsToOpenOrCloseCanvas
        {
            get { return inputsToOpenOrCloseCanvas; }
            private set { inputsToOpenOrCloseCanvas = value; }
        }

        public bool ShowOnlyOnDebugMode
        {
            get { return showOnlyOnDebugMode; }
            private set { showOnlyOnDebugMode = value; }
        }
        #endregion
    }
}