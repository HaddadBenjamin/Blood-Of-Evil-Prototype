using UnityEngine;
using System.Collections;
using System;
using BloodOfEvil.Player;

namespace BloodOfEvil.Enemies.Modules.IA
{
    using Scene;
    using Helpers;
    using ObjectInScene;

    using Utilities;
    using Utilities.Cameras;
    using Utilities.Serialization;

    using Entities.Modules.Movement;
    using Entities.Modules.Attributes;

    // Conclusion : l'arbre de comportement s'avère pas assez optimisé et complexifie trop le code, je repartirai donc sur une machine à états.
    public class EnemyIAModule : AEntityMovementModule, ISerializable
    {
        #region Fields
        [SerializeField]
        private float maximumDistanceFromInitialPosition = 10.0f;
        [SerializeField]
        private float aggroRange = 5.0f;
        [SerializeField]
        private float wanderRange = 10.0f;
        [SerializeField]
        private float waitTimeAfterPatrolOrGoToInitialPosition = 1.0f;
        [SerializeField]
        private float followPlayerTimeAfterToBeAttacked = 8.0f;

        private float distanceFromInitialPosition;
        private float distanceFromPlayer;
        private float distanceFromWanderPoint;

        private Transform initialPosition;
        private Transform wanderPoint;
        private Transform targetTransform;

        private bool goToInitialPosition;
        private bool isWaiting;
        private bool followOrAttackPlayer;

        private Timer waitTimer = new Timer(0.0f);
        private Timer followPlayerTimer = new Timer(0.0f);

        private Attribute attackRangeAttribute;
        private EntityLifeCategoryAttributes lifeCategoryAttrutes;

        private NavMeshHit navMeshHit;
        [SerializeField]
        private Renderer myMesh;

        //private LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
        private Vector2 circle;
        private RaycastHit raycastHit;
        private Ray ray;
        #endregion

        #region Update Methods
        private void UpdateMovement()
        {
            if (!this.CanMove)
                base.StopToMove();
            else
            {
                //base.UpdateLocomotionSpeedSmoothly();
                base.LocomotionSpeed = base.NavMeshAgent.desiredVelocity.magnitude;

                base.UpdateNavMeshAgent();
                base.UpdateDistanceFromDestination();

            }
        }

        private void UpdateAnimatorAndDistanceFromPoint()
        {
            base.AnimationModule.SetIsAttackingParameter(false);

            if (null != this.initialPosition)
                this.distanceFromInitialPosition = Vector3.Distance(base.MyTransform.position, this.initialPosition.position);

            if (null != this.targetTransform)
                this.distanceFromPlayer = Vector3.Distance(base.MyTransform.position, this.targetTransform.position);

            if (null != this.wanderPoint)
                this.distanceFromWanderPoint = Vector3.Distance(base.MyTransform.position, this.wanderPoint.position);
        }
        #endregion

        #region Unity Behaviour
        public void OnDestroy()
        {
            if (null != this.initialPosition)
                Destroy(this.initialPosition.gameObject);
            if (null != this.wanderPoint)
                Destroy(this.wanderPoint.gameObject);
        }
        #endregion

        #region Intern Behaviour
        private IEnumerator CreateIAGameObjects()
        {
            yield return new WaitForSeconds(0.1f);

            this.initialPosition = new GameObject().transform;
            this.wanderPoint = new GameObject().transform;
            this.initialPosition.name = "Initial Position";
            this.wanderPoint.name = "Wander Position";
            this.initialPosition.position = base.MyTransform.position;
            this.wanderPoint.position = this.FindAWanderPoint();

            ((ISerializable)this).Load();
        }

        #region Tasks
        public void ResetLife()
        {
            this.lifeCategoryAttrutes.ResetLifeToMaximumLife();
        }

        public void AttackTarget()
        {
            base.MyTransform.LookAt(this.targetTransform);
            base.AnimationModule.SetIsAttackingParameter(true);
        }

        /// <summary>
        /// Lance un raycast sur le nav mesh partant du sommet du mesh vers un point se trouvant autour de cette agent.
        /// </summary>
        /// <returns></returns>
        public Vector3 FindAWanderPoint()
        {
            if (null == initialPosition)
                return base.MyTransform.position;
            else
            {
                while (true)
                {
                    Camera playerCamera = PlayerServicesAndModulesContainer.Instance.PlayerCamera;

                    circle = UnityEngine.Random.insideUnitCircle * this.wanderRange;
                    ray = playerCamera.ScreenPointToRay(playerCamera.WorldToScreenPoint(base.MyTransform.position + new Vector3(circle.x, 0.0f, circle.y)));

                    if (Physics.Raycast(ray, out raycastHit) &&
                        NavMeshHelper.IsReachable(base.MyTransform.position, this.raycastHit.point) &&
                        raycastHit.collider.gameObject != gameObject &&
                        Vector3.Distance(base.MyTransform.position, this.raycastHit.point) > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)//, layerMask))
                        return raycastHit.point;

                    return MyTransform.position;
                }
            }
        }

        private void EnableWaitingState()
        {
            this.isWaiting = true;

            this.waitTimer = new Timer(this.waitTimeAfterPatrolOrGoToInitialPosition, false);
        }

        public void GoTo(Transform transformToGo)
        {
            if (null != transformToGo)
            {
                Vector3 transformPosition = transformToGo.position;

                if (base.CanMove &&
                    NavMeshHelper.IsReachable(base.MyTransform.position, transformPosition) &&
                    Vector3.Distance(base.MyTransform.position, transformPosition) > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)
                {
                    base.NavMeshAgent.SetDestination(transformPosition);

                    this.PreviousPosition = base.MyTransform.position;
                }

                base.MyTransform.LookAt(transformToGo);

                this.UpdateMovement();
            }
        }


