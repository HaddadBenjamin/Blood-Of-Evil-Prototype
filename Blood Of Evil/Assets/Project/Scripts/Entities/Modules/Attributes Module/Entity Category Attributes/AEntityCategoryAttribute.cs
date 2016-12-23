using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Attributes
{
    [System.Serializable]
    public abstract class AEntityCategoryAttribute
    {
        #region Fields
        protected AEntityAttributesModule attributeModule;
        protected const float PERCENTAGE_TO_UNIT = 0.01f;
        protected const float DEFAULT_PERCENT_AT_START = 100.0f;
        #endregion

        #region Constructor
        public AEntityCategoryAttribute(AEntityAttributesModule attributeModule)
        {
            this.attributeModule = attributeModule;
        }
        #endregion

        #region Virtual Behaviour
        public abstract void InitialzeAttributes();
        public abstract void CreateCallbacksAttributes();
        public virtual void Update() { }
        #endregion

        #region Public behaviour
        /// <summary>
        /// Permet de réduire la quantité de code écrite : au lieu d'écrire base.GetAttribute, un simple base.GetAttribute suffit.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public EntityAttributes GetAttribute(EEntityCategoriesAttributes category, string attributeID)
        {
            return this.attributeModule.GetAttribute(category, attributeID);
        }
        #endregion
    }
}