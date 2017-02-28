namespace BloodOfEvil.Entities.Modules.Attributes.Serialization
{
    [System.Serializable]
    public sealed class EntityAttributesArrayOfArraySerializable
    {
        #region Fields
        public EntityAttributesArraySerializable[] EntityAttributesArrayOfArray;
        #endregion

        #region Constructor
        public EntityAttributesArrayOfArraySerializable()
        {
            
        }
        #endregion

        #region
        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="entityAttributesArrayOfArray"></param>
        public EntityAttributesArrayOfArraySerializable(EntityAttributes[][] entityAttributesArrayOfArray)
        {
            this.EntityAttributesArrayOfArray = new EntityAttributesArraySerializable[entityAttributesArrayOfArray.Length];

            for (int entityAttributesArrayOfArrayIndex = 0; entityAttributesArrayOfArrayIndex < entityAttributesArrayOfArray.Length; entityAttributesArrayOfArrayIndex++)
                this.EntityAttributesArrayOfArray[entityAttributesArrayOfArrayIndex] = new EntityAttributesArraySerializable(entityAttributesArrayOfArray[entityAttributesArrayOfArrayIndex]);
        }

        public void Load(EntityAttributes[][] entityAttributesArrayOfArray)
        {
            for (int entityAttributesArrayOfArrayIndex = 0; entityAttributesArrayOfArrayIndex < EntityAttributesArrayOfArray.Length; entityAttributesArrayOfArrayIndex++)
                this.EntityAttributesArrayOfArray[entityAttributesArrayOfArrayIndex].Load(entityAttributesArrayOfArray[entityAttributesArrayOfArrayIndex]);
        }
        #endregion
    }
}