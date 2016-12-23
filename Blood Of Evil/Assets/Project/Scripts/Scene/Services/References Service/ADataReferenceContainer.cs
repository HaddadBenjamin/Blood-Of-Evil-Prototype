using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Scene.Services.References
{
    using Helpers;

    [System.Serializable]
    public abstract class ADataReferenceContainer<TReference> where TReference : UnityEngine.Object
    {
        #region Fields
        [SerializeField]
        protected TReference[] references;
        protected int[] hashIds;
        #endregion

        #region Public Behaviour
        public void Initialize()
        {
            ObjectContainerHelper.InitializeHashIds(
                Array.ConvertAll(this.references, reference => reference.name),
                ref this.hashIds);
        }

        public TReference Get(string refenceName)
        {
            return this.references[ObjectContainerHelper.GetHashCodeIndex(refenceName, this.hashIds)];
        }
        #endregion
    }
}