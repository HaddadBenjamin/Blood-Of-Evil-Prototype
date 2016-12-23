using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    using BloodOfEvil.Entities.Modules;

    // Noms d'attributs recommandé pour le dictionnaire d'attributs : Characteristic, Success, Shrine, Custom Name.
    public sealed class EntityAttributes
    {
        #region Fields
        private EntityAttributes percent = null;

        private Attribute atStart;
        private Attribute current;

        // Ne peut pas être un : Dictionary<INT, Attribute> car on ne peut pas reconvertir un hash ID vers une string.
        private Dictionary<string, Attribute> otherAttributes;

        private bool addDefaultCallbacks = false;

        const float PERCENT_TO_UNIT = 0.01f;
        const float DEFAULT_PERCENTAGE_AT_START = 100.0f;
        #endregion

        #region Properties
        public Attribute AtStart
        {
            get { return atStart; }
            private set { atStart = value; }
        }

        public Attribute Current
        {
            get { return current; }
            private set { current = value; }
        }

        public EntityAttributes Percent
        {
            get { return percent; }
            private set
            {
                percent = value;

                if (null != percent && this.AddDefaultCallbacks)
                    percent.Current.ValueListener(this.GetDefaultUpdateCallbacks);
            }
        }

        /// <summary>
        /// Cette property est juste présente pour pouvoir sérializer correctement cette clase.
        /// </summary>
        public bool AddDefaultCallbacks
        {
            get
            {
                return addDefaultCallbacks;
            }

            set
            {
                addDefaultCallbacks = value;
            }
        }

        /// <summary>
        /// Cette property est juste présente pour pouvoir sérializer correctement cette clase.
        /// </summary>
        public Dictionary<string, Attribute> OtherAttributes
        {
            get
            {
                return otherAttributes;
            }

            private set
            {
                otherAttributes = value;
            }
        }
        #endregion

        #region Constructor
        public EntityAttributes()
        {
            this.OtherAttributes = new Dictionary<string, Attribute>();

            this.Current = new Attribute();
            this.AtStart = new Attribute();
        }
        #endregion

        #region Initialization
        public void Initialize(string attributeName, float atStart = 0.0f, EntityAttributes percent = null, bool addDefaultCallbacks = true)
        {
            this.AddDefaultCallbacks = addDefaultCallbacks; // doit être avant percent pour qu'il puisse appeler son setteur.
            this.Percent = percent;

            this.Initialize(attributeName, atStart, addDefaultCallbacks);
        }

        public void InitializeDefaultPercentage(string attributeName, bool addDefaultCallbacks = true)
        {
            this.AddDefaultCallbacks = addDefaultCallbacks;

            this.Initialize(attributeName, DEFAULT_PERCENTAGE_AT_START, addDefaultCallbacks);
        }

        private void Initialize(string attributeName, float atStart, bool addDefaultCallbacks)
        {
            this.AddDefaultCallbacks = addDefaultCallbacks;

            if (addDefaultCallbacks)
                this.AddUpdateCallbacks(this.GetDefaultUpdateCallbacks);

            this.Current.Initialize(0.0f, attributeName);
            this.AtStart.Initialize(atStart, string.Format("{0} At Initialization", attributeName));
        }
        #endregion

        #region Public Behaviour
        public void AddUpdateCallbacks(Attribute.DelegateWithFloatParameter updateCallbacks)
        {
            this.AtStart.ValueListener(updateCallbacks);
        }

        public void AddUpdateCallbacksThenUpdate(Attribute.DelegateWithFloatParameter updateCallbacks)
        {
            this.AddUpdateCallbacks(updateCallbacks);

            this.AtStart.CallOnValueModified();

            foreach (KeyValuePair<string, Attribute> otherAttribute in this.OtherAttributes)
                otherAttribute.Value.CallOnValueModified();
        }

        public void GetDefaultUpdateCallbacks(float value)
        {
            this.Current.Value = this.GetDefaultUpdateValueWithPercent();
        }

        public float GetDefaultUpdateValueWithPercent()
        {
            return this.GetDefaultUpdateValueWithoutPercent() * this.GetPercentValue();
        }

        public float GetDefaultUpdateValueWithoutPercent()
        {
            return this.AtStart.Value + this.GetTotalValueOfAllOtherAttributes();
        }

        public float GetPercentValue()
        {
            return null == this.Percent ?
                1.0f :
                this.Percent.Current.Value * PERCENT_TO_UNIT;
        }

        public Attribute GetOtherAttribute(string attributeName)
        {
            return this.OtherAttributes[attributeName];
        }

        // C'est sale.
        public Dictionary<string, Attribute> GetOtherAttributes()
        {
            return this.OtherAttributes;
        }

        //public float GetOtherAttributeValue(string attributeName)
        //{
        //    return this.otherAttributes[attributeName].Value;
        //}

        public Attribute SetOtherAttribute(string attributeName, float value)
        {
            Attribute otherAttribute = null;

            if (this.OtherAttributes.ContainsKey(attributeName))
            {
                otherAttribute = this.OtherAttributes[attributeName];

                otherAttribute.Value = value;
            }
            else
            {
                otherAttribute = new Attribute(value, attributeName);

                this.OtherAttributes.Add(attributeName, otherAttribute);

                if (this.AddDefaultCallbacks)
                    otherAttribute.ValueListener(this.GetDefaultUpdateCallbacks);
            }

            return otherAttribute;
        }

        // C'est très moche d'avoir fait ce copié collé, mais pour le moment ce n'est pas important.
        public Attribute SetOtherAttributePlusEqual(string attributeName, float value)
        {
            Attribute otherAttribute = null;

            if (this.OtherAttributes.ContainsKey(attributeName))
            {
                otherAttribute = this.OtherAttributes[attributeName];

                otherAttribute.Value += value;
            }
            else
            {
                otherAttribute = new Attribute(value, attributeName);

                this.OtherAttributes.Add(attributeName, otherAttribute);

                if (this.AddDefaultCallbacks)
                    otherAttribute.ValueListener(this.GetDefaultUpdateCallbacks);
            }

            return otherAttribute;
        }

        public Attribute SetOtherAttributeLessEqual(string attributeName, float value)
        {
            Attribute otherAttribute = null;

            if (this.OtherAttributes.ContainsKey(attributeName))
            {
                otherAttribute = this.OtherAttributes[attributeName];

                otherAttribute.Value -= value;
            }
            else
            {
                otherAttribute = new Attribute(-value, attributeName);

                this.OtherAttributes.Add(attributeName, otherAttribute);

                if (this.AddDefaultCallbacks)
                    otherAttribute.ValueListener(this.GetDefaultUpdateCallbacks);
            }

            return otherAttribute;
        }

        public void ClearOtherAttributes()
        {
            this.otherAttributes.Clear();
        }
        #endregion

        #region Intern Behaviour
        private float GetTotalValueOfAllOtherAttributes()
        {
            return this.OtherAttributes.Sum(attribute => attribute.Value.Value);
        }
        #endregion
    }
}