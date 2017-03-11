using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Utilities
{
    [System.Serializable]
    public sealed class Timer
    {
        #region Fields
        private float alarm;
        private float elapsedTime;
        #endregion

        #region Constructor
        public Timer(float timer, bool alarmisRinging = true)
        {
            this.alarm = timer;
            this.elapsedTime = alarmisRinging ? this.alarm : 0.0f;
        }
        #endregion

        #region Properties
        public float Alarm
        {
            get { return alarm; }
            private set { alarm = value; }
        }

        public float ElapsedTime
        {
            get { return elapsedTime; }
            private set { elapsedTime = value; }
        }
        #endregion

        #region Behaviour Methods
        public bool IsRingingUpdated()
        {
            this.Update();

            bool alarmIsRinging = this.IsRinging();

            if (alarmIsRinging)
                this.Reset();

            return alarmIsRinging;
        }

        public bool IsRinging()
        {
            return this.elapsedTime >= this.alarm;
        }

        public void Update()
        {
            this.elapsedTime += Time.deltaTime;
        }

        public void Reset()
        {
            this.elapsedTime = 0.0f;
        }

        public float Ratio()
        {
            return this.elapsedTime / this.alarm;
        }

        public float GetTimeToWait()
        {
            return this.alarm - this.elapsedTime;
        }

        public void DecreaseElapsedTime(float elaspedTimeAdd)
        {
            this.elapsedTime -= elaspedTimeAdd;

            if (this.elapsedTime < 0.0f)
                this.elapsedTime = 0.0f;
        }
        #endregion
    }
}