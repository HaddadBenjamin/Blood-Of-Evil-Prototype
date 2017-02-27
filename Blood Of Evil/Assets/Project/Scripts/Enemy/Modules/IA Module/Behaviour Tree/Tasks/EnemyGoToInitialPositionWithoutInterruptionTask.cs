using UnityEngine;
using System.Collections;

//public sealed class EnemyGoToInitialPositionWithoutInterruptionTask : ABehaviourNodeTask<EnemyBlackboardModule>
//{
//    public override EBehaviourExecuteType Execute()
//    {
//        SceneServicesContainer.Instance.SceneState.UnRegisterEnemy(base.Blackboard.MyTransform); // ne peut pas être attaqué.

//        //Debug.LogFormat("position : {0}, initial position : {1}", base.MyTransform.position, initialPosition.position);
//        bool isArrivedToDestination = this.GoTo(base.Blackboard.InitialPosition);

//        if (isArrivedToDestination)
//        {
//            base.Blackboard.ResetLife();

//            SceneServicesContainer.Instance.SceneState.RegisterEnemy(base.Blackboard.MyTransform);
//        }
//        // Wait.. si pas de target proche

//        return isArrivedToDestination ?
//                EBehaviourExecuteType.Success :
//                EBehaviourExecuteType.In_Progress;
//    }

//    private bool GoTo(Transform transformToGo)
//    {
//        if (null != transformToGo)
//        {
//            if (base.Blackboard.DistanceFromDestination < base.Blackboard.DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)
//            {
//                base.Blackboard.MyTransform.LookAt(transformToGo);

//                Vector3 transformPosition = transformToGo.position;

//                if (base.Blackboard.CanMove &&
//                    NavMeshHelper.IsReachable(base.Blackboard.MyTransform.position, transformPosition))
//                {
//                    bool haveFinishToGoTo = !(Vector3.Distance(base.Blackboard.MyTransform.position, transformPosition) > base.Blackboard.DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH);

//                    if (!haveFinishToGoTo)
//                    {
//                        base.Blackboard.NavMeshAgent.SetDestination(transformPosition);

//                        base.Blackboard.PreviousPosition = base.Blackboard.MyTransform.position;
//                        base.Blackboard.MyTransform.LookAt(transformToGo);

//                        //base.Blackboard.UpdateMovement();
//                    }
//                    else
//                        return true; // On est arrivé à la position souhaité.
//                }
//            }
//        }

//        return false;
//    }
//}