using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Scene.Services.References
{
    using Helpers;
    using ObjectInScene;

    public abstract class AReferenceContainer<ReferenceClass> : AInitializableComponent where ReferenceClass : UnityEngine.Object
    {
        [SerializeField]
        protected ReferenceClass[] references;
        protected int[] hashIds;

        public override void Initialize()
        {
            ObjectContainerHelper.InitializeHashIds(
                Array.ConvertAll(this.references, reference => reference.name),
                ref this.hashIds);
        }

        public ReferenceClass Get(string referenceName)
        {
            return this.references[ObjectContainerHelper.GetHashCodeIndex(referenceName, hashIds)];
        }

        public int GetHashId(string referenceName)
        {
            return ObjectContainerHelper.GetHashCodeIndex(referenceName, hashIds);
        }
    }
}