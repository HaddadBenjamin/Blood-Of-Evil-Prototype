namespace BloodOfEvil.Entities.Modules.Attributes.Serialization
{
    using Entities.Modules.Attributes;

    //Attribute to Attribute Serializable & inverse
    [System.Serializable]
    public sealed class AttributeSerializable
    {
        #region Fields
        public string Title;
        public float Value;
        #endregion

        #region Save & Load Behaviour.
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="attribute"></param>
        public AttributeSerializable(Attribute attribute)
        {
            this.Title = attribute.Title;
            this.Value = attribute.Value;
        }

        public void Load(Attribute attribute)
        {
            attribute.Initialize(this.Title);
            attribute.Value = this.Value;
        }
        #endregion
    }
}