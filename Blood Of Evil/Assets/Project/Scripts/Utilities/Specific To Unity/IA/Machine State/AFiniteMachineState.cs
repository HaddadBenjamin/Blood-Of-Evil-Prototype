using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BloodOfEvil.Utilities
{
    using Helpers;
    
    /// <summary>
    /// Permet de créer une machine à états.
    /// </summary>
    public abstract class AFiniteMachineState<TMachineStateChildType, TStateEnumeration> : MonoBehaviour
            where TStateEnumeration : struct, IConvertible
            where TMachineStateChildType : AFiniteMachineState<TMachineStateChildType, TStateEnumeration>
    {
        #region Fields
        /// <summary>
        /// Ce sont tous les états de notre machine à états.
        /// </summary>
        protected AState<TMachineStateChildType, TStateEnumeration>[] states;
        /// <summary>
        /// C'est l'état courant de notre machine à état.
        /// </summary>
        private AState<TMachineStateChildType, TStateEnumeration> currentState;
        #endregion

        #region Unity Behaviours
        protected void Awake()
        {
            this.CreateStates();

            this.currentState = this.GetState(this.GetDefaultStateID());

            if (null == this.currentState)
            {
                Debug.LogFormat("Votre machine à état de type [<b><color=red>{0}</color></b>] ne possède pas l'état par défault que vous lui avez défini.",
                    this.GetType().BaseType.Name);
            }
        }

        /// <summary>
        /// Appel la mise à jour de la machine à état et appelle les méthodes de l'état courant et précédent : Enter, Stay, Quit.
        /// </summary>
        protected void Update()
        {
            int oldStateIndex = EnumerationHelper.GetIndex(this.currentState.StateID);
            TStateEnumeration newStateIndex =  this.GetUpdatedStateID();

            if (oldStateIndex != EnumerationHelper.GetIndex(newStateIndex))
            {
                this.states[oldStateIndex].QuitState();

                this.currentState = this.GetState(newStateIndex);

                this.currentState.EnterInState();
            }
            else
                this.currentState.StayInState();
        }
        #endregion

        #region Abstract Behaviour
        /// <summary>
        /// Cette méthode permet de créer les différents états de la machine à états.
        /// </summary>
        protected abstract void CreateStates();

        /// <summary>
        /// Permet de récupérer l'identifiant de l'état par défault.
        /// </summary>
        protected abstract TStateEnumeration GetDefaultStateID();

        /// <summary>
        /// C'est la méthode qui met à jour l'état courant et permet de le récupérer.
        /// </summary>
        protected abstract TStateEnumeration GetUpdatedStateID();
        #endregion

        #region Intern Behaviours
        /// <summary>
        /// Permet de récupérer l'état correspondant à un identifiant passé en paramètre.
        /// </summary>
        protected AState<TMachineStateChildType, TStateEnumeration> GetState(TStateEnumeration stateID)
        {
            return Array.Find(this.states, state => state.StateID.Equals(stateID));
        }
        #endregion

        #region Public Behaviour
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TStateEnumeration GetCurrentStateID()
        {
            return this.currentState.StateID;
        }
        #endregion
    }
}
