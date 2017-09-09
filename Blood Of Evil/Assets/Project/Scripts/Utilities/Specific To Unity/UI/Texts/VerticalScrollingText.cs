using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities
{
    using Extensions;
    
    public class VerticalScrollingText : MonoBehaviour
    {
        private RectTransform rectTransform;
        [SerializeField]
        private float scrolingSpeed = 50.0f;
        [SerializeField]
        private float idleTimer = 1.5f;
        private float idleTime = 0.0f;

        private void Awake()
        {
            this.rectTransform = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            this.idleTime += Time.deltaTime;

            if (this.idleTime >= this.idleTimer)
                this.Scroll();
        }

        public void Reset()
        {
            this.idleTime = 0.0f;

            this.rectTransform.SetLocalPositionY(0.0f);
        }

        private void Scroll()
        {
            this.rectTransform.SetLocalPositionY(Time.deltaTime * this.scrolingSpeed);

            if (this.rectTransform.localPosition.y > Mathf.Abs(this.rectTransform.GetHeight()))
                this.Reset();
        }
    }
}
