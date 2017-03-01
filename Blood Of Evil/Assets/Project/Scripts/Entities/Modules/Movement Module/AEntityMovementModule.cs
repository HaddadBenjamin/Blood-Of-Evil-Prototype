using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Entities.Modules.Movement
{
    using Enemies;
    using Player;
    using ObjectInScene;

    using Animations;
    public abstract class AEntityMovementModule : AInitializableComponent // ABlackboard
    {
        #region Fields
        private Transform myTransform;
        private Animator myAnimator;
        private NavMeshAgent navMeshAgent;

        private AEntityAnimationModule animationModule;

        private Vector3 previousPosition;

        [SerializeField]
        private EEntity entityType;

        [SerializeField]
        private float animationMovementSpeedRatio = 1.8f;

        private float distanceFromDestination;
        private float locomotionSpeed;

        // Sera des variables configurables dans l'inspector par la suite de sorte à s'adapter à pouvoir adapter le type de mouvements pour types de character.
        [SerializeField]
        public float DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH = 0.5f;
        [SerializeField]
        public float MAX_LOCOMOTION_SPEED = 1.0f;
        [SerializeField]
        public float MIN_LOCOMOTION_SPEED = 0.0f;
        [SerializeField]
        public float DISTANCE_FROM_DESTINATION_TO_MAX_LOCOMOTION_SPEED = 2.0f;
        [SerializeField]
        public float DISTANCE_FROM_OLD_POSITION_TO_SMOOTH_LOCOMOTION_SPEED = 2.0f;
        [SerializeField]
        public float DISTANCE_FROM_DESTINATION_TO_SMOOTH_LOCOMOTION_SPEED = 2.0f;
        [SerializeField]
        public float MINIMUM_LOCOMOTION_SPEED_WHEN_START_WALKING = 0.45f;
        [SerializeField]
        public float MINIMUM_LOCOMOTION_SPEED_WHEN_STOP_RUNNING = 0.15f;
        #endregion

        #region Override Behaviour
        public override void Initialize()
        {
            this.MyTransform = transform;
            this.MyAnimator = GetComponent<Animator>();
            this.NavMeshAgent = GetComponent<NavMeshAgent>();

            if (EEntity.Player == this.entityType)
                this.AnimationModule = PlayerServicesAndModulesContainer.Instance.AnimationModule;
            else
                this.AnimationModule = GetComponent<EnemyServicesAndModulesContainer>().AnimationModule;
        }
        #endregion

        #region Abstract Behaviour
        public abstract bool GetCanMove();
        #endregion

        #region Unity Behaviour
        void OnAnimatorMove()
        {
            // Permet d'adapdter la vitesse de déplacement en fonction de la vitesse de l'animation.
            this.NavMeshAgent.velocity =
                this.DistanceFromDestination < DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH || !this.CanMove ?
                    Vector3.zero :
                    this.MyAnimator.deltaPosition / Time.deltaTime * this.AnimationMovementSpeedRatio;
        }
        #endregion

        #region Properties
        public bool CanMove
        {
            get
            {
                return this.GetCanMove();

            }
        }

        public Transform MyTransform
        {
            get
            {
                return myTransform;
            }

            set
            {
                myTransform = value;
            }
        }

        public Animator MyAnimator
        {
            get
            {
                return myAnimator;
            }

            set
            {
                myAnimator = value;
            }
        }

        public NavMeshAgent NavMeshAgent
        {
            get
            {
                return navMeshAgent;
            }

            set
            {
                navMeshAgent = value;
            }
        }

        public AEntityAnimationModule AnimationModule
        {
            get
            {
                return animationModule;
            }

            set
            {
                animationModule = value;
            }
        }

        public float AnimationMovementSpeedRatio
        {
            protected get
            {
                return animationMovementSpeedRatio;
            }

            set
            {
                animationMovementSpeedRatio = value;
            }
        }

        public float DistanceFromDestination
        {
            get
            {
                return distanceFromDestination;
            }

            protected set
            {
                distanceFromDestination = value;
            }
        }

        protected float LocomotionSpeed
        {
            get
            {
                return locomotionSpeed;
            }

            set
            {
                locomotionSpeed = value;

                this.AnimationModule.SetLocomotionSpeedParameter(this.LocomotionSpeed);
            }
        }

        public Vector3 PreviousPosition
        {
            get
            {
                return previousPosition;
            }

            set
            {
                previousPosition = value;
            }
        }
        #endregion

        #region Unherited Behaviour
        protected bool IsNotDyingAndIsNotAttacking()
        {
            return !this.AnimationModule.GetIsAttackingParameter() &&
                    !this.AnimationModule.GetIsDyingParameter();
        }

        protected bool IsNotDying()
        {
            return !this.AnimationModule.GetIsDyingParameter();
        }

        protected bool IsArrivedToDestination()
        {
            return this.DistanceFromDestination < DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH;
        }

        protected void UpdateDistanceFromDestination()
        {
            this.DistanceFromDestination = Vector3.Distance(this.MyTransform.position, this.NavMeshAgent.destination);
        }

        protected void UpdateNavMeshAgent()
        {
            this.NavMeshAgent.updateRotation = false;
            this.NavMeshAgent.nextPosition = transform.position;
        }

        public void StopToMove()
        {
            this.LocomotionSpeed = MIN_LOCOMOTION_SPEED;

            this.NavMeshAgent.destination = this.MyTransform.position;
            this.NavMeshAgent.velocity = Vector3.zero;
            this.NavMeshAgent.Stop();
        }

        protected void UpdateLocomotionSpeedSmoothly()
        {
            if (this.DistanceFromDestination > DISTANCE_FROM_DESTINATION_TO_MAX_LOCOMOTION_SPEED)
            {
                float distanceFromOldPosition = Vector3.Distance(this.MyTransform.position, this.PreviousPosition);

                // Rendre fluide le passage d'idle à la marche à la course lorsque le joueur est loin de sa destination et proche de sa précédente position.
                if (distanceFromOldPosition < DISTANCE_FROM_OLD_POSITION_TO_SMOOTH_LOCOMOTION_SPEED)
                    this.LocomotionSpeed = distanceFromOldPosition / DISTANCE_FROM_OLD_POSITION_TO_SMOOTH_LOCOMOTION_SPEED + MINIMUM_LOCOMOTION_SPEED_WHEN_START_WALKING;
                else
                    this.LocomotionSpeed = MAX_LOCOMOTION_SPEED;
            }
            // Rendre fluide le passage de la course à la marche à l'idle lorsque le joueur est proche de sa destination.
            else if (this.DistanceFromDestination > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH)
                this.LocomotionSpeed = this.DistanceFromDestination / DISTANCE_FROM_DESTINATION_TO_SMOOTH_LOCOMOTION_SPEED + MINIMUM_LOCOMOTION_SPEED_WHEN_STOP_RUNNING;
            else
                this.LocomotionSpeed = MIN_LOCOMOTION_SPEED;
        }
        #endregion
    }
}