using System.Collections.Generic;

namespace BloodOfEvil.Entities.Modules.Attributes.Serialization
{
    [System.Serializable]
    public sealed class OtherAttributeKeyValueSerializable
    {
        #region Fields
        public string Key;
        public AttributeSerializable Value;
        #endregion

        #region Save & Load Behaviour.
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="otherAttributeKeyValuePair"></param>
        public OtherAttributeKeyValueSerializable(KeyValuePair<string, Attribute> otherAttributeKeyValuePair)
        {
            this.Key = otherAttributeKeyValuePair.Key;
            this.Value = new AttributeSerializable(otherAttributeKeyValuePair.Value);
        }

        public KeyValuePair<string, Attribute> Load(Attribute referenceAttribute)
        {
            this.Value.Load(referenceAttribute);

            return new KeyValuePair<string, Attribute>(this.Key, referenceAttribute);
        }
        #endregion
    }
}