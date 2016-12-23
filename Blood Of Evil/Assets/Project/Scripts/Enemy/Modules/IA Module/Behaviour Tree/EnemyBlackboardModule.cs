using UnityEngine;
using System.Collections;
using System;

// Redécouper la tâche de go to initial position
// Découper la tâche wander oint
// Si tâche in progress la stoquer
// Conclusion : l'arbre de comportement s'avère pas assez optimisé et complexifie trop le code, je repartirai donc sur une machine à états.

//public class EnemyBlackboardModule : AEntityMovementModule
//{
//    #region Update Methods
//    public override void UpdateBlackboardAtEachFrame()
//    {
//        base.UpdateBlackboardAtEachFrame();

//        this.UpdateAnimatorAndDistanceFromPoint();

//        base.UpdateDistanceFromDestination();

//        this.UpdateMovement();
//    }

//    private void UpdateMovement()
//    {
//        if (!this.CanMove)
//            base.StopToMove();
//        else
//        {
//            base.UpdateNavMeshAgent();
//            base.UpdateDistanceFromDestination();

//            base.UpdateLocomotionSpeedSmoothly();
//        }
//    }

//    private void UpdateAnimatorAndDistanceFromPoint()
//    {
//        base.AnimationModule.SetIsAttackingParameter(false);

//        if (null != this.InitialPosition)
//            this.DistanceFromInitialPosition = Vector3.Distance(base.MyTransform.position, this.InitialPosition.position);

//        if (null != this.TargetTransform)
//            this.DistanceFromPlayer = Vector3.Distance(base.MyTransform.position, this.TargetTransform.position);

//        if (null != this.WanderPoint)
//            this.DistanceFromWanderPoint = Vector3.Distance(base.MyTransform.position, this.WanderPoint.position);
//    }
//    #endregion

//    #region Generic Behaviour
//    protected IEnumerator GoToCoroutine(Transform transformToGo)
//    {
//        if (null != transformToGo)
//        {
//            while (base.DistanceFromDestination < DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)
//            {
//                base.MyTransform.LookAt(transformToGo);
//                this.GoTo(transformToGo);
//                base.UpdateDistanceFromDestination();

//                yield return new WaitForEndOfFrame();
//            }
//        }
//    }

//    //protected IEnumerator WaitCoroutine(float time, ABehaviourNodeDecorator conditionToWait)
//    //{
//    //    base.StopToMove();

//    //    Timer timer = new Timer(time, false);

//    //    while (!timer.IsRingingUpdated())
//    //    {
//    //        //this.InitializePerUpdate();

//    //        if (conditionToWait.CanPerformTask())
//    //            yield return new WaitForEndOfFrame();
//    //    }
//    //}

//    public void GoTo(Transform transformToGo)
//    {
//        if (null != transformToGo)
//        {
//            Vector3 transformPosition = transformToGo.position;

//            if (base.CanMove &&
//                NavMeshHelper.IsReachable(base.MyTransform.position, transformPosition) &&
//                Vector3.Distance(base.MyTransform.position, transformPosition) > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)
//            {
//                base.NavMeshAgent.SetDestination(transformPosition);

//                this.PreviousPosition = base.MyTransform.position;
//            }

//            base.MyTransform.LookAt(transformToGo);

//            this.UpdateMovement();
//        }
//    }
//    #endregion

//    #region Fields
//    [SerializeField]
//    private float maximumDistanceFromInitialPosition = 10.0f;
//    [SerializeField]
//    private float aggroRange = 5.0f;
//    [SerializeField]
//    private float wanderRange = 10.0f;

//    private float distanceFromInitialPosition;
//    private float distanceFromPlayer;
//    private float distanceFromWanderPoint;
    
//    private Transform initialPosition;
//    private Transform wanderPoint;
//    private Transform targetTransform;

//    private Attribute attackRangeAttribute;
//    private EntityLifeCategoryAttributes lifeCategoryAttrutes;

//    private NavMeshHit navMeshHit;
//    [SerializeField]
//    private Renderer myMesh;

//    //private LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
//    private Vector2 circle;
//    private RaycastHit raycastHit;
//    private Ray ray;
//    #endregion

//    #region Properties
//    public float DistanceFromInitialPosition
//    {
//        get
//        {
//            return distanceFromInitialPosition;
//        }

//        set
//        {
//            distanceFromInitialPosition = value;
//        }
//    }

//    public float DistanceFromPlayer
//    {
//        get
//        {
//            return distanceFromPlayer;
//        }

//        set
//        {
//            distanceFromPlayer = value;
//        }
//    }

//    public float DistanceFromWanderPoint
//    {
//        get
//        {
//            return distanceFromWanderPoint;
//        }

//        set
//        {
//            distanceFromWanderPoint = value;
//        }
//    }

//    public float MaximumDistanceFromInitialPosition
//    {
//        get
//        {
//            return maximumDistanceFromInitialPosition;
//        }

//        set
//        {
//            maximumDistanceFromInitialPosition = value;
//        }
//    }

//    public float AggroRange
//    {
//        get
//        {
//            return aggroRange;
//        }

//        set
//        {
//            aggroRange = value;
//        }
//    }

//    public float WanderRange
//    {
//        get
//        {
//            return wanderRange;
//        }

//        set
//        {
//            wanderRange = value;
//        }
//    }

//    public EntityLifeCategoryAttributes LifeCategoryAttrutes
//    {
//        get
//        {
//            return lifeCategoryAttrutes;
//        }

//        set
//        {
//            lifeCategoryAttrutes = value;
//        }
//    }

