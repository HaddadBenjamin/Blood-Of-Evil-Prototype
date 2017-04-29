using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Voici un example d'une énumération représentant l'identifiant d'un état.
    /// </summary>
    public enum EExampleState
    {
        Idle,
        Wander,
    }

    /// <summary>
    /// Ce script montre comment on peut créer un état.
    /// </summary>
    public class ExampleIdleState : AState<ExampleFiniteMachineState, EExampleState>
    {
        public ExampleIdleState(
            ExampleFiniteMachineState machineState,
            EExampleState enumerationState) :
            base(machineState, enumerationState)
        {
        }
    }
    public class ExampleWanderState : AState<ExampleFiniteMachineState, EExampleState>
    {
        public ExampleWanderState(
            ExampleFiniteMachineState machineState,
            EExampleState enumerationState) :
            base(machineState, enumerationState)
        {
        }
    }

    public class ExampleFiniteMachineState : AFiniteMachineState<ExampleFiniteMachineState, EExampleState>
    {
        protected override EExampleState GetDefaultStateID()
        {
            return EExampleState.Idle;
        }

        protected override void CreateStates()
        {
            base.states = new AState<ExampleFiniteMachineState, EExampleState>[]
            {
                new ExampleIdleState(this, EExampleState.Idle),
                new ExampleWanderState(this, EExampleState.Wander)
            };
        }

        protected override EExampleState GetUpdatedStateID()
        {
            return base.GetCurrentStateID().Equals(EExampleState.Idle) ? EExampleState.Wander : EExampleState.Idle;
        }
    }
}
