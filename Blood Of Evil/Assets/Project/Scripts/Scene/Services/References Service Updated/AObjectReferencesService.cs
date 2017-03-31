using System;
using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Utilities;
using UnityEngine;

namespace BloodOfEvil.Scene.Services
{
    /// <summary>
    /// Permet de stocker un certain type de resources et y donne accès facilement et rapidement.
    /// </summary>
    [System.Serializable]
    public abstract class AObjectReferencesService<TObjectType, TSingletonType> : ASingletonMonoBehaviour<TSingletonType>
                    where TObjectType : UnityEngine.Object
                    where TSingletonType : ASingletonMonoBehaviour<TSingletonType>
    {
        #region Fields
        [SerializeField, Tooltip("Ce sont les références vers vos objets.")]
        protected TObjectType[] references;
        /// <summary>
        /// Il s'agit des identifiants de hashs des références, passer par ces références est une optimisation.
        /// Car au lieu de comparer des chaînes de caractères, je compare des entiers.
        /// </summary>
        private int[] referencesIds;
        #endregion

        #region Public Behaviour
        /// <summary>
        /// Permet de récupérer une resource par son nom.
        /// </summary>
        public TObjectType Get(string name)
        {
            int nameID = name.GetHashCode();

            return this.references[Array.FindIndex(this.referencesIds, referenceID  => referenceID == nameID)];
        }
        #endregion

        #region Override Behaviour
        public override void InitializeSingletons()
        {
            this.referencesIds = Array.ConvertAll<TObjectType, int>(this.references, reference => reference.name.GetHashCode());
        }
        #endregion
    }
}
