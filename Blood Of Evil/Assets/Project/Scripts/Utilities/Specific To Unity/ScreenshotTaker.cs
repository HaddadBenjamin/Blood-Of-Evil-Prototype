using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities
{
    public class ScreenshotTaker : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private KeyCode screenshotKey;
        [SerializeField]
        private string screenshotPath;
        #endregion

        void Update()
        {
            if (Input.GetKeyDown(this.screenshotKey))
                Application.CaptureScreenshot(string.Format("{0}Screenshot_{1}.png", screenshotPath, Random.Range(0, 100000)));
        }
    }
}