//    public Transform InitialPosition
//    {
//        get
//        {
//            return initialPosition;
//        }

//        set
//        {
//            initialPosition = value;
//        }
//    }

//    public Transform WanderPoint
//    {
//        get
//        {
//            return wanderPoint;
//        }

//        set
//        {
//            wanderPoint = value;
//        }
//    }

//    public Transform TargetTransform
//    {
//        get
//        {
//            return targetTransform;
//        }

//        set
//        {
//            targetTransform = value;
//        }
//    }

//    public Attribute AttackRangeAttribute
//    {
//        get
//        {
//            return attackRangeAttribute;
//        }

//        private set
//        {
//            attackRangeAttribute = value;
//        }
//    }
//    #endregion

//    #region Intern Behaviour
//    public void ResetLife()
//    {
//        this.LifeCategoryAttrutes.ResetLifeToMaximumLife();
//    }

//    public void AttackTarget()
//    {
//        base.MyTransform.LookAt(this.TargetTransform);
//        base.AnimationModule.SetIsAttackingParameter(true);
//    }

//    /// <summary>
//    /// Lance un raycast sur le nav mesh partant du sommet du mesh vers un point se trouvant autour de cette agent.
//    /// </summary>
//    /// <returns></returns>
//    public Vector3 FindAWanderPoint()
//    {
//        if (null == InitialPosition)
//            return base.MyTransform.position;
//        else
//        {
//            while (true)
//            {
//                circle = UnityEngine.Random.insideUnitCircle * this.WanderRange;
//                ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(base.MyTransform.position + new Vector3(circle.x, 0.0f, circle.y)));

//                if (Physics.Raycast(ray, out raycastHit) &&
//                    NavMeshHelper.IsReachable(base.MyTransform.position, this.raycastHit.point) &&
//                    raycastHit.collider.gameObject != gameObject &&
//                    Vector3.Distance(base.MyTransform.position, this.raycastHit.point) > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)//, layerMask))
//                    return raycastHit.point;

//                return MyTransform.position;
//            }
//        }
//    }

//    private IEnumerator UpdateCoroutine()
//    {
//        while (true)
//        {
//            //this.InitializePerUpdate();
//            base.UpdateDistanceFromDestination();
//            this.UpdateMovement();

//            if (!this.LifeCategoryAttrutes.IsDeath)
//            {
//                if (this.DistanceFromInitialPosition > this.MaximumDistanceFromInitialPosition)
//                {
//                    SceneServicesContainer.Instance.SceneState.UnRegisterEnemy(base.MyTransform); // ne peut pas être attaqué.

//                    //Debug.LogFormat("position : {0}, initial position : {1}", base.MyTransform.position, initialPosition.position);
//                    yield return GoToCoroutine(this.InitialPosition);
//                    this.ResetLife();

//                    SceneServicesContainer.Instance.SceneState.RegisterEnemy(base.MyTransform);
//                    // Wait.. si pas de target proche
//                }
//                else if (this.DistanceFromPlayer < this.AggroRange &&
//                         this.DistanceFromPlayer > this.AttackRangeAttribute.Value)
//                {
//                    //Debug.LogFormat("position : {0}, go to player : {1}", base.MyTransform.position, targetTransform.position);
//                    this.GoTo(this.TargetTransform);
//                }
//                else if (DistanceFromPlayer < this.AttackRangeAttribute.Value)
//                {
//                    //Debug.Log("attack player");
//                    this.AttackTarget();
//                }
//                else if (this.DistanceFromWanderPoint > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)
//                {
//                    //Debug.LogFormat("position : {0}, wander point : {1}", base.MyTransform.position, wanderPoint.position);
//                    this.GoTo(this.WanderPoint); // If !notbeenattacked last 10 seconds | && not die
//                    // Wait..
//                }
//                else
//                {
//                    if (null != this.WanderPoint)
//                        this.WanderPoint.position = this.FindAWanderPoint();
//                }
//            }

//            yield return new WaitForEndOfFrame();
//        }
//    }
//    #endregion

//    #region Override Abstract Behaviour
//    public override void Initialize()
//    {
//        base.Initialize();

//        this.LifeCategoryAttrutes = GetComponent<EnemyServicesAndModulesContainer>().
//                                    AttributesModule.LifeCategoryAttributes;
//        this.AttackRangeAttribute = this.LifeCategoryAttrutes.GetAttribute(EEntityCategoriesAttributes.Attack, "Attack Range").Current;

//        base.MyTransform = transform;

//        this.TargetTransform = SceneServicesContainer.Instance.SceneState.Player;

//        //StartCoroutine(this.UpdateCoroutine());
//        StartCoroutine(this.CreateIAGameObjects());

//        Debug.Log("Initialize");
//    }

//    private IEnumerator CreateIAGameObjects()
//    {
//        yield return new WaitForSeconds(0.3f);

//        this.InitialPosition = new GameObject().transform;
//        this.WanderPoint = new GameObject().transform;
//        this.InitialPosition.name = "Initial Position";
//        this.WanderPoint.name = "Wander Position";
//        this.InitialPosition.position = base.MyTransform.position;
//        this.WanderPoint.position = this.FindAWanderPoint();
//    }

//    public void OnDestroy()
//    {
//        Destroy(this.InitialPosition.gameObject);
//        Destroy(this.WanderPoint.gameObject);
//    }

//    public override bool GetCanMove()
//    {
//        return base.IsNotDyingAndIsNotAttacking();
//    }
//    #endregion
//}
//