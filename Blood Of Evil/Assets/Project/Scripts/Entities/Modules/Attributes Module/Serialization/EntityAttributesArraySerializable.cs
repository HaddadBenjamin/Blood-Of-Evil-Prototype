namespace BloodOfEvil.Entities.Modules.Attributes.Serialization
{
    [System.Serializable]
    public sealed class EntityAttributesArraySerializable
    {
        #region Fields
        public EntityAttributeSerializable[] EntityAttributeArray;
        #endregion

        #region Save & Load.
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="entityAttributes"></param>
        public EntityAttributesArraySerializable(EntityAttributes[] entityAttributes)
        {
            this.EntityAttributeArray = new EntityAttributeSerializable[entityAttributes.Length];

            for (int entityAttributesIndex = 0; entityAttributesIndex < entityAttributes.Length; entityAttributesIndex++)
                this.EntityAttributeArray[entityAttributesIndex] = new EntityAttributeSerializable(entityAttributes[entityAttributesIndex]);
        }

        public void Load(EntityAttributes[] entityAttributes)
        {
            for (int entityAttributesIndex = 0; entityAttributesIndex < entityAttributes.Length; entityAttributesIndex++)
                this.EntityAttributeArray[entityAttributesIndex].Load(entityAttributes[entityAttributesIndex]);
        }
        #endregion
    }
}