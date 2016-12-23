using UnityEngine;

namespace BloodOfEvil.Player.Services.Canvases
{
    using Helpers;
    using Utilities;

    [RequireComponent(typeof(CanvasGroup))]
    public class FadeInFadeOutCanvas : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float timeToFade;
        [SerializeField]
        private EFadeMode mode;
        private Timer timer;
        private CanvasGroup canvasGroup;

        private const float MIN_IN_ALPHA_TO_STOP = 0.0f;
        private const float MAX_IN_ALPHA_TO_STOP = 0.001f;

        private const float MIN_OUT_ALPHA_TO_STOP = 0.999f;
        private const float MAX_OUT_ALPHA_TO_STOP = 1.0f;

        private const float MIN_ALPHA_TO_ENABLE_INTERACTIBILITY = 0.95f;
        private const float MAX_ALPHA_TO_ENABLE_INTERACTIBILITY = 1.0f;

        private const float MIN_ALPHA_TO_ENABLE_OPEN = 0.01f;
        private const float MAX_ALPHA_TO_ENABLE_OPEN = 1.0f;

        private const float START_MIN_IN_ALPHA = 0.002f;
        private const float START_MIN_OUT_ALPHA = 0.998f;
        #endregion

        #region Properties
        public EFadeMode Mode
        {
            get { return mode; }
            private set { mode = value; }
        }
        #endregion

        #region Unity Method
        void Update()
        {
            this.timer.Update();

            if (MathHelper.IsBetweenOrEqual(this.canvasGroup.alpha, MIN_IN_ALPHA_TO_STOP, MAX_IN_ALPHA_TO_STOP) ||
                MathHelper.IsBetweenOrEqual(this.canvasGroup.alpha, MIN_OUT_ALPHA_TO_STOP, MAX_OUT_ALPHA_TO_STOP))
                this.Stop();

            if (EFadeMode.In == this.mode) // Open
            {
                this.canvasGroup.alpha = this.FadeInCalcul();

                this.UpdateInteractibility();

            }
            else if (EFadeMode.Out == this.mode) // Close
            {
                this.canvasGroup.alpha = this.FadeOutCalcul();

                this.UpdateInteractibility();
            }
            //Debug.Log(this.FadeInCalcul());
        }
        #endregion

        #region Behaviour Methods
        public void Initalize()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();

            this.timer = new Timer(this.timeToFade, false);

            if (EFadeMode.In == this.mode)
                this.FadeIn();
            else if (EFadeMode.Out == this.mode)
                this.FadeOut();
        }

        public bool FadeIn()
        {
            bool canFadeInFadeOut = this.CanFadeInFadeOut();

            if (canFadeInFadeOut)
            {
                this.canvasGroup.alpha = START_MIN_OUT_ALPHA;

                this.timer.Reset();

                this.mode = EFadeMode.In;
            }

            return canFadeInFadeOut;
        }

        public bool FadeOut()
        {
            bool canFadeInFadeOut = this.CanFadeInFadeOut();

            if (canFadeInFadeOut)
            {
                this.canvasGroup.alpha = START_MIN_IN_ALPHA;

                this.timer.Reset();

                this.mode = EFadeMode.Out;
            }

            return canFadeInFadeOut;
        }

        public bool CanFadeInFadeOut()
        {
            return this.mode == EFadeMode.None;
        }

        public void Stop()
        {
            this.mode = EFadeMode.None;
        }

        private float FadeInCalcul()
        {
            return this.timer.GetTimeToWait() / this.timer.Alarm;
        }

        private float FadeOutCalcul()
        {
            return 1.0f - this.FadeInCalcul();
        }

        public bool IsInteractable()
        {
            return MathHelper.IsBetweenOrEqual(this.canvasGroup.alpha, MIN_ALPHA_TO_ENABLE_INTERACTIBILITY, MAX_ALPHA_TO_ENABLE_INTERACTIBILITY);
        }

        public void UpdateInteractibility()
        {
            this.canvasGroup.blocksRaycasts = this.canvasGroup.interactable = this.IsInteractable();
        }

        public bool IsOpen()
        {
            return MathHelper.IsBetweenOrEqual(this.canvasGroup.alpha, MIN_ALPHA_TO_ENABLE_OPEN, MAX_ALPHA_TO_ENABLE_OPEN) ||
                    EFadeMode.Out == this.Mode;
        }
        #endregion
    }
}