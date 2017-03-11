using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Utilities
{
    [Serializable]
    public class Intervale
    {
        #region Fields
        private float minimal;
        private float maximal;
        #endregion

        #region Properties
        public float Minimal
        {
            get { return minimal; }
            set { minimal = value; }
        }
        public float Maximal
        {
            get { return maximal; }
            set { maximal = value; }
        }
        #endregion

        #region Builder
        public Intervale() { }

        public Intervale(float minimal, float maximal)
        {
            this.Initialize(minimal, maximal);
        }
        #endregion

        #region Public Behaviour
        public void Initialize(float minimal, float maximal)
        {
            this.minimal = minimal;
            this.maximal = maximal;
        }

        public float Ratio()
        {
            return this.minimal / this.maximal;
        }

        public float Percent()
        {
            return this.Ratio() * 100.0f;
        }

        public float RandomBetweenValues()
        {
            return UnityEngine.Random.Range(this.minimal, this.maximal);
        }
        #endregion
    }
}