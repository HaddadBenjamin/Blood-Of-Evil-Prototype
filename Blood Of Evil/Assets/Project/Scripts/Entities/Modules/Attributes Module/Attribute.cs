using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    [System.Serializable]
    public class Attribute
    {
        #region Fields
        [SerializeField]
        private float value;
        [SerializeField]
        private string title;

        [SerializeField]
        public delegate void DelegateWithFloatParameter(float value);
        [SerializeField]
        private DelegateWithFloatParameter OnValueModified;
        #endregion

        #region Attributes
        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;

                CallOnValueModified();
            }
        }

        public string Title
        {
            get { return title; }
            private set { title = value; }
        }
        #endregion

        #region Constructors
        public Attribute() { }
        public Attribute(float value)
        {
            this.Value = value;
        }

        public Attribute(float value, string title) : this(value)
        {
            this.Title = title;
        }
        #endregion

        #region Public Behaviour
        public void Initialize(float value)
        {
            this.Value = value;
        }

        public void Initialize(string title)
        {
            this.Title = title;
        }

        public void Initialize(float value, string title)
        {
            this.Initialize(value);
            this.Title = title;
        }

        public void ValueListener(DelegateWithFloatParameter callback)
        {
            this.OnValueModified += callback;
        }

        public float ValueListenerAndGetValue(DelegateWithFloatParameter callback)
        {
            this.OnValueModified += callback;

            return this.Value;
        }

        public void ValueUnlistener(DelegateWithFloatParameter callback)
        {
            this.OnValueModified -= callback;
        }

        public void CallOnValueModified()
        {
            if (null != OnValueModified)
                OnValueModified(this.value);
        }
        #endregion
    }
}