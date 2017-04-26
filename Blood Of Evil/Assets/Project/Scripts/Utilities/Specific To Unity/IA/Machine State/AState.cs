using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace BloodOfEvil.Utilities
 {
    /// <summary>
    /// Chaque état d'une machine à états devra hériter de cette classe.
    /// </summary>
    public  abstract class AState<TMachineState, TStateEnumeration>
            where TMachineState : AFiniteMachineState<TMachineState, TStateEnumeration>
            where TStateEnumeration : struct, IConvertible
    {
        #region Fields
        /// <summary>
        /// C'est la machine à états, on en aura besoin pour accéder et mettre à jour certain de ses paramètres.
        /// </summary>
        protected TMachineState machineState;
        /// <summary>
        /// C'est l'identifiant de l'état.
        /// </summary>
        private TStateEnumeration stateID;
        #endregion

        #region properties
        public TStateEnumeration StateID
        {
            get
            {
                return stateID;
            }
        }
        #endregion

        #region Constructor
        public AState(TMachineState machineState, TStateEnumeration stateEnumeration)
        {
            this.machineState = machineState;
            this.stateID = stateEnumeration;
        }
        #endregion

        #region Virtual Behaviours
        /// <summary>
        /// Cette méthode appelé lorsque l'on vient de rentrer dans cet état.
        /// </summary>
        public virtual void EnterInState() { }

        /// <summary>
        /// Cette méthode appelé lorsque l'on reste dans cet état.
        /// </summary>
        public virtual void StayInState() { }

        /// <summary>
        /// Cette méthode appelé lorsque l'on vient de quitter cet état.
        /// </summary>
        public virtual void QuitState() { }
        #endregion
    }
}
