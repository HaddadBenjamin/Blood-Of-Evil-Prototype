using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities.Optimization
{
    public class GarbageCollectorCaller : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private bool callGarbageCollectorAtRegularInterval = false;
        [SerializeField]
        private float callGBTimer = 10.0f;
        private float callGBElapsedTime;
        #endregion

        #region Unity Behaviour
        private void Update()
        {
            if (this.callGarbageCollectorAtRegularInterval)
            {
                this.callGBElapsedTime += Time.deltaTime;

                if (this.callGBElapsedTime > this.callGBTimer)
                {
                    this.callGBElapsedTime = 0.0f;

                    this.CallGarbageCollector();
                }
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            this.CallGarbageCollector();
        }


        void OnApplicationPause(bool pauseStatus)
        {
            this.CallGarbageCollector();
        }
        #endregion

        #region Intern Behaviour
        /// <summary>
        /// Appele le garbage collector.
        /// Ceci permet de vider explicitement le références valant null du tas, celles qui sont donc gérées par le Garbage Collector.
        /// </summary>
        private void CallGarbageCollector()
        {
            System.GC.Collect();
        }
        #endregion
    }
}