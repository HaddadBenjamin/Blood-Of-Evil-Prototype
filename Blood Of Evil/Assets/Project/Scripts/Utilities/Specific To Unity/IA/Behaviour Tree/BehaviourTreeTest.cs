//                                          Root
//                                        SelectorA
////                                     /         \
//                              SequenceA           task
//                             /         \
//                    SelectorB           SelectorC
//                    Decorator           Decorator
//                        |                   |
//                       task                task

using System;
using UnityEngine;

namespace BloodOfEvil.Utilities.IA
{
    #region Tests classes
    public sealed class BehaviourTreeTest : MonoBehaviour
    {
        private BehaviourTreeHolder<CustomBlackboard> behaviourTree;

        void Start()
        {
            #region Creation de tous les différents noeuds et du blackboard
            TaskPrintSelectorA<CustomBlackboard> taskSelectorA = new TaskPrintSelectorA<CustomBlackboard>();
            TaskPrintSelectorB<CustomBlackboard> taskSelectorB = new TaskPrintSelectorB<CustomBlackboard>();
            TaskPrintSelectorC<CustomBlackboard> taskSelectorC = new TaskPrintSelectorC<CustomBlackboard>();

            BehaviourNodeSelector<CustomBlackboard> selectorA = new BehaviourNodeSelector<CustomBlackboard>();
            BehaviourNodeSelector<CustomBlackboard> selectorB = new BehaviourNodeSelector<CustomBlackboard>();
            BehaviourNodeSelector<CustomBlackboard> selectorC = new BehaviourNodeSelector<CustomBlackboard>();

            BehaviourNodeSequence<CustomBlackboard> sequenceA = new BehaviourNodeSequence<CustomBlackboard>();

            DecoratorSelectorB<CustomBlackboard> decoratorSelectorB = new DecoratorSelectorB<CustomBlackboard>();
            DecoratorSelectorC<CustomBlackboard> decoratorSelectorC = new DecoratorSelectorC<CustomBlackboard>();

            CustomBlackboard blackboard = new CustomBlackboard();
            #endregion

            #region Liaison des noeuds avec les autres
            selectorA.Nodes = new ABehaviourNode<CustomBlackboard>[]
            {
            sequenceA,
            taskSelectorA,
            };

            sequenceA.Nodes = new ABehaviourNode<CustomBlackboard>[]
            {
            selectorB,
            selectorC,
            };

            selectorB.Nodes = new ABehaviourNode<CustomBlackboard>[]
            {
            taskSelectorB
            };

            selectorC.Nodes = new ABehaviourNode<CustomBlackboard>[]
            {
            taskSelectorC
            };
            #endregion

            #region Liaison des decorators avec les noeuds
            selectorB.Decorator = decoratorSelectorB;
            selectorC.Decorator = decoratorSelectorC;
            #endregion

            behaviourTree = new BehaviourTreeHolder<CustomBlackboard>(selectorA, blackboard);
        }

        void Update()
        {
            behaviourTree.Execute();
            Debug.Log("END");
        }
    }

    #region Tasks
    public sealed class TaskPrintSelectorA<TBlackboard> : ABehaviourNodeTask<TBlackboard> where TBlackboard : ABlackboard
    {
        public override EBehaviourExecuteType Execute()
        {
            Debug.Log("Selector A");
            return EBehaviourExecuteType.Success;
        }
    }

    public sealed class TaskPrintSelectorB<TBlackboard> : ABehaviourNodeTask<TBlackboard> where TBlackboard : ABlackboard
    {
        public override EBehaviourExecuteType Execute()
        {
            Debug.Log("Selector B");
            return EBehaviourExecuteType.Success;
        }
    }

    public sealed class TaskPrintSelectorC<TBlackboard> : ABehaviourNodeTask<TBlackboard> where TBlackboard : ABlackboard
    {
        public override EBehaviourExecuteType Execute()
        {
            Debug.Log("Selector C");
            return EBehaviourExecuteType.Success;
        }
    }
    #endregion

    #region Decorator
    public sealed class DecoratorSelectorB<TBlackboard> : ABehaviourNodeDecorator<TBlackboard> where TBlackboard : ABlackboard
    {
        public override bool CanPerformTask()
        {
            return false;
        }
    }

    public sealed class DecoratorSelectorC<TBlackboard> : ABehaviourNodeDecorator<TBlackboard> where TBlackboard : ABlackboard
    {
        public override bool CanPerformTask()
        {
            return true;
        }
    }
    #endregion

    public sealed class CustomBlackboard : ABlackboard
    {
        public override void Initialize()
        {
        }

        /// <summary>
        /// On devra mettre à jour ici notre classe de data pour quelle soit cohérente avec le temps qui passe et notre gameplay.
        /// </summary>
        public override void UpdateBlackboardAtEachFrame()
        {

        }
    }
    #endregion
}
//voir si on peut récupérer le blackboard dans un decorateur snas que ça segfualt.