        private void GoToInitialPositionBehaviour()
        {
            this.GoTo(this.initialPosition);

            if (base.IsArrivedToDestination())
            {
                this.ResetLife();

                SceneServicesContainer.Instance.SceneStateModule.RegisterEnemy(base.MyTransform);

                this.goToInitialPosition = false;

                this.EnableWaitingState();
            }
        }

        private void Wander()
        {
            this.GoTo(this.wanderPoint);

            if (this.IsArrivedToDestination())
            {
                this.TryToFoundAWanderPoint();
                this.EnableWaitingState();
            }
        }

        private void Wait()
        {
            this.LocomotionSpeed = 0.0f;
            base.NavMeshAgent.destination = base.MyTransform.position;

            if (this.waitTimer.IsRinging())
            {

                this.waitTimer.Reset();
                this.isWaiting = false;

                //this.UpdateMovement();
            }
        }

        private void GoToInitialPosition()
        {
            SceneServicesContainer.Instance.SceneStateModule.UnRegisterEnemy(base.MyTransform); // ne peut pas être attaqué.
            this.goToInitialPosition = true;
        }

        private void TryToFoundAWanderPoint()
        {
            if (null != this.wanderPoint)
                this.wanderPoint.position = this.FindAWanderPoint();
        }
        #endregion

        #region Decorators
        private bool CanAttackTarget()
        {
            return this.distanceFromPlayer < this.attackRangeAttribute.Value;
        }

        private bool CanGoToWanderPoint()
        {
            return this.distanceFromWanderPoint > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH;
        }

        private bool CanGoToInitialPosition()
        {
            return this.distanceFromInitialPosition > this.maximumDistanceFromInitialPosition;
        }

        private bool CanGoToTarget()
        {
            return this.distanceFromPlayer < this.aggroRange &&
                   this.distanceFromPlayer > this.attackRangeAttribute.Value;
        }

        private bool CanWait()
        {
            //Debug.Log(this.isWaiting &&
            //       !this.CanGoToTarget() &&
            //       !this.CanAttackTarget());
            return this.isWaiting &&
                   !this.CanGoToTarget() &&
                   !this.CanAttackTarget();
        }
        #endregion

        /// <summary>
        /// Tous ce code dégueulasse devra être repasser en machine à états.
        /// </summary>
        /// <returns></returns>
        void Update()
        {
            this.UpdateAnimatorAndDistanceFromPoint();

            //this.UpdateMovement();

            this.waitTimer.Update();
            if (this.followPlayerTimer.IsRingingUpdated())
                this.followOrAttackPlayer = false;

            base.UpdateNavMeshAgent();
            //Debug.Log(this.waitTimer.GetTimeToWait());

            if (!this.lifeCategoryAttrutes.IsDeath)
            {
                if (this.goToInitialPosition)
                    this.GoToInitialPositionBehaviour();
                else if (this.followOrAttackPlayer)
                    this.GoTo(this.targetTransform);
                else if (this.followOrAttackPlayer && this.CanAttackTarget())
                    this.AttackTarget();
                else if (this.CanWait())
                    this.Wait();
                else if (this.CanGoToInitialPosition())
                    this.GoToInitialPosition();
                else if (this.CanGoToTarget())
                    this.GoTo(this.targetTransform);
                else if (this.CanAttackTarget())
                    this.AttackTarget();
                else if (this.CanGoToWanderPoint())
                    this.Wander();
            }
        }
        #endregion

        #region Override Abstract Behaviour
        public override void Initialize()
        {
            base.Initialize();

            this.lifeCategoryAttrutes = GetComponent<EnemyServicesAndModulesContainer>().
                                        AttributesModule.LifeCategoryAttributes;
            this.attackRangeAttribute = this.lifeCategoryAttrutes.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Range").Current;

            base.MyTransform = transform;

            this.targetTransform = SceneServicesContainer.Instance.SceneStateModule.Player;

            this.lifeCategoryAttrutes.ReduceLifeListener += delegate ()
            {
                this.followOrAttackPlayer = true;
                this.followPlayerTimer = new Timer(this.followPlayerTimeAfterToBeAttacked, false);
            };

            StartCoroutine(this.CreateIAGameObjects());
        }
        public override bool GetCanMove()
        {
            return base.IsNotDyingAndIsNotAttacking();
        }
        #endregion

        #region Interfaces Behaviour
        void ISerializable.Load()
        {
            SerializerHelper.Load<SerializablePositionAndRotation>(
                filename: this.GetFileName(),
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                onLoadSuccess: (SerializablePositionAndRotation data) =>
                {
                    data.Load(this.initialPosition);
                },
                onLoadError: () =>
                {
                    Debug.Log("pas d'inquiétude à avoir, c'est normal que ce fichier n'éxiste pas lorsque l'on a pas sauvegarder cette scène au moins fois.");
                });
        }

        void ISerializable.Save()
        {
            SerializerHelper.Save< SerializablePositionAndRotation>(
                filename: this.GetFileName(),
                isReplicatedNextTheBuild: false,
                isEncrypted: true,
                dataToSave : new SerializablePositionAndRotation(this.initialPosition));
        }

        private string GetFileName()
        {
            return SceneServicesContainer.Instance.
                    FileSystemConfiguration.EnemyIAInitialPositionFilename(
                    GetComponent<EnemyServicesAndModulesContainer>().SaveIndex);
        }
        #endregion
    }
}