using UnityEngine;
using System.Collections;
using System;

//public class EnemyBehaviourTreeModule : AInitializableComponent, IDataUpdatable
//{
//    #region Fields
//    private BehaviourTreeHolder<EnemyBlackboardModule> behaviourTree;
//    #endregion

//    #region Override Behaviour
//    public override void Initialize()
//    {
//        EnemyAttackPlayerTask taskAttackPlayer = new EnemyAttackPlayerTask();
//        EnemyGoToWanderPointTask taskGoToWanderPoint = new EnemyGoToWanderPointTask();
//        EnemyFindWanderPointTask taskFindAWanderPoint = new EnemyFindWanderPointTask();
//        EnemyGoToTargetTransformTask taskGoToTargetTransform = new EnemyGoToTargetTransformTask();
//        EnemyGoToInitialPositionWithoutInterruptionTask taskGoToInitialPositionWithoutInteruption = new EnemyGoToInitialPositionWithoutInterruptionTask();

//        BehaviourNodeSelector<EnemyBlackboardModule> mainSelector = new BehaviourNodeSelector<EnemyBlackboardModule>();

//        EnemyDistanceFromInitialPositionIsTooBigDecorator decoratorDistanceFromInitialPositionIsTooBig = new EnemyDistanceFromInitialPositionIsTooBigDecorator();
//        EnemyIsNotDeadDecorator decoratorIsNotDead = new EnemyIsNotDeadDecorator();
//        EnemyCanGoToPlayerDecorator decoratorcCanGoToPlayer = new EnemyCanGoToPlayerDecorator();
//        EnemyCanAttackPlayerDecorator decoratorCanAttackPlayer = new EnemyCanAttackPlayerDecorator();
//        EnemyCanGoToWanderPointDecorator decoratorCanGoToWanderPoint = new EnemyCanGoToWanderPointDecorator();
//        EnemyDoesWanderPointExistsDecorator decoratorDoesWanderPointExists = new EnemyDoesWanderPointExistsDecorator();

//        EnemyBlackboardModule blackboard = GetComponent<EnemyServicesAndModulesContainer>().Instance.EnemyBlackboardModule;

//        taskAttackPlayer.Decorator = decoratorCanAttackPlayer;
//        taskGoToWanderPoint.Decorator = decoratorCanGoToWanderPoint;
//        taskGoToTargetTransform.Decorator = decoratorcCanGoToPlayer;
//        taskFindAWanderPoint.Decorator = decoratorDoesWanderPointExists;
//        taskGoToInitialPositionWithoutInteruption.Decorator = decoratorDistanceFromInitialPositionIsTooBig;

//        mainSelector.Nodes = new ABehaviourNode<EnemyBlackboardModule>[]
//        {
//            taskGoToInitialPositionWithoutInteruption,
//            taskGoToTargetTransform,
//            taskAttackPlayer,
//            taskGoToWanderPoint,
//            taskFindAWanderPoint,
//        };
//        mainSelector.Decorator = decoratorIsNotDead;

//        behaviourTree = new BehaviourTreeHolder<EnemyBlackboardModule>(mainSelector, blackboard);
//    }
//    #endregion

//    #region Interfaces
//    void IDataUpdatable.Update()
//    {
//       behaviourTree.Execute();
//    }
//    #endregion
//}