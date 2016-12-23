using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Player.Modules.Movements
{
    using Scene;
    using Enemies;
    using Extensions;

    using Entities.Modules.Movement;
    using Entities.Modules.Attributes;

    using Services.Canvases;
    using Services.Keys;

    // Déterminer si on est en mouvement avnat de faire ça
    public sealed class PlayerMovementModule : AEntityMovementModule
    {
        #region Fields
        [SerializeField]
        private LayerMask raycastLayerMask;
        private RaycastHit raycasthHit;
        private NavMeshHit navMeshHit;
        private Ray ray;

        private const float PATHFINDING_PARTICLE_MIDDLE_HEIGHT = 1.0f;
        private const float LOCOMOTION_SPEED_TO_ENABLE_PATHFINDING_PARTICLE = 0.08f;
        private const float MOVE_KEY_DISTANCE = 125.0f;

        private CanvasesManagerService canvasService;
        private KeysService keysService;
        private EntityAttackCategoryAttributes attackAttributesCategory;
        private GameObject pathfindingParticuleGameObject;

        private bool followAndAttackEnemy = false;
        private EnemyServicesAndModulesContainer enemyServices;
        private Transform enemyTransform;
        #endregion

        #region Override Behaviour
        public override bool GetCanMove()
        {
            return !this.canvasService.ACanvasIsOpen &&
                    !this.keysService.IsDown(EPlayerInput.StopToMove) &&
                    base.IsNotDyingAndIsNotAttacking();
        }
        #endregion

        #region Unity Behaviour
        public override void Initialize()
        {
            base.Initialize();

            this.canvasService = PlayerServicesAndModulesContainer.Instance.CanvasesService;
            this.keysService = PlayerServicesAndModulesContainer.Instance.InputService;
            this.attackAttributesCategory = PlayerServicesAndModulesContainer.Instance.AttributesModule.AttackCategoryAttributes;

            this.pathfindingParticuleGameObject = PlayerServicesAndModulesContainer.Instance.GameObjectInSceneReferencesService.Get("Pathfinding Particle");
        }

        void Update()
        {
            if (!this.CanMove)
                base.StopToMove();
            else
            {
                base.UpdateNavMeshAgent();
                base.UpdateDistanceFromDestination();
                this.UpdateMoveKeys();
                base.UpdateLocomotionSpeedSmoothly();

                if (this.followAndAttackEnemy)
                    this.FollowAndAttackEnemy();
            }

            if (!this.followAndAttackEnemy)
                this.UpdatePathfindingParticule();

            // Permet de regarder la où pointe la souris lors d'un clique gauche.
            if (this.CanLookAtMousePosition())
                base.MyTransform.FolowMousePosition();
        }

        #endregion

        #region Public Behaviour
        public void OnDeath()
        {
            this.followAndAttackEnemy = false;
        }
        #endregion

        #region Intern Behaviour
        private void StopToFollowEnemy()
        {
            this.followAndAttackEnemy = false;
            this.SetMyDestination(this.MyTransform.position);
        }

        private bool DoesEnemyIsDeath()
        {
            return null == this.enemyServices ||
                   !SceneServicesContainer.Instance.SceneStateModule.DoesEnemyExists(this.enemyTransform);
        }

        private void GoEnoughtNearOfEnemyToAttackHim()
        {
            //Vector3 direction = this.MyTransform.position.GetDirectionToTargetPosition(this.enemyTransform.position);
            Vector3 destination = this.enemyTransform.position;
            //+ direction * (this.attackAttributesCategory.AttackRange.Current.Value);

            base.NavMeshAgent.SetDestination(destination);
        }

        private void AttackEnemy()
        {
            this.AnimationModule.SetIsAttackingParameter(true);
        }

        private void LookAtEnemy()
        {
            this.MyTransform.LookAt(this.enemyTransform);
        }

        private void FollowAndAttackEnemy()
        {
            if (this.DoesEnemyIsDeath())
                this.StopToFollowEnemy();
            else
            {
                this.LookAtEnemy();
                this.UpdatePathfindingParticule(this.enemyTransform.position);

                //Debug.LogFormat("Attack Enemy or go to player ? {0} : {1}", this.attackAttributesCategory.CanAttackTarget(this.enemyTransform) i++);

                if (this.attackAttributesCategory.CanAttackTarget(this.enemyTransform))
                    this.AttackEnemy();
                else
                    this.GoEnoughtNearOfEnemyToAttackHim();
                // Clean le code de l'attaque.
            }
        }

        private void SetTargetPosition(Vector3 offset, bool moveFromKeys = true)
        {
            this.followAndAttackEnemy = false;

            this.ray =
                moveFromKeys == true ?
                UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Camera.main.WorldToScreenPoint(base.MyTransform.position) + offset) :
                UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition + offset);

            if (Physics.Raycast(this.ray, out this.raycasthHit, 10000.0f, this.raycastLayerMask))// && //this.raycasthHit.collider.CompareTag("Nav Mesh") &&
                                                                                                 //NavMeshHelper.IsReachable(base.MyTransform.position, this.raycasthHit.point))
            {
                if (this.followAndAttackEnemy = this.raycasthHit.collider.tag == "Enemy")
                    this.UpdateEnemyServicesAndTransform();

                bool canGoToDestination = Vector3.Distance(base.MyTransform.position, this.raycasthHit.point) > DISTANCE_FROM_DESTINATION_TO_STOP_NAV_MESH &&
                                          !this.followAndAttackEnemy;

                if (canGoToDestination)
                    this.SetMyDestination(this.raycasthHit.point);

                // Permet d'éviter de repasser à la marche si le joueur était déjà entre la marche et la course.
                if (!this.keysService.DoesAMoveKeyHaveBeenPressed())
                    this.PreviousPosition = base.MyTransform.position;
            }

            if (this.IsNotDying())
                base.MyTransform.LookAt(base.NavMeshAgent.destination);
        }

        private void SetMyDestination(Vector3 destination)
        {
            base.NavMeshAgent.SetDestination(destination);

            this.UpdatePathfindingParticule();
        }

        private void UpdateEnemyServicesAndTransform()
        {
            this.enemyServices = this.raycasthHit.collider.GetComponent<EnemyServicesAndModulesContainer>();
            this.enemyTransform = this.enemyServices.transform;
        }

        private void UpdatePathfindingParticule(Vector3 destination)
        {
            this.pathfindingParticuleGameObject.transform.position =
                destination;

            this.pathfindingParticuleGameObject.SetActive(base.AnimationModule.GetLocomotionSpeedParameter() > LOCOMOTION_SPEED_TO_ENABLE_PATHFINDING_PARTICLE);
        }

        private void UpdatePathfindingParticule()
        {
            this.pathfindingParticuleGameObject.transform.position =
                base.NavMeshAgent.destination +
                Vector3.up * PATHFINDING_PARTICLE_MIDDLE_HEIGHT;

            this.pathfindingParticuleGameObject.SetActive(base.AnimationModule.GetLocomotionSpeedParameter() > LOCOMOTION_SPEED_TO_ENABLE_PATHFINDING_PARTICLE);
        }

        private bool CanLookAtMousePosition()
        {
            return !this.canvasService.ACanvasIsOpen &&
                    this.keysService.IsContiniouslyDown(EPlayerInput.Move) &&
                    this.keysService.IsContiniouslyDown(EPlayerInput.StopToMove) &&
                    base.IsNotDying();
        }

        private void UpdateMoveKeys()
        {
            Vector3 moveOffset = this.keysService.GetMoveKeysDirection();

            if (moveOffset != Vector3.zero)
                this.SetTargetPosition(moveOffset);
            else if (this.keysService.IsContiniouslyDown(EPlayerInput.Move))
                this.SetTargetPosition(Vector3.zero, false);
        }
        #endregion
    }
}