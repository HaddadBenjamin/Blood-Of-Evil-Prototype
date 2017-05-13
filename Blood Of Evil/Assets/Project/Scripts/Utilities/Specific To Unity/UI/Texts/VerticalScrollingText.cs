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

        private void Awake()
        {
            this.rectTransform = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            this.Scroll();
        }

        public void Reset()
        {
            this.rectTransform.SetLocalPositionY(0.0f);
        }

        private void Scroll()
        {
            this.rectTransform.SetLocalPositionY(Time.deltaTime * this.scrolingSpeed, true);
        }
    }
}
