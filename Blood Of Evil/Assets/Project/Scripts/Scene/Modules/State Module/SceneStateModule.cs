using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BloodOfEvil.Helpers;

namespace BloodOfEvil.Scene.Modules.State
{
    using Player;
    using Enemies;
    using Extensions;
    using ObjectInScene;

    using Entities;
    using Entities.Modules.Attributes;

    // Penser à optimiser mon GameState.
    /// <summary>
    /// Cette classe contient les objets serializable et accessible par tous le monde.
    /// Cette classe est un monobehaviour car elle fait un startCoroutine.
    /// </summary>
    public sealed class SceneStateModule : MonoBehaviour, ISerializable
    {
        #region Fields
        private Transform player;
        private int enemyIndex;
        private List<Transform> enemies = new List<Transform>();
        #endregion

        #region Properties
        public Transform Player
        {
            get { return player; }
            private set { player = value; }
        }
        #endregion

        #region Public Behaviour
        public void Reset()
        {
            this.enemyIndex = 0;
            this.enemies.Clear();
        }

        public void RegisterPlayer(Transform playerTransform)
        {
            this.player = playerTransform;
        }

        public void RegisterEnemy(Transform enemyTransform,
            bool raiseEnemyIndex = false)
        {
            if (!this.enemies.Contains(enemyTransform))
            {
                this.enemies.Add(enemyTransform);

                enemyTransform.GetComponent<EnemyServicesAndModulesContainer>().Instance.SaveIndex =
                    raiseEnemyIndex ?
                        this.enemyIndex++ :
                        this.enemyIndex;
            }
        }

        public void UnRegisterEnemy(Transform enemyTransform)
        {
            this.enemies.Remove(enemyTransform);
        }

        public void UnregisterPlayer()
        {
            this.player = null;
        }

        public Transform GetNearestEnemy()
        {

            return this.enemies.Count > 0 ?
                player.GetNearestTransform(this.enemies.ToArray()) :
                null;
        }

        public Transform GetNearestTarget(EEntity targetType)
        {
            return EEntity.Player == targetType ?
                this.Player :
                this.GetNearestEnemy();
        }

        public bool DoesEnemyExists(Transform enemyTransform)
        {
            return this.enemies.Exists(enemy => enemy == enemyTransform);
        }

        public AEntityAttributesModule GetTargetAttributesModules(Transform targetTransform,
            EEntity targetType)
        {
            if (null == targetTransform)
                return null;

            if (EEntity.Player == targetType)
            {
                var playerServicesContainer = PlayerServicesAndModulesContainer.Instance;

                return null != playerServicesContainer ?
                    playerServicesContainer.AttributesModule :
                    null;
            }
            else
            {
                var enemyServicesContainer = targetTransform.GetComponent<EnemyServicesAndModulesContainer>();

                return null != enemyServicesContainer ?
                    enemyServicesContainer.Instance.AttributesModule :
                    null;
            }
        }

        public int EnemyCount()
        {
            return this.enemies.Count;
        }
        #endregion

        #region Interface Behaviour
        void ISerializable.Load()
        {
            ((ISerializable) PlayerServicesAndModulesContainer.Instance).Load();

            StartCoroutine(this.LoadDiffered());
        }

        private IEnumerator LoadDiffered()
        {
            // Attend que les ennemis se soient iniialiser et que le scenestate est bien initialisé l'index de chaque ennemis puis charge leurs données.
            yield return new WaitForSeconds(0.25f);

            for (int enemyIndex = 0; enemyIndex < this.enemies.Count; enemyIndex++)
            {
                if (
                    SerializerHelper.DoesSaveDirectoryExists(
                        SceneServicesContainer.Instance.FileSystemConfiguration.GetEnemyDirectory(enemyIndex)))
                    ((ISerializable) this.enemies[enemyIndex].GetComponent<EnemyServicesAndModulesContainer>()).Load();
            }
        }

        void ISerializable.Save()
        {
            ((ISerializable)PlayerServicesAndModulesContainer.Instance).Save();

            for (int enemyIndex = 0; enemyIndex < this.enemies.Count; enemyIndex++)
            {
                ((ISerializable)this.enemies[enemyIndex].GetComponent<EnemyServicesAndModulesContainer>()).Save();
            }
        }
        #endregion
    }
}