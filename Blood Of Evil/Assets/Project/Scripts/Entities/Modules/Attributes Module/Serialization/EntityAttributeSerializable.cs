using System.Collections.Generic;

namespace BloodOfEvil.Entities.Modules.Attributes.Serialization
{
    [System.Serializable]
    public sealed class EntityAttributeSerializable
    {
        #region Fields
        public AttributeSerializable AtStart;
        public AttributeSerializable Current;
        public bool AddDefaultCallbacks;
        public List<OtherAttributeKeyValueSerializable> OtherAttributes;
        #endregion

        #region Save & Load Behaviour.
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="entityAttribute"></param>
        public EntityAttributeSerializable(EntityAttributes entityAttribute)
        {
            this.AtStart = new AttributeSerializable(entityAttribute.AtStart);
            this.Current = new AttributeSerializable(entityAttribute.Current);
            this.AddDefaultCallbacks = entityAttribute.AddDefaultCallbacks;
            this.OtherAttributes = new List<OtherAttributeKeyValueSerializable>();

            foreach (KeyValuePair<string, Attribute> otherAttribute in entityAttribute.OtherAttributes)
                this.OtherAttributes.Add(new OtherAttributeKeyValueSerializable(otherAttribute));
        }

        public void Load(EntityAttributes entityAttribute)
        {
            entityAttribute.AtStart.Initialize(this.AtStart.Value, this.AtStart.Title);
            entityAttribute.AddDefaultCallbacks = AddDefaultCallbacks;

            foreach (var otherAttribute in this.OtherAttributes)
                entityAttribute.SetOtherAttribute(otherAttribute.Key, otherAttribute.Value.Value);

            entityAttribute.Current.Initialize(this.Current.Value, this.Current.Title);
        }
        #endregion
    }
}